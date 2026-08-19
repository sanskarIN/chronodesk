# ChronoDesk User Guide

This guide explains how to use ChronoDesk after it is built or extracted from a release package. It focuses on user-visible behavior. For installation/build prerequisites see `setup.md`; for privacy details see `../PRIVACY.md`; for troubleshooting see `troubleshooting.md`.

## What ChronoDesk is

ChronoDesk is a local desktop clock and world-clock dashboard for Windows, macOS, and Linux. Its normal clock features do not require an account, cloud service, remote timezone API, telemetry service, or network connection.

The application provides:

- a large local clock;
- 12-hour and 24-hour display;
- optional seconds;
- date, weekday, ISO week, and calendar details;
- multiple world clocks;
- timezone search using the operating system/.NET timezone database;
- Focus and Mini display modes;
- optional always-on-top behavior;
- optional tray behavior;
- optional login startup;
- optional hourly/half-hourly/quarter-hourly chimes;
- quiet hours;
- local settings import/export;
- high-contrast and other accessibility preferences;
- user-initiated access to GitHub Releases and project/support links.

## First launch

On a fresh data directory, ChronoDesk opens an onboarding window.

The onboarding explains the main clock/world-clock concept, accessibility controls, and the local/private-by-default behavior. Completing onboarding persists `IsFirstRun=false`, so it should not appear again for the same settings store.

If you intentionally use a new `CHRONODESK_DATA_DIR` during development, that directory has its own first-run state.

## Main window overview

The main window has four logical areas.

### Header

The header contains:

- ChronoDesk title/tagline;
- Focus action;
- Mini action;
- Settings action;
- About action.

### Primary clock card

The large central card shows the current local clock using your selected preferences. Depending on settings it can include:

- time;
- weekday;
- date;
- ISO week number;
- calendar details such as day of year and UTC offset;
- local timezone display name.

Two quick actions let you toggle clock format and seconds without opening Settings.

### World clocks

The World Clocks section contains one card for each saved timezone. Each card displays:

- its display name;
- current time;
- date;
- timezone ID;
- Remove action.

ChronoDesk keeps at least one world clock and limits the saved list to 24.

### Timezone search / quick add

The Add Timezone section lets you search the timezone database that the operating system/.NET runtime exposes. Search is local and does not call a remote timezone API.

The result list shows timezone display names, IDs, and base UTC offsets. Select a result and choose **Add selected timezone**.

### Footer

The footer displays user-safe status information and the creator credit.

## Changing clock format

ChronoDesk supports:

- 24-hour format;
- 12-hour format.

You can switch from either:

- the main-window format toggle;
- Settings → Clock & Chime → clock format selector.

The preference is persisted.

## Showing or hiding seconds

Use the main-window seconds action or the Settings checkbox.

When seconds are disabled, the clock still updates internally but the formatted display omits seconds.

## Date, weekday, week number, and calendar details

Settings lets you independently choose whether to show:

- date;
- weekday;
- ISO week number;
- calendar details.

The calendar detail line can include day-of-year, ISO week, and UTC offset information.

Date and weekday formatting follows the current culture of the operating environment. Clock format is still controlled explicitly by your ChronoDesk 12/24-hour preference.

## World clocks

### Add a world clock

1. Focus the timezone search field.
2. Type part of a city, region, timezone display name, or timezone ID.
3. Choose a result.
4. Select **Add selected timezone**.

ChronoDesk rejects a duplicate timezone ID rather than creating two cards for the same resolved timezone.

### Remove a world clock

Select **Remove** on the desired card.

ChronoDesk prevents removal of the final remaining world clock so the dashboard always retains at least one clock.

### Portable timezone IDs

Windows and Unix-like systems may use different timezone ID conventions. ChronoDesk first tries the saved ID directly and then uses .NET's IANA/Windows conversion helpers where available.

If a saved timezone cannot be resolved on the current system, ChronoDesk displays UTC as a safe runtime fallback instead of crashing.

### Timezone database updates

ChronoDesk does not download a private timezone database. Update the operating system/runtime timezone data through normal system maintenance, then restart ChronoDesk so its in-memory catalog is rebuilt.

## Focus mode

Focus mode is intended for a distraction-reduced full-screen clock.

Enter/exit with:

```text
F11
```

While Focus mode is active, ChronoDesk hides application chrome such as the header, world-clock section, add-timezone section, and footer so the clock dominates the screen.

You can also press:

```text
Esc
```

to leave Focus mode.

Focus mode is temporary window state and is not saved as a persistent preference.

## Mini mode

Mini mode creates a compact clock window intended to stay visible while you work.

Toggle with:

```text
Ctrl+M
```

Mini mode:

- uses a compact window size;
- hides most application chrome;
- forces the window on top temporarily;
- remembers the previous normal size/position/topmost state for restoration.

Press `Esc` or `Ctrl+M` again to leave Mini mode.

Mini mode itself is not persisted. The normal **Always on top** preference is separate and is restored appropriately when Mini mode ends.

## Always on top

You can enable normal always-on-top behavior in Settings.

Shortcut:

```text
Ctrl+Shift+T
```

This toggles the persisted normal always-on-top preference from the main window.

Mini mode temporarily forces topmost regardless of the normal preference.

## Theme and appearance

Settings supports these theme choices:

- System;
- Light;
- Dark;
- High Contrast.

A separate High Contrast preference can also force the high-contrast palette.

Appearance settings include:

- layout;
- font family;
- clock font size;
- content spacing.

Clock font size and spacing are bounded to safe ranges when saved/imported so malformed configuration cannot produce unbounded values.

ChronoDesk does not download fonts from the network. A configured family name is resolved by the local UI/font environment.

## Accessibility preferences

Settings includes:

- reduced motion;
- high contrast;
- scalable clock typography;
- keyboard-accessible controls and shortcuts.

The application intentionally avoids decorative motion by default. Reduced Motion remains an explicit preference for present/future behavior.

Key interactive Settings controls whose visual labels are adjacent text have explicit automation names for assistive technology.

For release-level accessibility expectations see `accessibility.md`.

## Chimes

Chimes are disabled by default and must be enabled explicitly.

Available cadences:

- Hourly;
- Half-hourly;
- Quarter-hourly.

ChronoDesk suppresses duplicate playback within the same local calendar minute even though the UI clock ticks more frequently.

### Quiet hours

Enable quiet hours in Settings and provide start/end times.

Typical overnight example:

```text
Start: 22:00
End:   07:00
```

The start boundary is included and the end boundary is excluded.

When the start is later than the end, the quiet range crosses midnight. When start and end are equal, ChronoDesk treats the range as no quiet interval rather than silencing the entire day.

### Platform sound behavior

Sound is best-effort:

- Windows uses a local system beep sequence;
- macOS attempts the fixed `afplay` system helper and Glass sound;
- Linux attempts fixed local sound helpers (`canberra-gtk-play`, then `paplay`, then `aplay`) when available.

Missing optional helpers or playback failures must not stop the clock.

## Start with system

The **Start with system** preference is opt-in.

ChronoDesk uses user-scoped startup mechanisms:

- Windows current-user Run Registry value;
- macOS per-user LaunchAgent;
- Linux per-user XDG autostart desktop entry.

No administrator/root access is intended for normal startup configuration.

When ChronoDesk creates its startup entry, it passes:

```text
--background
```

If the application starts with that argument and **Minimize to tray** is enabled, the initialized main window hides.

If the OS does not support the implemented startup mechanism, the option cannot be applied successfully.

For exact platform artifacts see `platform-integration.md`.

## Minimize to tray

Default: enabled.

When enabled, closing the main window normally hides it instead of terminating the process, provided tray behavior is available and explicit shutdown was not requested.

Use the tray menu **Quit** action to exit intentionally.

When disabled, a normal window close is allowed to exit.

Tray behavior varies by operating system/desktop environment and should be validated on the actual desktop you use.

## Tray menu

Where Avalonia/the host desktop exposes a working tray implementation, ChronoDesk provides:

- Show;
- Focus;
- Mini;
- Quit.

**Show** restores/activates a hidden or minimized window.

**Focus** toggles focus mode.

**Mini** toggles mini mode.

**Quit** authorizes close and shuts down the application.

If tray initialization fails, ChronoDesk logs a safe event and keeps the main clock usable.

## Keyboard shortcut reference

| Shortcut | Action |
|---|---|
| `F11` | Toggle Focus mode |
| `Ctrl+M` | Toggle Mini mode |
| `Ctrl+K` | Focus timezone search |
| `Ctrl+,` | Open Settings |
| `Ctrl+Shift+T` | Toggle normal always-on-top preference |
| `Esc` | Exit Focus or Mini mode |

These are application-window shortcuts, not global system hotkeys.

## Settings window

Settings is organized into logical areas covering clock/chime, appearance, accessibility/behavior, privacy/data, and Updates & About.

### Save

Choose **Save** after editing preferences.

ChronoDesk validates quiet-hour input before attempting persistence. If validation or expected persistence/startup work fails, the Settings window remains usable and displays a safe localized status instead of claiming success.

### Cancel

Cancel closes Settings without applying the current unsaved control edits.

### Reset defaults

Reset applies fresh default preferences through the same normal settings transaction and reloads the controls.

Reset does not make first-run onboarding reappear.

If startup was enabled and defaults disable it, the startup integration is updated accordingly.

## Backup / export settings

Settings → Privacy/Data provides an export action.

The native save picker suggests:

```text
chronodesk-settings.json
```

Export writes a normalized settings document using the same atomic writer used for normal settings persistence.

An exported file can contain:

- display preferences;
- accessibility/behavior preferences;
- world-clock display labels and timezone IDs;
- chime/quiet-hour configuration.

Review the file before sharing because custom labels may be personally chosen text.

## Restore / import settings

Import accepts a user-selected JSON file.

Safety rules include:

- maximum file size 2 MiB;
- JSON parsing only;
- supported schema checks;
- string enum validation;
- normalized/bounded values;
- at most 24 valid world clocks;
- no imported command/executable/token/credential field.

Important: imported settings **cannot silently change login startup**. ChronoDesk preserves the current device's `StartWithSystem` value before applying the imported preferences.

An invalid import displays a safe error and does not intentionally replace the current good settings snapshot.

## Local data

ChronoDesk normally stores its application data under the current user's operating-system application-data location in a `ChronoDesk` directory.

Typical files:

```text
settings.json
logs/chronodesk.log.jsonl
```

If the settings JSON becomes malformed and can be preserved, ChronoDesk renames it to a timestamped `settings.json.corrupt-...json` file and starts from defaults.

Development/testing can override the base directory with `CHRONODESK_DATA_DIR`.

See `settings-reference.md` and `../PRIVACY.md` for details.

## Logs

ChronoDesk writes bounded local structured JSONL diagnostics for failures/events that need troubleshooting.

The logger:

- attempts to redact common email/secret patterns;
- limits field lengths;
- records exception type rather than arbitrary raw exception content;
- rotates the main log around 1 MiB;
- treats logging failure as nonfatal.

Redaction is defense in depth, not a guarantee that every possible private value can never appear. Review logs before sharing them publicly.

## Updates & About

ChronoDesk does not run a background update checker or download/update packages itself.

Settings → Updates & About shows the current semantic application version and offers:

- **Open GitHub Releases**;
- **Open About**.

The Releases action is deliberate/user-initiated and opens the public repository Releases page with your operating system's default browser handler.

ChronoDesk validates external destinations through a shared allowlist that accepts only absolute HTTPS and mailto URIs.

## About window

The About window provides:

- application/version information;
- project/repository link;
- MIT license/privacy information;
- funding link;
- business/support contacts;
- creator credit.

Project/support navigation uses the same safe external-link policy as Settings.

## Offline behavior

The primary clock, world-clock search/formatting, settings, startup configuration, tray, and chime decision logic do not require a ChronoDesk-operated server.

Network access occurs only when the user deliberately opens a project/release/support HTTPS link using the operating system handler. A mailto action similarly hands the URI to the configured mail handler.

ChronoDesk does not send telemetry or analytics in the documented implementation.

## If an external link does not open

The application should remain usable and report a safe status/failure when possible.

Check:

- a default HTTPS browser exists;
- a default mail application exists for mailto links;
- OS policy is not blocking handler launch.

Do not change ChronoDesk to accept arbitrary URL schemes merely to work around a broken system handler.

## If a timezone cannot be found

Try:

- a broader city/region fragment;
- the timezone ID used by your operating system;
- updating the operating system timezone data;
- restarting ChronoDesk after timezone-data updates.

A previously saved but unavailable timezone falls back to UTC at runtime.

## If startup does not work

Confirm:

- Start with system is enabled in the saved settings;
- the user-scoped startup artifact exists for your platform;
- the application/executable has not moved;
- OS policy/security software is not suppressing login startup;
- on Linux, the desktop environment honors XDG autostart;
- on macOS, launchd can load the user LaunchAgent;
- on Windows, the current-user Run entry references the correct executable.

See `platform-integration.md` and `troubleshooting.md`.

## If chimes do not play

Confirm chimes are enabled and current local time is outside quiet hours.

On Linux/macOS, optional system helper/sound files must exist. Missing helpers are expected to fail gracefully.

The clock should continue updating even if audio is unavailable.

## If the app seems to disappear when closed

With **Minimize to tray** enabled, this is expected close-to-hide behavior. Use the tray icon to restore the window or Quit.

If your desktop does not present the tray icon reliably, disable Minimize to tray until the platform-specific tray behavior is confirmed.

## Privacy before sharing diagnostics/screenshots

Before attaching any file or screenshot to a public issue:

- remove unrelated notifications;
- remove usernames/private filesystem paths when not needed;
- remove tokens/credentials;
- review custom world-clock labels;
- sanitize logs/settings to the smallest relevant excerpt;
- never share a signing/private key or real secret.

Security vulnerabilities should follow `../SECURITY.md`, not a public issue.

## Release archive verification

Official tagged release automation is designed to publish each archive with a sibling `.sha256` file.

Verify the checksum before treating a downloaded archive as intact. Checksums detect byte changes but do not provide the same publisher-identity guarantee as platform code signing/notarization.

See `release.md` for maintainer release details.

## Where to get help

For ordinary support, see `../SUPPORT.md`.

For security vulnerabilities, follow `../SECURITY.md`.

For development questions, use `development.md`, `architecture.md`, `configuration-reference.md`, and the repository's issue/PR process.
