# ChronoDesk Architecture

## Goals

ChronoDesk is a shared cross-platform application with thin platform hosts. The architecture prioritizes deterministic clock logic, offline-first operation, a small dependency surface, platform isolation, testability, accessible UI, and a contributor workflow that does not require every platform workload for every change.

## Dependency rule

```text
ChronoDesk.Desktop ─┐
ChronoDesk.Android ─┤
ChronoDesk.iOS ─────┼──> ChronoDesk.App
ChronoDesk.Browser ─┘          │
                               ├──> ChronoDesk.Core
                               └──> ChronoDesk.Infrastructure ───> ChronoDesk.Core

ChronoDesk.Core ──X──> Avalonia / OS APIs / filesystem
```

Rules:

1. Platform hosts may reference `ChronoDesk.App`.
2. `ChronoDesk.App` may reference `ChronoDesk.Core` and `ChronoDesk.Infrastructure`.
3. `ChronoDesk.Infrastructure` may reference `ChronoDesk.Core`.
4. `ChronoDesk.Core` must never reference Avalonia or platform implementation APIs.
5. Platform-specific startup/packaging code belongs in a host project or a guarded infrastructure adapter, not in Core.

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

Core objects remain UI/framework independent. Settings use records so updated immutable-style snapshots can be created with `with` expressions while list properties are replaced rather than globally mutated.

### `ChronoDesk.Infrastructure`

Implements guarded environment boundaries:

- `JsonSettingsStore` — local JSON persistence/import/export.
- `SystemTimeZoneCatalog` — timezone discovery and ID mapping through `TimeZoneInfo`.
- `PlatformStartupManager` — current-user Windows/macOS/Linux startup integration; reports unsupported on mobile/browser rather than assuming a process path exists.
- `SystemChimePlayer` — best-effort local desktop system chime playback and safe no-op behavior on unsupported platforms.
- `SafeFileLogger` — structured JSONL logging with redaction where filesystem access is available.
- `AppPaths` — application-data path resolution for filesystem-backed runtimes.

### `ChronoDesk.App`

This is now a **platform-neutral Avalonia library**, not the executable entry point.

It owns:

- `App` composition and application-lifetime detection;
- `AppServices`;
- theme palette application;
- `MainWindowViewModel`;
- world-clock card view models;
- desktop `MainWindow`, onboarding, Settings, and About views;
- `MainView`, the responsive single-view shell used by mobile/tablet/browser hosts;
- shared localization/resources/assets;
- desktop tray composition when the active lifetime is a classic desktop lifetime.

`App.OnFrameworkInitializationCompleted` branches by Avalonia lifetime:

- `IClassicDesktopStyleApplicationLifetime` → `MainWindow`.
- `ISingleViewApplicationLifetime` → `MainView`.

The shared project intentionally contains no `Main` method.

### `ChronoDesk.Desktop`

Thin Windows/macOS/Linux host:

- references `ChronoDesk.App`;
- references `Avalonia.Desktop`;
- owns the desktop `Program.Main` entry point;
- owns the Windows application manifest;
- publishes as assembly/executable name `ChronoDesk`;
- supports `win-x64`, `win-arm64`, `linux-x64`, `linux-arm64`, `osx-x64`, and `osx-arm64` release RIDs.

### `ChronoDesk.Android`

Thin Android host:

- targets `net10.0-android`;
- references `Avalonia.Android` and `ChronoDesk.App`;
- owns the launcher activity;
- application ID: `com.sanskar.chronodesk`;
- package display version: `2.6.0.2`;
- numeric version code: `2602`;
- uses Avalonia's single-view application lifetime.

### `ChronoDesk.iOS`

Shared Apple mobile/tablet host:

- targets `net10.0-ios`;
- references `Avalonia.iOS` and `ChronoDesk.App`;
- owns `AppDelegate`, native entry point, and `Info.plist`;
- supports iPhone and iPad orientations;
- application ID: `com.sanskar.chronodesk`;
- Apple marketing version: `2.6.0`;
- Apple build number: `2602`;
- in-app/shared version remains canonical `2.6.0.2`.

### `ChronoDesk.Browser`

WebAssembly host:

- targets `net10.0-browser`;
- uses `Microsoft.NET.Sdk.WebAssembly`;
- references `Avalonia.Browser` and `ChronoDesk.App`;
- boots Avalonia into the HTML element `#out`;
- owns the static HTML/CSS/JavaScript shell;
- publishes a static `wwwroot` site suitable for HTTP(S) hosting.

The browser is sandboxed. It does not receive desktop tray/startup/window-management capabilities.

## Runtime flow

### Shared composition

```text
Platform host starts
  │
  └─ configures Avalonia App
       │
       ├─ App creates AppServices
       │    ├─ SafeFileLogger
       │    ├─ JsonSettingsStore
       │    ├─ SystemTimeZoneCatalog
       │    ├─ PlatformStartupManager
       │    ├─ SystemChimePlayer
       │    ├─ ClockFormatter
       │    └─ ChimePolicy
       │
       ├─ App creates MainWindowViewModel
       │
       ├─ classic desktop lifetime ──> MainWindow
       │                               └─ optional tray integration
       │
       └─ single-view lifetime ─────> MainView
                                       └─ Android / iOS / iPadOS / Browser
```

### Desktop initialization

`MainWindow.Opened`:

1. loads settings;
2. rebuilds world-clock cards;
3. applies theme/window preferences;
4. starts the 250 ms display timer;
5. shows onboarding on first run;
6. honors `--background` only for desktop minimize-to-tray behavior.

### Single-view initialization

`MainView` attachment:

1. initializes the shared view model once;
2. completes the first-run flag without opening a desktop modal window;
3. starts the same 250 ms display timer;
4. exposes clock format, seconds, world-clock search/add/remove, and responsive status UI;
5. stops the timer when detached from the visual tree.

The 250 ms timer does **not** imply 4 Hz persistence. It refreshes displayed time and gives chime policy a reliable opportunity to observe time boundaries. Settings are written only after explicit preference/world-clock/onboarding changes.

## Clock calculation

`TimeProvider` supplies UTC time to `MainWindowViewModel`. The production default is `TimeProvider.System`; tests can inject another provider.

For each tick:

1. read the current UTC instant;
2. convert it through `TimeZoneInfo`;
3. format display text with `ClockFormatter` and current culture;
4. update the local clock and world-clock view models;
5. ask `ChimePolicy` whether the instant is a valid cadence boundary;
6. suppress duplicate/quiet-hours chimes.

The business rules are the same on every host.

## Timezone strategy

ChronoDesk does not bundle or remotely fetch a private timezone database. `SystemTimeZoneCatalog` snapshots `TimeZoneInfo.GetSystemTimeZones()` when the app starts.

For persisted/imported IDs, `Resolve` attempts:

1. exact ID lookup;
2. IANA → Windows mapping exposed by .NET;
3. Windows → IANA mapping exposed by .NET;
4. UTC fallback.

A restart rebuilds the catalog after host/runtime timezone data changes.

See ADR 0003.

## Settings persistence

On filesystem-backed hosts, the default path is conceptually:

```text
Environment.SpecialFolder.ApplicationData/ChronoDesk/settings.json
```

Development can override the application data root with `CHRONODESK_DATA_DIR` where a normal filesystem path is meaningful.

`JsonSettingsStore`:

1. enforces a 2 MiB maximum on imported/read settings documents;
2. accepts comments/trailing commas for resilience;
3. deserializes enum values as strings;
4. rejects schema versions newer than the app supports;
5. normalizes ranges/list invariants;
6. writes a unique temporary file in the destination directory;
7. flushes the temporary file;
8. moves it over the destination;
9. deletes leftover temporary files in `finally` where possible.

WebAssembly runs inside a browser sandbox and uses .NET WebAssembly virtual-filesystem semantics. It must not assume desktop absolute paths or unrestricted filesystem persistence across browser sessions.

## Settings schema evolution

`AppSettings.CurrentSchemaVersion` is the compatibility boundary. Future migrations must be explicit and tested.

Recommended sequence:

```text
read JSON envelope
  -> inspect schemaVersion
  -> migrate N to N+1 stepwise
  -> deserialize/normalize current model
  -> write current schema only after successful flow
```

ADR 0002 records the persistence choice.

## Platform capability model

Not every operating environment exposes the same concepts. ChronoDesk separates **platform support** from **feature parity** instead of pretending desktop-only capabilities exist everywhere.

| Capability | Desktop | Android/iOS/iPadOS | Browser |
|---|---:|---:|---:|
| Clock/world clocks | Yes | Yes | Yes |
| Shared responsive Avalonia UI | Yes | Yes | Yes |
| Tray icon | Where supported | No | No |
| Focus/mini desktop window modes | Yes | No | No |
| Start with OS desktop session | Yes | No | No |
| Process-based native chime helper | Yes/best effort | No-op currently | No-op |
| Local filesystem model | Native | App sandbox | WASM sandbox/virtual FS |

Unsupported capabilities must fail safely or report `IsSupported == false`; they must not crash app initialization.

## Startup integration

Startup is disabled by default.

- Windows: current-user `Run` registry value.
- macOS: current-user LaunchAgent plist.
- Linux: current-user XDG autostart desktop file.
- Android/iOS/iPadOS/Browser: unsupported by `PlatformStartupManager`.

The manager now tolerates a missing `Environment.ProcessPath`, which is important for sandboxed/non-desktop runtimes. `IsSupported` requires both a supported desktop OS and a non-empty executable path.

No administrator/root privileges are requested.

## Chime integration

The chime policy is platform-independent; playback is platform-specific.

- Windows: two short `Console.Beep` tones.
- macOS: fixed `/usr/bin/afplay` and a fixed system sound.
- Linux: fixed known local sound helpers are attempted when present.
- non-desktop hosts: safe no-op with current implementation.

Unix launches use `ProcessStartInfo.ArgumentList` with `UseShellExecute=false`; user-controlled text is not interpolated into a shell command.

Failure to provide native playback never stops clock operation.

## Logging

`SafeFileLogger` stores only:

- timestamp;
- severity;
- short event name;
- user-safe message;
- exception type.

It does not serialize arbitrary exception messages or settings content. Common email/secret assignment patterns are redacted. Log write failures are non-fatal.

## UI state

Persistent product state is kept in `AppSettings`.

Desktop-only ephemeral state—focus/mini mode, restored dimensions, one in-flight window timer tick—stays in `MainWindow`.

Single-view ephemeral state—one in-flight timer tick and visual-tree attachment lifecycle—stays in `MainView`.

This prevents transient view state from being written continuously.

## Theme and accessibility

Avalonia Fluent supplies common theme/control behavior. ChronoDesk adds a small shared resource palette:

- surface;
- card;
- muted foreground;
- accent;
- border.

The shared application applies System, Light, Dark, and High Contrast variants. Mobile/browser use the same theme resources. `MainView` is deliberately vertically scrollable, touch-friendly, orientation-tolerant, and avoids desktop window assumptions.

## Build architecture

Because the solution includes projects with workload-specific TFMs, CI builds hosts separately:

```text
Desktop job matrix
  ├─ Windows
  ├─ macOS
  └─ Linux

Android job
  └─ install android workload -> build ChronoDesk.Android

iOS job
  └─ macOS + ios workload -> build simulator host

Browser job
  └─ wasm-tools -> build ChronoDesk.Browser
```

This same separation should be used locally. A contributor changing only desktop/shared code does not need Android, iOS, and WASM workloads installed.

## Release architecture

Tagged releases package:

- `win-x64`
- `win-arm64`
- `linux-x64`
- `linux-arm64`
- `osx-x64`
- `osx-arm64`
- Browser WebAssembly static site

Android/iOS distribution artifacts are not automatically signed because production signing credentials must remain private. Their source/build validity is enforced in CI; store packaging is a protected release step.

## Error handling

Rules:

- domain services validate arguments and return deterministic results;
- recoverable settings corruption returns safe defaults;
- local file/startup failures are surfaced safely;
- unsupported platform capabilities degrade without crashing initialization;
- chime/tray failures do not stop the clock;
- logs avoid raw exception details that could contain private paths/data;
- explicit cancellation is propagated.

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
