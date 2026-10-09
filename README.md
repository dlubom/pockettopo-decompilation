# PocketTopo 1.372 — reproducible decompilation

A local research repository containing the original input, a script that
reproduces the decompilation, and the resulting C# and IL code. The script
uses ILSpyCmd and pinned .NET 2.0 reference assemblies. It does not require
Wine, a .NET SDK, or running PocketTopo. Tools are downloaded to a local
cache without being installed globally.

## Quick start

Requirements: Python 3.9 or later, `curl`, and network access for the first
reproduction. Reproduction and byte-for-byte matching have been verified
on macOS arm64. The script also selects pinned runtimes for macOS x64,
Linux arm64/x64, and Windows arm64/x64; those platforms have not been tested.

Run from the repository root:

```sh
python3 scripts/decompile.py --check
```

This command verifies the original archive, downloads packages with pinned
versions and checksums, regenerates C# and IL in a temporary directory, and
compares every file with `decompiled/`. Exit code `0` means a byte-for-byte
match, including the manifest and file inventory. Differences or decompiler
diagnostics result in exit code `1`. The stored output remains available.

Check integrity without tools or network access:

```sh
python3 scripts/decompile.py --verify
```

Repeat reproduction without downloads once the cache has been populated:

```sh
python3 scripts/decompile.py --check --offline
```

Save fresh output to a new directory:

```sh
python3 scripts/decompile.py --output /tmp/pockettopo-new-result
```

The `--output` directory must not already exist. Running without arguments
creates `decompiled/` if it is missing; if output already exists, the script
regenerates and compares it, as with `--check`. Input and default output
paths are resolved relative to the script, so it can also be run from
another directory. `--cache PATH` selects an alternative package cache.

## Contents

```text
inputs/PocketTopo-1.372.zip  author's original package, preserved byte for byte
tools.lock.json            versions, URLs, and SHA-256/SHA-512 hashes of tools and inputs
scripts/decompile.py       download, verification, decompilation, and comparison
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
python3 -m unittest discover -s tests -v
```

The tests do not download packages. The full integration check is `--check`,
which runs the pinned decompiler and compares the entire output.
