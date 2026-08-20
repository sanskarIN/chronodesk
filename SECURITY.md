# Security Policy

ChronoDesk is an offline-first cross-platform clock and world-clock application. It reads local configuration, can open user-selected/imported settings on supported hosts, integrates with user-session startup mechanisms on desktop, can open fixed support links, can use limited desktop OS facilities for optional chimes, and now has Android, iOS/iPadOS, and WebAssembly hosts. Security reports are taken seriously across every supported host.

## Supported versions

The current source/release-candidate version is **`2.6.0.2`**. Until that exact version is tagged and published after the documented release gates pass, security fixes continue to land on `main` and are included in the next verified four-part release.

After tagged releases exist, the newest supported release is the primary supported line unless release notes or a security advisory explicitly state otherwise. Do not infer support solely from a version number; consult the latest release/security information when one exists.

## Reporting a vulnerability

**Do not open a public GitHub issue for an unpatched vulnerability.**

Preferred reporting path:

1. Use the repository's private GitHub security-advisory reporting flow when available under the repository **Security** tab.
2. If that is not available, email **sanskarin@outlook.in** with the subject `ChronoDesk security report`.

Support contact: **supportramsandesh@gmail.com**.

A useful report includes:

- affected commit/tag/version;
- exact host/platform and architecture, such as Windows x64, Android, iPadOS, or Browser/WebAssembly;
- concise reproduction steps;
- security impact;
- whether user interaction is required;
- suggested mitigation if known.

Please avoid sending real passwords, tokens, signing certificates/private keys, private user files, or unrelated personal data. Use synthetic proof-of-concept data whenever possible.

## Coordinated disclosure

The maintainer will review a good-faith report, attempt to reproduce it, determine severity and affected versions/platforms, and prepare a fix or mitigation when warranted. Public disclosure should wait until a fix or practical mitigation is available unless earlier disclosure is legally required or necessary to address active harm.

No bounty is promised by this policy.

## Security boundaries

ChronoDesk intentionally:

- does not require a remote account;
- does not embed application API credentials;
- does not run a local privileged service;
- uses user-scoped startup registration only on supported desktop hosts;
- reports desktop startup integration unsupported on mobile/browser rather than invoking invalid APIs;
- limits imported settings documents to a small maximum size;
- validates settings schema and normalizes values;
- rejects numeric enum representations in imported JSON;
- bounds imported font/world-clock/timezone text and converts it to single-line values;
- preserves the current device startup preference when settings are imported;
- best-effort rolls startup integration back if the matching settings write fails;
- writes settings through a temporary file before replacement where native filesystem semantics support it;
- preserves invalid/corrupt settings data rather than executing/interpreting arbitrary content where that filesystem operation is available;
- does not quarantine a potentially valid settings file solely because of a transient read/I/O failure;
- allows only fixed `https` and `mailto` support destinations from the About window;
- uses argument lists rather than shell command strings for optional Unix desktop chime helpers;
- redacts common email/secret patterns from structured logs;
- treats browser filesystem/process/registry access as sandboxed/unsupported rather than assuming desktop privileges;
- keeps Android/iOS production signing material out of source control;
- verifies canonical and platform package-version mappings in CI;
- rejects release tags that do not exactly match the canonical application version;
- generates SHA-256 checksums for desktop/browser release ZIP artifacts;
- uses GitHub CodeQL, Dependency Review, Dependabot, and NuGet vulnerability inspection in repository automation.

## Import threat model

A settings export is user-controlled input when imported, even if its extension is `.json`. ChronoDesk therefore treats imported data as untrusted configuration rather than a command or trusted executable backup.

Current controls include:

- maximum 2 MiB input size;
- JSON parsing only; no script/template execution;
- supported schema-version check;
- string-enum parsing with numeric values disabled;
- settings normalization and bounded world-clock count;
- bounded/single-line user-display text;
- case-insensitive duplicate world-clock/timezone removal;
- no imported OS startup side effect;
- no imported executable path, URI, shell command, token, signing credential, or password field.

Import hardening has deterministic malformed-input/fuzz regression coverage in the test project.

The full import/export UI is currently a desktop workflow. Any future mobile/browser import path must preserve the same validation boundary and use the host's supported picker/storage model rather than bypassing its sandbox.

## Local settings failure model

On filesystem-backed hosts, ChronoDesk distinguishes invalid data from temporary availability failures:

- malformed/schema-invalid settings can be moved to a timestamped `.corrupt-...json` file and replaced with safe defaults;
- transient `IOException` read failures fall back to safe defaults without renaming/deleting the original settings file;
- permission failures are not bypassed and are surfaced as local-data availability problems.

This avoids turning an availability problem into unnecessary data loss.

WebAssembly runs inside the browser runtime's filesystem/storage model. Native atomic-rename/recovery semantics must not be assumed to be identical in the browser without validation.

## Platform integration notes

### Windows desktop

Startup uses the current user's `Software\\Microsoft\\Windows\\CurrentVersion\\Run` key. The application does not request administrator privileges.

### macOS desktop

Startup writes a per-user LaunchAgent file under the user's `Library/LaunchAgents` folder only when explicitly enabled.

### Linux desktop

Startup writes a per-user XDG autostart desktop file only when explicitly enabled.

### Android

The Android host uses the application sandbox and does not use desktop startup/tray/process integrations. Production distribution must use protected Android signing credentials. Keystores/passwords/private key material must never enter Git history or public CI artifacts.

### iOS / iPadOS

The Apple host uses the application sandbox and does not use desktop startup/tray/process integrations. Device/App Store distribution requires protected Apple signing/provisioning material. Certificates/private keys/provisioning secrets/App Store credentials must remain in approved protected release infrastructure.

### Browser / WebAssembly

The Browser host is sandboxed by the browser. ChronoDesk must not depend on unrestricted native filesystem access, registry writes, desktop startup integration, arbitrary process execution, or desktop tray/window-management APIs. The application is loaded from a web origin, so the hosting origin and transport security become part of deployment security even though the clock itself has no remote backend requirement.

### Chimes

Windows uses a local system beep path. macOS/Linux playback can use fixed OS executable paths and fixed system-sound arguments where present. User-provided text is not interpolated into a shell command. Current mobile/browser hosts safely degrade rather than invoking unsupported desktop process helpers.

## Release integrity

For a four-part release such as `v2.6.0.2`:

- `scripts/check-version.ps1 -Tag <tag>` must confirm the tag exactly matches canonical/project/platform metadata;
- desktop/browser release ZIPs are produced from tagged source by GitHub Actions;
- desktop ZIPs include license/readme/changelog/privacy/security/support documents;
- the Browser ZIP contains the published static site plus license/privacy documentation;
- `SHA256SUMS.txt` is generated from published ZIPs;
- downloaded artifacts should be verified against the checksum file before being treated as release evidence.

Checksums detect artifact changes after generation but do not establish publisher identity in the same way as platform signing/notarization.

Android/iOS production packages are intentionally not signed with committed/public credentials. When protected signing automation is introduced, unsigned PR validation and signed protected release jobs must remain separate.

## Dependency policy

- NuGet and GitHub Actions dependency updates are monitored by Dependabot.
- Pull requests run Dependency Review.
- CodeQL analyzes the C# shared/desktop build graph under .NET 10.
- Platform-aware CI separately compiles Android, iOS/iPadOS, and Browser hosts with the workloads required by those targets.
- Desktop CI runs `dotnet list ... --vulnerable --include-transitive` and fails when NuGet reports vulnerable packages through the expected marker.
- Dependencies should be removed when the standard library is sufficient.

## Secret handling

ChronoDesk requires no application production API secrets. Never commit:

- access tokens;
- API keys;
- passwords;
- Android `.jks`/`.keystore` files or signing passwords;
- Apple private keys/certificate private material;
- provisioning secrets;
- store automation credentials;
- real user data;
- private endpoints;
- credentials in screenshots or logs.

`.env.example` contains placeholders/configuration names only.

If signing automation is added, use protected GitHub environments/secrets or another approved secret store; fork pull requests must never receive production signing credentials.

## Hardening contributions

Security-hardening pull requests are welcome. Keep changes focused, add regression tests where practical, explain threat assumptions, preserve host sandbox boundaries, and avoid weakening cross-platform support or accessibility without a strong reason.
