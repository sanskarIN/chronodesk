# ChronoDesk Privacy

_Last updated: 2026-08-19_

ChronoDesk is designed as a local, offline-first desktop clock. Core clock, world-clock, settings, accessibility, focus/mini, startup-preference, chime-policy, and version-display features do not require a ChronoDesk account or remote application service.

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

The About screen contains visible links for GitHub, Buy Me a Coffee, and email. The Settings **Updates** section contains a visible **Open official releases** action. ChronoDesk opens one of those destinations only after the user activates the corresponding control. Those destinations are handled by the user's browser/mail client and are then subject to the destination provider's privacy practices.

ChronoDesk does **not** poll GitHub, a release API, or any application server in the background to check for updates. The current application version is read from local assembly metadata. Opening the official Releases page is the only update-related network action implemented by the application and is entirely user initiated.

The application does not contain a telemetry SDK, advertising SDK, analytics endpoint, cloud database, background update tracker, or built-in sign-in flow.

## Timezone data

Timezone definitions come from `TimeZoneInfo` on the host. ChronoDesk does not upload your timezone list to obtain time values. OS/runtime timezone maintenance is outside the application and is managed through normal operating-system/runtime updates.

## Import and export

Users can export settings to a JSON file and later import a settings JSON file.

An export can include:

- the preferences listed above;
- world-clock labels;
- timezone IDs.

It does not intentionally include log history, credentials, or unrelated files. Users control where exported files are stored and should review them before sharing.

Imported settings files are size-bounded using the opened file stream, parsed as JSON, required to have an object root, schema-checked/migrated, and normalized before becoming active. Imported text fields are converted to bounded single-line values, duplicate timezone cards are removed, and unsupported enum representations are rejected.

An imported file is **not allowed to enable or disable operating-system startup registration**. ChronoDesk preserves the current local startup preference during import; changing startup still requires an explicit user preference change in Settings.

## Corrupt settings recovery

If the normal settings document cannot be parsed or validated, ChronoDesk attempts to preserve it with a collision-resistant timestamp-and-randomized `.corrupt-...json` suffix and returns to safe defaults. That preserved file remains local and may contain the same preference data that existed in the original settings file.

ChronoDesk does not upload a corrupt settings file for diagnostics or recovery.

## Startup integration

Startup is opt-in.

- Windows: current-user Run key.
- macOS: current-user LaunchAgent.
- Linux: current-user XDG autostart file.

ChronoDesk does not request machine-wide startup installation through these settings. If startup integration is changed successfully but persisting the matching preference then fails, ChronoDesk makes a best-effort attempt to restore the previous startup state before surfacing the save failure. That rollback uses its own non-cancelled operation so cancellation of the failed settings save does not itself prevent restoration of the previous startup registration.

## Tray behavior

Minimize-to-tray is a local desktop preference. ChronoDesk hides its main window on close or background startup only when the current Avalonia desktop integration exposes a usable native tray-menu exporter. When reliable tray restoration is unavailable, ChronoDesk leaves the main window accessible instead of intentionally creating an unreachable hidden process.

## Optional chimes

Chimes are disabled by default. When enabled, ChronoDesk uses local system facilities. On macOS/Linux, the implementation may invoke fixed system sound utilities/paths if they exist. No user-provided text is inserted into those process arguments. Process output is not captured or sent elsewhere.

## Deleting your ChronoDesk data

To remove application preference/log data:

1. Quit ChronoDesk completely from the tray/menu, or close it normally if tray integration is unavailable.
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
