# ChronoDesk GitHub Maintenance

This document describes repository settings that complement the files committed under `.github/`. Some settings live in GitHub rather than Git, so they must be configured by a repository administrator and kept aligned with the workflows that actually exist.

## Default branch

Default branch: `main`.

Do not force-push or rewrite published release history on `main` except for an exceptional recovery procedure with a documented reason.

## Recommended branch protection / ruleset

Apply a GitHub ruleset to `main` after successful workflow runs have established the exact check names.

Recommended controls:

- require a pull request before merge for normal collaborative changes;
- require at least one approval when independent reviewers are available;
- dismiss stale approvals when the diff changes materially;
- require conversation resolution;
- require status checks that correspond to the repository workflows;
- require branches to be up to date when strict merge-base validation is desired;
- block force pushes;
- block branch deletion;
- require linear history only if the selected merge strategy intentionally uses it;
- allow an administrator emergency path only when necessary, with follow-up documentation.

Do not invent required status-check names from badge labels. Select the exact contexts GitHub exposes after the current workflows have completed.

Expected workflow families:

- `CI`
- `CodeQL`
- `Dependency Review` on pull requests
- `Release` on verified four-part version tags

The `CI` workflow produces separate checks for:

- Desktop / Ubuntu / .NET 10
- Desktop / Windows / .NET 10
- Desktop / macOS / .NET 10
- Android / .NET 10
- iOS and iPadOS / .NET 10
- Browser / WebAssembly / .NET 10

CodeQL uses .NET 10 and an explicit shared/desktop build graph instead of full-solution autobuild, because the solution contains workload-specific Android/iOS/WebAssembly projects.

The repository source cannot prove that branch protection/rulesets are enabled. Treat actual settings verification as a release gate and record it in `what_changed.md` or release evidence.

## Merge strategy

The repository currently permits merge, squash, and rebase. For a clean public history:

- prefer squash for noisy external PR iteration when preserving individual commits adds little value;
- prefer rebase/merge when a contributor intentionally prepared multiple atomic meaningful commits worth retaining;
- never squash merely to hide security-sensitive review context that should remain documented elsewhere.

Maintainer direct commits may be appropriate during repository bootstrap or a documented emergency, but normal feature/platform work should use reviewable branches/PRs.

## Labels

Suggested label set:

| Label | Purpose |
|---|---|
| `bug` | Confirmed/reported defect |
| `enhancement` | Focused product improvement |
| `needs-triage` | Needs maintainer classification |
| `accessibility` | Accessibility-specific work |
| `security` | Public-safe security hardening only; private vulnerabilities use advisories/private reporting |
| `privacy` | Data/privacy behavior |
| `performance` | Measured performance work |
| `documentation` | Docs-only/primarily docs |
| `tests` | Test infrastructure/coverage |
| `platform-windows` | Windows-specific behavior |
| `platform-macos` | macOS-specific behavior |
| `platform-linux` | Linux-specific behavior |
| `platform-android` | Android-specific behavior |
| `platform-ios` | iPhone/iOS-specific behavior |
| `platform-ipados` | iPadOS-specific behavior |
| `platform-browser` | WebAssembly/browser-specific behavior |
| `dependencies` | Dependency automation |
| `dotnet` | .NET/NuGet updates |
| `github-actions` | Workflow action updates |
| `release` | Release/versioning/artifact/signing work |
| `good first issue` | Narrow contributor-friendly task |
| `help wanted` | Maintainer explicitly welcomes assistance |
| `blocked` | Waiting on an external prerequisite |

Issue forms already request core labels; Dependabot configuration requests dependency labels. If a requested label does not exist, create it in repository settings so automation stays consistent.

## Milestones

Use milestones for real release coordination rather than every small task. Suggested milestones:

- `v2.6.0.2 Cross-Platform Release Candidate`
- `v2.6.0.3 Maintenance` when a revision-only follow-up is needed
- `v2.7.0.0` for a future feature-oriented milestone when appropriate

A milestone should contain only work necessary or intentionally targeted for that release. Move non-blocking scope rather than keeping a release permanently open.

## Discussions

If GitHub Discussions is enabled, suggested categories:

- **Announcements** — maintainer-only release/project announcements;
- **Ideas** — early product proposals before a focused issue exists;
- **Q&A** — development/user questions that are not confirmed bugs;
- **Show and tell** — themes/workspace setups or integrations built around ChronoDesk.

Security reports must not be posted in Discussions. Route them through `SECURITY.md`.

## Issues

The repository commits YAML forms for bug reports and feature requests and disables blank issues.

Triage flow:

1. check for security-sensitive content; move disclosure to the private process if needed;
2. check for duplicate issues;
3. verify reproduction/expected behavior;
4. apply area/platform labels;
5. classify blocker/release impact;
6. link a milestone only when the issue is actually targeted;
7. close with a specific reason when not planned/duplicate/completed.

Ask reporters to name the exact platform/architecture/runtime when relevant, for example `Android`, `iPadOS`, `browser-wasm`, `win-arm64`, or `linux-x64`.

Do not ask users to publish full settings/log files when a minimal sanitized excerpt will do.

## Pull requests

The PR template requires formatting/build/test/security/privacy/accessibility review. Maintainers should additionally verify:

- the change respects the Core → Infrastructure/App → thin-host dependency rules;
- reusable behavior is not duplicated across platform hosts;
- persistent data changes include compatibility/migration thought;
- UI changes include keyboard/touch/high-contrast/narrow-width review as applicable;
- platform-specific changes have safe capability fallbacks;
- browser/mobile code does not assume unrestricted desktop APIs;
- new dependencies are justified;
- version-bearing changes preserve canonical and platform package mappings;
- production mobile signing material is never committed;
- documentation and changelog are synchronized.

Run `scripts/check-version.ps1` and `scripts/check-markdown-links.ps1` locally when PowerShell is available; CI also enforces the relevant checks.

## Dependabot

`.github/dependabot.yml` updates:

- NuGet packages weekly;
- GitHub Actions weekly.

Dependency PRs must still pass normal CI/security checks. Avoid merging a major dependency update solely because automation opened it; inspect release notes/compatibility and run affected host builds/UI smoke tests.

## CodeQL and dependency review

`CodeQL` runs on `main` pushes, pull requests, and a weekly schedule. It installs .NET 10 and manually builds the shared desktop graph so analysis is deterministic even though mobile/browser workloads are optional on that runner.

`Dependency Review` runs on pull requests and rejects dependency changes according to its configured policy.

Security tooling is defense in depth. A green automated scan does not replace source review of local process launching, startup integration, import parsing, filesystem writes, external URI handling, browser sandbox assumptions, or mobile signing configuration.

## Funding

`.github/FUNDING.yml` points to:

```text
https://buymeacoffee.com/sanskarIN
```

Funding must remain optional and non-intrusive. Do not gate issues, releases, features, or security response behind donations.

## Releases

ChronoDesk uses four-component canonical versions: `MAJOR.MINOR.PATCH.REVISION`. The current source version is `2.6.0.2`.

Tags matching `v*.*.*.*` trigger `.github/workflows/release.yml`. The workflow invokes `scripts/check-version.ps1 -Tag <tag>` and fails if the tag does not exactly match canonical project metadata.

Current automated release packages:

```text
win-x64
win-arm64
linux-x64
linux-arm64
osx-x64
osx-arm64
browser-wasm
```

Desktop ZIPs bundle license/readme/changelog/privacy/security/support documents. The Browser ZIP contains the published static site plus license/privacy documents. The release workflow publishes `SHA256SUMS.txt` covering the ZIPs.

Android/iOS/iPadOS source hosts are compiled in CI. Production store artifacts are intentionally outside unsigned public CI until protected signing/provisioning infrastructure is configured. Never add signing secrets directly to workflow YAML or Git history.

Follow `docs/release.md` before pushing a release tag. Do not tag merely to test whether workflows compile.

## Repository About section

Recommended About metadata:

**Description:**

> An offline-first cross-platform digital clock and world-clock dashboard for Windows, macOS, Linux, Android, iOS/iPadOS, and WebAssembly, built with C# .NET 10 and Avalonia UI.

**Website:**

If no dedicated project website exists, use the repository itself or leave the website field empty rather than inventing a URL.

**Topics:**

```text
clock
digital-clock
world-clock
timezone
avalonia
avalonia-ui
dotnet
csharp
cross-platform
windows
macos
linux
android
ios
ipados
webassembly
wasm
open-source
accessibility
offline-first
```

## Security repository settings

Recommended when available for the repository/account:

- private vulnerability reporting;
- Dependabot alerts;
- dependency graph;
- secret scanning/push protection;
- CodeQL/default code scanning consistent with the committed workflow;
- protected GitHub environments for any future signing/notarization/store credentials.

Do not assume a feature is enabled solely because a workflow/configuration file exists; administrators must verify settings in GitHub.

## Audit cadence

At each release candidate:

- run the canonical/platform version consistency verifier;
- review branch/ruleset check contexts;
- review stale labels/milestones;
- review dependency/security alerts;
- inspect Actions permissions;
- ensure old artifacts/workflow runs do not contain sensitive data;
- verify release checksum generation and artifact contents;
- verify protected signing environments if mobile signing automation is introduced;
- confirm About description/topics still match the supported platform matrix;
- confirm README badges point at current workflow filenames;
- verify the exact release commit is green across Desktop, Android, iOS/iPadOS, Browser, CodeQL, and dependency review.
