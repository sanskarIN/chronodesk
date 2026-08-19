#!/usr/bin/env python3
"""Verify that the canonical repository reference documents every tracked file."""

from __future__ import annotations

import re
import subprocess
import sys
from pathlib import Path
from typing import Iterable

ROOT = Path(__file__).resolve().parents[1]
REFERENCE_PATH = ROOT / "docs" / "repository-reference.md"
INVENTORY_PATTERN = re.compile(r"^- `([^`]+)` — ", re.MULTILINE)


def read_tracked_files(root: Path = ROOT) -> set[str]:
    """Return Git's authoritative tracked-file set for the repository."""
    completed = subprocess.run(
        ["git", "ls-files", "-z"],
        cwd=root,
        check=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
    )
    return {
        item.decode("utf-8")
        for item in completed.stdout.split(b"\0")
        if item
    }


def parse_documented_files(text: str) -> set[str]:
    """Parse canonical inventory entries from repository-reference.md."""
    return set(INVENTORY_PATTERN.findall(text))


def compare_inventory(
    tracked_files: Iterable[str],
    documented_files: Iterable[str],
) -> tuple[list[str], list[str]]:
    """Return sorted missing and stale documentation inventory entries."""
    tracked = set(tracked_files)
    documented = set(documented_files)
    return sorted(tracked - documented), sorted(documented - tracked)


def main() -> int:
    if not REFERENCE_PATH.is_file():
        print(f"Documentation inventory is missing: {REFERENCE_PATH.relative_to(ROOT)}")
        return 1

    try:
        tracked = read_tracked_files()
    except (OSError, subprocess.CalledProcessError) as exception:
        print(f"Could not read tracked files from Git: {exception}")
        return 1

    documented = parse_documented_files(REFERENCE_PATH.read_text(encoding="utf-8"))
    missing, stale = compare_inventory(tracked, documented)

    if missing:
        print("Tracked files missing from docs/repository-reference.md:")
        for path in missing:
            print(f"  - {path}")

    if stale:
        print("Documentation inventory entries that are not tracked files:")
        for path in stale:
            print(f"  - {path}")

    if missing or stale:
        print(
            "Update docs/repository-reference.md so each tracked file has exactly "
            "one canonical '- `path` — description' inventory entry."
        )
        return 1

    print(f"Documentation inventory covers all {len(tracked)} tracked files.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
