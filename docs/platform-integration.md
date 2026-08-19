# ChronoDesk Platform Integration Reference

This document describes the operating-system boundaries implemented by ChronoDesk: login startup, tray integration, sound playback, timezone discovery, filesystem/data locations, external URI handling, window behavior, and release packaging expectations.

## Design principles

Platform integration follows these rules:

- user-scoped rather than machine-wide where possible;
- no administrator/root requirement for ordinary features;
- optional OS facilities fail safely instead of taking down the clock;
- user data stays local;
- external processes are fixed, local helpers rather than commands assembled from user input;
- startup artifacts are deterministic and testable without modifying the test host's real login configuration;
- platform claims are not considered release-verified until exercised on real GUI environments.

## Platform detection

`StartupPlatformDetector` maps the current runtime to:

- Windows;
- macOS;
- Linux;
- Unsupported.

`PlatformStartupManager.IsSupported` is false only for the unsupported case.

Tests inject a platform value directly so all startup branches can be exercised from one runner without changing its real operating-system login state.

## Windows startup integration

Mechanism: current-user Registry Run key.

Registry location:

```text
HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run
```

Value name:

```text
ChronoDesk
```

Enabled value form:

```text
"<absolute executable path>" --background
```

Characteristics:

- current-user only;
- no UAC/admin requirement expected for the ordinary current-user Run key;
- executable path is quoted;
- `--background` tells the app it was launched as a desktop-session startup action;
- enabled detection requires the configured value to contain the current ChronoDesk executable path, ignoring case;
- disabling deletes the value.

`SystemStartupRegistry` is the production Registry adapter. Tests replace it with `FakeStartupRegistry`.

### Windows release validation

Before release, verify on a real Windows 11 user session:

- enabling creates the expected current-user value;
- disabling removes it;
- path quoting works from a directory containing spaces;
- login/restart launches the packaged application;
- `--background` plus default `MinimizeToTray=true` results in the intended tray/background state;
- changing installation location does not leave a misleading enabled-state result;
- uninstall/manual removal guidance handles stale Run values if distribution later gains an installer.

## macOS startup integration

Mechanism: per-user LaunchAgent property list.

Path:

```text
~/Library/LaunchAgents/com.sanskar.chronodesk.plist
```

The generated property list:

- label: `com.sanskar.chronodesk`;
- program argument 1: absolute ChronoDesk executable path;
- program argument 2: `--background`;
- `RunAtLoad=true`.

The executable path is XML-escaped before insertion.

Disabling removes the LaunchAgent file if present.

`IStartupFileSystem` separates artifact generation from real filesystem access. `SystemStartupFileSystem` is used in production; tests use an in-memory fake.

### macOS release validation

Validate separately for packaged x64 and arm64 builds where practical:

- LaunchAgent file creation/removal;
- valid plist parsing;
- executable path with XML-sensitive characters/spaces;
- login launch in a real user GUI session;
- background/tray/menu-bar behavior;
- behavior when app quarantine/Gatekeeper/signing state differs;
- absence of an administrator requirement.

The current source test proves generated content/path rules but does not prove launchd accepts and launches the artifact in every macOS release environment.

## Linux startup integration

Mechanism: XDG autostart desktop entry.

Preferred configuration base:

```text
$XDG_CONFIG_HOME
```

Fallback:

```text
~/.config
```

Autostart path:

```text
<config-home>/autostart/chronodesk.desktop
```

Generated entry includes:

```ini
[Desktop Entry]
Type=Application
Version=1.0
Name=ChronoDesk
Comment=Start ChronoDesk with the desktop session
Exec="<escaped executable path>" --background
Terminal=false
X-GNOME-Autostart-enabled=true
```

The executable is quoted and backslash/double-quote characters are escaped for the desktop-entry Exec field.

Disabling removes the desktop entry if present.

### Linux release validation

Validate on representative desktop sessions, at minimum one GNOME-family and one KDE-family environment when available:

- XDG path selection;
- fallback to `~/.config/autostart`;
- login-session launch;
- executable paths containing spaces;
- tray/status-notifier visibility;
- behavior when the desktop environment does not expose legacy tray/status icons;
- background startup and restore-from-tray behavior.

Source tests validate artifact creation but do not replace real-session testing because desktop autostart/tray implementations vary across distributions and desktop environments.

## Background startup behavior

All generated startup mechanisms pass:

```text
--background
```

On main-window opening, ChronoDesk checks command-line arguments case-insensitively. When `--background` is present **and** `MinimizeToTray` is enabled, the initialized window hides.

If minimize-to-tray is disabled, a background-start invocation does not force hiding.

## Startup transaction consistency

Changing `StartWithSystem` is coordinated with settings persistence:

1. detect whether the requested value differs from the current snapshot;
2. apply the platform startup state if supported;
3. save settings;
4. if settings save fails, attempt to restore the previous platform startup state;
5. log rollback cancellation/failure separately.

This is a best-effort two-system transaction. It reduces inconsistent states but cannot provide filesystem/registry atomicity across OS configuration and JSON storage.

Imported settings never change `StartWithSystem`; ChronoDesk replaces the imported value with the current local value before applying imported preferences.

## Tray integration

`App` attempts to create an Avalonia `TrayIcon` using `Assets/chronodesk.ico`.

Tray menu actions:

- Show;
- Focus;
- Mini;
- Quit.

Failure to initialize the tray is logged as `tray.initialize_failed` and does not prevent the main application from running.

### Close/minimize interaction

When `MinimizeToTray=true`, ordinary close is cancelled and the window hides unless explicit shutdown was authorized through `AllowClose`.

Tray Quit:

1. calls `AllowClose`;
2. stops the UI timer;
3. shuts down the classic desktop lifetime.

Because tray support differs by shell/window manager, real-desktop validation is mandatory before stable release.

## Windows/macOS/Linux chime playback

`SystemChimePlayer` is best-effort and local-only.

### Windows

Uses two `Console.Beep` tones:

- 880 Hz for 110 ms;
- 1047 Hz for 160 ms.

Playback runs through a cancellable task.

Real-device validation is required because console/beep support can vary by environment/session/audio configuration.

### macOS

Attempts:

```text
/usr/bin/afplay /System/Library/Sounds/Glass.aiff
```

If the fixed player path is absent or execution fails, the method returns without crashing the clock.

### Linux

Attempts fixed helpers in order:

1. `/usr/bin/canberra-gtk-play --id message`;
2. `/usr/bin/paplay /usr/share/sounds/freedesktop/stereo/message.oga`;
3. `/usr/bin/aplay /usr/share/sounds/alsa/Front_Center.wav`.

The first successful helper stops fallback processing.

No shell is invoked. Arguments are added individually through `ProcessStartInfo.ArgumentList`, stdout/stderr are redirected, and nonzero exit code is treated as failure.

Linux sound helpers are optional; missing helpers do not stop the application.

## Timezone integration

ChronoDesk obtains timezone data from:

```csharp
TimeZoneInfo.GetSystemTimeZones()
```

No remote timezone service is used.

Zones are ordered by base UTC offset and then display name.

Resolution flow:

1. direct `TimeZoneInfo.FindSystemTimeZoneById`;
2. try IANA → Windows ID conversion;
3. try Windows → IANA ID conversion;
4. UTC fallback.

This allows settings moved between Windows and Unix-like systems to recover many timezone identifiers when .NET has a mapping.

Search is local, case-insensitive under the current culture, split into whitespace terms, and bounded to 1–200 results. The main UI requests at most 60.

## Local data paths

`AppPaths` resolves application data using `Environment.SpecialFolder.ApplicationData` and appends `ChronoDesk`.

If that special folder resolves blank, `AppContext.BaseDirectory` is used as a fallback.

`CHRONODESK_DATA_DIR` overrides the base for development/test isolation.

Files under the data directory currently include:

```text
settings.json
logs/chronodesk.log.jsonl
logs/chronodesk-<timestamp>.log.jsonl   # rotated archives
settings.json.corrupt-<timestamp>.json # only when corrupt settings are preserved
```

Exported settings can be written elsewhere only after explicit user file-picker selection.

## External URI integration

`ExternalLinkLauncher` is the shared boundary used by About and Settings.

Allowed URI schemes:

- absolute `https`;
- absolute `mailto`.

Rejected examples:

- `http`;
- `file`;
- script/custom schemes;
- relative URLs/paths;
- malformed or empty input.

After validation, the URI is passed to the OS using shell execution so the user's default browser/mail handler can open it.

This is user-initiated navigation. ChronoDesk does not itself fetch the URL or inspect the remote response.

## File-picker integration

Settings import/export uses Avalonia storage provider dialogs.

Export picker:

- JSON file type;
- suggested name `chronodesk-settings.json`;
- default extension `.json`.

Import picker:

- single selection only;
- JSON file filter.

Picker UI behavior is a native/platform boundary. Validation and persistence below the picker are separately automated.

## Window/platform behavior

ChronoDesk uses Avalonia desktop windows rather than separate platform-native view implementations.

Platform-sensitive behaviors include:

- window decoration/full-screen differences;
- topmost semantics;
- tray visibility;
- focus restoration after external handlers;
- native file pickers;
- display scaling/high-DPI behavior;
- screen-reader accessibility;
- high-contrast interaction with OS themes.

These are covered by manual release checklists in `accessibility.md` and `release.md`.

## Windows application manifest and icon

The App project references `app.manifest`. Windows builds conditionally use `Assets/chronodesk.ico` as the application icon.

Manifest changes can affect execution level, DPI behavior, compatibility, or host interpretation and therefore require Windows validation.

## Release runtime identifiers

Tagged release automation publishes:

| OS | RID | Archive |
|---|---|---|
| Windows | `win-x64` | ZIP |
| Linux | `linux-x64` | tar.gz |
| macOS Intel | `osx-x64` | tar.gz |
| macOS Apple Silicon | `osx-arm64` | tar.gz |

Unix builds use tarballs so executable permission bits can survive extraction.

All four packages are self-contained and single-file at release publish time.

## Unsupported platforms

ChronoDesk can potentially run wherever Avalonia/.NET desktop support exists, but the repository's release/support contract is currently scoped to the documented Windows/macOS/Linux targets.

`PlatformStartupManager` throws `PlatformNotSupportedException` when asked to change startup state on an unsupported platform and reports `IsSupported=false`.

Do not claim startup/tray/chime support for a new OS until the implementation, tests, setup documentation, release packaging, and real-platform validation are added.

## Platform-change checklist

For any platform-specific change:

- keep the Core layer platform-agnostic;
- place OS behavior behind an abstraction where it benefits testing;
- add deterministic tests for generated paths/content/decision rules;
- avoid shell command strings derived from user input;
- maintain graceful fallback for optional facilities;
- update this document;
- update setup/troubleshooting/release/accessibility documentation;
- test the real packaged application on the affected OS before marking release gates complete.
