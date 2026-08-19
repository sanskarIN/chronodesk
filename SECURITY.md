# Security Policy

ChronoDesk is an offline-first desktop clock, but it still reads local configuration, opens user-selected import files, integrates with user-session startup mechanisms, can launch fixed support links, and executes limited OS facilities for optional chimes. Security reports are taken seriously.

## Supported versions

The current source/release-candidate version is **`2.6.0.2`**. Until that exact version is tagged and published after the documented release gates pass, security fixes continue to land on `main` and are included in the next verified four-part release.

After tagged releases exist, the newest supported release is the primary supported line unless a release note explicitly states otherwise. Do not infer support solely from a version number; consult the latest release notes/security advisory when one exists.

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
- limits imported settings files to a small maximum size;
- validates settings schema and normalizes values;
- rejects numeric enum representations in imported JSON;
- bounds imported font/world-clock/timezone text and converts it to single-line values;
- preserves the current device startup preference when settings are imported;
- best-effort rolls startup integration back if the matching settings write fails;
- writes settings through a temporary file before replacement;
- preserves invalid/corrupt settings data rather than executing/interpreting arbitrary content;
- does not quarantine a potentially valid settings file solely because of a transient read/I/O failure;
- allows only fixed `https` and `mailto` support destinations from the About window;
- uses argument lists rather than a shell command string for optional Unix chime helpers;
- redacts common email/secret patterns from structured logs;
- verifies four-part project version consistency in CI;
- rejects release tags that do not exactly match the canonical application version;
- generates SHA-256 checksums for release ZIP artifacts;
- uses GitHub CodeQL, dependency review, Dependabot, and NuGet vulnerability inspection in repository automation.

## Import threat model

A settings export is user-controlled input when it is imported, even if its extension is `.json`. ChronoDesk therefore treats an imported document as untrusted configuration rather than as a command or trusted backup.

Current controls include:

- maximum 2 MiB file size;
- JSON parsing only; no script/template execution;
- supported schema-version check;
- string-enum parsing with numeric values disabled;
- settings normalization and bounded world-clock count;
- bounded/single-line user-display text;
- case-insensitive duplicate world-clock/timezone removal;
- no imported OS startup side effect;
- no imported executable path, URI, shell command, token, or credential field.

Import hardening has deterministic malformed-input/fuzz regression coverage in the test project.

## Local settings failure model

ChronoDesk distinguishes invalid data from temporary availability failures:

- malformed/schema-invalid settings can be moved to a timestamped `.corrupt-...json` file and replaced with safe defaults;
- transient `IOException` read failures fall back to safe defaults without renaming/deleting the original settings file;
- permission failures are not bypassed and are surfaced to the application as local-data availability problems.

This avoids turning an availability problem into unnecessary data loss.

## Platform integration notes

### Windows startup

The app uses the current user's `Software\\Microsoft\\Windows\\CurrentVersion\\Run` key. It does not request administrator privileges.

### macOS startup

The app writes a per-user LaunchAgent file under the user's `Library/LaunchAgents` folder only when startup is enabled.

### Linux startup

The app writes a per-user XDG autostart desktop file only when startup is enabled.

### Chimes

Windows uses a system beep path. macOS/Linux playback uses fixed OS executable paths and fixed system-sound arguments when those tools/files exist. User-provided text is not interpolated into a shell command.

## Release integrity

For a four-part release such as `v2.6.0.2`:

- `scripts/check-version.ps1 -Tag <tag>` must confirm the tag exactly matches project metadata;
- release ZIPs are produced from the tagged source by GitHub Actions;
- each ZIP includes the project license/readme/changelog/privacy/security/support documents;
- `SHA256SUMS.txt` is generated from the release ZIPs and published with the release;
- downloaded artifacts should be verified against the checksum file before being treated as release evidence.

Checksums detect accidental or malicious artifact changes after generation but do not provide publisher identity in the way platform code signing/notarization would. Signing/notarization remains a separate future capability and private signing material must never be committed to the repository.

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

## Hardening contributions

Security-hardening pull requests are welcome. Keep the change focused, add a regression test where practical, explain threat assumptions, and avoid weakening cross-platform support or accessibility without a strong reason.
