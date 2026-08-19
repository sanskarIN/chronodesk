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
- Settings Updates & About section with current version display, user-initiated GitHub Releases access, and access to the full About dialog without background update traffic.
- PII/secret-pattern-redacting structured JSONL logger.
- Editable SVG logo plus application ICO asset.
- Native Avalonia vector rendering for the About-screen logo.
- About screen with project, license, support, GitHub, funding, and **Made by the Sanskar** credit.
- English-first `.resx` localization resource architecture for user-facing application strings, including a companion resource catalog for the Settings update/About surface.
- xUnit coverage for clock formatting, calendar details, quiet hours, chime cadence, settings normalization, persistence/recovery, timezone lookup, and startup-preference consistency.
- Deterministic startup-adapter tests using isolated filesystem/registry boundaries for Windows, macOS, Linux, unsupported-platform, cleanup, escaping, and cancellation behavior.
- Deterministic property-style tests for quiet-hour and settings invariants.
- Deterministic malformed-import fuzz coverage and oversized-import rejection.
- Avalonia headless XUnit smoke tests for primary windows and focus/mini transitions.
- Headless Settings-window interaction coverage for save mapping, quiet-hour validation, startup preference changes, reset-to-defaults behavior, and the Updates/About surface.
- Release/version-display regression tests for preview, stable, prerelease, build-metadata, and fallback version forms.
- External-link allowlist regression tests covering HTTPS/mailto acceptance and rejection of HTTP, file, script, relative, and empty targets.
- Repository-local Markdown link validation in CI.
- High-confidence committed-credential pattern scanning in CI.
- Machine-enforced tracked-file documentation inventory using `git ls-files`, with standard-library regression tests for inventory parsing and missing/stale path detection.
- Three-platform CI for formatting, build, tests, and NuGet vulnerability inspection.
- CodeQL security analysis.
- Pull-request dependency review.
- Dependabot for NuGet and GitHub Actions.
- Cross-platform tagged release packaging workflow with tag-derived version stamping, release preflight, platform-appropriate archives, SHA-256 sidecars, checksum verification, and prerelease publication support.
- GitHub issue forms, pull-request template, funding configuration, and repository policy documents.
- Canonical documentation hub plus deep runtime-behavior, settings-schema, build/configuration, platform-integration, localization, CI/CD, automated-test, and exhaustive tracked-file references.

### Changed

- Updated the Avalonia 11 baseline to the current 11.3.18 maintenance patch used throughout application and headless tests.
- Updated GitHub Actions workflow action majors to maintained versions and explicitly configured .NET 9 before CodeQL autobuild.
- Defined preview assembly/package metadata as `0.1.0-preview` while release verification remains incomplete.
- About and Settings version labels now derive their display value from informational/SemVer assembly metadata and strip build metadata from the user-facing version.
- Tagged release builds override preview metadata from the semantic Git tag so published binaries cannot silently retain the development version.
- Linux/macOS release artifacts use `tar.gz` instead of ZIP so executable permission bits are retained; Windows remains ZIP.
- Release workflow write permission is scoped to the final GitHub release publication job.
- Refactored platform startup integration behind injectable platform, filesystem, and registry boundaries so generated startup artifacts can be verified without altering the test runner's real login configuration.
- Refactored Settings save/reset event flows into deterministic async operations that remain wired to the same UI handlers and can be exercised headlessly.
- Consolidated reusable settings/startup/timezone/chime/logger test doubles under the test project.
- Centralized application external-link launching behind a HTTPS/mailto allowlist shared by About and Settings.
- Added explicit automation names to Settings controls whose visual labels were previously adjacent text only, improving screen-reader semantics.
- CI Repository integrity and tag-time Release preflight now reject incomplete/stale `docs/repository-reference.md` coverage for any tracked file.
- README, contributor workflow, development guide, testing guide, and release procedure now link and enforce the canonical deep-documentation set.

### Fixed

- Settings-save failure now makes a best-effort rollback when an explicit startup integration change had already been applied.
- Imported settings preserve the device's current startup preference instead of allowing a portable JSON file to enable or disable operating-system startup registration.
- About-screen branding no longer relies on unsupported built-in SVG image decoding and remains visible across theme variants.
- Tagged release binaries no longer depend on the repository's fixed preview version for their user-facing release identity.
- Unix release archives no longer risk dropping the executable permission bit through ZIP packaging.
- Documentation inventory parsing ignores fenced syntax examples so explanatory code blocks cannot be mistaken for tracked-file entries.

### Security

- Bounded imported settings documents.
- JSON settings schema-version validation.
- Numeric enum values are rejected in settings JSON; enum strings must be recognized by the serializer.
- Imported font, world-clock label/ID, and timezone-ID text is length-bounded and normalized to single-line values.
- Runtime-null nested settings values and invalid in-memory enum values are normalized to safe defaults.
- URI scheme allow-listing is centralized for About and Settings external links; only HTTPS and mailto destinations are accepted.
- Fixed executable/argument use for Unix system chime helpers.
- User-scoped startup integration.
- Imported backup files cannot silently change startup registration.
- No required application secrets or remote credentials.
- CI scans repository text for high-confidence private-key and common credential/token patterns without printing matched values.
- Release publication verifies four archive/checksum pairs before creating the GitHub Release.

### Documentation

- Complete README baseline.
- Contribution, support, security, privacy, code-of-conduct, roadmap, architecture, setup, development, testing, release, troubleshooting, accessibility, performance, GitHub-maintenance, release-note-template, and ADR documentation.
- Added `docs/README.md` as the canonical technical documentation navigation/source-of-truth guide.
- Added `docs/runtime-behavior.md` covering process startup, composition, clock tick, world clocks, settings transactions/rollback, import/export, focus/mini/tray, themes, chimes, error containment, network behavior, and shutdown.
- Added `docs/settings-reference.md` documenting every persistent setting, default, enum, bound, normalization rule, quiet-hours/world-clock behavior, schema/import/export, and atomic persistence rule.
- Added `docs/configuration-reference.md` documenting SDK/MSBuild/package/project/environment/editor/Git configuration and build-mode/version behavior.
- Added `docs/platform-integration.md` documenting Windows Registry, macOS LaunchAgent, Linux XDG autostart, tray, sound, timezone, local-path, file-picker, external-handler, and release-RID boundaries.
- Added `docs/localization.md` documenting ResourceManager/.resx structure, culture rules, XAML usage, accessibility/security translation requirements, and future translation workflow.
- Added `docs/ci-cd.md` documenting CI, CodeQL, Dependency Review, Dependabot, repository validators, release preflight/package/checksum/publication, and permissions.
- Added `docs/test-catalog.md` mapping every xUnit/headless test, shared fake, and Python validator test to its production contract and manual-test boundary.
- Added `docs/repository-reference.md` with a canonical responsibility entry for every tracked repository file; completeness is enforced automatically.
- Privacy documentation explicitly covers safe import/startup behavior.
- Testing and roadmap documents are aligned with property, fuzz, startup-adapter, headless UI, repository-integrity, and documentation-inventory coverage.
- Release/security guidance distinguishes automated repository-integrity checks from required human review and native-desktop release validation.
- Release guidance documents semantic-tag stamping, prerelease behavior, Windows ZIP/Unix tarball formats, checksum sidecars, post-download verification, and tracked-file documentation completeness.

## Release policy

A versioned section will be added only when the clean-checkout verification in `docs/release.md` has been completed and a corresponding Git tag is ready to publish.
