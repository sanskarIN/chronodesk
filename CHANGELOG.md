# Changelog

All notable ChronoDesk changes are documented here. The project uses semantic-version-style release tags and aims to follow the spirit of Keep a Changelog without claiming a release before its verification gates are complete.

## [Unreleased]

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
- Deterministic startup-adapter tests using isolated filesystem/registry boundaries for Windows, macOS, Linux, unsupported-platform, cleanup, escaping, and cancellation behavior.
- Deterministic property-style tests for quiet-hour and settings invariants.
- Deterministic malformed-import fuzz coverage and oversized-import rejection.
- Avalonia headless XUnit smoke tests for primary windows and focus/mini transitions.
- Headless Settings-window interaction coverage for save mapping, quiet-hour validation, startup preference changes, and reset-to-defaults behavior.
- Repository-local Markdown link validation in CI.
- High-confidence committed-credential pattern scanning in CI.
- Three-platform CI for formatting, build, tests, and NuGet vulnerability inspection.
- CodeQL security analysis.
- Pull-request dependency review.
- Dependabot for NuGet and GitHub Actions.
- Cross-platform tagged release packaging workflow.
- GitHub issue forms, pull-request template, funding configuration, and repository policy documents.

### Changed

- Updated the Avalonia 11 baseline to the current 11.3.18 maintenance patch used throughout application and headless tests.
- Updated GitHub Actions workflow action majors to maintained versions and explicitly configured .NET 9 before CodeQL autobuild.
- Defined preview assembly/package metadata as `0.1.0-preview` while release verification remains incomplete.
- Refactored platform startup integration behind injectable platform, filesystem, and registry boundaries so generated startup artifacts can be verified without altering the test runner's real login configuration.
- Refactored Settings save/reset event flows into deterministic async operations that remain wired to the same UI handlers and can be exercised headlessly.
- Consolidated reusable settings/startup/timezone/chime/logger test doubles under the test project.

### Fixed

- Settings-save failure now makes a best-effort rollback when an explicit startup integration change had already been applied.
- Imported settings preserve the device's current startup preference instead of allowing a portable JSON file to enable or disable operating-system startup registration.
- About-screen branding no longer relies on unsupported built-in SVG image decoding and remains visible across theme variants.

### Security

- Bounded imported settings documents.
- JSON settings schema-version validation.
- Numeric enum values are rejected in settings JSON; enum strings must be recognized by the serializer.
- Imported font, world-clock label/ID, and timezone-ID text is length-bounded and normalized to single-line values.
- Runtime-null nested settings values and invalid in-memory enum values are normalized to safe defaults.
- URI scheme allow-listing for About links.
- Fixed executable/argument use for Unix system chime helpers.
- User-scoped startup integration.
- Imported backup files cannot silently change startup registration.
- No required application secrets or remote credentials.
- CI scans repository text for high-confidence private-key and common credential/token patterns without printing matched values.

### Documentation

- Complete README baseline.
- Contribution, support, security, privacy, code-of-conduct, roadmap, architecture, setup, development, testing, release, troubleshooting, accessibility, performance, GitHub-maintenance, release-note-template, and ADR documentation.
- Privacy documentation explicitly covers safe import/startup behavior.
- Testing and roadmap documents are aligned with property, fuzz, startup-adapter, and headless UI coverage.
- Release/security guidance distinguishes automated repository-integrity checks from required human review and native-desktop release validation.

## Release policy

A versioned section will be added only when the clean-checkout verification in `docs/release.md` has been completed and a corresponding Git tag is ready to publish.
