import os
from pathlib import Path
import shutil
import subprocess
import tempfile
import unittest


@unittest.skipUnless(os.name == "posix", "The local uv wrapper requires POSIX")
class LocalUvTests(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory(prefix="local uv ")
        self.root = Path(self.temporary.name)
        (self.root / "bin").mkdir()
        self.wrapper = self.root / "bin" / "uv"
        shutil.copy2(Path(__file__).resolve().parents[1] / "bin" / "uv", self.wrapper)
        self.env = os.environ.copy()
        self.env.pop("UV_OFFLINE", None)

    def tearDown(self):
        self.temporary.cleanup()

    def run_wrapper(self, *arguments):
        return subprocess.run(
            [str(self.wrapper), *arguments],
            env=self.env,
            capture_output=True,
            text=True,
            check=False,
        )

    def install_fake_uv(self, version="0.11.8"):
        binary = self.root / ".cache" / "uv-bin" / "uv"
        binary.parent.mkdir(parents=True)
        binary.write_text(
            "#!/bin/sh\n"
            'if [ "$1" = --version ]; then\n'
            f"    echo 'uv {version}'\n"
            "    exit 0\n"
            "fi\n"
            'printf "%s\\n" "$UV_CACHE_DIR" "$UV_PYTHON_INSTALL_DIR" '
            '"$UV_PROJECT_ENVIRONMENT" "$UV_MANAGED_PYTHON" "$@"\n',
            encoding="utf-8",
        )
        binary.chmod(0o755)

    def test_offline_bootstrap_rejects_missing_uv_without_creating_cache(self):
        result = self.run_wrapper("sync", "--offline")
        self.assertEqual(result.returncode, 1)
        self.assertIn("Local uv is missing", result.stderr)
        self.assertFalse((self.root / ".cache").exists())

    def test_environment_offline_flag_rejects_missing_uv(self):
        self.env["UV_OFFLINE"] = "true"
        result = self.run_wrapper("--version")
        self.assertEqual(result.returncode, 1)
        self.assertIn("Local uv is missing", result.stderr)
        self.assertFalse((self.root / ".cache").exists())

    def test_existing_uv_must_match_pinned_version(self):
        self.install_fake_uv(version="0.0.1")
        result = self.run_wrapper("sync", "--offline")
        self.assertEqual(result.returncode, 1)
        self.assertIn("version mismatch", result.stderr)

    def test_changed_installer_is_rejected_before_execution(self):
        fake_bin = self.root / "fake-bin"
        fake_bin.mkdir()
        curl = fake_bin / "curl"
        curl.write_text(
            "#!/bin/sh\n"
            "while [ $# -gt 0 ]; do\n"
            '    if [ "$1" = --output ]; then\n'
            "        shift\n"
            '        printf \'touch "%s"\\n\' "$BOOTSTRAP_SENTINEL" > "$1"\n'
            "        exit 0\n"
            "    fi\n"
            "    shift\n"
            "done\n"
            "exit 2\n",
            encoding="utf-8",
        )
        curl.chmod(0o755)
        sentinel = self.root / "installer-was-executed"
        self.env["PATH"] = str(fake_bin) + os.pathsep + self.env["PATH"]
        self.env["BOOTSTRAP_SENTINEL"] = str(sentinel)
        result = self.run_wrapper("--version")
        self.assertEqual(result.returncode, 1)
        self.assertIn("installer checksum mismatch", result.stderr)
        self.assertFalse(sentinel.exists())
        self.assertEqual(list((self.root / ".cache").iterdir()), [])

    def test_wrapper_keeps_paths_local_and_preserves_arguments(self):
        self.install_fake_uv()
        self.env["UV_CACHE_DIR"] = "/unwanted/global/cache"
        self.env["UV_PROJECT_ENVIRONMENT"] = "/unwanted/global/environment"
        result = self.run_wrapper("run", "--offline", "argument with spaces")
        self.assertEqual(result.returncode, 0, result.stderr)
        self.assertEqual(
            result.stdout.splitlines(),
            [
                str(self.root / ".cache" / "uv"),
                str(self.root / ".cache" / "python"),
                str(self.root / ".venv"),
                "1",
                "run",
                "--offline",
                "argument with spaces",
            ],
        )


if __name__ == "__main__":
    unittest.main()
