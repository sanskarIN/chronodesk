# ChronoDesk Privacy

_Last updated: 2026-08-19_

ChronoDesk is designed as a local, offline-first desktop clock. Core clock, world-clock, settings, accessibility, focus/mini, startup-preference, and chime-policy features do not require a ChronoDesk account or remote application service.

## Data ChronoDesk stores locally

ChronoDesk may store the following on the current device:

- clock format and display preferences;
- theme, layout, font, size, and spacing preferences;
- accessibility preferences;
- startup/minimize/always-on-top preferences;
- chime cadence and quiet-hour preferences;
- world-clock labels and timezone IDs;
- first-run/onboarding completion state;
- structured diagnostic log entries containing event names and user-safe messages.

The settings schema is defined by `ChronoDesk.Core.Models.AppSettings` and is serialized to JSON.

## Where local data is stored

The default data directory is derived from .NET's current-user `Environment.SpecialFolder.ApplicationData`, inside a `ChronoDesk` folder.

Developers can override the directory with:

```text
CHRONODESK_DATA_DIR
```

This environment variable is a local configuration option, not a secret.

## Logs

ChronoDesk logs operational events to JSON Lines (`.jsonl`). The logger:

- avoids writing raw settings documents;
- records exception type rather than arbitrary exception message content;
- redacts common email patterns;
- redacts common token/secret/password/API-key assignment patterns;
- caps event/message lengths;
- rotates the main log after approximately 1 MiB.

Redaction reduces risk but is not a guarantee that every possible sensitive pattern can be recognized. Review a log excerpt before sharing it publicly.

## Network behavior

ChronoDesk's clock functionality does not require a network request. It uses the timezone data exposed by the local operating system/.NET runtime.

The About screen contains visible links for GitHub, Buy Me a Coffee, and email. ChronoDesk opens one of those destinations only after the user activates the corresponding control. Those destinations are handled by the user's browser/mail client and are then subject to the destination provider's privacy practices.

The application does not contain a telemetry SDK, advertising SDK, analytics endpoint, cloud database, or built-in sign-in flow.

## Timezone data

Timezone definitions come from `TimeZoneInfo` on the host. ChronoDesk does not upload your timezone list to obtain time values. OS/runtime timezone maintenance is outside the application and is managed through normal operating-system/runtime updates.

## Import and export

Users can export settings to a JSON file and later import a settings JSON file.

An export can include:

- the preferences listed above;
- world-clock labels;
- timezone IDs.

It does not intentionally include log history, credentials, or unrelated files. Users control where exported files are stored and should review them before sharing.

Imported settings files are size-bounded, parsed as JSON, schema-checked, and normalized before becoming active. Imported text fields are converted to bounded single-line values and unsupported enum representations are rejected.

An imported file is **not allowed to enable or disable operating-system startup registration**. ChronoDesk preserves the current local startup preference during import; changing startup still requires an explicit user preference change in Settings.

## Invalid and temporarily unreadable settings

If the normal settings document is malformed or otherwise fails data/schema validation, ChronoDesk attempts to preserve it with a timestamped `.corrupt-...json` suffix and returns to safe defaults. That preserved file remains local and may contain the same preference data that existed in the original settings file.

A temporary operating-system I/O failure is treated differently. If the settings document cannot be read because of a transient read failure (for example, the file is temporarily unavailable), ChronoDesk returns safe defaults for that launch attempt but does **not** rename or quarantine the original settings document. A later read can therefore recover the original preferences after the transient condition clears.

Permission failures are also handled as local-data availability problems by the application layer; ChronoDesk does not attempt to bypass filesystem permissions.

## Startup integration

Startup is opt-in.

- Windows: current-user Run key.
- macOS: current-user LaunchAgent.
- Linux: current-user XDG autostart file.

ChronoDesk does not request machine-wide startup installation through these settings. If startup integration is changed successfully but persisting the matching preference then fails, ChronoDesk makes a best-effort attempt to restore the previous startup state before surfacing the save failure.

## Optional chimes

Chimes are disabled by default. When enabled, ChronoDesk uses local system facilities. On macOS/Linux, the implementation may invoke fixed system sound utilities/paths if they exist. No user-provided text is inserted into those process arguments.

## Deleting your ChronoDesk data

To remove application preference/log data:

1. Quit ChronoDesk completely from the tray/menu.
2. Disable **Start ChronoDesk when I sign in** first if it is enabled, or remove the documented user-level startup entry manually.
3. Delete the `ChronoDesk` application-data directory for your current user.
4. Delete any settings exports you created elsewhere if you no longer want them.

There is no ChronoDesk cloud account to delete because the application does not create one.

## Children and sensitive data

ChronoDesk is a utility and is not designed to collect sensitive profile information. Do not place passwords, tokens, private keys, or sensitive personal information into world-clock labels or filenames.

## Changes to this document

Privacy-impacting changes should update this file in the same pull request and be called out in release notes when user behavior meaningfully changes.

## Contact

- Business: sanskarin@outlook.in
- Business: sanskarin.business@gmail.com
- Support: supportramsandesh@gmail.com
- GitHub: https://github.com/sanskarIN

Funding: https://buymeacoffee.com/sanskarIN

**Made by the Sanskar**
