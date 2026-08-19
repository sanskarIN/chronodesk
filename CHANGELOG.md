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
- About screen with project, license, support, GitHub, funding, and **Made by the Sanskar** credit.
- xUnit coverage for clock formatting, calendar details, quiet hours, chime cadence, settings normalization, persistence/recovery, and timezone lookup.
- Three-platform CI for formatting, build, tests, and NuGet vulnerability inspection.
- CodeQL security analysis.
- Pull-request dependency review.
- Dependabot for NuGet and GitHub Actions.
- Cross-platform tagged release packaging workflow.
- GitHub issue forms, pull-request template, funding configuration, and repository policy documents.

### Security

- Bounded imported settings documents.
- JSON settings schema-version validation.
- URI scheme allow-listing for About links.
- Fixed executable/argument use for Unix system chime helpers.
- User-scoped startup integration.
- No required application secrets or remote credentials.

### Documentation

- Complete README baseline.
- Contribution, support, security, privacy, code-of-conduct, roadmap, architecture, setup, development, testing, release, troubleshooting, accessibility, performance, and ADR documentation.

## Release policy

A versioned section will be added only when the clean-checkout verification in `docs/release.md` has been completed and a corresponding Git tag is ready to publish.
