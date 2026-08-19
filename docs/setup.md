# ChronoDesk Setup

## Requirements

ChronoDesk targets .NET 9 and Avalonia desktop environments.

Required for development:

- Git
- .NET 9 SDK
- a supported desktop session on Windows, macOS, or Linux for running the UI

Optional:

- Visual Studio / Rider / VS Code with C# support
- platform packaging/signing tools for release engineering

## Verify the SDK

```bash
dotnet --info
```

The repository includes `global.json` with a .NET 9 SDK baseline and `latestFeature` roll-forward policy.

## Clone

```bash
git clone https://github.com/sanskarIN/chronodesk.git
cd chronodesk
```

## Restore

```bash
dotnet restore ChronoDesk.sln
```

## Build

```bash
dotnet build ChronoDesk.sln -c Release --no-restore
```

## Run

```bash
dotnet run --project src/ChronoDesk.App/ChronoDesk.App.csproj
```

## Test

```bash
dotnet test ChronoDesk.sln -c Release
```

## Formatting check

```bash
dotnet format ChronoDesk.sln --verify-no-changes
```

## Development data isolation

By default, ChronoDesk writes current-user settings/logs under the platform application-data directory. To keep development data inside a disposable folder, set:

### PowerShell

```powershell
$env:CHRONODESK_DATA_DIR = "$PWD/.local-data"
dotnet run --project src/ChronoDesk.App/ChronoDesk.App.csproj
```

### bash / zsh

```bash
export CHRONODESK_DATA_DIR="$PWD/.local-data"
dotnet run --project src/ChronoDesk.App/ChronoDesk.App.csproj
```

Do not commit `.local-data` or real user settings.

## Windows notes

ChronoDesk runs without administrator privileges. When **Start ChronoDesk when I sign in** is enabled, the application writes a current-user Run entry only.

For manual startup verification:

1. build/run ChronoDesk normally;
2. open Settings → Behavior;
3. enable startup and save;
4. verify the current-user startup entry exists;
5. disable startup and save;
6. verify the entry is removed.

Do not change machine-wide startup policy for this test.

## macOS notes

When startup is enabled, ChronoDesk creates a per-user LaunchAgent plist under:

```text
~/Library/LaunchAgents/com.sanskar.chronodesk.plist
```

The release workflow targets both `osx-x64` and `osx-arm64`.

Unsigned local builds may be subject to normal macOS Gatekeeper behavior when copied between machines. Signing/notarization is a release infrastructure concern and is not bypassed by the application.

## Linux notes

When startup is enabled, ChronoDesk writes a per-user autostart file under `$XDG_CONFIG_HOME/autostart` or `~/.config/autostart`.

Tray support depends on the desktop environment/status-notifier implementation available to Avalonia. Optional chime playback attempts common system sound helpers when installed. The main clock itself does not depend on those helpers.

If UI libraries required by your distribution are missing, install them through the distribution's supported package-management documentation rather than copying random shared libraries from third-party sites.

## Timezone data

ChronoDesk uses `TimeZoneInfo` and therefore the timezone database available to the OS/.NET runtime. Update timezone data through normal OS/runtime maintenance. Restart ChronoDesk after such an update to rebuild its in-memory catalog.

## Clean checkout verification

Before a release, perform the setup on a fresh clone without build artifacts:

```bash
git clean -xfd
dotnet restore ChronoDesk.sln
dotnet format ChronoDesk.sln --verify-no-changes --no-restore
dotnet build ChronoDesk.sln -c Release --no-restore
dotnet test ChronoDesk.sln -c Release --no-build
```

`git clean -xfd` permanently removes untracked/ignored files in the clone. Use it only in a disposable clean checkout, never in a working directory containing uncommitted work you need.

## Next documents

- `docs/development.md`
- `docs/testing.md`
- `docs/troubleshooting.md`
- `docs/release.md`
