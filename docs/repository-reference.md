# ChronoDesk Repository File Reference

This is the canonical tracked-file inventory for ChronoDesk. Every Git-tracked file must appear exactly once in the inventory below using the format checked by `scripts/check_documentation_inventory.py`.

The reference is intentionally more detailed than a directory tree: it explains why each file exists, which subsystem owns it, and what kind of change should cause it to be reviewed.

## How to use this reference

- New contributor: find a filename here before editing it to understand its responsibility.
- Reviewer: use the description to identify neighboring documentation/tests that should change with it.
- Maintainer: add, move, rename, or delete an inventory entry in the same change as the tracked file.
- CI: `scripts/check_documentation_inventory.py` compares these entries with `git ls-files` and rejects missing or stale paths.

Directories are organizational only and are not inventory entries because Git tracks files, not empty directories.

## Repository root: build, policy, product, and handoff

These files define repository-wide behavior or are conventional GitHub/open-source entry points.

- `.editorconfig` — Repository-wide text formatting and C# code-style policy; combines with warnings-as-errors/code-style-in-build so formatting/style defects can fail CI.
- `.env.example` — Documents that ChronoDesk requires no secrets and exposes only the optional `CHRONODESK_DATA_DIR` local-data override.
- `.gitattributes` — Normalizes line endings and declares known binary file types so Git does not corrupt binary assets or create avoidable cross-platform diffs.
- `.gitignore` — Excludes build/test output, IDE state, local runtime data, local environment/secrets, package output, coverage, and signing material from normal tracking.
- `CHANGELOG.md` — Release/unreleased change history; tag-time release validation requires an exact release heading before packaging.
- `CODE_OF_CONDUCT.md` — Community participation and behavior expectations for project spaces.
- `CONTRIBUTING.md` — Contributor workflow, quality expectations, commit discipline, and repository-development rules.
- `ChronoDesk.sln` — Four-project .NET solution containing Core, Infrastructure, App, and Tests with Debug/Release solution configurations.
- `Directory.Build.props` — Shared `net9.0`, nullable, analyzer, deterministic-build, warnings-as-errors, code-style, and repository metadata applied to all .NET projects.
- `Directory.Packages.props` — Central NuGet version catalog for Avalonia and test dependencies with central transitive pinning enabled.
- `LICENSE` — MIT license governing source distribution and reuse.
- `PRIVACY.md` — Canonical privacy statement covering local data, logging, external navigation, update behavior, import/export, and absence of telemetry/cloud requirements.
- `README.md` — Public product entry point: feature overview, platform support, quick start, testing/build summary, release artifact expectations, and project links.
- `ROADMAP.md` — Implementation/release-readiness roadmap separating completed source work from evidence-based native/platform release gates.
- `SECURITY.md` — Vulnerability reporting process plus secure-development, credential, external-process, dependency, release-integrity, and trust-boundary guidance.
- `SUPPORT.md` — Support routes and issue-reporting/triage guidance distinct from private security reporting.
- `global.json` — Pins the repository to the .NET 9 SDK family starting at 9.0.100 with latest-feature roll-forward and no prerelease SDK selection.
- `what_changed.md` — Primary cross-session implementation handoff recording current phase, concrete changes, verification state, limitations, and exact continuation tasks.

## GitHub community and repository automation

### Funding and contribution templates

- `.github/FUNDING.yml` — Configures the repository's GitHub Sponsors/funding surface for the project's supported funding destination.
- `.github/ISSUE_TEMPLATE/bug_report.yml` — Structured bug-report form designed to collect reproducible environment, behavior, and diagnostic context.
- `.github/ISSUE_TEMPLATE/config.yml` — Controls issue-template chooser behavior and external contact links shown by GitHub.
- `.github/ISSUE_TEMPLATE/feature_request.yml` — Structured feature-request form that asks for problem/use-case context rather than only proposed implementation.
- `.github/pull_request_template.md` — Pull-request checklist/template prompting scope, validation, documentation, security/privacy, and release-impact review.

### Dependency maintenance

- `.github/dependabot.yml` — Weekly, separately labeled NuGet and GitHub Actions dependency-update schedules using Asia/Kolkata maintenance times and scoped commit prefixes.

### Workflows

- `.github/workflows/ci.yml` — Pull-request/main CI: repository-integrity validators plus three-OS .NET 9 restore/format/Release build/test/coverage/vulnerability matrix and test artifact upload.
- `.github/workflows/codeql.yml` — C# CodeQL static-analysis workflow for main pushes/PRs plus weekly scheduled analysis with the minimum security-event write permission needed for results.
- `.github/workflows/dependency-review.yml` — PR dependency-diff gate that fails moderate-or-higher dependency risk and denies configured GPL-3.0/AGPL-3.0 license additions.
- `.github/workflows/release.yml` — Semantic-tag release pipeline: metadata/integrity/build/test preflight, four self-contained RID packages, checksums, post-download checksum verification, and least-privilege GitHub Release publication.

## Documentation hub and operational references

- `docs/README.md` — Canonical documentation index, document ownership model, source-of-truth precedence, maintenance rules, and documentation quality gates.
- `docs/accessibility.md` — Accessibility implementation notes and real-platform manual validation checklist for keyboard, focus, screen reader, contrast, scaling, and external-handler interactions.
- `docs/architecture.md` — System architecture, dependency direction, composition, persistence/platform seams, testability boundaries, runtime flow, and architectural constraints.
- `docs/ci-cd.md` — Complete CI, CodeQL, Dependency Review, Dependabot, repository-validator, release-pipeline, permission, checksum, and branch-protection reference.
- `docs/configuration-reference.md` — Deep SDK/MSBuild/package/project/environment/.editorconfig/.gitattributes/.gitignore/build-mode configuration reference.
- `docs/development.md` — Day-to-day contributor workflow, project placement rules, settings changes, platform/process/logging/test/performance conventions, and handoff discipline.
- `docs/github-maintenance.md` — GitHub-side repository maintenance guidance including branches, rules/protection, issues/PRs, Actions, dependency/security settings, labels, and releases.
- `docs/localization.md` — ResourceManager/.resx architecture, culture/format rules, XAML/resource usage, accessibility/privacy translation requirements, and translation workflow.
- `docs/performance.md` — Current performance model, timer/persistence assumptions, measurement expectations, and rules against speculative complexity.
- `docs/platform-integration.md` — Windows/macOS/Linux startup artifacts, tray, chime helpers, timezone, local paths, file pickers, external URI, packaging, and native-validation boundaries.
- `docs/release-notes-template.md` — Maintainer template for release notes including platform artifacts, checksum guidance, verification notes, known limitations, and upgrade/support sections.
- `docs/release.md` — Authoritative release procedure and release gates from clean checkout through native validation, metadata, artifacts, integrity, GitHub checks, and publication.
- `docs/repository-reference.md` — This machine-enforced canonical inventory describing the purpose of every tracked repository file.
- `docs/runtime-behavior.md` — Process lifecycle, composition, initialization, ticking, settings transaction/rollback, focus/mini/tray, chime, network, errors, and shutdown behavior.
- `docs/settings-reference.md` — Complete persistent settings schema, defaults, bounds, normalization, quiet-hour/world-clock semantics, atomic persistence, import/export, and UI mapping.
- `docs/setup.md` — Platform-oriented prerequisites, clone/build/run instructions, local data isolation, and initial environment/setup notes.
- `docs/test-catalog.md` — File-by-file automated test and test-double catalog mapping each suite file to the production contract it protects and explicit manual-test boundaries.
- `docs/testing.md` — Test strategy and quality-gate commands covering unit/integration/headless/repository checks, CI matrix, and manual validation expectations.
- `docs/troubleshooting.md` — Diagnostic guidance for startup, settings, tray, chime, timezone, import/export, UI, logging, packaging, and platform-specific failure cases.

## Architecture decision records

ADRs explain why durable design choices were made. A superseding decision should normally be a new ADR rather than silently rewriting history.

- `docs/adr/0001-modular-desktop-monolith.md` — Decision to use one desktop application split into Core/Infrastructure/App modules instead of premature service/process decomposition.
- `docs/adr/0002-json-settings-persistence.md` — Decision to use local JSON settings with explicit schema/normalization instead of a database or cloud store.
- `docs/adr/0003-system-timezone-database.md` — Decision to rely on the operating system/.NET timezone database and portable ID conversion instead of a remote timezone API.
- `docs/adr/0004-user-scoped-startup-integration.md` — Decision to implement opt-in per-user startup artifacts for Windows/macOS/Linux rather than privileged machine-wide installation hooks.
- `docs/adr/0005-redacted-local-logging.md` — Decision to use bounded local structured logging with safe messages/redaction instead of telemetry or raw exception-content collection.
- `docs/adr/0006-resource-based-localization.md` — Decision to place user-facing text in .NET resource catalogs so localization can evolve without embedding language throughout views.

## Documentation assets

- `docs/assets/screenshot-placeholder.svg` — Explicit non-production screenshot placeholder; release metadata validation intentionally blocks tagged packaging while README still presents it as the release screenshot.

## Repository integrity/release scripts

- `scripts/check_documentation_inventory.py` — Compares `git ls-files` with the canonical inventory in this document and fails on undocumented tracked files or stale documentation entries.
- `scripts/check_markdown_links.py` — Offline validator for repository-local Markdown link/image targets, missing paths, and repository-escaping local references.
- `scripts/check_release_metadata.py` — Tag-time validator for supported semantic tag syntax, matching changelog release heading, and replacement of the explicit README screenshot placeholder.
- `scripts/check_repository_secrets.py` — Defense-in-depth scanner for high-confidence committed private-key/token/credential patterns that reports location/type without echoing matched secret values.

### Script tests

- `scripts/tests/test_check_documentation_inventory.py` — Standard-library unit tests for inventory-line parsing and missing/stale/exact tracked-file comparison behavior.
- `scripts/tests/test_check_release_metadata.py` — Standard-library unit tests for stable/prerelease release metadata success and expected tag/changelog/screenshot readiness failures.

## Core project: contracts, models, and pure policy

Core intentionally avoids Avalonia, filesystem, Registry, external process, or UI dependencies.

### Core project file

- `src/ChronoDesk.Core/ChronoDesk.Core.csproj` — Minimal domain/application-logic project definition with shared repository build properties and no ChronoDesk project references.

### Abstractions

- `src/ChronoDesk.Core/Abstractions/IAppLogger.cs` — Safe logging contract used by application/infrastructure without coupling Core consumers to a file logger implementation.
- `src/ChronoDesk.Core/Abstractions/IChimePlayer.cs` — Asynchronous optional chime playback contract separating chime policy from OS audio implementation.
- `src/ChronoDesk.Core/Abstractions/ISettingsStore.cs` — Persistent settings load/save/import/export contract consumed by the view model and implemented by local JSON storage.
- `src/ChronoDesk.Core/Abstractions/IStartupManager.cs` — User-scoped startup support/enabled-state/update contract abstracting Registry/LaunchAgent/XDG details.
- `src/ChronoDesk.Core/Abstractions/ITimeZoneCatalog.cs` — Timezone enumeration/search/resolve contract hiding the system timezone database implementation.

### Models and enums

- `src/ChronoDesk.Core/Models/AppSettings.cs` — Central immutable persistent settings record plus normalization/security invariants, default values, collection limits, text bounds, enum repair, and schema version.
- `src/ChronoDesk.Core/Models/ChimeSettings.cs` — Chime enabled state, cadence enum, and nested quiet-hours configuration with safe defaults.
- `src/ChronoDesk.Core/Models/ClockFormat.cs` — Stable 12-hour/24-hour preference enum used by persistence, formatting, and Settings UI mapping.
- `src/ChronoDesk.Core/Models/ClockLayout.cs` — Stable centered/compact/dashboard layout enum consumed by main-window presentation behavior.
- `src/ChronoDesk.Core/Models/ClockSnapshot.cs` — Immutable formatted clock result carrying display strings, converted local instant, and timezone display name from Core to view models.
- `src/ChronoDesk.Core/Models/QuietHours.cs` — Quiet-hours value object and interval predicate supporting same-day/overnight ranges with inclusive start/exclusive end and equal-bounds-as-disabled semantics.
- `src/ChronoDesk.Core/Models/ThemeMode.cs` — Stable system/light/dark/high-contrast theme enum persisted by settings and interpreted by App theme composition.
- `src/ChronoDesk.Core/Models/TimeZoneDescriptor.cs` — Search/display descriptor for system timezones, including ID, display name, base UTC offset, and searchable text representation.
- `src/ChronoDesk.Core/Models/WorldClock.cs` — Persisted world-clock identity/label/timezone record plus factory that creates trimmed labels/IDs with a unique GUID-style identifier.

### Services

- `src/ChronoDesk.Core/Services/ChimePolicy.cs` — Pure decision service enforcing enabled state, quiet hours, exact cadence boundaries, and same-local-minute duplicate suppression.
- `src/ChronoDesk.Core/Services/ClockFormatter.cs` — Pure formatting service converting an instant through `TimeZoneInfo` and producing culture-aware 12/24-hour, date, weekday, ISO week, calendar detail, offset, and zone-name output.

## Infrastructure project: local persistence and OS adapters

### Infrastructure project and paths

- `src/ChronoDesk.Infrastructure/ChronoDesk.Infrastructure.csproj` — Infrastructure project definition referencing Core and inheriting central build/package policy.
- `src/ChronoDesk.Infrastructure/AppPaths.cs` — Resolves local application data/settings/log paths from `CHRONODESK_DATA_DIR`, OS ApplicationData, or executable-base fallback.
- `src/ChronoDesk.Infrastructure/Properties/AssemblyInfo.cs` — Grants the test assembly internal visibility for narrow deterministic infrastructure seams without making those types public product API.

### Logging

- `src/ChronoDesk.Infrastructure/Logging/SafeFileLogger.cs` — Bounded local JSONL logger with email/secret-pattern sanitization, safe exception-type-only error metadata, 1 MiB rotation, synchronization, and nonfatal write failures.

### Persistence

- `src/ChronoDesk.Infrastructure/Persistence/JsonSettingsStore.cs` — Production settings store: 2 MiB read/import bound, camel-case/string-enum JSON, schema checks, normalization, atomic temporary-file replacement, export/import, corrupt-file preservation, and safe logging.

### Startup/platform boundaries

- `src/ChronoDesk.Infrastructure/Platform/IStartupFileSystem.cs` — Internal minimal filesystem abstraction used solely to make startup artifact generation/deletion deterministic in tests.
- `src/ChronoDesk.Infrastructure/Platform/IStartupRegistry.cs` — Internal minimal current-user Registry abstraction used to test Windows startup behavior without touching the host Registry.
- `src/ChronoDesk.Infrastructure/Platform/PlatformStartupManager.cs` — Production `IStartupManager`: Windows HKCU Run value, macOS user LaunchAgent, Linux XDG autostart entry, quoting/escaping, enable detection, disable cleanup, cancellation, and unsupported-platform behavior.
- `src/ChronoDesk.Infrastructure/Platform/StartupPlatform.cs` — Internal startup platform enum/detector separating runtime OS detection from startup behavior so tests can inject Windows/macOS/Linux/unsupported states.
- `src/ChronoDesk.Infrastructure/Platform/SystemChimePlayer.cs` — Best-effort local sound adapter using Windows beep or fixed macOS/Linux helper paths/arguments with cancellation and graceful helper failure.
- `src/ChronoDesk.Infrastructure/Platform/SystemStartupFileSystem.cs` — Production implementation of startup filesystem operations using `System.IO` directory/file APIs.
- `src/ChronoDesk.Infrastructure/Platform/SystemStartupRegistry.cs` — Windows production adapter for reading/setting/deleting current-user Registry string values with analyzer-scoped Windows platform annotations.

### Timezone integration

- `src/ChronoDesk.Infrastructure/Time/SystemTimeZoneCatalog.cs` — Cached OS timezone catalog with ordered local search, bounded result counts, direct resolve, IANA↔Windows conversion fallback, and final UTC fallback.

## Avalonia application project

### Project/runtime composition

- `src/ChronoDesk.App/ChronoDesk.App.csproj` — Desktop executable project: development semantic version, Windows manifest/icon, Core+Infrastructure references, Avalonia/Fluent/Inter packages, Debug diagnostics, and Avalonia asset registration.
- `src/ChronoDesk.App/Program.cs` — Process entry point that configures/starts the Avalonia classic desktop lifetime.
- `src/ChronoDesk.App/App.axaml` — Application-level Avalonia resources/theme declarations and inclusion of the shared design system.
- `src/ChronoDesk.App/App.axaml.cs` — Desktop application lifecycle: main view-model/window construction, settings-driven palette/theme updates, best-effort tray creation, tray menu actions, and tray disposal.
- `src/ChronoDesk.App/AppServices.cs` — Explicit composition root constructing production logger/settings/timezone/startup/chime/domain services, with an alternate constructor for deterministic tests.
- `src/ChronoDesk.App/AppVersionProvider.cs` — Reads/normalizes assembly informational/version metadata for user-visible SemVer, removes build metadata, preserves prerelease identifiers, and provides assembly/development fallback.
- `src/ChronoDesk.App/ExternalLinkLauncher.cs` — Central external-navigation security boundary permitting only absolute HTTPS/mailto URIs before asking the OS default handler to open them.
- `src/ChronoDesk.App/Properties/AssemblyInfo.cs` — Grants test assembly internal access to selected application testability seams such as deterministic Settings operations/version/link helpers.
- `src/ChronoDesk.App/app.manifest` — Windows application manifest consumed by the App project; changes can affect Windows execution/compatibility/DPI host behavior and require native validation.

### Assets and styling

- `src/ChronoDesk.App/Assets/chronodesk-logo.svg` — Vector product logo used by repository/application presentation surfaces where scalable branding is appropriate.
- `src/ChronoDesk.App/Assets/chronodesk.ico` — Multi-resolution application/tray icon resource and Windows application icon input.
- `src/ChronoDesk.App/Styles/DesignSystem.axaml` — Shared Avalonia styles/resources for cards, typography, controls, spacing, focus-friendly UI, and consistent visual design across windows.

### Localization resources

- `src/ChronoDesk.App/Localization/Strings.cs` — Primary ResourceManager facade exposing named application strings and culture-aware format helper to XAML/code.
- `src/ChronoDesk.App/Localization/Strings.resx` — Neutral/default primary user-facing text catalog for main window, onboarding, Settings, About, tray, statuses, accessibility labels, and support/privacy messaging.
- `src/ChronoDesk.App/Localization/SettingsExtras.cs` — ResourceManager facade for the companion Settings Updates & About catalog introduced independently of the large primary resource file.
- `src/ChronoDesk.App/Localization/SettingsExtras.resx` — Neutral/default strings for Updates & About actions, version/update/privacy messaging, creator credit, and external-link failure status.

### View models

- `src/ChronoDesk.App/ViewModels/ObservableObject.cs` — Minimal `INotifyPropertyChanged` base providing reusable field-setting/property-notification behavior for presentation models.
- `src/ChronoDesk.App/ViewModels/WorldClockCardViewModel.cs` — Presentation model for one persisted world clock; resolves formatted date/time display from one supplied instant/settings snapshot.
- `src/ChronoDesk.App/ViewModels/MainWindowViewModel.cs` — Main orchestration state: initialization, clock/world-clock refresh, timezone search/add/remove, chime execution, settings transactions/startup rollback, import safety, export/reset, onboarding state, and user status text.

### Main window

- `src/ChronoDesk.App/Views/MainWindow.axaml` — Main clock/dashboard layout: header actions, primary clock, world-clock cards, timezone search/results/add action, footer status/credit, bindings, and key automation semantics.
- `src/ChronoDesk.App/Views/MainWindow.axaml.cs` — Main window lifecycle and interactions: 250 ms non-overlapping timer, initialization/onboarding/background hide, close-to-tray, focus/mini modes, size/position/topmost restoration, Settings/About/actions, timezone actions, and keyboard shortcuts.

### Settings window

- `src/ChronoDesk.App/Views/SettingsWindow.axaml` — Tabbed Settings UI covering clock/chime, appearance, accessibility, behavior, privacy/data backup/restore, Updates & About, semantic automation names, and save/cancel/status controls.
- `src/ChronoDesk.App/Views/SettingsWindow.axaml.cs` — Settings control mapping, invariant quiet-time parsing, awaitable save/reset flows, import/export native pickers, startup-aware view-model transaction calls, version display, Releases navigation, About dialog, and localized error/status handling.

### Onboarding window

- `src/ChronoDesk.App/Views/OnboardingWindow.axaml` — First-run modal content introducing local clock/world-clock/accessibility/privacy behavior and continue action through localized resources.
- `src/ChronoDesk.App/Views/OnboardingWindow.axaml.cs` — Onboarding window initialization and completion handler that persists `IsFirstRun=false` through the main view model before closing.

### About window

- `src/ChronoDesk.App/Views/AboutWindow.axaml` — About UI for product identity, semantic version, project/funding/support contacts, MIT/privacy information, and creator credit.
- `src/ChronoDesk.App/Views/AboutWindow.axaml.cs` — About initialization/version display plus fixed project/funding/mailto navigation through the centralized safe external-link launcher.

## .NET automated tests

### Project/bootstrap

- `tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj` — Non-packable xUnit/headless/coverage test project referencing all production projects and central test packages.
- `tests/ChronoDesk.Tests/AvaloniaTestSetup.cs` — Registers the production App with Avalonia's headless test platform so windows/resources can be created without a GUI session.

### Core/domain tests

- `tests/ChronoDesk.Tests/AppSettingsTests.cs` — Example-based normalization tests for visual bounds/defaults, invalid/null repair, world-clock validity/de-duplication/count, and bounded single-line imported text.
- `tests/ChronoDesk.Tests/ChimePolicyTests.cs` — Chime cadence, same-minute duplicate suppression, quarter-hour boundaries, and quiet-hour suppression tests independent of real audio.
- `tests/ChronoDesk.Tests/ClockFormatterTests.cs` — Deterministic invariant-culture tests for 12/24-hour formatting, seconds visibility, hidden fields, ISO week, calendar details, and UTC offset.
- `tests/ChronoDesk.Tests/DomainPropertyTests.cs` — Fixed-seed randomized/property-style tests for quiet-hour reference equivalence and AppSettings normalization idempotence/bounds/uniqueness.
- `tests/ChronoDesk.Tests/QuietHoursTests.cs` — Boundary tests for overnight ranges, disabled behavior, inclusive start/exclusive end, and equal start/end semantics.

### Infrastructure/persistence/platform tests

- `tests/ChronoDesk.Tests/JsonSettingsStoreTests.cs` — Real temporary-filesystem tests for normalized save/load, export/import, numeric-enum rejection, corrupt JSON default fallback, and corrupt-file preservation.
- `tests/ChronoDesk.Tests/PlatformStartupManagerTests.cs` — Isolated Windows/macOS/Linux/unsupported startup tests for command/artifact content, quoting/XML escaping, XDG fallback, enable/disable detection/cleanup, and cancellation.
- `tests/ChronoDesk.Tests/SettingsImportFuzzTests.cs` — Deterministic malformed-byte import corpus plus >2 MiB rejection, asserting malformed imports cannot mutate the primary settings file.
- `tests/ChronoDesk.Tests/SystemTimeZoneCatalogTests.cs` — Host-timezone tests for nonempty/UTC catalog, invalid-ID UTC fallback, case-insensitive search, and result bounds.

### Application/view-model/security/version tests

- `tests/ChronoDesk.Tests/AppVersionProviderTests.cs` — SemVer display normalization tests covering preview/stable/prerelease, build-metadata removal, assembly fallback, and development fallback.
- `tests/ChronoDesk.Tests/ExternalLinkLauncherTests.cs` — URI allowlist regression tests proving HTTPS/mailto acceptance and HTTP/file/script/relative/empty rejection without launching external handlers.
- `tests/ChronoDesk.Tests/MainWindowViewModelTests.cs` — Settings orchestration tests for startup rollback on persistence failure, imported-startup suppression, and exactly-once explicit startup changes.

### Avalonia headless tests

- `tests/ChronoDesk.Tests/HeadlessUiSmokeTests.cs` — Real-view headless smoke/state tests for localized windows/resources, required controls, mini/focus round trips, Updates & About controls, and preview version surfaces.
- `tests/ChronoDesk.Tests/SettingsWindowHeadlessTests.cs` — Real Settings-control interaction tests for save mapping/startup preference, invalid quiet-time no-persist behavior, and reset persistence/control reload.

### Shared fakes

- `tests/ChronoDesk.Tests/Fakes/FakeStartupFileSystem.cs` — In-memory startup filesystem that records directories/files and supports seed/write/read/delete for platform startup tests.
- `tests/ChronoDesk.Tests/Fakes/FakeStartupRegistry.cs` — In-memory current-user Registry key/value implementation used for Windows startup tests on any CI host OS.
- `tests/ChronoDesk.Tests/Fakes/MemorySettingsStore.cs` — In-memory settings store with saved/imported tracking and injectable save failure for view-model/headless transaction tests.
- `tests/ChronoDesk.Tests/Fakes/NullAppLogger.cs` — No-op logger removing unrelated disk/log effects from tests not concerned with logging.
- `tests/ChronoDesk.Tests/Fakes/NullChimePlayer.cs` — No-op chime adapter allowing clock/view-model tests to execute without audio/process side effects.
- `tests/ChronoDesk.Tests/Fakes/RecordingStartupManager.cs` — Startup fake that records requested states so tests can assert transaction order/count without OS changes.
- `tests/ChronoDesk.Tests/Fakes/UtcTimeZoneCatalog.cs` — Minimal deterministic UTC timezone catalog used where tests need stable timezones but are not testing the real system database.

## Inventory maintenance contract

The canonical line syntax is:

```text
- `relative/path/from/repository/root` — Human-readable responsibility.
```

The validator intentionally compares exact relative paths.

When a file is renamed/moved:

1. move the file;
2. replace its old inventory entry with the new path;
3. update specialized documentation links/references;
4. run `python3 scripts/check_documentation_inventory.py`;
5. run `python3 scripts/check_markdown_links.py`;
6. run the relevant tests/build gates.

When a file is deleted, delete its inventory entry in the same change. When a file is added, add its inventory entry immediately—even for assets, generated-neutral resource source files, CI templates, or small test fakes.
