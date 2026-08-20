# ChronoDesk Troubleshooting

Use this guide for common local setup, build, packaging, and runtime problems across the supported ChronoDesk hosts. For reproducible defects not covered here, use the repository bug-report form and provide only sanitized diagnostic information.

## First identify the host

ChronoDesk has separate host projects:

| Host | Project |
|---|---|
| Windows / macOS / Linux | `src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj` |
| Android | `src/ChronoDesk.Android/ChronoDesk.Android.csproj` |
| iOS / iPadOS | `src/ChronoDesk.iOS/ChronoDesk.iOS.csproj` |
| Browser / WebAssembly | `src/ChronoDesk.Browser/ChronoDesk.Browser.csproj` |

`src/ChronoDesk.App` is the shared Avalonia application library and is not the executable desktop entry point.

## The project does not restore

Start with:

```bash
dotnet --info
dotnet workload list
```

Check that the .NET 10 SDK family required by `global.json` is installed.

Do **not** use a full `dotnet restore ChronoDesk.sln` as the first troubleshooting step unless all workload-specific projects are supported on that machine. Restore the host you are working on instead.

### Desktop

```bash
dotnet restore src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj
```

### Android

```bash
dotnet workload install android
dotnet restore src/ChronoDesk.Android/ChronoDesk.Android.csproj
```

Also verify JDK 17 and the Android SDK/tooling are available.

### iOS / iPadOS

Run on macOS with compatible Xcode:

```bash
dotnet workload install ios
dotnet restore src/ChronoDesk.iOS/ChronoDesk.iOS.csproj
```

### Browser

```bash
dotnet workload install wasm-tools
dotnet restore src/ChronoDesk.Browser/ChronoDesk.Browser.csproj
```

If NuGet access is unavailable, restore cannot obtain packages that are not cached. ChronoDesk's runtime clock features do not require the internet, but development restore normally uses NuGet package sources.

## Build fails with formatting or analyzer warnings

ChronoDesk treats compiler/analyzer warnings as errors. For shared/desktop work:

```bash
dotnet format src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj
dotnet format tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj
dotnet build src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj -c Release
```

Review the first diagnostic instead of disabling warnings globally. If a platform API needs an OS guard, platform annotation, or host-specific implementation, add the narrow correct boundary rather than suppressing an analyzer category.

## Why does full-solution build fail on my machine?

`ChronoDesk.sln` intentionally contains Desktop, Android, iOS, Browser, shared App, Infrastructure, Core, and Tests projects. Android/iOS/WebAssembly use workload-specific target frameworks.

A machine that has only the desktop SDK workload can work normally on shared/Desktop code by building the Desktop and Tests projects directly. CI validates the additional platform hosts on runners with the correct workloads.

## Desktop app no longer runs from `ChronoDesk.App`

This command is obsolete:

```text
dotnet run --project src/ChronoDesk.App/ChronoDesk.App.csproj
```

Use:

```bash
dotnet run --project src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj
```

The shared App project is intentionally a library so Android, iOS/iPadOS, Browser, and Desktop can reuse it.

## Android workload/build problems

Verify:

```bash
dotnet --info
dotnet workload list
java -version
```

Expected development prerequisites include:

- .NET 10 SDK;
- Android workload;
- JDK 17;
- Android SDK/platform/build tools;
- an emulator or device for deployment testing.

Then rebuild only the Android host:

```bash
dotnet restore src/ChronoDesk.Android/ChronoDesk.Android.csproj
dotnet build src/ChronoDesk.Android/ChronoDesk.Android.csproj -c Debug --no-restore
```

If compile succeeds but deployment fails, separate the issue into:

1. project compilation;
2. Android SDK/device/emulator connectivity;
3. application startup/runtime behavior.

Report the Android API level, ABI, emulator/device model, .NET workload version, and exact first error. Do not publish device identifiers or account details unnecessarily.

## iOS / iPadOS build problems

Apple targets require macOS and compatible Xcode tooling. Verify:

```bash
dotnet --info
dotnet workload list
xcodebuild -version
```

Then:

```bash
dotnet restore src/ChronoDesk.iOS/ChronoDesk.iOS.csproj
dotnet build src/ChronoDesk.iOS/ChronoDesk.iOS.csproj -c Debug --no-restore
```

Distinguish simulator compilation from device/App Store signing. A simulator build can validate much of the host without production signing credentials. Device/store failures involving certificates/provisioning must be debugged without posting private signing material publicly.

ChronoDesk canonical version `2.6.0.2` maps to Apple marketing version `2.6.0` and build number `2602`.

## Browser/WebAssembly build problems

Verify the workload:

```bash
dotnet workload list
dotnet workload install wasm-tools
```

Build:

```bash
dotnet restore src/ChronoDesk.Browser/ChronoDesk.Browser.csproj
dotnet build src/ChronoDesk.Browser/ChronoDesk.Browser.csproj -c Release --no-restore
```

Publish:

```bash
dotnet publish src/ChronoDesk.Browser/ChronoDesk.Browser.csproj -c Release -o publish/browser
```

Serve `publish/browser/wwwroot` through HTTP(S). Do not double-click `index.html` from a `file://` URL; the WebAssembly runtime loads framework/module assets through browser requests.

When reporting browser startup failures, include browser/version, hosting method, the first console error, and network request failure status if relevant. Remove private hostnames/tokens before posting logs.

## Browser settings do not survive reload

The Browser host runs inside a sandbox and currently uses the .NET WebAssembly runtime filesystem model. Browser/runtime/hosting persistence semantics can differ from native application-data directories.

Do not assume desktop filesystem persistence. Verify behavior on the actual deployment host. If persistence is a release requirement for a specific browser deployment and the runtime filesystem is not persistent there, track that as a browser-storage adapter issue rather than attempting unrestricted native filesystem access.

## The window opens with default settings unexpectedly

On filesystem-backed hosts, ChronoDesk falls back to defaults when the settings file cannot be parsed safely. Look in the ChronoDesk application-data area for a file similar to:

```text
settings.json.corrupt-YYYYMMDD-HHMMSS.json
```

If present, the malformed settings document was preserved for manual recovery where the host filesystem allowed it.

Do not paste the entire file into a public issue without reviewing it; world-clock labels are user-controlled text.

## Settings do not persist on a filesystem-backed host

Possible causes:

- the application/user sandbox cannot write the selected directory;
- `CHRONODESK_DATA_DIR` points to a read-only/unavailable path on a host where that override is meaningful;
- endpoint security software blocked native temporary-file replacement;
- the filesystem became unavailable/full.

For desktop development, test with an isolated writable path:

```bash
export CHRONODESK_DATA_DIR="$PWD/.local-data"
```

PowerShell:

```powershell
$env:CHRONODESK_DATA_DIR = "$PWD/.local-data"
```

Do not solve permissions by running ChronoDesk as administrator/root as a normal operating mode.

## A timezone displays UTC after import or migration

ChronoDesk first tries the stored timezone ID, then .NET's Windows/IANA conversion helpers. When no matching timezone is available on the current host/runtime, it deliberately falls back to UTC rather than crashing.

Actions:

1. update platform/runtime timezone data through normal supported updates;
2. restart ChronoDesk;
3. search for the intended timezone and re-add the card;
4. remove the unavailable imported card if appropriate.

## Timezone search does not show a recently changed rule

The timezone catalog is built when ChronoDesk initializes. After updating host timezone/tzdata/runtime data, close/restart the app or reload the host as appropriate.

## The tray icon is missing

Tray integration is **desktop-only** and depends on the operating system/desktop environment.

Try:

- verify the Desktop host is still running before assuming it exited;
- disable **Hide to tray when closing** if your desktop does not expose a usable tray/status area;
- on Linux, record the desktop environment and status-notifier/AppIndicator support when filing a bug;
- use normal main-window close behavior with minimize-to-tray disabled as the fallback.

Android, iOS/iPadOS, and Browser do not use the desktop tray feature.

## Closing the desktop main window appears to do nothing

When **Hide to tray when closing the main window** is enabled, closing hides the desktop window instead of exiting. Use the tray **Quit** action to exit completely.

If your desktop does not provide a usable tray icon, reopen ChronoDesk if needed and disable minimize-to-tray in Settings → Behavior.

## Start-with-system does not work

Start-with-system is intentionally **desktop-only**.

### Windows

ChronoDesk uses the current-user Run key. Corporate/group policy or endpoint-security products can override/block startup entries.

### macOS

Check:

```text
~/Library/LaunchAgents/com.sanskar.chronodesk.plist
```

A moved/deleted executable path can make an existing LaunchAgent stale; disable/re-enable the preference after moving the desktop executable.

### Linux

Check:

```text
$XDG_CONFIG_HOME/autostart/chronodesk.desktop
```

or:

```text
~/.config/autostart/chronodesk.desktop
```

Android/iOS/iPadOS/Browser do not expose this desktop adapter; `PlatformStartupManager.IsSupported` is false there.

## Chime is enabled but silent

The cross-platform chime **policy** is separate from native playback.

Desktop checks:

- current time is on the configured cadence boundary;
- quiet hours do not contain the current local time;
- system audio is not muted;
- on macOS, `/usr/bin/afplay` and the expected system sound are available;
- on Linux, one of the fixed supported local helpers/files is available.

The current mobile/browser host does not emulate desktop process-based playback. Unsupported playback must not stop the clock.

## The chime repeats

The policy suppresses repeat playback within the same local minute. If repeated sounds occur, capture:

- ChronoDesk version/commit;
- platform/architecture;
- timezone;
- cadence;
- quiet-hour settings;
- exact local timestamps of repeats.

Do not include unrelated system logs or private data.

## Mini/focus mode problems

Mini and focus modes are desktop-window features.

For mini-mode restoration issues, report OS, display scaling, monitor layout, and steps. Window managers can constrain/reposition windows after monitor/work-area changes.

`F11` focus mode uses the desktop window manager's full-screen state for the monitor containing the window. Move the window to the desired display before entering focus mode.

These modes do not apply to Android/iOS/iPadOS/Browser single-view hosts.

## Mobile screen does not fit after rotation

The single-view shell is vertically scrollable and intended to tolerate portrait/landscape changes. If content becomes unreachable:

- record Android/iOS/iPadOS version, device class, orientation, and display/text scaling;
- confirm whether the problem occurs after a fresh launch or only after rotation/resume;
- capture a screenshot with private notifications/account data removed;
- verify that scrolling remains available.

Do not work around a layout defect by hard-coding one device's pixel size.

## Browser page is clipped or incorrectly scaled

Verify:

- a normal responsive viewport is active;
- the app is served over HTTP(S);
- browser zoom/text scaling;
- mobile browser safe-area/notch behavior;
- whether the issue reproduces in another modern browser.

The Browser shell includes responsive sizing and safe-area padding; report any browser-specific deviation with sanitized console details.

## High contrast looks incorrect

Report the exact platform theme/high-contrast setting and a screenshot containing no private data. Verify whether the problem occurs with:

- ChronoDesk High Contrast setting;
- host-level contrast/theme only;
- both combined.

On mobile/browser, also report text scaling/browser zoom. Do not fix contrast by hard-coding a color that works in only one theme or host.

## Import is rejected

The desktop import flow rejects a file when it is missing/unreadable, too large, invalid JSON, empty, or declares a settings schema newer than the running ChronoDesk supports.

Use an export created by a compatible ChronoDesk version. Do not manually add secrets or arbitrary data to an export.

## About/support links do not open

ChronoDesk delegates validated `https`/`mailto` destinations to the active host. If no compatible browser/mail handler exists, the action can fail without affecting clock operation.

## Logs

On filesystem-backed hosts, logs are normally under:

```text
<application-data>/ChronoDesk/logs/chronodesk.log.jsonl
```

They are structured JSON Lines and rotate near 1 MiB. The logger redacts common email/secret patterns, but always review excerpts before sharing.

Browser/mobile persistence locations can be sandboxed and may not map to a user-visible desktop path.

## Clean reset

The exact reset method depends on the host.

### Desktop

Use Settings → Data & Privacy → **Reset defaults**, or for a full local reset:

1. disable desktop startup if enabled;
2. quit ChronoDesk fully;
3. back up exports you intentionally want to keep;
4. delete the ChronoDesk current-user application-data directory;
5. launch again.

### Android / iOS / iPadOS

Use the operating system's app-data clearing/uninstall controls when a complete native sandbox reset is needed.

### Browser

Use browser/site-storage controls for the ChronoDesk hosting origin when resetting persisted browser-side runtime data.

There is no ChronoDesk cloud account to reset/delete.

## Still stuck?

See `SUPPORT.md` and open a sanitized bug report when appropriate. Include the exact host/platform and architecture/runtime where relevant.

- Support: supportramsandesh@gmail.com
- Business: sanskarin@outlook.in
- GitHub: https://github.com/sanskarIN/chronodesk
