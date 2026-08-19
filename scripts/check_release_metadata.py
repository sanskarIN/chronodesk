#!/usr/bin/env python3
"""Validate release-only repository metadata for a semantic version tag."""

from __future__ import annotations

import argparse
import re
import sys
from dataclasses import dataclass
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
SEMVER_TAG = re.compile(
    r"^v(?P<version>(?:0|[1-9][0-9]*)\."
    r"(?:0|[1-9][0-9]*)\."
    r"(?:0|[1-9][0-9]*)"
    r"(?:-[0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*)?)$"
)
SCREENSHOT_PLACEHOLDER_MARKERS = (
    "docs/assets/screenshot-placeholder.svg",
    "ChronoDesk screenshot placeholder",
)


@dataclass(frozen=True)
class ReleaseMetadataResult:
    version: str | None
    errors: tuple[str, ...]

    @property
    def is_valid(self) -> bool:
        return not self.errors


def validate_release_metadata(tag: str, changelog: str, readme: str) -> ReleaseMetadataResult:
    match = SEMVER_TAG.fullmatch(tag.strip())
    if match is None:
        return ReleaseMetadataResult(
            version=None,
            errors=(f"unsupported semantic release tag: {tag}",),
        )

    version = match.group("version")
    errors: list[str] = []

    version_heading = re.compile(
        rf"^##\s+\[{re.escape(version)}\](?:\s+-\s+.+)?\s*$",
        re.MULTILINE,
    )
    if version_heading.search(changelog) is None:
        errors.append(
            f"CHANGELOG.md must contain a versioned '## [{version}]' release heading before tag {tag} is published"
        )

    for marker in SCREENSHOT_PLACEHOLDER_MARKERS:
        if marker in readme:
            errors.append(
                "README.md still contains the release screenshot placeholder; replace it with a verified release-build capture before tagging"
            )
            break

    return ReleaseMetadataResult(version=version, errors=tuple(errors))


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--tag", required=True, help="Git release tag, for example v0.1.0-rc.1")
    parser.add_argument(
        "--root",
        type=Path,
        default=ROOT,
        help="Repository root (defaults to the parent of this scripts directory)",
    )
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    root = args.root.resolve()

    try:
        changelog = (root / "CHANGELOG.md").read_text(encoding="utf-8")
        readme = (root / "README.md").read_text(encoding="utf-8")
    except OSError as exception:
        print(f"Release metadata validation could not read repository files: {exception}", file=sys.stderr)
        return 1

    result = validate_release_metadata(args.tag, changelog, readme)
    if not result.is_valid:
        print("Release metadata validation failed:", file=sys.stderr)
        for error in result.errors:
            print(f"  - {error}", file=sys.stderr)
        return 1

    print(f"Release metadata is ready for {args.tag} ({result.version}).")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
