# Changelog

All notable ChronoDesk changes are documented here. ChronoDesk uses four-component release versions (`MAJOR.MINOR.PATCH.REVISION`) and does not claim a published release before its verification gates are complete.

## [Unreleased — target 2.6.0.2]

### Added

- Cross-platform Avalonia architecture with a shared `ChronoDesk.App` library and thin platform hosts.
- `ChronoDesk.Desktop` host for Windows, macOS, and Linux.
- Windows release targets for x64 and arm64.
- macOS release targets for x64 and arm64.
- Linux release targets for x64 and arm64.
- `ChronoDesk.Android` host targeting `net10.0-android`.
- `ChronoDesk.iOS` host targeting `net10.0-ios` for iPhone and iPadOS.
- `ChronoDesk.Browser` host targeting `net10.0-browser` / WebAssembly.
- Responsive `MainView` single-view UI for Android, iOS/iPadOS, and browser runtimes.
- Touch-friendly clock-format/seconds controls and world-clock search/add/remove flows in the single-view shell.
- Browser `wwwroot` application shell, responsive CSS, JavaScript .NET bootstrap, and runtime configuration.
- Platform-aware CI jobs for Desktop, Android, iOS/iPadOS, and Browser workloads.
- WebAssembly static-site release ZIP generation.
- ARM64 desktop release packaging for Windows and Linux in addition to existing Apple Silicon support.
- Cross-platform version validation for desktop, Android, and Apple package metadata.
- Headless Avalonia smoke coverage for the single-view mobile/browser shell.
- Platform-aware bug report and feature request forms covering desktop, Android, iOS/iPadOS, and Browser/WebAssembly.
- Host-aware pull-request validation checklist for shared, desktop, mobile, and browser changes.
- 12/24-hour clock formats and seconds toggle.
- Date, weekday, ISO week number, and optional calendar/UTC-offset details.
- Multiple locally persisted world-clock cards with runtime timezone search.
- Full-screen desktop focus mode.
- Always-on-top desktop mini mode.
- Configurable normal desktop always-on-top behavior.
- System tray actions for Show, Focus, Mini, and Quit where supported.
- First-run desktop onboarding.
- Theme selection, high-contrast palette, reduced-motion preference, configurable typography, spacing, and clock layouts.
- Hourly, half-hourly, and quarter-hourly optional desktop chimes.
- Quiet hours with overnight-range handling.
- User-controlled startup integration for Windows, macOS, and Linux.
- Local JSON settings with atomic writes and corrupt-file preservation where filesystem semantics permit it.
- Settings import/export and defaults reset in the desktop settings workflow.
- PII/secret-pattern-redacting structured JSONL logger.
- Editable SVG logo plus application ICO asset.
- Native Avalonia vector rendering for the About-screen logo.
- About screen with project, license, support, GitHub, funding, and **Made by the Sanskar** credit.
- English-first `.resx` localization resource architecture.
- xUnit coverage for clock formatting, calendar details, quiet hours, chime cadence, settings normalization, persistence/recovery, timezone lookup, and startup-preference consistency.
- Deterministic property-style tests for quiet-hour and settings invariants.
- Deterministic malformed-import fuzz coverage and oversized-import rejection.
- Avalonia headless XUnit smoke tests for primary desktop windows and focus/mini transitions.
- Headless regression coverage for the full four-part About version.
- Repository-local PowerShell verifier for Markdown file/directory links.
- CodeQL, Dependency Review, and Dependabot automation.
- GitHub issue forms, pull-request template, funding configuration, and repository policy documents.

### Changed

- Upgraded the repository SDK/TFM baseline from .NET 9 to **.NET 10** so current Android/iOS workloads can participate in the same solution.
- `global.json` now pins the .NET 10 SDK family.
- `ChronoDesk.App` changed from the desktop executable into a reusable platform-neutral Avalonia library.
- Desktop `Program.Main` and Windows manifest moved into `ChronoDesk.Desktop`.
- `App` now selects either the classic desktop lifetime (`MainWindow`) or single-view lifetime (`MainView`).
- The application resource URI now resolves from the renamed shared `ChronoDesk.App` assembly.
- Solution registration now includes Desktop, Android, iOS/iPadOS, Browser, Core, Infrastructure, shared App, and Tests projects.
- CI no longer assumes a full-solution restore is valid on runners without mobile/browser workloads; every host is restored/built with the workload it needs.
- CodeQL now uses .NET 10 with an explicit shared/Desktop build graph instead of stale .NET 9 solution autobuild behavior.
- Release automation now publishes six desktop RID ZIPs plus a Browser/WebAssembly ZIP and SHA-256 checksums.
- Android package metadata maps canonical version `2.6.0.2` to display version `2.6.0.2` and numeric version code `2602`.
- Apple package metadata maps canonical version `2.6.0.2` to marketing version `2.6.0` and build number `2602` while the in-app version stays `2.6.0.2`.
- Updated the Avalonia baseline to 11.3.18 throughout application/host/headless packages.
- Release tags use four components and must exactly match canonical application version.
- README, setup, architecture, development, testing, release, roadmap, contribution, support, security, privacy, troubleshooting, accessibility, performance, GitHub-maintenance, release-note-template, and final-audit documentation now describe the complete cross-platform host/workload model.
- Desktop-only behavior is documented as capability-specific instead of being falsely represented as available on mobile/browser.

### Fixed

- `PlatformStartupManager` no longer assumes `Environment.ProcessPath` is always available. Mobile/browser runtimes now safely report startup integration unsupported instead of failing during service construction.
- Single-view hosts no longer need a desktop modal onboarding window; first-run state is completed through the shared view-model flow.
- Single-view timer lifecycle starts on visual-tree attachment and stops on detachment to avoid unnecessary updates after the view leaves the active tree.
- A failed first-run persistence write in `MainView` is logged without preventing the already-initialized clock timer from starting.
- Single-view format/seconds/world-clock button actions now contain persistence failures instead of leaking unhandled async event-handler exceptions.
- Settings-save failure makes a best-effort rollback when an explicit startup integration change had already been applied.
- Imported settings preserve the device's current startup preference instead of allowing a portable JSON file to silently alter operating-system startup registration.
- About-screen branding no longer relies on unsupported built-in SVG image decoding and remains visible across theme variants.
- About displays the complete four-part application version instead of truncating the revision component.
- An unreadable settings file no longer leaves clock/timezone/world-clock state uninitialized; safe defaults render with a warning instead.
- Temporary settings read failures no longer attempt to quarantine/rename a potentially valid settings file as corrupt.
- Exiting focus mode restores the pre-focus window state instead of always forcing a normal window.
- Unix chime helper processes no longer redirect unconsumed output streams.

### Security

- Production mobile signing keys/provisioning credentials are intentionally excluded from source and unsigned CI jobs.
- Browser host respects WebAssembly sandbox boundaries and does not introduce registry/process/startup assumptions.
- Security/privacy/support documentation now defines Android/iOS application-sandbox boundaries and protected signing requirements.
- Bounded imported settings documents.
- JSON settings schema-version validation.
- Numeric enum values are rejected in settings JSON; enum strings must be recognized.
- Imported font, world-clock label/ID, and timezone-ID text is bounded and normalized.
- Runtime-null nested settings values and invalid in-memory enum values normalize to safe defaults.
- Case-insensitive world-clock ID/timezone deduplication prevents inconsistent duplicate state.
- URI scheme allow-listing for About links.
- Fixed executable/argument use for Unix desktop chime helpers.
- User-scoped desktop startup integration.
- Imported backup files cannot silently change startup registration.
- Release publication rejects tags that do not match canonical project version.
- Published ZIPs receive SHA-256 checksum entries.
- No required application secrets or remote credentials.

### Documentation

- Complete README baseline updated for Windows, macOS, Linux, Android, iOS/iPadOS, and Browser/WebAssembly.
- Setup guide includes workload installation/build commands for every host.
- Architecture guide documents shared application/lifetime boundaries and thin platform hosts.
- Development guide explains host-scoped restore/build workflows and platform-specific code rules.
- Testing guide defines platform-aware CI gates and manual desktop/mobile/browser validation.
- Release guide covers six desktop RID packages, Browser static packaging, and protected mobile signing responsibilities.
- Roadmap tracks cross-platform implementation and remaining native/emulator/browser evidence separately.
- Contributing/support/troubleshooting guides use host-specific .NET 10 workflows rather than obsolete full-solution .NET 9 commands.
- Accessibility/performance guides now include touch, orientation, browser zoom, mobile lifecycle, WebAssembly main-thread work, and real-device evidence requirements.
- GitHub-maintenance guidance includes mobile/browser labels, exact CI families, .NET 10 CodeQL, protected signing environments, and cross-platform repository About metadata.
- Release-note and final-audit templates now require evidence for Desktop, Android, iOS/iPadOS, Browser, CodeQL, Dependency Review, package integrity, and signing boundaries.

## Release policy

A final `## [2.6.0.2] - YYYY-MM-DD` section will replace the target header only when the clean-checkout and cross-platform verification gates in `docs/release.md` have completed and the corresponding `v2.6.0.2` Git tag is ready to publish.
