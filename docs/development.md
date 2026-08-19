# ChronoDesk Development Guide

## Daily workflow

From the repository root:

```bash
dotnet restore ChronoDesk.sln
dotnet build ChronoDesk.sln
dotnet test ChronoDesk.sln
```

Before committing:

```bash
dotnet format ChronoDesk.sln --verify-no-changes
dotnet build ChronoDesk.sln -c Release
dotnet test ChronoDesk.sln -c Release
```

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

### App

`src/ChronoDesk.App`

Put Avalonia views, presentation state, theme composition, tray/window behavior, and user interaction here.

Avoid moving business rules into click handlers merely because the behavior is triggered by a button.

## Explicit dependency wiring

`AppServices` is the composition root. It creates concrete infrastructure services and domain services once and passes them to the main view model.

ChronoDesk currently does not need a dependency-injection container. If composition becomes meaningfully complex, document the reason for introducing one in an ADR before adding a framework solely for convenience.

## Settings changes

Use a new `AppSettings` snapshot with a `with` expression, then call `MainWindowViewModel.UpdateSettingsAsync`.

That method:

1. normalizes values;
2. updates user startup integration when the startup preference changed;
3. persists settings;
4. rebuilds world-clock cards;
5. refreshes the clock;
6. notifies the application theme/window layer.

Do not write directly to `settings.json` from UI code.

## Settings schema changes

When adding a persistent field:

1. decide whether the default preserves old behavior;
2. increment `CurrentSchemaVersion` when compatibility requires a migration boundary;
3. add migration logic before relying on the new field if old serialized data needs transformation;
4. add a persistence/migration test;
5. update `PRIVACY.md` if the stored data category changes;
6. update `CHANGELOG.md` for user-visible changes.

Never repurpose an existing serialized field to mean something incompatible.

## Time and timezone changes

Prefer UTC instants as inputs to Core. Convert to the target zone at the formatting/policy boundary.

Use `TimeProvider` for new time-dependent application logic so tests can inject deterministic time.

Avoid manual UTC offset math for civil time. Use `TimeZoneInfo` so DST and platform timezone rules are applied by the runtime.

## UI changes

Keep reusable visual rules in `Styles/DesignSystem.axaml` instead of duplicating margins/corner radii/button styles across windows.

For a new user-facing control:

- make it keyboard reachable;
- provide a visible label or automation name;
- keep target size usable;
- avoid using color as the only state signal;
- verify high-contrast behavior;
- check that large clock font settings do not make the window unusable.

## Focus and mini modes

Focus/mini state is intentionally ephemeral and lives in `MainWindow`.

When changing these modes, verify:

- entering/exiting repeatedly;
- `Esc` behavior;
- `F11` behavior;
- `Ctrl+M` behavior;
- restoration of window dimensions/position;
- restoration of always-on-top preference;
- tray Show/Focus/Mini actions;
- interaction with close/minimize-to-tray behavior.

## Platform-specific code

Use runtime guards such as:

```csharp
if (OperatingSystem.IsWindows())
{
    // Windows-only operation
}
```

Use platform annotations where analyzers need them. Keep machine-wide privileges out of settings features unless a future feature has a compelling, reviewed reason.

Platform adapters must fail safely and should not stop the core clock when an optional OS facility is unavailable.

## External process policy

ChronoDesk currently uses external processes only for fixed local sound helpers on Unix-like systems. New process launches require review.

Rules:

- no shell command string built from user input;
- prefer a fixed executable path;
- pass arguments through `ProcessStartInfo.ArgumentList`;
- set `UseShellExecute=false` unless intentionally opening a validated user-facing URI;
- document why the process is needed;
- preserve a no-op/graceful fallback when the feature is optional.

## Logging

Use short stable event names such as:

```text
settings.load_failed
chime.play_failed
tray.initialize_failed
```

Pass a user-safe message. Do not place imported JSON, arbitrary exception messages, credentials, email addresses, or sensitive filesystem content into the log message.

## Testing style

Prefer deterministic tests with explicit dates/timezones/cultures. Avoid tests that rely on the developer's wall clock or network access.

For timezone tests, use UTC where the test does not specifically need DST behavior. When testing DST, construct the target timezone intentionally and skip only when a platform genuinely cannot provide the required fixture.

## Performance

Do not optimize the 250 ms display timer by adding background complexity without measurement. Clock formatting is small; persistence does not happen on every tick.

If a performance-sensitive feature is introduced, add a benchmark/measurement note to `docs/performance.md` before and after optimization.

## Commit discipline

Prefer one coherent change per commit. Good boundaries include one service, one test area, one view, one policy document, or one CI capability.

Requested project commit identity for local maintainer work:

```bash
git config user.name "Sanskar"
git config user.email "sanskarin@outlook.in"
```

## Updating the handoff

`what_changed.md` is the continuation source of truth. Update it after meaningful milestones with:

- completed work;
- files changed;
- verification commands/results;
- limitations;
- next exact tasks;
- recent meaningful commits.
