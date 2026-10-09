# Repository Guidelines

## Project Structure & Module Organization

This research repository reproduces the decompilation of PocketTopo 1.372.

- `inputs/PocketTopo-1.372.zip`: original archive; preserve its bytes.
- `scripts/decompile.py`: downloads, integrity checks, extraction, decompilation, and comparison.
- `tools.lock.json`: pinned tool versions, download URLs, and artifact hashes.
- `decompiled/`: generated C#, IL, localization resources, logs, `manifest.json`, and `SHA256SUMS`.
- `analysis/ANALYSIS.txt`: code map and research observations.
- `tests/test_decompile.py`: reproduction and extraction safety tests.
- `.cache/downloads/`: ignored local tool cache.

## Build, Test, and Development Commands

Run commands from the repository root. Reproduction requires Python 3.9+, `curl`, and network access for uncached packages; no global .NET SDK or Wine installation is required.

- `python3 scripts/decompile.py --verify`: verify input, output checksums, and provenance without network access.
- `python3 -m unittest discover -s tests -v`: run the unit suite without downloading packages.
- `python3 scripts/decompile.py --check`: regenerate in temporary storage and compare every output byte with the committed result.
- `python3 scripts/decompile.py --check --offline`: repeat reproduction using cached packages only.
- `python3 scripts/decompile.py --output /tmp/pockettopo-new-result`: generate into a destination that must not already exist.

The generated C# project has not been recompiled; reproduction checks do not establish application runtime correctness.

## Coding Style & Naming Conventions

Use English for all project-authored documentation, code identifiers, comments, docstrings, user-facing messages, commit messages, and PR descriptions. Preserve original input bytes and generated localization resources in their source languages.

Follow the existing Python style: four-space indentation, `snake_case` functions and variables, `PascalCase` classes, and uppercase constants. Retain type annotations in the script and use `pathlib.Path` for filesystem operations. Use LF endings as specified in `.gitattributes`. No formatter or linter is configured.

## Testing Guidelines

Use standard-library `unittest`, `test_*.py` files, and descriptive `test_*` methods. Add regression cases for changed integrity, extraction, cache, or comparison behavior. Use temporary directories and mocks to keep unit tests independent of downloads. No numerical coverage threshold is configured; run the full reproduction check for pipeline changes.

## Artifact Integrity

Do not hand-edit generated output. Script or lockfile changes invalidate manifest provenance: regenerate into a fresh directory and update the complete output together. Keep algorithm experiments or ports separate from `decompiled/`. Preserve source attribution and pinned hashes.

## Commit & Pull Request Guidelines

Use concise English imperative commit messages, such as `Add reproducible PocketTopo 1.372 decompilation`, and keep commits focused. PRs should explain the change, link relevant issues, list validation commands and results, and identify any regenerated artifacts or tool-version changes.
