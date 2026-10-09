#!/usr/bin/env python3
"""Odtworz dekompilacje PocketTopo przy uzyciu przypietych artefaktow."""

import argparse
import hashlib
import json
import os
from pathlib import Path, PurePosixPath
import platform
import re
import shutil
import stat
import subprocess
import sys
import tarfile
import tempfile
from typing import Any
import zipfile

ROOT = Path(__file__).resolve().parents[1]
DIAGNOSTIC_MARKERS = (
    "Unknown result type (might be due to invalid IL or missing references)",
    "Could not decompile",
    "Invalid IL code",
)


class DecompilationError(Exception):
    """Nie mozna potwierdzic poprawnego, powtarzalnego wyniku."""


def digest(path: Path, algorithm: str = "sha256") -> str:
    hasher = hashlib.new(algorithm)
    with path.open("rb") as stream:
        for block in iter(lambda: stream.read(1024 * 1024), b""):
            hasher.update(block)
    return hasher.hexdigest()


def require_digest(path: Path, spec: dict[str, Any]) -> None:
    if not path.is_file() or path.is_symlink():
        raise DecompilationError(f"Brak zwyklego pliku: {path}")
    algorithm = "sha512" if "sha512" in spec else "sha256"
    if digest(path, algorithm) != spec[algorithm]:
        raise DecompilationError(f"Niezgodna suma {algorithm}: {path}")


def safe_path(root: Path, name: str) -> Path:
    relative = PurePosixPath(name)
    if (
        not name
        or "\\" in name
        or ":" in name
        or relative.is_absolute()
        or ".." in relative.parts
    ):
        raise DecompilationError(f"Niebezpieczna sciezka: {name!r}")
    target = root.joinpath(*relative.parts)
    resolved = target.resolve()
    base = root.resolve()
    if resolved != base and base not in resolved.parents:
        raise DecompilationError(f"Sciezka wychodzi poza katalog: {name!r}")
    return target


def extract_archive(archive: Path, target: Path) -> None:
    target.mkdir(parents=True)
    if zipfile.is_zipfile(archive):
        with zipfile.ZipFile(archive) as source:
            for member in source.infolist():
                safe_path(target, member.filename)
                if stat.S_ISLNK(member.external_attr >> 16):
                    raise DecompilationError("Dowiazanie symboliczne w ZIP")
            source.extractall(target)
        return
    with tarfile.open(archive) as source:
        for member in source.getmembers():
            destination = safe_path(target, member.name)
            if member.issym() or member.islnk():
                anchor = destination.parent if member.issym() else target
                if (
                    "\\" in member.linkname
                    or ":" in member.linkname
                    or PurePosixPath(member.linkname).is_absolute()
                ):
                    raise DecompilationError("Niebezpieczne dowiazanie TAR")
                linked = (anchor / member.linkname).resolve()
                if (
                    linked != target.resolve()
                    and target.resolve() not in linked.parents
                ):
                    raise DecompilationError("Dowiazanie TAR wychodzi poza katalog")
            elif not member.isfile() and not member.isdir():
                raise DecompilationError(f"Nieobslugiwany wpis TAR: {member.name}")
        source.extractall(target)


def fetch_package(spec: dict[str, Any], cache: Path, offline: bool) -> Path:
    archive_name = spec["archive"]
    if Path(archive_name).name != archive_name:
        raise DecompilationError("Nazwa archiwum w lockfile nie jest nazwa pliku")
    cache.mkdir(parents=True, exist_ok=True)
    cached = cache / archive_name
    if cached.exists():
        require_digest(cached, spec)
        return cached
    if offline:
        raise DecompilationError(f"Brak pakietu w cache offline: {cached}")
    print(f"Pobieranie {archive_name}...", flush=True)
    with tempfile.TemporaryDirectory(prefix="download-", dir=cache) as scratch:
        temporary = Path(scratch) / archive_name
        command = [
            "curl",
            "--fail",
            "--silent",
            "--show-error",
            "--location",
            "--retry",
            "2",
            "--connect-timeout",
            "15",
            "--max-time",
            "120",
            "--output",
            str(temporary),
            spec["url"],
        ]
        subprocess.run(command, check=True)
        require_digest(temporary, spec)
        temporary.replace(cached)
    return cached


def runtime_id() -> str:
    systems = {"Darwin": "osx", "Linux": "linux", "Windows": "win"}
    machines = {"arm64": "arm64", "aarch64": "arm64", "amd64": "x64", "x86_64": "x64"}
    try:
        return systems[platform.system()] + "-" + machines[platform.machine().lower()]
    except KeyError as exc:
        raise DecompilationError(
            "Obslugiwane sa macOS/Linux/Windows, arm64 lub x64"
        ) from exc


def files_in(root: Path) -> dict[str, Path]:
    if not root.is_dir() or root.is_symlink():
        raise DecompilationError(f"Brak zwyklego katalogu: {root}")
    files = {}
    for path in root.rglob("*"):
        if path.is_symlink():
            raise DecompilationError(f"Dowiazanie w wynikach: {path}")
        if path.is_file():
            files[path.relative_to(root).as_posix()] = path
    return files


def write_checksums(root: Path) -> None:
    lines = [
        f"{digest(path)}  {name}\n"
        for name, path in sorted(files_in(root).items())
        if name != "SHA256SUMS"
    ]
    (root / "SHA256SUMS").write_bytes("".join(lines).encode("utf-8"))


def verify_checksums(root: Path) -> None:
    expected = {}
    for line in (root / "SHA256SUMS").read_text(encoding="utf-8").splitlines():
        match = re.fullmatch(r"([0-9a-f]{64})  (.+)", line)
        if not match:
            raise DecompilationError("Niepoprawny wpis SHA256SUMS")
        value, name = match.groups()
        if name in expected or name == "SHA256SUMS":
            raise DecompilationError(f"Powtorzony/niedozwolony wpis: {name}")
        safe_path(root, name)
        expected[name] = value
    actual = files_in(root)
    if set(actual) != set(expected) | {"SHA256SUMS"} or not expected:
        raise DecompilationError("Lista wynikow nie zgadza sie z SHA256SUMS")
    for name, value in expected.items():
        require_digest(actual[name], {"sha256": value})


def compare_trees(expected: Path, actual: Path) -> None:
    expected_files = files_in(expected)
    actual_files = files_in(actual)
    differences = [
        name
        for name in sorted(set(expected_files) | set(actual_files))
        if name not in expected_files
        or name not in actual_files
        or digest(expected_files[name]) != digest(actual_files[name])
    ]
    if differences:
        raise DecompilationError(
            "Roznice wzgledem repozytorium:\n" + "\n".join(differences)
        )


def verify_result(root: Path, lock: dict[str, Any]) -> None:
    verify_checksums(root)
    manifest = json.loads((root / "manifest.json").read_text(encoding="utf-8"))
    expected = {
        "schema_version": 1,
        "source_archive_sha256": lock["application"]["sha256"],
        "assemblies_sha256": lock["application"]["assemblies_sha256"],
        "lockfile_sha256": digest(ROOT / "tools.lock.json"),
        "script_sha256": digest(Path(__file__)),
    }
    for name, value in expected.items():
        if manifest.get(name) != value:
            raise DecompilationError(
                f"Manifest nie odpowiada biezacemu wejsciu/procedurze: {name}"
            )


def normalize_text(root: Path) -> None:
    for path in root.rglob("*"):
        if path.is_file() and path.suffix in {".cs", ".csproj", ".resx", ".il", ".txt"}:
            path.write_bytes(path.read_bytes().replace(b"\r\n", b"\n"))


def il_counts(il: str) -> dict[str, int]:
    methods = list(re.finditer(r"^\s*\.method\s", il, re.MULTILINE))
    imports = 0
    abstract = 0
    for index, method in enumerate(methods):
        end = methods[index + 1].start() if index + 1 < len(methods) else len(il)
        header = il[method.start() : end].split("{", 1)[0]
        imports += "pinvokeimpl(" in header
        abstract += " abstract " in header
    bodies = len(re.findall(r"^\s*// Method begins at RVA", il, re.MULTILINE))
    if len(methods) != imports + abstract + bodies:
        raise DecompilationError("Nie wszystkie metody zostaly rozliczone w IL")
    return {
        "types": len(re.findall(r"^\s*\.class\s", il, re.MULTILINE)),
        "methods": len(methods),
        "managed_il_bodies": bodies,
        "native_imports": imports,
        "abstract_methods": abstract,
    }


def generate(lock: dict[str, Any], scratch: Path, cache: Path, offline: bool) -> Path:
    runtime = fetch_package(lock["dotnet"]["runtimes"][runtime_id()], cache, offline)
    ilspy = fetch_package(lock["ilspy"], cache, offline)
    references = fetch_package(lock["references"], cache, offline)
    for name, archive in [
        ("runtime", runtime),
        ("ilspy", ilspy),
        ("references", references),
    ]:
        extract_archive(archive, scratch / name)
    extract_archive(ROOT / lock["application"]["archive"], scratch / "input")
    for name, value in lock["application"]["assemblies_sha256"].items():
        require_digest(safe_path(scratch / "input", name), {"sha256": value})
    output = scratch / "result"
    output.mkdir()
    logs = output / "logs"
    logs.mkdir()
    environment = os.environ.copy()
    environment.update(
        {
            "DOTNET_CLI_TELEMETRY_OPTOUT": "1",
            "DOTNET_SKIP_FIRST_TIME_EXPERIENCE": "1",
            "DOTNET_CLI_HOME": str(scratch / "cli-home"),
            "DOTNET_MULTILEVEL_LOOKUP": "0",
        }
    )
    host = scratch / "runtime" / ("dotnet.exe" if os.name == "nt" else "dotnet")
    base = [
        str(host),
        str(scratch / "ilspy" / lock["ilspy"]["entrypoint"]),
        "--disable-updatecheck",
        "-r",
        str(scratch / "references" / lock["references"]["directory"]),
    ]
    assembly = scratch / "input" / lock["application"]["main_assembly"]
    jobs = [
        (
            "main-csharp",
            ["-p", "--nested-directories", "-o", str(output / "csharp"), str(assembly)],
        ),
        ("main-il", ["-il", str(assembly)]),
    ]
    for culture in lock["localizations"]:
        satellite = assembly.parent / culture / "PocketTopo.resources.dll"
        jobs.append(
            (
                culture,
                [
                    "-p",
                    "--nested-directories",
                    "-o",
                    str(output / "localizations" / culture),
                    str(satellite),
                ],
            )
        )
    statuses = {}
    for name, arguments in jobs:
        print(f"Dekompilacja: {name}...", flush=True)
        process = subprocess.run(
            base + arguments, cwd=scratch, env=environment, capture_output=True
        )
        statuses[name] = process.returncode
        (logs / (name + ".stderr.txt")).write_bytes(process.stderr)
        if name == "main-il":
            (output / "PocketTopo.il").write_bytes(process.stdout)
        else:
            (logs / (name + ".stdout.txt")).write_bytes(process.stdout)
        if process.returncode or process.stderr:
            raise DecompilationError(
                f"ILSpy zglosil problem ({name}):\n"
                + process.stderr.decode("utf-8", errors="replace")
            )
    normalize_text(output)
    for path in (output / "csharp").rglob("*.cs"):
        text = path.read_text(encoding="utf-8-sig")
        if any(marker in text for marker in DIAGNOSTIC_MARKERS):
            raise DecompilationError(f"Diagnostyka dekompilatora w C#: {path.name}")
    csharp_files = list((output / "csharp").rglob("*.cs"))
    counts = {
        "csharp_files": len(csharp_files),
        "csharp_lines": sum(
            len(path.read_text(encoding="utf-8-sig").splitlines())
            for path in csharp_files
        ),
    }
    counts.update(il_counts((output / "PocketTopo.il").read_text(encoding="utf-8")))
    manifest = {
        "schema_version": 1,
        "application": "PocketTopo " + lock["application"]["version"],
        "source_archive_sha256": lock["application"]["sha256"],
        "assemblies_sha256": lock["application"]["assemblies_sha256"],
        "lockfile_sha256": digest(ROOT / "tools.lock.json"),
        "script_sha256": digest(Path(__file__)),
        "tools": {
            "ilspycmd": lock["ilspy"]["version"],
            "dotnet_runtime": lock["dotnet"]["version"],
            "reference_assemblies": lock["references"]["package"]
            + " "
            + lock["references"]["version"],
        },
        "main_exe_counts": counts,
        "decompilation_exit_codes": statuses,
        "rebuilt_application_tested": False,
    }
    (output / "manifest.json").write_bytes(
        (json.dumps(manifest, indent=2) + "\n").encode("utf-8")
    )
    write_checksums(output)
    verify_checksums(output)
    return output


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    modes = parser.add_mutually_exclusive_group()
    modes.add_argument(
        "--check",
        action="store_true",
        help="odtworz i porownaj wynik bez zmieniania repozytorium",
    )
    modes.add_argument(
        "--verify",
        action="store_true",
        help="sprawdz zapisane sumy, bez narzedzi i sieci",
    )
    parser.add_argument("--output", type=Path, help="nowy katalog na odtworzony wynik")
    parser.add_argument("--cache", type=Path, default=ROOT / ".cache" / "downloads")
    parser.add_argument(
        "--offline",
        action="store_true",
        help="korzystaj tylko z juz pobranych pakietow",
    )
    args = parser.parse_args()
    target = args.output.resolve() if args.output else ROOT / "decompiled"
    lock = json.loads((ROOT / "tools.lock.json").read_text(encoding="utf-8"))
    if lock["schema_version"] != 1:
        raise DecompilationError("Nieobslugiwana wersja tools.lock.json")
    require_digest(ROOT / lock["application"]["archive"], lock["application"])
    if args.verify:
        verify_result(target, lock)
        print("Sumy oryginalu i wszystkich wynikow sa zgodne.")
        return 0
    checking = args.check or (not args.output and target.exists())
    if checking:
        verify_result(target, lock)
    elif target.exists():
        raise DecompilationError("Katalog --output juz istnieje; wybierz nowy katalog")
    with tempfile.TemporaryDirectory(prefix="pockettopo-decompile-") as directory:
        output = generate(lock, Path(directory), args.cache, args.offline)
        if checking:
            compare_trees(target, output)
            print("Odtworzony wynik jest identyczny bajt w bajt z repozytorium.")
        else:
            shutil.copytree(output, target)
            print(f"Zapisano wynik: {target}")
    return 0


if __name__ == "__main__":
    try:
        sys.exit(main())
    except (
        DecompilationError,
        OSError,
        ValueError,
        KeyError,
        tarfile.TarError,
        zipfile.BadZipFile,
        subprocess.CalledProcessError,
    ) as error:
        print(f"Blad: {error}", file=sys.stderr)
        sys.exit(1)
