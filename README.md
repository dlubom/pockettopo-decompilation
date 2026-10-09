# PocketTopo 1.372 — reproducible decompilation

A local research repository containing the original input, a script that
reproduces the decompilation, and the resulting C# and IL code. The script
uses ILSpyCmd and pinned .NET 2.0 reference assemblies. It does not require
Wine, a .NET SDK, or running PocketTopo. Tools are downloaded to a local
cache without being installed globally.

## Quick start

Requirements: a POSIX shell, `curl`, and `sha256sum` or `shasum` for the local
development setup, plus network access for the first run. The wrapper installs
uv 0.11.8 locally, and uv downloads CPython 3.12.12 and the locked Ruff package.
No global uv, Ruff, or Python installation is needed for this setup.
Reproduction and byte-for-byte matching have been verified
on macOS arm64. The script also selects pinned runtimes for macOS x64,
Linux arm64/x64, and Windows arm64/x64; those platforms have not been tested.

Run from the repository root:

```sh
./bin/uv sync --locked
./bin/uv run --locked python scripts/decompile.py --check
```

This command verifies the original archive, downloads packages with pinned
versions and checksums, regenerates C# and IL in a temporary directory, and
compares every file with `decompiled/`. Exit code `0` means a byte-for-byte
match, including the manifest and file inventory. Differences or decompiler
diagnostics result in exit code `1`. The stored output remains available.

Check integrity without tools or network access:

```sh
./bin/uv run --locked --offline python scripts/decompile.py --verify
```

Repeat reproduction without downloads once the cache has been populated:

```sh
./bin/uv run --locked --offline python scripts/decompile.py --check --offline
```

Save fresh output to a new directory:

```sh
./bin/uv run --locked python scripts/decompile.py --output /tmp/pockettopo-new-result
```

The `--output` directory must not already exist. Running without arguments
creates `decompiled/` if it is missing; if output already exists, the script
regenerates and compares it, as with `--check`. Input and default output
paths are resolved relative to the script, so it can also be run from
another directory. `--cache PATH` selects an alternative package cache.

## Local development environment

Always use `./bin/uv` from the repository root. This wrapper checks the pinned
uv installer SHA-256 before executing it, checks the installed uv version,
and keeps uv, managed Python, tool storage, and package caches under `.cache/`.
The project environment is `.venv/`; Ruff is installed there rather than
globally. The installer does not modify shell profiles or the global PATH.
The local wrapper supports macOS and Linux; it has been tested on macOS arm64.

`.python-version` pins the development interpreter, `pyproject.toml` pins uv
and Ruff, and `uv.lock` records package artifacts and their hashes. The script
itself still supports standalone Python 3.9+ execution on its supported
platforms. `tools.lock.json` separately pins the decompilation tools.

```sh
./bin/uv sync --locked
./bin/uv run --locked ruff check .
./bin/uv run --locked ruff format --check .
./bin/uv run --locked python -m unittest discover -s tests -v
```

`--locked` rejects stale project metadata instead of rewriting the lockfile.
After the initial setup, rebuild the environment from cached packages with
`./bin/uv sync --locked --offline`; add `--offline` to `uv run` commands to
disable uv downloads as well. Full offline reproduction also requires the
decompiler packages in `.cache/downloads/` and the script's own `--offline`.
The wrapper fails without downloading if offline mode is requested before
local uv has been installed.

Ruff checks project-authored Python and excludes inputs, generated output,
and caches. To apply formatting, run `./bin/uv run --locked ruff format .`.
Formatting `scripts/decompile.py` changes its provenance hash: regenerate
into a fresh directory and replace the complete output before verification.

## Contents

```text
inputs/PocketTopo-1.372.zip  author's original package, preserved byte for byte
tools.lock.json            versions, URLs, and SHA-256/SHA-512 hashes of tools and inputs
scripts/decompile.py       download, verification, decompilation, and comparison
bin/uv                     pinned local uv bootstrap and environment wrapper
pyproject.toml             project metadata, pinned tools, and Ruff configuration
uv.lock                    development package versions and artifact hashes
.python-version            pinned development Python version
decompiled/csharp/         main program, .csproj project, and resources
decompiled/localizations/  de, fr, pl, and sl resources
decompiled/PocketTopo.il   complete IL code of the main EXE
decompiled/manifest.json   provenance, script and lockfile hashes, and statistics
decompiled/SHA256SUMS      checksums of all output files
decompiled/logs/           stdout/stderr from individual ILSpy invocations
analysis/ANALYSIS.txt      code map and initial observations
tests/test_decompile.py    integrity and safe reproduction tests
.cache/downloads/          downloaded tools, ignored by Git
```

`decompiled/` is generated output. Use a separate working directory for
algorithm changes or a port, keeping this output as a reference. The script
compares actual files, not just declared checksums. It also rejects modified
archives, missing or extra output files, and unsafe paths in packages.

Project-authored documentation and code use English. Original inputs and
generated localization resources retain their source languages to preserve
artifact integrity.

## Input and tools

- [The author's PocketTopo, version 1.372](https://paperless.bheeb.ch/PocketTopo13.html).
  The archive was downloaded from the
  [official URL](https://paperless.bheeb.ch/download/PocketTopo.zip)
  and preserved in `inputs/`.
- [ILSpyCmd](https://github.com/icsharpcode/ILSpy): `9.1.0.7988`.
- .NET runtime: `8.0.31`; archives and their SHA-512 hashes come from
  [Microsoft release metadata](https://builds.dotnet.microsoft.com/dotnet/release-metadata/8.0/releases.json).
- [Microsoft.NETFramework.ReferenceAssemblies.net20](https://www.nuget.org/packages/Microsoft.NETFramework.ReferenceAssemblies.net20/1.0.3): `1.0.3`.

SHA-256 of the original `PocketTopo.exe`:

```text
048cf57b874238ac142f1e4d3446af639ce3760b4a91724ea00bcc320eb5c371
```

SHA-256 of the preserved archive:

```text
2f06a51ed7e5cf3755339cd3c9e645779fb7295eaf66643c83c92ab65b12f3b9
```

`tools.lock.json` also contains hashes for all four localization assemblies.
Moving the repository does not require an existing PocketTopo installation.
Tool packages remain in the cache for offline use; they are not part of
the Git history.

## How the output is generated

The script verifies the input and packages, extracts them into a fresh
temporary directory, and runs ILSpyCmd through the downloaded .NET runtime.
For the main EXE, it uses `-p --nested-directories` for the C# project and
`-il` for IL. It then decompiles the four localization assemblies. `-r`
points to the pinned .NET 2.0 reference assemblies, and
`--disable-updatecheck` disables additional update checks.

Text output is normalized to LF line endings; binary resources remain
unchanged. The manifest omits execution dates and temporary paths so that
successive runs can be compared. It includes SHA-256 hashes of the script
and lockfile, making procedural changes visible in the output provenance.

The first decompilation attempt used .NET 2.0 libraries from a Wine
installation. Switching to the pinned reference package changed only local
variable names in three UI files: `MainForm.cs`, `MapView.cs`, and
`SketchView.cs`. This repository's output comes from the procedure described
here.

## Output status

Main EXE: 56 C# files, 15,469 lines, 73 types, and 849 methods — 829 with IL
bodies, 16 abstract methods, and 4 native imports. All six decompiler
invocations completed with exit code `0` and no diagnostics.

This is a reconstruction from a compiled program. It does not recover the
author's comments, project history, or all original variable names. The
generated project has not been recompiled; byte-for-byte decompilation
matching does not test the behavior of a rebuilt application. The system
implementations in `coredll.dll` and the DistoX firmware are outside the
input EXE. The reconstructed code remains attributable to Beat Heeb's
program.

## Script tests

```sh
./bin/uv run --locked --offline python -m unittest discover -s tests -v
```

The tests do not download packages. The full integration check is `--check`,
which runs the pinned decompiler and compares the entire output.
