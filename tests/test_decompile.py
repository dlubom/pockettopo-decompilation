import hashlib
import importlib.util
import io
import json
from pathlib import Path
import tarfile
import tempfile
import unittest
from unittest.mock import patch
import zipfile

SPEC = importlib.util.spec_from_file_location(
    "decompile", Path(__file__).resolve().parents[1] / "scripts/decompile.py"
)
decompile = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(decompile)


class ReproductionContractTests(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory()
        self.root = Path(self.temporary.name)

    def tearDown(self):
        self.temporary.cleanup()

    def test_archive_hash_rejects_changed_bytes(self):
        source = self.root / "source.zip"
        source.write_bytes(b"original")
        expected = {"sha256": hashlib.sha256(b"original").hexdigest()}
        decompile.require_digest(source, expected)
        source.write_bytes(b"changed")
        with self.assertRaises(decompile.DecompilationError):
            decompile.require_digest(source, expected)

    def test_offline_cache_requires_exact_pinned_package(self):
        cache = self.root / "cache"
        spec = {"archive": "tool.zip", "sha256": hashlib.sha256(b"tool").hexdigest()}
        with self.assertRaises(decompile.DecompilationError):
            decompile.fetch_package(spec, cache, offline=True)
        (cache / "tool.zip").write_bytes(b"wrong")
        with self.assertRaises(decompile.DecompilationError):
            decompile.fetch_package(spec, cache, offline=True)
        (cache / "tool.zip").write_bytes(b"tool")
        self.assertEqual(
            decompile.fetch_package(spec, cache, offline=True), cache / "tool.zip"
        )

    def test_checksums_reject_extra_missing_and_changed_results(self):
        output = self.root / "output"
        output.mkdir()
        (output / "ID.cs").write_bytes(b"class ID {}\n")
        decompile.write_checksums(output)
        decompile.verify_checksums(output)
        (output / "extra.txt").write_bytes(b"unexpected")
        with self.assertRaises(decompile.DecompilationError):
            decompile.verify_checksums(output)
        (output / "extra.txt").unlink()
        (output / "ID.cs").write_bytes(b"modified")
        with self.assertRaises(decompile.DecompilationError):
            decompile.verify_checksums(output)
        (output / "ID.cs").unlink()
        with self.assertRaises(decompile.DecompilationError):
            decompile.verify_checksums(output)

    def test_checksums_reject_duplicate_and_traversal_entries(self):
        digest = hashlib.sha256(b"secret").hexdigest()
        (self.root / "secret").write_bytes(b"secret")
        output = self.root / "output"
        output.mkdir()
        for entry in [digest + "  ../secret\n", (digest + "  item.txt\n") * 2]:
            with self.subTest(entry=entry):
                (output / "SHA256SUMS").write_text(entry)
                with self.assertRaises(decompile.DecompilationError):
                    decompile.verify_checksums(output)

    def test_tree_comparison_detects_content_and_inventory_changes(self):
        expected = self.root / "expected"
        actual = self.root / "actual"
        expected.mkdir()
        actual.mkdir()
        for folder in [expected, actual]:
            (folder / "ID.cs").write_bytes(b"identical")
        decompile.compare_trees(expected, actual)
        (actual / "ID.cs").write_bytes(b"different")
        with self.assertRaises(decompile.DecompilationError):
            decompile.compare_trees(expected, actual)
        (actual / "ID.cs").write_bytes(b"identical")
        (actual / "extra.txt").touch()
        with self.assertRaises(decompile.DecompilationError):
            decompile.compare_trees(expected, actual)

    def test_zip_traversal_is_rejected_before_extracting_any_file(self):
        archive = self.root / "unsafe.zip"
        with zipfile.ZipFile(archive, "w") as source:
            source.writestr("good.txt", b"good")
            source.writestr("../escaped.txt", b"escape")
        output = self.root / "output"
        with self.assertRaises(decompile.DecompilationError):
            decompile.extract_archive(archive, output)
        self.assertFalse((output / "good.txt").exists())
        self.assertFalse((self.root / "escaped.txt").exists())

    def test_tar_external_symlink_is_rejected_before_extraction(self):
        archive = self.root / "unsafe.tar.gz"
        with tarfile.open(archive, "w:gz") as source:
            item = tarfile.TarInfo("good.txt")
            item.size = 4
            source.addfile(item, io.BytesIO(b"good"))
            link = tarfile.TarInfo("link")
            link.type = tarfile.SYMTYPE
            link.linkname = "/etc"
            source.addfile(link)
        output = self.root / "output"
        with self.assertRaises(decompile.DecompilationError):
            decompile.extract_archive(archive, output)
        self.assertFalse((output / "good.txt").exists())

    def test_tar_allows_internal_relative_symlink(self):
        archive = self.root / "safe.tar.gz"
        with tarfile.open(archive, "w:gz") as source:
            item = tarfile.TarInfo("library.txt")
            item.size = 4
            source.addfile(item, io.BytesIO(b"data"))
            directory = tarfile.TarInfo("links")
            directory.type = tarfile.DIRTYPE
            directory.mode = 0o755
            source.addfile(directory)
            link = tarfile.TarInfo("links/library.txt")
            link.type = tarfile.SYMTYPE
            link.linkname = "../library.txt"
            source.addfile(link)
        output = self.root / "output"
        decompile.extract_archive(archive, output)
        self.assertEqual((output / "links/library.txt").read_bytes(), b"data")

    def test_manifest_rejects_stale_procedure_even_with_valid_checksums(self):
        output = self.root / "output"
        output.mkdir()
        lock = json.loads((decompile.ROOT / "tools.lock.json").read_text())
        manifest = {
            "schema_version": 1,
            "source_archive_sha256": lock["application"]["sha256"],
            "assemblies_sha256": lock["application"]["assemblies_sha256"],
            "lockfile_sha256": decompile.digest(decompile.ROOT / "tools.lock.json"),
            "script_sha256": decompile.digest(Path(decompile.__file__)),
        }
        (output / "manifest.json").write_text(json.dumps(manifest))
        decompile.write_checksums(output)
        decompile.verify_result(output, lock)
        manifest["script_sha256"] = "0" * 64
        (output / "manifest.json").write_text(json.dumps(manifest))
        decompile.write_checksums(output)
        with self.assertRaises(decompile.DecompilationError):
            decompile.verify_result(output, lock)

    def test_normalization_keeps_binary_resources_unchanged(self):
        (self.root / "ID.cs").write_bytes(b"line\r\nnext\r\n")
        (self.root / "app.ico").write_bytes(b"binary\r\nresource")
        decompile.normalize_text(self.root)
        self.assertEqual((self.root / "ID.cs").read_bytes(), b"line\nnext\n")
        self.assertEqual((self.root / "app.ico").read_bytes(), b"binary\r\nresource")

    def test_existing_external_output_is_not_overwritten(self):
        output = self.root / "user-files"
        output.mkdir()
        (output / "keep.txt").write_bytes(b"keep")
        with patch("sys.argv", ["decompile.py", "--output", str(output)]):
            with self.assertRaises(decompile.DecompilationError):
                decompile.main()
        self.assertEqual((output / "keep.txt").read_bytes(), b"keep")

    def test_runtime_selection_reports_unsupported_platform(self):
        with (
            patch("platform.system", return_value="Darwin"),
            patch("platform.machine", return_value="arm64"),
        ):
            self.assertEqual(decompile.runtime_id(), "osx-arm64")
        with patch("platform.machine", return_value="unsupported"):
            with self.assertRaises(decompile.DecompilationError):
                decompile.runtime_id()


if __name__ == "__main__":
    unittest.main()
