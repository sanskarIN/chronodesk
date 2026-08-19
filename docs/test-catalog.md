# ChronoDesk Automated Test Catalog

This document maps every tracked test-support and automated test file to the production contract it protects. It complements `testing.md`, which explains how to run the suite and the overall quality strategy.

## Test project

### `tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj`

Defines the non-packable xUnit test project and references Core, Infrastructure, and App so the suite can cover pure domain logic, real local persistence, platform adapter generation, view-model orchestration, and Avalonia headless UI behavior.

Packages:

- `Avalonia.Headless.XUnit` — headless Avalonia application/tests;
- `Microsoft.NET.Test.Sdk` — test-host integration;
- `xunit` — test framework;
- `xunit.runner.visualstudio` — runner/discovery adapter, private asset;
- `coverlet.collector` — code-coverage collector, private asset.

CI runs this project through the solution on Ubuntu, Windows, and macOS.

## Headless Avalonia bootstrap

### `tests/ChronoDesk.Tests/AvaloniaTestSetup.cs`

Registers `ChronoDesk.App.App` as the Avalonia test application and configures the Avalonia headless platform.

This allows view/window construction and deterministic UI-state assertions without a real display server or desktop session.

It does **not** emulate:

- native tray/status-notifier behavior;
- native file-picker UI;
- real window-manager focus/topmost semantics;
- system screen readers;
- platform accessibility APIs;
- actual default browser/mail handlers;
- physical audio playback.

Those remain manual/platform validation boundaries.

## Domain settings and invariants

### `tests/ChronoDesk.Tests/AppSettingsTests.cs`

Protects `AppSettings.Normalize` invariants.

Coverage includes:

- non-finite clock size falling back to 96;
- content spacing clamped to the minimum;
- blank font family falling back to Inter;
- malformed world clocks removed;
- duplicate world-clock IDs removed;
- at least one Local clock retained/created;
- world-clock list limited to 24;
- invalid clock/theme/layout enum values repaired;
- runtime-null nested `Chime`/world-clock collections repaired;
- imported font/clock text bounded to documented maximum lengths;
- CR/LF/control input flattened rather than retained as multiline metadata.

When changing normalization, update this file and `settings-reference.md` together.

### `tests/ChronoDesk.Tests/DomainPropertyTests.cs`

Adds deterministic property-style coverage beyond hand-picked cases.

Current randomized loops use fixed seeds so failures are reproducible.

Coverage:

- 5,000 quiet-hours combinations compared against an independent reference predicate;
- 2,000 normalization samples covering NaN/infinities/extreme visual values and duplicate clock IDs;
- normalization idempotence (`Normalize().Normalize()` produces the same relevant state);
- normalized clock-size/spacing/count bounds;
- world-clock ID uniqueness after normalization.

This is deliberately deterministic pseudo-random testing, not a nondeterministic fuzzer.

## Quiet hours and chime policy

### `tests/ChronoDesk.Tests/QuietHoursTests.cs`

Covers explicit quiet-hour boundary semantics:

- default-style overnight range (`22:00` → `07:00`);
- start inclusive;
- end exclusive;
- before/after-midnight portions;
- disabled quiet hours always false;
- equal start/end treated as no quiet period rather than a 24-hour block.

### `tests/ChronoDesk.Tests/ChimePolicyTests.cs`

Covers product-level chime decisions independently of real sound playback:

- hourly boundary allowed when enabled;
- repeated evaluation in the same local minute suppressed;
- quarter-hour boundaries at `00`, `15`, `30`, and `45`;
- non-boundary minute rejected;
- quiet hours suppress an otherwise valid hourly chime.

Platform audio execution is intentionally not asserted here.

## Clock formatting

### `tests/ChronoDesk.Tests/ClockFormatterTests.cs`

Uses explicit UTC instants, UTC timezone, settings, and invariant culture for deterministic formatting.

Coverage:

- 24-hour time with seconds;
- 12-hour time without seconds;
- hidden date/weekday fields becoming empty;
- ISO week output;
- optional calendar details;
- UTC offset rendering.

If localization restructures the fixed Core calendar labels, update these expectations without weakening ISO/date-time correctness assertions.

## Settings persistence and import safety

### `tests/ChronoDesk.Tests/JsonSettingsStoreTests.cs`

Uses real temporary directories/files and the production `JsonSettingsStore`.

Coverage:

- normalized save/load round trip;
- portable export/import round trip;
- numeric enum values rejected by the string-enum serializer policy;
- corrupt primary JSON falls back to defaults;
- corrupt primary file is moved to a timestamped preservation filename rather than silently overwritten.

The test cleans its temporary directory in `finally` blocks.

### `tests/ChronoDesk.Tests/SettingsImportFuzzTests.cs`

Protects import robustness against untrusted local files.

Coverage:

- 100 fixed-seed random byte files (1–2,047 bytes) are fed to import;
- expected malformed-input exceptions are tolerated by the test harness;
- the primary `settings.json` is asserted byte-for-byte unchanged after every corpus member;
- files larger than the 2 MiB import limit are rejected with `InvalidDataException`.

This is a deterministic malformed-input corpus, not a claim of exhaustive fuzzing or formal parser verification.

## Timezone catalog

### `tests/ChronoDesk.Tests/SystemTimeZoneCatalogTests.cs`

Runs against the host runtime's real `TimeZoneInfo` database.

Coverage:

- system timezone collection is non-empty;
- UTC appears in the catalog;
- invalid IDs fall back to UTC;
- search is case-insensitive under current culture;
- result count respects the requested bound.

Because the host's timezone database differs across operating systems, CI's three-OS matrix provides useful portability coverage without hard-coding a platform-specific zone list.

## View-model transaction behavior

### `tests/ChronoDesk.Tests/MainWindowViewModelTests.cs`

Uses shared in-memory/fake services to isolate settings transaction semantics.

Coverage:

- enabling startup followed by persistence failure triggers startup rollback (`true` then `false`);
- failed persistence does not replace the live settings snapshot;
- imported settings cannot enable startup from an imported file;
- ordinary imported preferences such as `ShowSeconds` still apply;
- explicit local startup preference changes are applied exactly once and persisted.

This file protects the critical boundary between external OS startup configuration and durable JSON settings.

## Platform startup adapter generation

### `tests/ChronoDesk.Tests/PlatformStartupManagerTests.cs`

Uses fake registry/filesystem adapters, explicit platform values, and injected paths. It never modifies the test machine's actual startup configuration.

Windows coverage:

- enable writes quoted executable + `--background` to the current-user Run value;
- enable detection succeeds after the expected write;
- disable removes the value;
- unrelated executable value does not count as enabled.

macOS coverage:

- LaunchAgent path under the injected user profile;
- parent directory creation;
- XML escaping of `&`, `<`, `>` in executable path;
- `--background` argument included;
- disable removes an existing LaunchAgent.

Linux coverage:

- `XDG_CONFIG_HOME` honored;
- `~/.config` fallback used when XDG config home is absent;
- executable path containing a space is quoted in `Exec=`;
- expected autostart flag emitted;
- disable removes existing desktop entry.

General coverage:

- unsupported platform reports unsupported and rejects writes;
- pre-cancelled tokens cancel reads/writes.

These tests prove generated artifact/decision behavior, not that Registry/login, launchd, GNOME, KDE, or a particular distribution will execute the artifact successfully. Real-session tests remain required.

## External navigation policy

### `tests/ChronoDesk.Tests/ExternalLinkLauncherTests.cs`

Tests URI policy without launching an external process.

Allowed:

- absolute HTTPS;
- absolute mailto.

Rejected:

- HTTP;
- file URIs;
- JavaScript/script-style URIs;
- relative paths;
- empty input.

This protects About/Settings from accidentally broadening the allowed external URI surface.

## Version display

### `tests/ChronoDesk.Tests/AppVersionProviderTests.cs`

Protects user-facing semantic version normalization.

Coverage:

- preview version retained;
- stable version retained;
- `+build` metadata removed from display;
- prerelease suffix retained while build metadata is removed;
- whitespace trimmed;
- missing informational version falls back to three-part assembly version;
- missing all usable metadata falls back to `development`.

Tagged release workflow stamping is additionally enforced in `.github/workflows/release.yml`; this test file focuses on presentation normalization.

## Headless UI smoke behavior

### `tests/ChronoDesk.Tests/HeadlessUiSmokeTests.cs`

Exercises the real Avalonia views using the headless platform.

Coverage:

- Main window title resolves from localized resources;
- primary clock/search/result controls load;
- mini mode reduces to expected dimensions, forces topmost, and restores normal size;
- focus mode hides and restores application chrome;
- Settings loads primary format/theme/accessibility/startup/chime controls;
- Updates & About buttons load;
- Settings shows the development preview semantic version;
- Settings extras resource catalog loads expected neutral text;
- onboarding window loads localized title;
- About window loads localized title/version text.

This catches XAML/resource/control-name regressions while remaining independent of a real display server.

## Headless Settings interaction

### `tests/ChronoDesk.Tests/SettingsWindowHeadlessTests.cs`

Exercises the actual Settings controls and internal awaitable save/reset operations.

Save coverage:

- edits format/theme/seconds/startup/quiet-hour controls;
- persists mapped values;
- applies startup preference once;
- parses quiet-hour start/end values.

Validation coverage:

- invalid quiet-hour text returns failure;
- no settings persistence occurs;
- no startup integration call occurs;
- localized validation status is displayed.

Reset coverage:

- restores default format/theme/seconds/startup values;
- disables startup once when it was enabled;
- persists defaults;
- reloads visible controls;
- displays localized restored status.

Native import/export picker clicks are not simulated here because the OS picker itself is outside the deterministic headless boundary.

## Shared test doubles

The files under `tests/ChronoDesk.Tests/Fakes/` are reusable deterministic adapters. They avoid duplicated nested test classes and make behavior explicit.

### `Fakes/FakeStartupFileSystem.cs`

In-memory implementation of the internal startup filesystem boundary.

Responsibilities include recording created directories, storing seeded/written text files, testing file existence, reading test content, and deleting files.

Used by `PlatformStartupManagerTests` so startup generation never touches the runner's real home/config folders.

### `Fakes/FakeStartupRegistry.cs`

In-memory implementation of the internal startup Registry boundary.

Supports seeding/getting/setting/deleting current-user string values without invoking Windows Registry APIs.

Allows Windows startup behavior to be tested even from non-Windows CI hosts.

### `Fakes/MemorySettingsStore.cs`

In-memory `ISettingsStore` used by view-model/headless Settings tests.

Tracks loaded/saved/imported settings and supports deliberate save failure. It lets tests assert persistence attempts and rollback behavior without filesystem noise.

### `Fakes/NullAppLogger.cs`

No-op `IAppLogger` for tests where log persistence is not under test.

Prevents unrelated filesystem logging from changing deterministic test behavior.

### `Fakes/NullChimePlayer.cs`

No-op `IChimePlayer` for tests that may execute clock ticks but do not intend to test physical/process-based audio.

### `Fakes/RecordingStartupManager.cs`

Deterministic `IStartupManager` that records requested enabled states. Used to assert startup transaction ordering/count without touching OS configuration.

### `Fakes/UtcTimeZoneCatalog.cs`

Minimal deterministic `ITimeZoneCatalog` centered on UTC for view-model/UI tests whose purpose is not to exercise the real OS timezone database.

## Python repository-validator tests

### `scripts/tests/test_check_release_metadata.py`

Uses Python standard-library `unittest` to protect release metadata validation.

Coverage includes acceptable stable/prerelease version metadata and failure cases for invalid/missing tag/changelog/screenshot readiness conditions.

It keeps the release gate testable without third-party Python dependencies.

### `scripts/tests/test_check_documentation_inventory.py`

Protects the tracked-file documentation coverage validator.

It covers inventory parsing and missing/stale comparison behavior so the completeness gate itself does not become an untested string-processing script.

## Repository validation scripts as testable quality gates

The following are not xUnit tests but are deterministic automated validation programs run by CI:

- `scripts/check_markdown_links.py` — local Markdown link/image integrity;
- `scripts/check_documentation_inventory.py` — every tracked file documented exactly in the canonical file reference;
- `scripts/check_repository_secrets.py` — common high-confidence committed credential patterns;
- `scripts/check_release_metadata.py` — tag/changelog/screenshot release readiness.

## Coverage collected by CI

CI invokes:

```bash
dotnet test ChronoDesk.sln \
  --configuration Release \
  --no-build \
  --collect:"XPlat Code Coverage" \
  --results-directory TestResults
```

The repository currently collects coverage artifacts but does not advertise or enforce a fixed numeric percentage threshold. A high percentage alone would not prove correctness for platform GUI behavior, and a threshold should not be introduced without deciding which generated/XAML/platform code is meaningfully measurable.

## Manual validation not replaced by automated tests

The following remain explicit manual release gates:

- real Windows tray behavior;
- real macOS menu/tray behavior on x64/arm64 as available;
- real GNOME/KDE tray/status-notifier behavior;
- actual Windows Registry login launch;
- actual macOS launchd LaunchAgent execution;
- actual XDG autostart execution;
- native file-picker UX and permissions;
- real sound output/helper availability;
- packaged archive launch/executable permissions;
- keyboard-only traversal on real desktops;
- screen readers/platform accessibility APIs;
- high contrast and OS scaling/text settings;
- external default browser/mail-handler focus handoff;
- code signing/notarization behavior if introduced.

See `accessibility.md`, `platform-integration.md`, and `release.md`.

## Test-change rules

When production behavior changes:

- update the closest deterministic test first or in the same commit;
- do not replace meaningful assertions with broad “does not throw” checks;
- use explicit dates, zones, cultures, and fixed random seeds;
- avoid real network access;
- avoid writing real user startup configuration;
- use temporary directories for filesystem integration tests;
- keep test doubles narrow and reusable;
- update this catalog when a test file is added, renamed, moved, or changes responsibility;
- update `repository-reference.md` for every tracked-file change.
