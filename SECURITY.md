# Security Policy

ChronoDesk is an offline-first desktop clock, but it still reads local configuration, opens user-selected import files, integrates with user-session startup mechanisms, can launch fixed support/release links, and executes limited OS facilities for optional chimes. Security reports are taken seriously.

## Supported versions

Before the first stable release, security fixes are made on the `main` branch and included in the next tagged release candidate/release. After stable releases begin, the newest stable release is the primary supported line unless a release note says otherwise.

## Reporting a vulnerability

**Do not open a public GitHub issue for an unpatched vulnerability.**

Preferred reporting path:

1. Use the repository's private GitHub security-advisory reporting flow when available under the repository **Security** tab.
2. If that is not available, email **sanskarin@outlook.in** with the subject `ChronoDesk security report`.

Support contact: **supportramsandesh@gmail.com**.

A useful report includes:

- affected commit/tag/version;
- operating system and architecture;
- concise reproduction steps;
- security impact;
- whether user interaction is required;
- suggested mitigation if known.

Please avoid sending real passwords, tokens, private user files, or unrelated personal data. Use synthetic proof-of-concept data whenever possible.

## Coordinated disclosure

The maintainer will review a good-faith report, attempt to reproduce it, determine severity and affected versions, and prepare a fix or mitigation when warranted. Public disclosure should wait until a fix or practical mitigation is available, unless earlier disclosure is legally required or necessary to address active harm.

No bounty is promised by this policy.

## Security boundaries

ChronoDesk intentionally:

- does not require a remote account;
- does not embed API credentials;
- does not run a local privileged service;
- uses user-scoped startup registration;
- validates and escapes generated startup registration content;
- rejects control characters in startup executable paths and embedded quote characters in Windows Run commands;
- writes macOS/Linux startup registration through a temporary file before atomic replacement;
- limits imported settings files to a small maximum size using the opened file stream before parsing;
- requires a JSON object root for settings;
- validates/migrates settings schema and normalizes values;
- rejects negative/future schema versions and numeric enum representations;
- bounds imported font/world-clock/timezone text and converts it to single-line values;
- removes duplicate clock IDs and duplicate timezone IDs during normalization;
- preserves the current device startup preference when settings are imported;
- best-effort rolls startup integration back if the matching settings write fails, using a non-cancelled rollback operation;
- writes settings through a temporary file before replacement;
- preserves corrupt settings with collision-resistant recovery names rather than executing/interpreting arbitrary content;
- centralizes product/support/funding/release links and permits only `https` and `mailto` URI schemes;
- rejects credential-bearing HTTPS destinations in the external-link launcher;
- performs no background update polling; the official Releases page opens only after explicit user action;
- uses argument lists rather than a shell command string for optional Unix chime helpers;
- does not redirect unused helper-process output streams;
- avoids hiding the main window when reliable tray restoration is unavailable;
- redacts common email/secret patterns from structured logs;
- uses GitHub CodeQL, dependency review, Dependabot, NuGet vulnerability inspection, and a high-signal tracked-file secret scan in repository automation.

## Import threat model

A settings export is user-controlled input when it is imported, even if its extension is `.json`. ChronoDesk therefore treats an imported document as untrusted configuration rather than as a command or trusted backup.

Current controls include:

- maximum 2 MiB file size checked from the opened stream;
- JSON object-root requirement;
- JSON parsing only; no script/template execution;
- supported schema-version migration/check;
- negative/future schema rejection;
- string-enum parsing with numeric values disabled;
- settings normalization and bounded world-clock count;
- duplicate timezone normalization;
- bounded/single-line user-display text;
- no imported OS startup side effect;
- no imported executable path, URI, shell command, token, or credential field.

Import hardening has deterministic malformed-input/fuzz regression coverage in the test project.

## Startup registration threat model

Startup registration is a user-controlled preference but the registration content itself is generated only from the current ChronoDesk executable path and a fixed `--background` argument.

Current controls include:

- no user-supplied startup command field;
- executable-path control-character rejection;
- embedded quote rejection for the Windows Run command;
- XML escaping for the macOS LaunchAgent document;
- desktop-entry escaping for the Linux `Exec` value;
- exact expected-registration comparison when checking whether startup is enabled;
- current-user scope only;
- atomic replacement for file-based macOS/Linux registration;
- deterministic pure registration builders covered by unit tests without modifying real user startup locations;
- rollback of an already-applied startup change if matching settings persistence fails;
- rollback does not reuse a caller token that may have been cancelled by the failed save.

Real desktop startup behavior remains a manual release check because session managers, registry permissions, and desktop environments cannot be fully represented by pure unit tests.

## Platform integration notes

### Windows startup

The app uses the current user's `Software\\Microsoft\\Windows\\CurrentVersion\\Run` key. It does not request administrator privileges.

### macOS startup

The app writes a per-user LaunchAgent file under the user's `Library/LaunchAgents` folder only when startup is enabled.

### Linux startup

The app writes a per-user XDG autostart desktop file only when startup is enabled.

### Tray

ChronoDesk hides on close/background start only when the current Avalonia tray implementation exposes a native menu exporter that provides a reliable restoration route. If that capability is unavailable, the app remains visible/closeable rather than intentionally creating an unreachable hidden process.

### Chimes

Windows uses a local system beep path. macOS/Linux playback uses fixed OS executable paths and fixed system-sound arguments when those tools/files exist. User-provided text is not interpolated into a shell command. Standard output/error is not captured because ChronoDesk does not consume it.

### Updates

The current version is read from local assembly informational metadata. ChronoDesk does not automatically query GitHub or another update service. The Settings Updates control opens one fixed HTTPS GitHub Releases destination only after explicit user activation and through the same URI allow-list as other external product links.

## Dependency policy

- NuGet and GitHub Actions dependency updates are monitored by Dependabot.
- Pull requests run dependency review.
- CodeQL analyzes C# changes.
- CI runs `dotnet list ... --vulnerable --include-transitive` and fails when NuGet reports vulnerable packages through the expected result marker.
- Dependencies should be removed when the standard library is sufficient.

## Secret handling

ChronoDesk requires no production secrets. Never commit:

- access tokens;
- API keys;
- signing/private keys;
- real user data;
- private endpoints;
- passwords;
- credentials in screenshots or logs.

`.env.example` contains placeholders/configuration names only.

CI additionally scans tracked text files for a small set of high-signal private-key/token formats. To reduce accidental exposure in logs, the scanner reports the file and rule name but intentionally does **not** print the matched value. This check supplements GitHub security features and human review; it is not a guarantee that every possible secret format can be detected.

## Hardening contributions

Security-hardening pull requests are welcome. Keep the change focused, add a regression test where practical, explain threat assumptions, and avoid weakening cross-platform support or accessibility without a strong reason.
