# ChronoDesk Troubleshooting

Use this guide for common local setup and runtime problems. For reproducible defects not covered here, use the repository bug-report form and provide only sanitized diagnostic information.

## Find your local diagnostics first

Open **Settings → Data & Privacy → Local diagnostics**.

ChronoDesk shows read-only local values for:

- application version;
- operating system description;
- .NET runtime description;
- process architecture;
- application data directory;
- settings-file path;
- diagnostic-log path.

These values are read locally and are not uploaded by ChronoDesk. Review filesystem paths and log contents before sharing them publicly because paths can reveal local usernames/folder names.

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

ChronoDesk falls back to defaults when its settings file cannot be parsed or validated safely. Look at the settings path shown in **Settings → Data & Privacy → Local diagnostics** and inspect its parent directory for a file similar to:

```text
settings.json.corrupt-YYYYMMDD-HHMMSSfff-<random-id>.json
```

If present, the original malformed settings document was preserved for manual inspection/recovery. The timestamp + random suffix avoids collisions when failures happen very close together.

Do not paste that entire file into a public issue without reviewing it; world-clock labels and local paths are user-controlled data.

## Settings do not persist

Possible causes:

- the current user cannot write the application-data directory;
- `CHRONODESK_DATA_DIR` points to a read-only/unavailable path;
- endpoint security software blocked the temporary-file replacement;
- the filesystem became unavailable/full.

Use the read-only paths in **Local diagnostics** to confirm which data location the running app resolved.

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

Imported settings are normalized so two cards cannot continue to represent the same timezone ID case-insensitively.

## Timezone search does not show a recently changed rule

The timezone catalog is built when ChronoDesk starts. After updating OS timezone/tzdata packages, quit ChronoDesk completely and launch it again.

## The tray icon is missing

Tray integration depends on the platform/desktop environment.

ChronoDesk now hides its main window only when Avalonia reports a native tray-menu exporter that provides a reliable restoration route. If that integration is unavailable for the session, minimize-to-tray/background hiding is disabled and the main window remains accessible.

When reporting tray behavior, include:

- the OS/desktop environment from Local diagnostics;
- Windows/macOS/Linux version;
- GNOME/KDE/status-notifier/AppIndicator details where relevant;
- whether the tray icon/menu ever appeared;
- whether Show/Focus/Mini/Quit worked.

A tray failure must not stop the main clock or intentionally strand it as an unreachable hidden process.

## Closing the main window exits instead of hiding

If **Hide to tray when closing the main window** is enabled but reliable tray-menu restoration is unavailable, ChronoDesk intentionally does not hide the only window. Closing therefore proceeds normally so the app remains controllable.

When a reliable tray integration is available, closing with that preference enabled hides the main window and the tray **Quit** action exits fully.

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

If a startup change succeeds but saving the matching preference then fails, ChronoDesk makes a best-effort rollback to the prior startup state. That rollback is deliberately independent of a cancelled settings-save token.

## Chime is enabled but silent

The clock/chime policy can be working even when an OS sound facility is unavailable.

Check:

- current time is exactly on the configured cadence boundary;
- quiet hours do not contain the current local time;
- system audio is not muted;
- on macOS, the normal `/usr/bin/afplay` and system sound are available;
- on Linux, one of the fixed supported local sound helpers/files is available.

ChronoDesk intentionally does not download a sound player or execute arbitrary shell commands to make chimes work. Helper stdout/stderr is not redirected or uploaded.

## The chime repeats

The policy suppresses repeat playback within the same local minute. If repeated sounds occur, capture:

- ChronoDesk version from Local diagnostics;
- timezone;
- cadence;
- quiet-hour settings;
- exact local timestamps of repeats.

Do not include unrelated system logs or private data.

## Mini mode/window does not restore exactly

ChronoDesk restores its recorded dimensions/position after leaving mini mode. Window managers may constrain or reposition windows when monitors, scale factors, work areas, or virtual desktops change while mini mode is active.

The current normal always-on-top preference is reapplied when mini mode exits. If that is reproducibly incorrect on an unchanged monitor setup, report the OS, display scaling, monitor layout, preference value, and exact steps.

## Focus mode does not cover the expected monitor

`F11` uses the window manager's full-screen state for the monitor containing the window. Move the window to the desired display before entering focus mode and retry.

## System theme does not update live

When Theme is **System**, ChronoDesk listens to Avalonia's actual theme variant and recomputes its custom palette when the OS changes between light and dark.

If the palette stays stale:

1. confirm Theme is actually set to **System** rather than explicit Light/Dark;
2. switch the OS theme while ChronoDesk remains running;
3. record the OS/desktop environment;
4. report whether native controls changed but custom cards did not, or whether nothing changed.

Explicit Light/Dark modes are expected not to follow later OS theme changes.

## High contrast looks incorrect

Report the exact OS theme/high-contrast setting and a screenshot containing no private data. Verify whether the problem occurs with:

- ChronoDesk's **High contrast** setting;
- the OS-level theme only;
- both combined.

Do not fix contrast by hard-coding a color that works in one theme but removes dynamic palette behavior elsewhere.

## Import is rejected

An import is rejected when the file is missing/unreadable, above the 2 MiB bound, invalid JSON, has a non-object root, contains unsupported enum representation, or declares an invalid/newer settings schema.

Use an export created by a compatible ChronoDesk version. Do not manually add secrets or arbitrary data to an export.

## Updates says it does not check automatically

That is intentional. ChronoDesk does not run a release-feed poller or background update tracker.

Settings → Updates reads the current version from the local assembly and exposes **Open official releases**. Only clicking that action asks the OS browser to open the fixed HTTPS GitHub Releases page.

## Releases/About/support links do not open

ChronoDesk delegates fixed `https`/`mailto` destinations to the OS. If the browser/mail handler is unavailable or unsupported, the action fails safely and Settings remains usable.

The visible repository/email information can still be copied manually. ChronoDesk does not fall back to executing a shell command or opening an arbitrary user-provided URI.

## Logs

Use **Settings → Data & Privacy → Local diagnostics** to see the exact current log path.

By default it is similar to:

```text
<application-data>/ChronoDesk/logs/chronodesk.log.jsonl
```

Logs are structured JSON Lines and rotate near 1 MiB. Rotated filenames include timestamp precision plus a random suffix so rapid repeated rotations do not collide.

The logger redacts common email/secret assignment patterns, bounds logged fields, and records exception type instead of arbitrary exception message content. Always review excerpts before sharing; redaction is defense-in-depth, not a proof that every possible sensitive string format is recognized.

## Clean reset

To reset preferences from the UI, use Settings → Data & Privacy → **Reset defaults**.

For a complete local reset:

1. disable startup if enabled;
2. quit ChronoDesk fully;
3. back up any settings export you intentionally want to keep;
4. use Local diagnostics to locate the ChronoDesk user application-data directory;
5. delete that directory;
6. launch ChronoDesk again.

This removes local settings/logs; there is no ChronoDesk cloud account.

## Still stuck?

See `SUPPORT.md` and open a sanitized bug report when appropriate.

- Support: supportramsandesh@gmail.com
- Business: sanskarin@outlook.in
- GitHub: https://github.com/sanskarIN/chronodesk
