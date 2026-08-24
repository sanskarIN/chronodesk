# Changelog

All notable ChronoDesk changes are documented here. ChronoDesk uses four-component release versions (`MAJOR.MINOR.PATCH.REVISION`) and does not claim a published release before its verification gates are complete.

## [Unreleased — target 2.7.0.0]

### Added

- Inline editable labels for saved world-clock cards while preserving each clock's ID and timezone identity.
- Keyboard world-clock label editing: Enter saves and Escape restores the persisted label.
- Regression coverage for world-clock rename persistence, normalization, identity preservation, normalized status text, and blank-label rejection.
- `docs/next-version.md` as the dedicated next-version development and validation handoff.

### Changed

- Set next-version branch `Version`, `PackageVersion`, `AssemblyVersion`, and `FileVersion` metadata to `2.7.0.0`.
- World-clock rename status now uses the normalized persisted label rather than raw editor input.
- World-clock cards separate label editing and action controls into responsive rows with bounded card widths for narrower desktop layouts.
- Enter key handling is marked complete before asynchronous label persistence begins so the edit keystroke does not bubble during the save.
- The About headless smoke test derives the expected four-part version from the application assembly instead of embedding a release-specific version literal.

### Fixed

- Blank world-clock label submissions no longer leave an empty editor value; the persisted label is restored without a settings write.
- A version bump no longer makes the About smoke test fail solely because a previous release version was hardcoded in the assertion.

### Validation

- Development is isolated on `next-version-2.7.0.0` in draft PR #21.
- PR #21 must remain unmerged until the `2.6.0.2` release/tag decision is complete.
- Local build/test success is not claimed from the chat environment; CI, CodeQL, and Dependency Review remain authoritative automated evidence for the branch head.

## [Unreleased release candidate — target 2.6.0.2]

### Added

- .NET 9 solution with Core, Infrastructure, Avalonia App, and test projects.
- Cross-platform Avalonia digital clock dashboard.
- 12/24-hour clock formats and seconds toggle.
- Date, weekday, ISO week number, and optional calendar/UTC-offset details.
- Multiple locally persisted world-clock cards with OS timezone search.
- Full-screen focus mode.
- Always-on-top mini mode.
- Configurable normal always-on-top behavior.
- System tray actions for Show, Focus, Mini, and Quit where supported.
- First-run onboarding.
- Theme selection, high-contrast palette, reduced-motion preference, configurable typography, spacing, and clock layouts.
- Hourly, half-hourly, and quarter-hourly optional chimes.
- Quiet hours with overnight-range handling.
- User-controlled startup integration for Windows, macOS, and Linux.
- Local JSON settings with atomic writes and corrupt-file preservation.
- Settings import/export and defaults reset.
- PII/secret-pattern-redacting structured JSONL logger.
- Editable SVG logo plus application ICO asset.
- Native Avalonia vector rendering for the About-screen logo.
- About screen with project, license, support, GitHub, funding, and **Made by the Sanskar** credit.
- English-first `.resx` localization resource architecture for user-facing application strings.
- xUnit coverage for clock formatting, calendar details, quiet hours, chime cadence, settings normalization, persistence/recovery, timezone lookup, and startup-preference consistency.
- Deterministic property-style tests for quiet-hour and settings invariants.
- Deterministic malformed-import fuzz coverage and oversized-import rejection.
- Avalonia headless XUnit smoke tests for primary windows and focus/mini transitions.
- Headless regression coverage for the full four-part About version.
- Three-platform CI for version validation, formatting, local Markdown-link verification, build, tests, and NuGet vulnerability inspection.
- Repository-local PowerShell verifier for Markdown file/directory links.
- Repository-local PowerShell verifier for four-part application/package/assembly/file version consistency and tag matching.
- Final-audit verification record separating automated checks from native desktop release gates.
- CodeQL security analysis.
- Pull-request dependency review.
- Dependabot for NuGet and GitHub Actions.
- Cross-platform tagged release packaging workflow.
- Release ZIPs bundle license, README, changelog, privacy, security, and support documents.
- Release workflow generates `SHA256SUMS.txt` for published ZIP artifacts.
- GitHub issue forms, pull-request template, funding configuration, and repository policy documents.

### Changed

- Updated the Avalonia 11 baseline to the current 11.3.18 maintenance patch used throughout application and headless tests.
- Updated GitHub Actions workflow action majors to maintained versions and explicitly configured .NET 9 before CodeQL autobuild.
- Set `Version`, `PackageVersion`, `AssemblyVersion`, and `FileVersion` to `2.6.0.2`.
- Release tags now use four components and must exactly match the application version.
- Imported world clocks now use case-insensitive ID and timezone-ID uniqueness, matching interactive world-clock behavior.

### Fixed

- Settings-save failure now makes a best-effort rollback when an explicit startup integration change had already been applied.
- Imported settings preserve the device's current startup preference instead of allowing a portable JSON file to enable or disable operating-system startup registration.
- About-screen branding no longer relies on unsupported built-in SVG image decoding and remains visible across theme variants.
- About now displays the complete four-part application version instead of truncating the revision component.
- An unreadable settings file no longer leaves the clock, timezone search, or world-clock collection uninitialized; safe defaults are rendered with a warning instead.
- Temporary settings read failures no longer attempt to quarantine/rename a potentially valid settings file as corrupt.
- Exiting focus mode now restores the pre-focus window state instead of always forcing a normal window.
- Unix chime helper processes no longer redirect unconsumed output streams, removing an avoidable pipe-stall risk.

### Security

- Bounded imported settings documents.
- JSON settings schema-version validation.
- Numeric enum values are rejected in settings JSON; enum strings must be recognized by the serializer.
- Imported font, world-clock label/ID, and timezone-ID text is length-bounded and normalized to single-line values.
- Runtime-null nested settings values and invalid in-memory enum values are normalized to safe defaults.
- Case-insensitive world-clock ID/timezone deduplication prevents inconsistent duplicate state from portable settings files.
- URI scheme allow-listing for About links.
- Fixed executable/argument use for Unix system chime helpers.
- User-scoped startup integration.
- Imported backup files cannot silently change startup registration.
- Release publication rejects tags that do not match the canonical project version.
- Published ZIPs receive deterministic SHA-256 checksum entries for integrity verification.
- No required application secrets or remote credentials.

### Documentation

- Complete README baseline.
- Contribution, support, security, privacy, code-of-conduct, roadmap, architecture, setup, development, testing, release, troubleshooting, accessibility, performance, GitHub-maintenance, release-note-template, final-audit, and ADR documentation.
- Release documentation now uses `2.6.0.2` and the four-component version/tag convention consistently.
- Privacy documentation explicitly covers safe import/startup behavior.
- Testing and roadmap documents are aligned with property, fuzz, headless UI, persistence I/O, version, and repository-local documentation-link coverage.

## Release policy

The `2.6.0.2` release candidate remains gated by the clean-checkout and native release verification in `docs/release.md`; its final `## [2.6.0.2] - YYYY-MM-DD` section and `v2.6.0.2` tag must not be created until those gates pass. The `2.7.0.0` development line is intentionally isolated from `main` until that release decision is complete.
