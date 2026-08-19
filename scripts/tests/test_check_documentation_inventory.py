import sys
import unittest
from pathlib import Path

SCRIPTS_DIR = Path(__file__).resolve().parents[1]
if str(SCRIPTS_DIR) not in sys.path:
    sys.path.insert(0, str(SCRIPTS_DIR))

import check_documentation_inventory as inventory  # noqa: E402


class DocumentationInventoryTests(unittest.TestCase):
    def test_parse_documented_files_reads_only_canonical_inventory_lines(self) -> None:
        text = """
# Example

- `README.md` — Product overview.
- `docs/setup.md` — Setup guide.

Inline `src/App.cs` is not an inventory entry.
- `missing-description.md`
"""

        self.assertEqual(
            {"README.md", "docs/setup.md"},
            inventory.parse_documented_files(text),
        )

    def test_compare_inventory_reports_missing_and_stale_paths(self) -> None:
        missing, stale = inventory.compare_inventory(
            {"README.md", "src/App.cs", "docs/new file.md"},
            {"README.md", "old/deleted.cs"},
        )

        self.assertEqual(["docs/new file.md", "src/App.cs"], missing)
        self.assertEqual(["old/deleted.cs"], stale)

    def test_compare_inventory_accepts_exact_match(self) -> None:
        tracked = {"README.md", "scripts/check.py"}

        missing, stale = inventory.compare_inventory(tracked, tracked)

        self.assertEqual([], missing)
        self.assertEqual([], stale)


if __name__ == "__main__":
    unittest.main()
