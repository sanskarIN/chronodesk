# ChronoDesk Final Audit

This document records the release-oriented audit criteria for ChronoDesk. It separates checks that are enforced by repository automation from checks that require a real desktop session so the project does not claim verification that has not actually occurred.

## Scope

The final audit covers:

- source structure and dependency boundaries;
- settings persistence and import safety;
- clock, world-clock, chime, focus, mini-mode, startup, tray, and accessibility behavior;
- automated tests and CI gates;
- local documentation integrity;
- dependency and static-security checks;
- release packaging configuration;
- remaining native-desktop validation.

## Repository-level automated gates

The `CI` workflow is configured to run on Ubuntu, Windows, and macOS and performs:

1. .NET 9 restore;
2. `dotnet format` verification;
3. repository-local Markdown link verification through `scripts/check-markdown-links.ps1`;
4. Release configuration build;
5. the xUnit test suite, including Avalonia headless tests and coverage collection;
6. transitive NuGet vulnerability inspection;
7. test-result artifact upload.

Separate workflows provide CodeQL analysis, pull-request dependency review, Dependabot updates, and tagged release packaging.

A configured workflow is not the same as a passing run. Before a release tag, inspect the exact release commit in GitHub Actions and require every applicable check to be green.

## Source audit findings addressed

The final source audit added or strengthened the following behavior:

- unreadable local settings fall back to safe defaults while still populating the main clock, world clocks, and timezone search;
- focus mode restores the window state that existed before full-screen mode, including a maximized state;
- imported world-clock IDs and timezone IDs are deduplicated case-insensitively to keep imported state consistent with the interactive add-clock rules;
- system chime helper processes no longer redirect output streams that ChronoDesk does not consume, avoiding an unnecessary process-pipe stall risk;
- local Markdown links are checked in CI so documentation drift becomes a build-time failure.

Regression tests accompany the settings fallback, focus-state restoration, and world-clock normalization changes.

## Security and privacy review

The repository currently uses these relevant controls:

- no required network account or remote service for core clock operation;
- user-scoped startup integration;
- bounded settings imports;
- string-only enum deserialization for settings;
- schema-version validation;
- bounded and single-line imported text normalization;
- atomic settings writes;
- corrupt-settings preservation where possible;
- startup-preference protection during portable settings import;
- structured logging with redaction rules;
- external-link scheme restrictions;
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
- a successful CI/CodeQL/dependency-security run for the exact release commit.

See `docs/release.md`, `docs/accessibility.md`, `ROADMAP.md`, and `docs/github-maintenance.md` for the corresponding checklists.

## Release decision rule

Do not publish a stable `v1.0.0` solely because the source audit is complete. A stable release requires all automated checks for the release commit plus the documented native-desktop gates above. Any failed gate is either fixed before release or explicitly re-scoped and documented in release notes.
