# ChronoDesk Setup

## Requirements

ChronoDesk targets **.NET 10** and Avalonia 11.3.x with dedicated hosts for desktop, Android, iOS/iPadOS, and WebAssembly.

Common development requirements:

- Git
- .NET 10 SDK
- an editor/IDE with C# support

Platform-specific requirements:

| Target | Additional requirements |
|---|---|
| Windows/macOS/Linux | A graphical desktop session for running the UI. |
| Android | `.NET Android` workload, JDK 17, Android SDK, emulator or physical device for deployment. |
| iOS/iPadOS | macOS, compatible Xcode, `.NET iOS` workload, simulator or provisioned device. |
| Browser | `.NET WebAssembly` tools workload and a modern WebAssembly-capable browser. |

## Verify the SDK

```bash
dotnet --info
```

The repository `global.json` pins the .NET 10 SDK family with a `latestFeature` roll-forward policy.

## Clone

```bash
git clone https://github.com/sanskarIN/chronodesk.git
cd chronodesk
```

## Important: restore by host

`ChronoDesk.sln` contains all platform projects. Restoring/building the entire solution therefore requires all mobile and browser workloads. For normal development, restore the host you are working on instead.

## Windows, macOS, and Linux

Restore and run:

```bash
dotnet restore src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj
dotnet run --project src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj
```

Release build:

```bash
dotnet build src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj -c Release --no-restore
```

Shared tests:

```bash
dotnet restore tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj
dotnet test tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj -c Release --no-restore
```

Formatting:

```bash
dotnet format src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj --verify-no-changes --no-restore
dotnet format tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj --verify-no-changes --no-restore
```

### Windows notes

ChronoDesk runs without administrator privileges. When **Start ChronoDesk when I sign in** is enabled, it writes a current-user Run entry only.

Desktop release RIDs include:

```text
win-x64
win-arm64
```

### macOS notes

When startup is enabled, ChronoDesk creates a per-user LaunchAgent:

```text
~/Library/LaunchAgents/com.sanskar.chronodesk.plist
```

Desktop release RIDs include:

```text
osx-x64
osx-arm64
```

Unsigned local builds can be subject to normal Gatekeeper rules when copied between machines. Code signing/notarization is a release-infrastructure responsibility and is not bypassed by ChronoDesk.

### Linux notes

When startup is enabled, ChronoDesk writes a per-user autostart file under `$XDG_CONFIG_HOME/autostart` or `~/.config/autostart`.

Desktop release RIDs include:

```text
linux-x64
linux-arm64
```

Tray support depends on the desktop environment/status-notifier implementation available to Avalonia. Optional chime playback attempts common system sound helpers when installed. The core clock does not depend on those helpers.

## Android

### Install the workload

```bash
dotnet workload install android
```

Verify installed workloads:

```bash
dotnet workload list
```

### Restore and build

```bash
dotnet restore src/ChronoDesk.Android/ChronoDesk.Android.csproj
dotnet build src/ChronoDesk.Android/ChronoDesk.Android.csproj -c Debug --no-restore
```

Release package build:

```bash
dotnet publish src/ChronoDesk.Android/ChronoDesk.Android.csproj -c Release
```

The project uses application ID `com.sanskar.chronodesk`, Android package format `apk`, display version `2.6.0.2`, and numeric version code `2602`.

For device/emulator deployment, use the Android SDK/ADB or your IDE's .NET Android deployment support. Production Play Store packages must be signed with maintainer-controlled credentials; signing secrets must never be committed to this repository.

## iOS and iPadOS

Apple builds must run on macOS with a compatible Xcode installation.

### Install the workload

```bash
dotnet workload install ios
```

### Restore and build

```bash
dotnet restore src/ChronoDesk.iOS/ChronoDesk.iOS.csproj
dotnet build src/ChronoDesk.iOS/ChronoDesk.iOS.csproj -c Debug --no-restore
```

CI chooses a simulator RID based on the macOS runner architecture. For local simulator testing, select the appropriate simulator/device through Xcode or your .NET IDE tooling.

ChronoDesk's canonical version remains `2.6.0.2`. Apple package metadata uses marketing version `2.6.0` and build number `2602` because Apple's marketing-version format is three-component.

Device/App Store distribution requires the maintainer's Apple signing identity and provisioning configuration. Do not commit certificates, private keys, passwords, or provisioning secrets.

## Browser / WebAssembly

### Install the workload

```bash
dotnet workload install wasm-tools
```

### Restore and run

```bash
dotnet restore src/ChronoDesk.Browser/ChronoDesk.Browser.csproj
dotnet run --project src/ChronoDesk.Browser/ChronoDesk.Browser.csproj
```

### Publish a static site

```bash
dotnet publish src/ChronoDesk.Browser/ChronoDesk.Browser.csproj -c Release -o publish/browser
```

Deploy the generated `publish/browser/wwwroot` directory through an HTTP(S) static-file host. Do not open the published `index.html` directly with a `file://` URL because the WebAssembly runtime loads modules and framework assets through browser requests.

The browser runtime is sandboxed. Desktop-only functionality such as tray icons, start-with-system registration, always-on-top, and native desktop process-based chimes is intentionally unavailable there.

## Development data isolation

Filesystem-backed hosts normally write settings/logs beneath the current user's application-data directory. For disposable desktop development data, set:

### PowerShell

```powershell
$env:CHRONODESK_DATA_DIR = "$PWD/.local-data"
dotnet run --project src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj
```

### bash / zsh

```bash
export CHRONODESK_DATA_DIR="$PWD/.local-data"
dotnet run --project src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj
```

Do not commit `.local-data` or real user settings. Browser builds use WebAssembly virtual-filesystem semantics and should not be configured with desktop absolute paths.

## Timezone data

ChronoDesk uses `TimeZoneInfo` and therefore the timezone database/data exposed by the current OS and .NET runtime. Update timezone information through normal platform/runtime maintenance, then restart ChronoDesk to rebuild its in-memory catalog.

## Repository verification

Version and documentation-link checks:

```powershell
./scripts/check-version.ps1
./scripts/check-markdown-links.ps1
```

CI intentionally validates hosts separately:

- Desktop/shared tests on Windows, macOS, and Linux.
- Android build on Linux with the Android workload.
- iOS/iPadOS simulator build on macOS with the iOS workload.
- Browser build on Linux with `wasm-tools`.

This avoids requiring every contributor to install every platform workload just to work on one host.

## Clean checkout verification

For desktop/shared work in a disposable clone:

```bash
git clean -xfd
dotnet restore src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj
dotnet restore tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj
dotnet format src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj --verify-no-changes --no-restore
dotnet build src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj -c Release --no-restore
dotnet test tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj -c Release --no-restore
```

`git clean -xfd` permanently removes untracked/ignored files in the clone. Use it only in a disposable clean checkout, never in a working directory containing uncommitted work you need.

Then install and validate each additional workload required by the release/platform matrix.

## Next documents

- `docs/architecture.md`
- `docs/development.md`
- `docs/testing.md`
- `docs/troubleshooting.md`
- `docs/release.md`
