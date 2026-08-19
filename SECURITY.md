# Security Policy

ChronoDesk is an offline-first desktop clock, but it still reads local configuration, opens user-selected import files, integrates with user-session startup mechanisms, can launch user-initiated project/support links, and executes limited OS facilities for optional chimes. Security reports are taken seriously.

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
- limits imported settings files to a small maximum size;
- validates settings schema and normalizes values;
- rejects numeric enum representations in imported JSON;
- bounds imported font/world-clock/timezone text and converts it to single-line values;
- preserves the current device startup preference when settings are imported;
- best-effort rolls startup integration back if the matching settings write fails;
- writes settings through a temporary file before replacement;
- preserves corrupt settings rather than executing/interpreting arbitrary content;
- routes application external-link requests through one allowlist that accepts only absolute HTTPS and mailto URIs;
- exposes update navigation only as an explicit user action that opens the public GitHub Releases page rather than running a background updater/downloader;
- uses argument lists rather than a shell command string for optional Unix chime helpers;
- redacts common email/secret patterns from structured logs;
- uses GitHub CodeQL, dependency review, Dependabot, NuGet vulnerability inspection, and a high-confidence committed-credential scan in repository automation;
- gates tagged packaging behind repository-integrity, formatting, Release build, tests, and dependency-vulnerability preflight;
- scopes GitHub release write permission to the final publication job;
- creates SHA-256 sidecars for every release archive and verifies all downloaded archive/checksum pairs before publication.

## External-link threat model

About and Settings contain project/support destinations that leave the application. They are treated as controlled external-navigation requests rather than arbitrary commands.

Current controls include:

- only absolute URIs are accepted;
- HTTPS and mailto are the only allowed schemes;
- HTTP, file, script-style, relative, and empty targets are rejected by the shared launcher policy;
- the current application uses fixed project/support destinations rather than imported/user-controlled URLs;
- failure to launch the operating-system handler is non-fatal;
- update navigation is user-initiated and does not fetch, parse, or execute release metadata inside ChronoDesk.

The URI allowlist has deterministic regression tests.

## Import threat model

A settings export is user-controlled input when it is imported, even if its extension is `.json`. ChronoDesk therefore treats an imported document as untrusted configuration rather than as a command or trusted backup.

Current controls include:

- maximum 2 MiB file size;
- JSON parsing only; no script/template execution;
- supported schema-version check;
- string-enum parsing with numeric values disabled;
- settings normalization and bounded world-clock count;
- bounded/single-line user-display text;
- no imported OS startup side effect;
- no imported executable path, URI, shell command, token, or credential field.

Import hardening has deterministic malformed-input/fuzz regression coverage in the test project.

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

The tagged Release workflow is designed to reduce accidental publication of an invalid or mismatched build:

1. validate the semantic release tag;
2. run repository-local documentation and committed-credential checks;
3. restore, verify formatting, build Release, run tests, and inspect NuGet vulnerability output;
4. stamp package/file/informational version metadata from the release tag;
5. create a Windows ZIP or Unix `tar.gz` archive appropriate to the target;
6. generate a SHA-256 sidecar per archive;
7. download all package artifacts into the publication job;
8. verify exactly four archives and four checksum files and compare each calculated digest;
9. create the GitHub Release only after those checks pass.

Checksums protect integrity verification of the published bytes; they are not code signatures and do not provide publisher identity/authenticity equivalent to platform code signing. Signing/notarization remains a separate future release capability.

## Dependency policy

- NuGet and GitHub Actions dependency updates are monitored by Dependabot.
- Pull requests run dependency review.
- CodeQL analyzes C# changes.
- CI and release preflight run `dotnet list ... --vulnerable --include-transitive` and fail when NuGet reports vulnerable packages through the expected result marker.
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

`scripts/check_repository_secrets.py` scans committed text files for high-confidence private-key and credential/token patterns as part of CI and release preflight. A passing scan is only one defense: it does not prove that every private file, screenshot, endpoint, identifier, or novel credential format is absent. Release preparation must still include a human review of staged files and generated artifacts.

If a real secret is ever committed, removing it in a later commit is not sufficient. Revoke or rotate it immediately and follow the appropriate Git history remediation process before treating the incident as resolved.

## Hardening contributions

Security-hardening pull requests are welcome. Keep the change focused, add a regression test where practical, explain threat assumptions, and avoid weakening cross-platform support or accessibility without a strong reason.
