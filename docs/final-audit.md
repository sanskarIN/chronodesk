# ChronoDesk Final Audit

This document records the release-oriented audit criteria for ChronoDesk version `2.6.0.2`. It separates checks that are enforced by repository automation from checks that require a real desktop session so the project does not claim verification that has not actually occurred.

## Scope

The final audit covers:

- source structure and dependency boundaries;
- exact four-part version metadata;
- settings persistence and import safety;
- clock, world-clock, chime, focus, mini-mode, startup, tray, and accessibility behavior;
- automated tests and CI gates;
- local documentation integrity;
- dependency and static-security checks;
- release packaging configuration and artifact integrity;
- remaining native-desktop validation.

## Current source version

The canonical application project declares all of the following as `2.6.0.2`:

- `Version`;
- `PackageVersion`;
- `AssemblyVersion`;
- `FileVersion`.

`scripts/check-version.ps1` enforces four numeric components, equality across those properties, assembly component bounds, and exact release-tag matching when `-Tag` is supplied. The About UI preserves all four assembly version components.

## Repository-level automated gates

The `CI` workflow is configured to run on Ubuntu, Windows, and macOS and performs:

1. four-part version metadata verification through `scripts/check-version.ps1`;
2. .NET 9 restore;
3. `dotnet format` verification;
4. repository-local Markdown link verification through `scripts/check-markdown-links.ps1`;
5. Release configuration build;
6. the xUnit test suite, including Avalonia headless tests and coverage collection;
7. transitive NuGet vulnerability inspection;
8. test-result artifact upload.

Separate workflows provide CodeQL analysis, pull-request dependency review, Dependabot updates, and tagged release packaging.

A configured workflow is not the same as a passing run. Before a release tag, inspect the exact release commit in GitHub Actions and require every applicable check to be green.

## Source audit findings addressed

The final source audits added or strengthened the following behavior:

- unreadable local settings fall back to safe defaults while still populating the main clock, world clocks, and timezone search;
- transient settings I/O failures do not rename/quarantine a potentially valid settings document as corrupt;
- focus mode restores the window state that existed before full-screen mode, including a maximized state;
- imported world-clock IDs and timezone IDs are deduplicated case-insensitively to keep imported state consistent with the interactive add-clock rules;
- system chime helper processes no longer redirect output streams that ChronoDesk does not consume, avoiding an unnecessary process-pipe stall risk;
- About displays the complete `2.6.0.2` version rather than truncating the revision component;
- local Markdown links are checked in CI so documentation drift becomes a build-time failure;
- version metadata is checked in CI and again against the release tag during tagged packaging.

Regression tests accompany the settings fallback, transient settings-read behavior, focus-state restoration, world-clock normalization, and About version changes.

## Release artifact hardening

The tagged release workflow now:

- accepts four-component version tags (`v*.*.*.*`);
- rejects a tag that does not exactly equal `v` + the project version;
- creates self-contained ZIPs for `win-x64`, `linux-x64`, `osx-x64`, and `osx-arm64`;
- bundles `LICENSE`, `README.md`, `CHANGELOG.md`, `PRIVACY.md`, `SECURITY.md`, and `SUPPORT.md` into each ZIP;
- generates `SHA256SUMS.txt` for all release ZIPs;
- publishes the checksum file with the GitHub Release.

A checksum file provides artifact-integrity evidence; it is not a substitute for code signing/notarization.

## Security and privacy review

The repository currently uses these relevant controls:

- no required network account or remote service for core clock operation;
- user-scoped startup integration;
- bounded settings imports;
- string-only enum deserialization for settings;
- schema-version validation;
- bounded and single-line imported text normalization;
- atomic settings writes;
- corrupt-settings preservation for invalid data where possible;
- non-destructive fallback for transient settings I/O failures;
- startup-preference protection during portable settings import;
- structured logging with redaction rules;
- external-link scheme restrictions;
- release-tag/version consistency validation;
- SHA-256 release artifact checksums;
- CodeQL, dependency review, Dependabot, and NuGet vulnerability checks.

Before every public release, additionally inspect the tagged tree for accidental credentials, private data, generated local settings, signing material, or copied logs.

## Documentation audit

The repository documentation set includes the required project, contribution, support, security, privacy, release, architecture, testing, accessibility, performance, troubleshooting, roadmap, ADR, and handoff documents.

The local link verifier checks repository-relative Markdown destinations. It intentionally does not assert that external websites are reachable, because external availability is nondeterministic and should not make an otherwise reproducible offline build fail.

## Native desktop gates still requiring manual validation

The following checks require actual supported desktop environments and must remain release gates rather than being marked complete by source inspection alone:

- Windows 11 tray behavior, minimize-to-tray, startup enable/disable, chime playback, keyboard use, and screen-reader review;
- current macOS Intel/Apple Silicon behavior for tray/menu integration, LaunchAgent startup, chime playback, keyboard use, VoiceOver review, and app lifecycle;
- representative Linux GNOME/KDE behavior for tray support, XDG autostart, available chime helpers, keyboard use, and accessibility tooling;
- real release-build screenshots;
- clean-checkout publish validation for every advertised runtime identifier;
- final branch-protection/status-check verification on GitHub;
- a successful CI/CodeQL/dependency-security run for the exact release commit;
- downloaded release ZIP checksum verification;
- exact `2.6.0.2` version display/file metadata verification on packaged binaries.

See `docs/release.md`, `docs/accessibility.md`, `ROADMAP.md`, and `docs/github-maintenance.md` for the corresponding checklists.

## Repository-settings gate

Files in Git cannot prove that GitHub repository settings are enabled. Before tagging, an administrator must confirm the actual `main` branch/ruleset protection, required check contexts, security settings, and Actions permissions in the repository configuration.

## Release decision rule

Do not publish/tag `v2.6.0.2` solely because the source audit is complete. The release requires all automated checks for the exact release commit plus the documented native-desktop and repository-settings gates above. Any failed gate is either fixed before release or explicitly re-scoped and documented in release notes.
