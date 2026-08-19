# Changelog

All notable ChronoDesk changes are documented here. The project uses semantic-version-style release tags and aims to follow the spirit of Keep a Changelog without claiming a release before its verification gates are complete.

## [Unreleased]

### Added

- .NET 9 solution with Core, Infrastructure, Avalonia App, and test projects.
- Cross-platform Avalonia digital clock dashboard.
- 12/24-hour clock formats and seconds toggle.
- Date, weekday, ISO week number, and optional calendar/UTC-offset details.
- Multiple locally persisted world-clock cards with OS timezone search.
- Undo for the most recently removed world-clock card.
- Localized timezone-search result/empty-state feedback.
- Full-screen focus mode.
- Always-on-top mini mode.
- Configurable normal always-on-top behavior.
- System tray actions for Show, Focus, Mini, and Quit where supported.
- First-run onboarding.
- Theme selection, high-contrast palette, reduced-motion preference, configurable typography, spacing, and clock layouts.
- Hourly, half-hourly, and quarter-hourly optional chimes.
- Quiet hours with overnight-range handling.
- User-controlled startup integration for Windows, macOS, and Linux.
- Deterministic startup-registration document builders for Windows, macOS, and Linux.
- Local JSON settings with atomic writes and corrupt-file preservation.
- Explicit stepwise settings-schema migration pipeline, including compatibility for pre-versioned/schema-0 development documents.
- Settings import/export and defaults reset.
- PII/secret-pattern-redacting structured JSONL logger.
- Editable SVG logo plus application ICO asset.
- Native Avalonia vector rendering for the About-screen logo.
- About screen with project, license, support, GitHub, funding, and **Made by the Sanskar** credit.
- English-first `.resx` localization resource architecture for user-facing application strings.
- xUnit coverage for clock formatting, calendar details, quiet hours, chime cadence, settings normalization, persistence/recovery, schema migration, timezone lookup, startup registration, startup-preference consistency, world-clock capacity/undo, tray visibility policy, and URI validation.
- Deterministic property-style tests for quiet-hour and settings invariants.
- Deterministic malformed-import fuzz coverage and oversized-import rejection.
- Avalonia headless XUnit smoke tests for primary windows, world-clock feedback controls, focus/mini transitions, and mini-mode topmost restoration.
- Three-platform CI for documentation links, tracked-file secret patterns, formatting, build, tests, and NuGet vulnerability inspection.
- High-signal tracked-file secret verification that intentionally does not print matched secret values.
- Deterministic local Markdown-link verification for repository documentation.
- CodeQL security analysis.
- Pull-request dependency review.
- Dependabot for NuGet and GitHub Actions.
- Cross-platform tagged release packaging workflow.
- SHA-256 sidecar files for release archives plus a verified release integrity manifest.
- GitHub issue forms, pull-request template, funding configuration, and repository policy documents.

### Changed

- Updated the Avalonia 11 baseline to the current 11.3.18 maintenance patch used throughout application and headless tests.
- Updated GitHub Actions workflow action majors to maintained versions and explicitly configured .NET 9 before CodeQL autobuild.
- Defined preview assembly/package metadata as `0.1.0-preview` while release verification remains incomplete.
- Startup integration now compares the generated registration with the expected current executable registration rather than treating any same-path registration file as enabled.
- macOS/Linux startup registration writes use temporary files followed by atomic replacement.
- External support/project/funding destinations are centralized behind a reusable `https`/`mailto` launcher policy.
- Settings reads determine the source schema from JSON before deserialization and migrate older supported schemas before normalization.
- World-clock capacity now comes from one domain constant used by normalization, add behavior, localized feedback, and tests.
- Minimize-to-tray/background hiding now requires reliable tray menu restoration to be available for the current desktop session.
- CodeQL and dependency-review workflows now cancel superseded runs for the same ref to reduce queue buildup during granular development.

### Fixed

- Settings-save failure now makes a best-effort rollback when an explicit startup integration change had already been applied.
- Imported settings preserve the device's current startup preference instead of allowing a portable JSON file to enable or disable operating-system startup registration.
- About-screen branding no longer relies on unsupported built-in SVG image decoding and remains visible across theme variants.
- World-clock removal now preserves enough state to restore the removed card at its prior dashboard position.
- Adding a world clock at the 24-card capacity no longer reports success for an entry that normalization would discard; the add is rejected before persistence with explicit feedback.
- Leaving mini mode now restores the current saved always-on-top preference instead of a stale pre-mini value.
- Closing or background-starting ChronoDesk can no longer hide the only window when reliable tray restoration is unavailable.
- Pre-versioned settings documents no longer rely on a C# property initializer to appear current; missing `schemaVersion` is explicitly treated as legacy schema `0` and migrated to the current schema.

### Security

- Bounded imported settings documents.
- JSON settings schema-version validation.
- Negative and future settings schema versions are rejected rather than guessed or silently reinterpreted.
- Numeric enum values are rejected in settings JSON; enum strings must be recognized by the serializer.
- Imported font, world-clock label/ID, and timezone-ID text is length-bounded and normalized to single-line values.
- Runtime-null nested settings values and invalid in-memory enum values are normalized to safe defaults.
- URI scheme allow-listing for external application links; `http`, local-file, credential-bearing HTTPS, script, and relative destinations are rejected.
- Fixed executable/argument use for Unix system chime helpers.
- Startup executable paths reject control characters; Windows Run-command paths also reject embedded quote characters.
- User-scoped startup integration.
- Imported backup files cannot silently change startup registration.
- CI scans tracked text files for high-signal private-key/token patterns without echoing matched values.
- No required application secrets or remote credentials.

### Documentation

- Complete README baseline.
- Contribution, support, security, privacy, code-of-conduct, roadmap, architecture, setup, development, testing, release, troubleshooting, accessibility, performance, GitHub-maintenance, release-note-template, and ADR documentation.
- Privacy documentation explicitly covers safe import/startup behavior.
- Testing and roadmap documents are aligned with property, fuzz, headless UI, startup-registration, migration, tray-safety, and repository-verification coverage.
- Release documentation includes ZIP checksum and integrity-manifest verification on PowerShell, Linux, and macOS.
- ADR 0007 records explicit stepwise settings-schema migration behavior and test requirements for future schema changes.

## Release policy

A versioned section will be added only when the clean-checkout verification in `docs/release.md` has been completed and a corresponding Git tag is ready to publish.
