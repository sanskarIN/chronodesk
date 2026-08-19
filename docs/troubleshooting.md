# ChronoDesk Troubleshooting

Use this guide for common local setup and runtime problems. For reproducible defects not covered here, use the repository bug-report form and provide only sanitized diagnostic information.

## The project does not restore

Run:

```bash
dotnet --info
dotnet restore ChronoDesk.sln
```

Check that a .NET 9 SDK is installed and that `global.json` can resolve an installed .NET 9 feature band through its roll-forward policy.

If NuGet access is unavailable, restore cannot obtain packages that are not already cached. ChronoDesk itself does not need the internet after dependencies/build artifacts are present, but development restore normally uses NuGet package sources.

## Build fails with formatting or analyzer warnings

ChronoDesk treats compiler/analyzer warnings as errors. Run:

```bash
dotnet format ChronoDesk.sln
dotnet build ChronoDesk.sln -c Release
```

Review the first diagnostic rather than disabling warnings globally. If a platform API needs an operating-system guard or annotation, add the narrow correct guard instead of suppressing the entire analyzer category.

## The window opens with default settings unexpectedly

ChronoDesk falls back to defaults when its settings file cannot be parsed safely. Look in the ChronoDesk application-data directory for a file similar to:

```text
settings.json.corrupt-YYYYMMDD-HHMMSS.json
```

If present, the original malformed settings document was preserved for manual inspection/recovery.

Do not paste that entire file into a public issue without reviewing it; world-clock labels are user-controlled text.

## Settings do not persist

Possible causes:

- the current user cannot write the application-data directory;
- `CHRONODESK_DATA_DIR` points to a read-only/unavailable path;
- endpoint security software blocked the temporary-file replacement;
- the filesystem became unavailable/full.

For development, test with an isolated writable path:

```bash
export CHRONODESK_DATA_DIR="$PWD/.local-data"
```

PowerShell:

```powershell
$env:CHRONODESK_DATA_DIR = "$PWD/.local-data"
```

Do not solve permissions by running ChronoDesk as administrator/root as a normal operating mode.

## A timezone displays UTC after import

ChronoDesk first tries the stored timezone ID, then .NET's Windows/IANA conversion helpers. When no matching timezone is available on the current OS/runtime, it deliberately falls back to UTC rather than crashing.

Actions:

1. update the operating system/runtime timezone data through normal system updates;
2. restart ChronoDesk;
3. search for the intended timezone and re-add the card;
4. remove the unavailable imported card if appropriate.

## Timezone search does not show a recently changed rule

The timezone catalog is built when ChronoDesk starts. After updating OS timezone/tzdata packages, quit ChronoDesk completely and launch it again.

## The tray icon is missing

Tray integration depends on the platform/desktop environment.

Try:

- verify ChronoDesk is still running before assuming the app exited;
- disable **Hide to tray when closing** if your desktop does not expose a usable tray/status area;
- on Linux, record the desktop environment and status-notifier/AppIndicator support when filing a bug;
- use the normal main-window close behavior with minimize-to-tray disabled as the fallback.

A tray failure should not stop the main clock.

## Closing the main window appears to do nothing

When **Hide to tray when closing the main window** is enabled, closing the window hides it instead of exiting. Use the tray **Quit** action to exit completely.

If your desktop does not provide a tray icon, reopen ChronoDesk if needed and disable minimize-to-tray in Settings → Behavior.

## Startup does not work

First confirm the preference is enabled and settings were successfully saved.

### Windows

ChronoDesk uses the current-user Run key. Corporate/group policy or endpoint-security products can override/block startup entries.

### macOS

Check the per-user file:

```text
~/Library/LaunchAgents/com.sanskar.chronodesk.plist
```

A moved/deleted application path can make an existing LaunchAgent stale; disable/re-enable the preference after moving the executable.

### Linux

Check:

```text
$XDG_CONFIG_HOME/autostart/chronodesk.desktop
```

or:

```text
~/.config/autostart/chronodesk.desktop
```

Desktop environments can apply their own autostart policy.

## Chime is enabled but silent

The clock/chime policy can be working even when an OS sound facility is unavailable.

Check:

- current time is exactly on the configured cadence boundary;
- quiet hours do not contain the current local time;
- system audio is not muted;
- on macOS, the normal `/usr/bin/afplay` and system sound are available;
- on Linux, one of the fixed supported local sound helpers/files is available.

ChronoDesk intentionally does not download a sound player or execute arbitrary shell commands to make chimes work.

## The chime repeats

The policy suppresses repeat playback within the same local minute. If repeated sounds occur, capture:

- ChronoDesk version/commit;
- timezone;
- cadence;
- quiet-hour settings;
- exact local timestamps of repeats.

Do not include unrelated system logs or private data.

## Mini mode/window does not restore exactly

ChronoDesk restores its recorded dimensions/position after leaving mini mode. Window managers may constrain or reposition windows when monitors, scale factors, work areas, or virtual desktops change while mini mode is active.

If reproducible on an unchanged monitor setup, report the OS, display scaling, monitor layout, and steps.

## Focus mode does not cover the expected monitor

`F11` uses the window manager's full-screen state for the monitor containing the window. Move the window to the desired display before entering focus mode and retry.

## High contrast looks incorrect

Report the exact OS theme/high-contrast setting and a screenshot containing no private data. Verify whether the problem occurs with:

- ChronoDesk's **High contrast** setting;
- the OS-level theme only;
- both combined.

Do not fix contrast by hard-coding a color that works in one theme but removes dynamic palette behavior elsewhere.

## Import is rejected

An import is rejected when the file is missing/unreadable, too large, invalid JSON, empty, or declares a settings schema newer than the running ChronoDesk supports.

Use an export created by a compatible ChronoDesk version. Do not manually add secrets or arbitrary data to an export.

## About links do not open

ChronoDesk delegates fixed `https`/`mailto` destinations to the OS. If no browser/mail handler is configured, the action can fail without affecting the app. The visible URL/email remains available for manual use.

## Logs

Logs are normally under:

```text
<application-data>/ChronoDesk/logs/chronodesk.log.jsonl
```

They are structured JSON Lines and automatically rotate near 1 MiB. The logger redacts common email/secret patterns, but always review excerpts before sharing.

## Clean reset

To reset preferences from the UI, use Settings → Data & Privacy → **Reset defaults**.

For a complete local reset:

1. disable startup if enabled;
2. quit ChronoDesk fully;
3. back up any settings export you intentionally want to keep;
4. delete the ChronoDesk user application-data directory;
5. launch ChronoDesk again.

This removes local settings/logs; there is no ChronoDesk cloud account.

## Still stuck?

See `SUPPORT.md` and open a sanitized bug report when appropriate.

- Support: supportramsandesh@gmail.com
- Business: sanskarin@outlook.in
- GitHub: https://github.com/sanskarIN/chronodesk
