from __future__ import annotations

import importlib.util
import sys
import unittest
from pathlib import Path

SCRIPT_PATH = Path(__file__).resolve().parents[1] / "check_release_metadata.py"
SPEC = importlib.util.spec_from_file_location("check_release_metadata", SCRIPT_PATH)
if SPEC is None or SPEC.loader is None:
    raise RuntimeError("Unable to load check_release_metadata.py for tests.")
MODULE = importlib.util.module_from_spec(SPEC)
sys.modules[SPEC.name] = MODULE
SPEC.loader.exec_module(MODULE)


class ReleaseMetadataValidatorTests(unittest.TestCase):
    def test_accepts_stable_release_with_versioned_changelog_and_real_screenshot(self) -> None:
        result = MODULE.validate_release_metadata(
            "v1.2.3",
            "# Changelog\n\n## [1.2.3] - 2026-08-19\n\n- Ready.\n",
            "# ChronoDesk\n\n![ChronoDesk running on Windows](docs/assets/chronodesk-windows.png)\n",
        )

        self.assertTrue(result.is_valid)
        self.assertEqual("1.2.3", result.version)

    def test_accepts_prerelease_heading(self) -> None:
        result = MODULE.validate_release_metadata(
            "v0.1.0-rc.1",
            "# Changelog\n\n## [0.1.0-rc.1]\n\n- Candidate.\n",
            "# ChronoDesk\n\n![Release candidate](docs/assets/chronodesk-rc.png)\n",
        )

        self.assertTrue(result.is_valid)
        self.assertEqual("0.1.0-rc.1", result.version)

    def test_rejects_unversioned_changelog(self) -> None:
        result = MODULE.validate_release_metadata(
            "v1.0.0",
            "# Changelog\n\n## [Unreleased]\n\n- Work in progress.\n",
            "# ChronoDesk\n\n![Real capture](docs/assets/chronodesk.png)\n",
        )

        self.assertFalse(result.is_valid)
        self.assertTrue(any("CHANGELOG.md" in error for error in result.errors))

    def test_rejects_placeholder_screenshot(self) -> None:
        result = MODULE.validate_release_metadata(
            "v1.0.0",
            "# Changelog\n\n## [1.0.0]\n\n- Ready.\n",
            "# ChronoDesk\n\n![ChronoDesk screenshot placeholder](docs/assets/screenshot-placeholder.svg)\n",
        )

        self.assertFalse(result.is_valid)
        self.assertTrue(any("placeholder" in error.lower() for error in result.errors))

    def test_rejects_non_semantic_tag(self) -> None:
        result = MODULE.validate_release_metadata(
            "release-1",
            "# Changelog\n\n## [1.0.0]\n",
            "# ChronoDesk\n",
        )

        self.assertFalse(result.is_valid)
        self.assertIsNone(result.version)
        self.assertTrue(any("semantic" in error.lower() for error in result.errors))


if __name__ == "__main__":
    unittest.main()
