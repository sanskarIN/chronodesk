# ChronoDesk Privacy

_Last updated: 2026-08-20_

ChronoDesk is designed as a local, offline-first clock and world-clock application across Windows, macOS, Linux, Android, iOS/iPadOS, and WebAssembly-capable browsers. Core clock/world-clock functionality does not require a ChronoDesk account, advertising identifier, analytics service, cloud database, or remote application backend.

## Data ChronoDesk may store locally

Depending on the host and features available there, ChronoDesk may store:

- clock format and display preferences;
- theme, layout, font, size, and spacing preferences;
- accessibility preferences;
- desktop startup/minimize/always-on-top preferences;
- chime cadence and quiet-hour preferences;
- world-clock labels and timezone IDs;
- first-run/onboarding completion state;
- structured diagnostic log entries containing event names and user-safe messages where filesystem-backed logging is available.

The settings schema is defined by `ChronoDesk.Core.Models.AppSettings` and is serialized as JSON by the current settings store.

## Where local data is stored

### Filesystem-backed hosts

On desktop and supported mobile application sandboxes, ChronoDesk resolves its data directory from .NET's current-user `Environment.SpecialFolder.ApplicationData` and places application files beneath a `ChronoDesk` folder when the runtime exposes a normal filesystem-backed location.

Developers can override the data directory where a conventional filesystem path is meaningful with:

```text
CHRONODESK_DATA_DIR
```

This environment variable is a local development/configuration option, not a secret.

### Browser / WebAssembly

A browser build runs inside the browser sandbox and does **not** have unrestricted access to the user's native filesystem, registry, startup/session configuration, or arbitrary desktop process APIs.

The current WebAssembly host uses the .NET WebAssembly runtime filesystem model. Persistence across reloads/browser sessions can depend on runtime/browser/hosting behavior. ChronoDesk therefore does not claim that a desktop filesystem path or desktop-style persistent storage semantics exist in the browser. Browser persistence behavior must be validated on the deployment host and documented accurately for a release.

## Logs

Where filesystem-backed logging can be created, ChronoDesk writes operational events as JSON Lines (`.jsonl`). The logger:

- avoids writing raw settings documents;
- records exception type rather than arbitrary exception message content;
- redacts common email patterns;
- redacts common token/secret/password/API-key assignment patterns;
- caps event/message lengths;
- rotates the main log after approximately 1 MiB;
- treats logging failures as non-fatal.

Redaction reduces risk but is not a guarantee that every possible sensitive pattern can be recognized. Review a log excerpt before sharing it publicly.

Browser/mobile sandbox restrictions may change whether/where such a log is physically persisted. ChronoDesk does not upload those logs to a ChronoDesk service.

## Network behavior

ChronoDesk's core clock functionality does not require a network request. It uses timezone data exposed by the host operating system/.NET runtime.

The About/support experience can contain visible GitHub, funding, or email destinations. A destination is opened only after user action and is then handled by the host browser/mail application, subject to that provider's own privacy practices.

The application does not contain a telemetry SDK, advertising SDK, analytics endpoint, cloud database, or built-in sign-in flow.

A WebAssembly build itself is delivered by a web host, so loading the application necessarily involves normal browser requests to that hosting origin for the application/runtime assets. That transport is separate from ChronoDesk adding analytics or a remote clock backend.

## Timezone data

Timezone definitions come from `TimeZoneInfo` on the host/runtime. ChronoDesk does not upload your world-clock list to obtain current time values. Host/runtime timezone maintenance is managed through normal platform/runtime updates.

## Import and export

The full Settings import/export workflow is currently a desktop experience.

An export can include:

- display/preferences listed above;
- world-clock labels;
- timezone IDs.

It does not intentionally include log history, credentials, mobile signing material, or unrelated files. Users control where an exported file is stored and should review it before sharing.

Imported settings documents are size-bounded, parsed as JSON, schema-checked, and normalized before becoming active. Imported text fields are converted to bounded single-line values and unsupported enum representations are rejected.

An imported file is **not allowed to enable or disable desktop operating-system startup registration**. ChronoDesk preserves the current local startup preference during import; changing startup requires an explicit supported desktop preference change.

## Invalid and temporarily unreadable settings

On filesystem-backed hosts, if the normal settings document is malformed or otherwise fails supported data/schema validation, ChronoDesk attempts to preserve it with a timestamped `.corrupt-...json` suffix and returns to safe defaults. The preserved file remains local and can contain the same preference data that existed in the original settings file.

A temporary I/O failure is treated differently. If the settings document cannot be read because of a transient read failure, ChronoDesk returns safe defaults for that initialization attempt but does **not** rename or quarantine the original settings document. A later read can therefore recover the original preferences when the transient condition clears.

Permission failures are handled as local-data availability problems; ChronoDesk does not attempt to bypass host filesystem/application-sandbox permissions.

Browser virtual-filesystem behavior can differ from a native filesystem, so corrupt-file preservation/rename semantics must not be assumed to be identical to desktop behavior without browser validation.

## Startup integration

Start-with-system integration is opt-in and intentionally desktop-only:

- Windows: current-user Run key.
- macOS: current-user LaunchAgent.
- Linux: current-user XDG autostart file.
- Android/iOS/iPadOS/Browser: the desktop startup adapter reports unsupported.

ChronoDesk does not request machine-wide startup installation through these settings. If a supported startup change succeeds but persisting the matching preference then fails, ChronoDesk makes a best-effort attempt to restore the previous desktop startup state before surfacing the save failure.

## Optional chimes

Chimes are disabled by default.

Desktop playback can use local system facilities. On macOS/Linux, the implementation can invoke fixed local system sound utilities/paths when present. No user-provided text is inserted into those process arguments.

The current mobile/browser shared shell does not fabricate a desktop process-based chime mechanism where that capability is unavailable; unsupported playback safely degrades rather than requiring a remote sound service.

## Mobile application permissions and signing

ChronoDesk's current clock/world-clock mobile host does not require a ChronoDesk account or cloud credential. Any future native permission (for example, notifications) must be documented before release and requested only when the corresponding feature actually needs it.

Production Android/iOS distribution signing credentials are maintainer release secrets. They are not user data and must never be committed into the application/repository. Store/platform providers can process installation, purchase/account, crash, or distribution metadata under their own policies independently of ChronoDesk's local application design.

## Deleting your ChronoDesk data

The exact deletion mechanism depends on the host:

- Desktop: quit ChronoDesk completely, disable start-with-system first if enabled, then remove the current-user `ChronoDesk` application-data directory and any exported settings files you created.
- Android/iOS/iPadOS: use the operating system's application-data removal/uninstall controls for the installed app, and remove any user-created exports separately if future mobile export support is enabled.
- Browser: clear the relevant site/application storage for the hosting origin using browser controls; browser cache/storage behavior is controlled by the browser/runtime and host.

There is no ChronoDesk cloud account to delete because ChronoDesk does not create one.

## Children and sensitive data

ChronoDesk is a utility and is not designed to collect sensitive profile information. Do not place passwords, tokens, private keys, or sensitive personal information into world-clock labels or filenames.

## Changes to this document

Privacy-impacting changes should update this file in the same pull request and be called out in release notes when user behavior meaningfully changes, including changes to browser persistence, mobile permissions, network behavior, or external services.

## Contact

- Business: sanskarin@outlook.in
- Business: sanskarin.business@gmail.com
- Support: supportramsandesh@gmail.com
- GitHub: https://github.com/sanskarIN

Funding: https://buymeacoffee.com/sanskarIN

**Made by the Sanskar**
