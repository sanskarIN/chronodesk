# Changelog

All notable ChronoDesk changes are documented here. ChronoDesk uses four-component release versions (`MAJOR.MINOR.PATCH.REVISION`) and does not claim a published release before its verification gates are complete.

## [Unreleased — target 2.6.0.2]

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
- Windows startup command generation/matching regression coverage.
- Linux XDG autostart `Exec` quoting/escaping regression coverage.
- macOS LaunchAgent plist-generation regression coverage.
- Three-platform CI for version validation, formatting, local Markdown-link verification, build, tests, and NuGet vulnerability inspection.
- Repository-local PowerShell verifier for Markdown file/directory links.
- Repository-local PowerShell verifier for four-part application/package/assembly/file version consistency and tag matching.
- Final-audit verification record separating automated checks from native desktop release gates.
- CodeQL security analysis.
- Pull-request dependency review.
- Dependabot for NuGet and GitHub Actions.
- Cross-platform tagged release packaging workflow.
- Self-contained x64 and arm64 release ZIP targets for Windows, Linux, and macOS.
- Release ZIPs bundle license, README, changelog, privacy, security, and support documents.
- Release workflow generates `SHA256SUMS.txt` for published ZIP artifacts.
- GitHub issue forms, pull-request template, funding configuration, and repository policy documents.

### Changed

- Updated the Avalonia 11 baseline to the current 11.3.18 maintenance patch used throughout application and headless tests.
- Updated GitHub Actions workflow action majors to maintained versions and explicitly configured .NET 9 before CodeQL autobuild.
- Set `Version`, `PackageVersion`, `AssemblyVersion`, and `FileVersion` to `2.6.0.2`.
- Release tags now use four components and must exactly match the application version.
- Release packaging now targets `win-x64`, `win-arm64`, `linux-x64`, `linux-arm64`, `osx-x64`, and `osx-arm64`.
- Windows startup command generation, macOS LaunchAgent generation, and Linux desktop-entry generation are isolated from OS mutation so their serialized platform artifacts can be tested deterministically.
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
- Windows startup detection now requires the canonical ChronoDesk Run-key command instead of accepting any registry command that merely contains the current executable path as a substring.
- Linux XDG autostart executable paths now escape backslashes, quotes, dollar signs, backticks, and literal percent characters according to desktop-entry `Exec` parsing rules, while rejecting path forms that cannot be represented safely.

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
- README, roadmap, and release guidance now advertise the six x64/arm64 desktop release RIDs consistently.
- Privacy documentation explicitly covers safe import/startup behavior.
- Testing and roadmap documents are aligned with property, fuzz, headless UI, persistence I/O, version, startup-artifact, and repository-local documentation-link coverage.

## Release policy

A final `## [2.6.0.2] - YYYY-MM-DD` section will replace the target header only when the clean-checkout verification in `docs/release.md` has been completed and the corresponding `v2.6.0.2` Git tag is ready to publish.
