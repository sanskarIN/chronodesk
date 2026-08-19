# ChronoDesk Architecture

## Goals

ChronoDesk is a modular desktop monolith. The architecture prioritizes deterministic clock logic, offline operation, small dependency surface, platform isolation, testability, and a straightforward contributor experience.

## Dependency rule

```text
ChronoDesk.App
   ├── references ChronoDesk.Core
   └── references ChronoDesk.Infrastructure

ChronoDesk.Infrastructure
   └── references ChronoDesk.Core

ChronoDesk.Core
   └── references only the .NET runtime/BCL
```

`ChronoDesk.Core` must never reference Avalonia or platform implementation APIs.

## Projects

### `ChronoDesk.Core`

Owns stable product concepts and rules:

- `AppSettings`
- `ClockFormat`
- `ClockLayout`
- `ThemeMode`
- `WorldClock`
- `QuietHours`
- `ChimeSettings` / `ChimeInterval`
- `ClockSnapshot`
- `TimeZoneDescriptor`
- `ClockFormatter`
- `ChimePolicy`
- persistence/platform/logging interfaces

Core objects are intentionally small. Settings use records so the UI can create updated immutable-style snapshots with `with` expressions, while list properties are replaced rather than globally mutated by application services.

### `ChronoDesk.Infrastructure`

Implements boundaries that interact with the environment:

- `JsonSettingsStore` — local JSON persistence/import/export.
- `SystemTimeZoneCatalog` — timezone discovery and ID mapping through `TimeZoneInfo`.
- `PlatformStartupManager` — current-user Windows/macOS/Linux startup integration.
- `SystemChimePlayer` — best-effort local system chime playback.
- `SafeFileLogger` — structured JSONL logging with redaction.
- `AppPaths` — deterministic current-user data path resolution.

### `ChronoDesk.App`

Owns Avalonia composition and presentation:

- application lifetime and service composition (`AppServices`);
- theme palette application;
- tray menu composition;
- `MainWindowViewModel`;
- world-clock card view models;
- main dashboard;
- onboarding;
- settings/import/export UI;
- About/support/funding UI;
- focus/mini mode and keyboard handling.

## Runtime flow

```text
App starts
  │
  ├─ creates AppServices
  │    ├─ SafeFileLogger
  │    ├─ JsonSettingsStore
  │    ├─ SystemTimeZoneCatalog
  │    ├─ PlatformStartupManager
  │    ├─ SystemChimePlayer
  │    ├─ ClockFormatter
  │    └─ ChimePolicy
  │
  ├─ creates MainWindowViewModel
  ├─ creates MainWindow
  ├─ creates tray integration when available
  │
  └─ MainWindow.Opened
       ├─ loads AppSettings
       ├─ rebuilds world-clock cards
       ├─ applies theme/window preferences
       ├─ begins 250 ms UI timer
       └─ shows onboarding when IsFirstRun
```

The 250 ms dispatcher timer does **not** imply 4 Hz business state writes. It only refreshes the displayed time and gives the chime policy a reliable opportunity to observe a boundary second. Settings are written only after a user preference change/import/reset/onboarding completion.

## Clock calculation

`TimeProvider` supplies UTC time to `MainWindowViewModel`. The production default is `TimeProvider.System`; tests can inject another provider.

For each tick:

1. get the current UTC instant;
2. convert it through `TimeZoneInfo`;
3. format display text with `ClockFormatter` and the current culture;
4. update the local clock and each world-clock view model;
5. ask `ChimePolicy` whether the instant is a valid cadence boundary;
6. suppress the chime when quiet hours contain the local time or that same local minute already chimed.

## Timezone strategy

ChronoDesk does not bundle or remotely fetch a private timezone database. `SystemTimeZoneCatalog` snapshots `TimeZoneInfo.GetSystemTimeZones()` when the app starts.

For imported settings, `Resolve` attempts:

1. exact ID lookup;
2. IANA → Windows mapping exposed by .NET;
3. Windows → IANA mapping exposed by .NET;
4. UTC fallback.

A restart rebuilds the catalog after an OS/runtime timezone update.

See ADR 0003.

## Settings persistence

Default settings path:

```text
Environment.SpecialFolder.ApplicationData/ChronoDesk/settings.json
```

Development can override the application data root with `CHRONODESK_DATA_DIR`.

`JsonSettingsStore`:

1. enforces a 2 MiB maximum on imported/read settings documents;
2. ignores JSON comments/trailing commas for resilience;
3. deserializes enum values as strings;
4. rejects a schema version newer than the app supports;
5. normalizes ranges/list invariants;
6. writes a uniquely named temporary file in the destination directory;
7. flushes the temporary file;
8. moves it over the destination;
9. deletes a leftover temporary file in `finally` when needed.

When the main settings file is malformed or unreadable for supported recoverable reasons, the app logs a safe error, attempts to rename the file with a `.corrupt-TIMESTAMP.json` suffix, and falls back to defaults.

## Settings schema evolution

`AppSettings.CurrentSchemaVersion` is the compatibility boundary. Future migrations should be explicit and tested. Never silently reinterpret a field with incompatible semantics.

Recommended migration sequence:

```text
read JSON envelope
  -> inspect schemaVersion
  -> migrate N to N+1 stepwise
  -> deserialize/normalize current model
  -> write current schema only after successful user/application flow
```

ADR 0002 records the persistence choice.

## Startup integration

Startup remains disabled by default. A change is applied only when the user saves a changed setting.

- Windows: current-user `Run` registry value.
- macOS: current-user LaunchAgent plist.
- Linux: current-user XDG autostart desktop file.

The startup command contains a `--background` flag. After settings load, the main window hides itself when that flag is present and minimize-to-tray is enabled.

No administrator/root privileges are requested.

## Chime integration

The chime policy is platform-independent; playback is platform-specific.

- Windows: two short `Console.Beep` tones.
- macOS: fixed `/usr/bin/afplay` path and fixed system sound path.
- Linux: fixed known local sound helpers are attempted in order when present.

Unix process launches use `ProcessStartInfo.ArgumentList` with `UseShellExecute=false`; user-controlled text is not interpolated into a shell command.

If playback cannot be provided on the current desktop, clock operation continues.

## Logging

`SafeFileLogger` writes JSON Lines and intentionally stores only:

- timestamp;
- severity;
- short event name;
- user-safe message;
- exception type (for errors).

It does not serialize arbitrary exception messages or settings content. Common email and secret assignment patterns are redacted. Log writing failures are non-fatal.

## UI state

Persistent product state is kept in `AppSettings`. Ephemeral window state such as whether focus/mini mode is currently active, restored window dimensions, or one in-flight timer tick stays in `MainWindow`.

This avoids writing transient window state several times per second.

## Theme and accessibility

Avalonia Fluent theme provides native theme/control behavior. ChronoDesk adds a small resource palette:

- surface;
- card;
- muted foreground;
- accent;
- border.

`ThemeMode.System` follows the application/system theme variant. Light, Dark, and High Contrast apply explicit variants/palettes. Reduced motion is stored and the current app deliberately avoids non-essential animations, so there is no animation engine to disable.

## Error handling

Rules:

- domain services validate arguments and return deterministic results;
- recoverable settings corruption returns safe defaults;
- local file/startup failures are surfaced as user-safe settings status text;
- chime/tray failures do not stop the clock;
- logs avoid raw exception details that could contain private paths/data;
- cancellation is propagated when an operation was explicitly cancelled.

## Limits

Intentional limits reduce accidental complexity:

- maximum 24 world-clock cards;
- maximum 2 MiB settings input;
- maximum approximately 1 MiB active log before rotation;
- no database server;
- no web backend;
- no authentication;
- no telemetry;
- no privileged daemon.

## Architecture changes

Durable architecture changes require an ADR under `docs/adr/`. Update this document and the dependency diagram in the same pull request.
