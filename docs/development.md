# ChronoDesk Development Guide

## Daily workflow

ChronoDesk's solution contains desktop, Android, iOS/iPadOS, and Browser projects. Do **not** use a full-solution restore/build as the default workflow unless all platform workloads are installed.

For shared/desktop work from the repository root:

```bash
dotnet restore src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj
dotnet restore tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj
dotnet build src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj
dotnet test tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj
```

Before committing shared/desktop changes:

```bash
dotnet format src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj --verify-no-changes --no-restore
dotnet format tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj --verify-no-changes --no-restore
dotnet build src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj -c Release --no-restore
dotnet test tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj -c Release --no-restore
pwsh ./scripts/check-version.ps1
pwsh ./scripts/check-markdown-links.ps1
```

CI performs the additional workload-specific Android, iOS/iPadOS, and Browser builds.

## Project boundaries

### Core

`src/ChronoDesk.Core`

Put product rules here when they do not require UI, filesystem, registry, process, or other platform APIs. Examples:

- formatting policy;
- quiet-hour calculations;
- settings invariants;
- chime cadence rules;
- domain models and interfaces.

### Infrastructure

`src/ChronoDesk.Infrastructure`

Put implementations of local/platform boundaries here:

- JSON persistence;
- timezone discovery;
- startup adapters;
- chime playback;
- structured logging;
- local path resolution.

Infrastructure may reference Core. Core may not reference Infrastructure.

Platform implementations must be explicitly guarded. A desktop-only facility must return unsupported or safely no-op on Android/iOS/browser rather than assuming desktop APIs exist.

### Shared app

`src/ChronoDesk.App`

This is the platform-neutral Avalonia application library. Put shared presentation state, localization, resources, themes, view models, desktop views, and the single-view mobile/browser shell here.

It contains no executable `Main` method.

Keep business rules out of click handlers even when a button triggers them.

### Desktop host

`src/ChronoDesk.Desktop`

Owns only desktop bootstrapping/packaging concerns:

- `Program.Main`;
- `Avalonia.Desktop` host setup;
- Windows manifest/application icon metadata;
- executable assembly name.

Do not put reusable clock logic here.

### Android host

`src/ChronoDesk.Android`

Owns Android packaging and launcher activity. Reusable UI belongs in `ChronoDesk.App`.

Build locally after installing the workload:

```bash
dotnet workload install android
dotnet restore src/ChronoDesk.Android/ChronoDesk.Android.csproj
dotnet build src/ChronoDesk.Android/ChronoDesk.Android.csproj -c Debug --no-restore
```

### iOS/iPadOS host

`src/ChronoDesk.iOS`

Owns the Apple application delegate, entry point, Info.plist, and package metadata. Build on macOS:

```bash
dotnet workload install ios
dotnet restore src/ChronoDesk.iOS/ChronoDesk.iOS.csproj
dotnet build src/ChronoDesk.iOS/ChronoDesk.iOS.csproj -c Debug --no-restore
```

### Browser host

`src/ChronoDesk.Browser`

Owns the WebAssembly entry point and static `wwwroot` shell:

```bash
dotnet workload install wasm-tools
dotnet restore src/ChronoDesk.Browser/ChronoDesk.Browser.csproj
dotnet build src/ChronoDesk.Browser/ChronoDesk.Browser.csproj -c Release --no-restore
```

Browser code must respect the browser sandbox; do not introduce assumptions about unrestricted filesystem/process/registry access.

## Application lifetimes

`ChronoDesk.App.App` supports two Avalonia lifetime families:

- `IClassicDesktopStyleApplicationLifetime` → `MainWindow` and optional tray integration.
- `ISingleViewApplicationLifetime` → `MainView` for Android, iOS/iPadOS, and Browser.

When adding shared functionality, decide whether it belongs in the view model/service layer or is truly lifetime-specific.

Do not open desktop modal windows from single-view code.

## Explicit dependency wiring

`AppServices` is the composition root. It creates concrete infrastructure/domain services once and passes them to the main view model.

ChronoDesk currently does not need a dependency-injection container. If composition becomes meaningfully complex, document the reason for introducing one in an ADR before adding a framework solely for convenience.

## Settings changes

Use a new `AppSettings` snapshot with a `with` expression, then call `MainWindowViewModel.UpdateSettingsAsync`.

That method:

1. normalizes values;
2. updates startup integration only when supported and when the preference changed;
3. persists settings;
4. rebuilds world-clock cards;
5. refreshes the clock;
6. notifies the application theme/view layer.

Do not write directly to `settings.json` from UI code.

## Settings schema changes

When adding a persistent field:

1. decide whether the default preserves old behavior;
2. increment `CurrentSchemaVersion` when compatibility requires a migration boundary;
3. add explicit migration logic if old serialized data needs transformation;
4. add persistence/migration tests;
5. update `PRIVACY.md` if the stored data category changes;
6. update `CHANGELOG.md` for user-visible changes;
7. consider browser/mobile sandbox behavior rather than assuming a desktop path.

Never repurpose an existing serialized field to mean something incompatible.

## Time and timezone changes

Prefer UTC instants as inputs to Core. Convert to the target zone at the formatting/policy boundary.

Use `TimeProvider` for new time-dependent application logic so tests can inject deterministic time.

Avoid manual UTC offset math. Use `TimeZoneInfo` so DST and platform timezone rules are applied by the runtime.

Any timezone behavior used by `MainView` must be tested without relying on a specific desktop timezone database shape.

## UI changes

Keep reusable visual rules in `Styles/DesignSystem.axaml` rather than duplicating margins/corner radii/button styles.

For a new user-facing control:

- make desktop keyboard navigation usable;
- make touch targets usable on phone/tablet;
- provide a visible label or automation name;
- avoid color as the only state signal;
- verify high-contrast behavior;
- test text scaling;
- verify both desktop-window and single-view layouts when the feature is shared;
- avoid assumptions about a fixed screen/window size.

## Desktop-only modes

Focus/mini state intentionally lives in `MainWindow` and is not part of the mobile/browser shell.

When changing these modes, verify:

- entering/exiting repeatedly;
- `Esc`, `F11`, and `Ctrl+M` behavior;
- restoration of window dimensions/position;
- restoration of always-on-top preference;
- tray Show/Focus/Mini actions;
- interaction with close/minimize-to-tray behavior.

Do not emulate these concepts on mobile/browser merely for feature-count parity.

## Single-view shell

`MainView` is used on Android, iOS/iPadOS, and Browser. It owns only presentation lifecycle/timer wiring and forwards actions to `MainWindowViewModel`.

When changing it, verify:

- portrait and landscape layouts;
- narrow browser widths;
- timezone search/add/remove;
- clock format and seconds toggles;
- attachment/detachment timer lifecycle;
- no invocation of desktop-only windows/tray/startup APIs.

## Platform-specific code

Use runtime guards such as:

```csharp
if (OperatingSystem.IsWindows())
{
    // Windows-only operation
}
```

Prefer an explicit capability (`IsSupported`) when callers need to decide whether to expose a behavior. Use platform annotations where analyzers require them.

Machine-wide privileges are outside the current design. Platform adapters must fail safely and must not stop the core clock when an optional OS facility is unavailable.

## External process policy

ChronoDesk currently uses external processes only for fixed local desktop sound helpers on Unix-like systems. New process launches require review.

Rules:

- no shell command string built from user input;
- prefer a fixed executable path;
- pass arguments through `ProcessStartInfo.ArgumentList`;
- set `UseShellExecute=false` unless intentionally opening a validated user-facing URI;
- document why the process is needed;
- preserve a no-op/graceful fallback when optional;
- never make Browser/mobile initialization depend on `Process` support.

## Logging

Use short stable event names such as:

```text
settings.load_failed
chime.play_failed
tray.initialize_failed
single_view.initialize_failed
```

Pass a user-safe message. Do not place imported JSON, arbitrary exception messages, credentials, email addresses, signing material, or sensitive filesystem content into logs.

## Testing style

Prefer deterministic tests with explicit dates/timezones/cultures. Avoid tests that depend on wall clock, network access, or a specific host timezone.

Headless tests should exercise reusable Avalonia surfaces such as `MainView` without requiring an emulator/device.

Platform-host compilation remains a CI responsibility because Android/iOS/WASM workloads are intentionally optional for ordinary desktop contributors.

## Performance

Do not optimize the 250 ms display timer by adding background complexity without measurement. Clock formatting is small; persistence does not happen on every tick.

On mobile/browser, also consider lifecycle and resource constraints. Timers must stop when the single-view control detaches.

If a performance-sensitive feature is introduced, add a benchmark/measurement note to `docs/performance.md` before and after optimization.

## Versioning

Canonical version: `2.6.0.2`.

`scripts/check-version.ps1` verifies:

- shared App `Version`, `PackageVersion`, `AssemblyVersion`, `FileVersion`;
- desktop package version fields;
- Android `ApplicationDisplayVersion` and positive numeric `ApplicationVersion`;
- iOS marketing version mapping and positive build number;
- optional release tag equality (`v2.6.0.2`).

Apple's three-component marketing version is `2.6.0`, while its build number is `2602`; the canonical shared/in-app version stays `2.6.0.2`.

## Commit discipline

Prefer one coherent change per commit. Good boundaries include one platform host, one service, one test area, one view, one policy document, or one CI capability.

Requested project commit identity for local maintainer work:

```bash
git config user.name "Sanskar"
git config user.email "sanskarin@outlook.in"
```

Never commit production Android keystores, Apple certificates/private keys, provisioning secrets, or passwords.

## Updating the handoff

`what_changed.md` is the continuation source of truth. Update it after meaningful milestones with:

- completed work;
- files changed;
- verification commands/results;
- platform/workload limitations;
- next exact tasks;
- recent meaningful commits.
