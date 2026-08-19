#!/usr/bin/env python3
"""Validate repository-local Markdown link targets without network access."""

from __future__ import annotations

import re
import sys
from pathlib import Path
from urllib.parse import unquote, urlsplit

ROOT = Path(__file__).resolve().parents[1]
SKIPPED_PARTS = {".git", "bin", "obj", "TestResults"}
INLINE_LINK = re.compile(r"!?\[[^\]]*\]\(([^)]+)\)")
REFERENCE_LINK = re.compile(r"^\s*\[[^\]]+\]:\s*(\S+)", re.MULTILINE)
URI_SCHEMES = {"http", "https", "mailto", "tel", "data"}


def markdown_files() -> list[Path]:
    return sorted(
        path
        for path in ROOT.rglob("*.md")
        if not any(part in SKIPPED_PARTS for part in path.parts)
    )


def normalize_destination(raw: str) -> str:
    destination = raw.strip()
    if destination.startswith("<") and ">" in destination:
        destination = destination[1 : destination.index(">")]
    elif " " in destination:
        destination = destination.split(" ", 1)[0]
    return destination.strip()


def local_target(source: Path, destination: str) -> Path | None:
    if not destination or destination.startswith("#"):
        return None

    parsed = urlsplit(destination)
    if parsed.scheme.lower() in URI_SCHEMES or parsed.netloc:
        return None

    target_text = unquote(parsed.path)
    if not target_text:
        return None

    if target_text.startswith("/"):
        target = ROOT / target_text.lstrip("/")
    else:
        target = source.parent / target_text

    return target.resolve(strict=False)


def main() -> int:
    failures: list[str] = []
    files = markdown_files()

    for source in files:
        text = source.read_text(encoding="utf-8")
        destinations = [*INLINE_LINK.findall(text), *REFERENCE_LINK.findall(text)]
        for raw in destinations:
            destination = normalize_destination(raw)
            target = local_target(source, destination)
            if target is None:
                continue

            try:
                target.relative_to(ROOT)
            except ValueError:
                failures.append(
                    f"{source.relative_to(ROOT)}: link escapes repository: {destination}"
                )
                continue

            if not target.exists():
                failures.append(
                    f"{source.relative_to(ROOT)}: missing target: {destination}"
                )

    if failures:
        print("Markdown link validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"  - {failure}", file=sys.stderr)
        return 1

    print(f"Validated local link targets across {len(files)} Markdown files.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
