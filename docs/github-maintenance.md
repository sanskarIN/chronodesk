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
- require branches to be up to date when the project needs strict merge-base validation;
- block force pushes;
- block branch deletion;
- require linear history only if the selected merge strategy intentionally uses it;
- allow repository administrators an emergency path only when necessary, with follow-up documentation.

Do not invent required status-check names from badge labels. Select the exact check contexts produced by GitHub after CI has run.

Expected workflow families currently committed:

- `CI`
- `CodeQL`
- `Dependency Review` on pull requests
- `Release` on verified four-part version tags

The matrix CI job produces separate OS check runs; configure protection from the exact contexts visible in GitHub.

The repository source cannot prove that branch protection/rulesets are enabled. Treat actual repository-settings verification as a release gate and record it in `what_changed.md`/release evidence.

## Merge strategy

The repository currently permits merge, squash, and rebase. For a clean public history:

- prefer squash for noisy external PR iteration when preserving individual commits adds little value;
- prefer rebase/merge when a contributor intentionally prepared multiple atomic meaningful commits worth retaining;
- never squash merely to hide security-sensitive review context that should remain documented elsewhere.

Maintainer direct commits may be appropriate during repository bootstrap or a documented emergency, but normal post-bootstrap feature work should increasingly use reviewable branches/PRs.

## Labels

Suggested label set:

| Label | Purpose |
|---|---|
| `bug` | Confirmed/reported defect |
| `enhancement` | Focused product improvement |
| `needs-triage` | Needs maintainer classification |
| `accessibility` | Accessibility-specific work |
| `security` | Public-safe security hardening only; private vulnerabilities use advisories |
| `privacy` | Data/privacy behavior |
| `performance` | Measured performance work |
| `documentation` | Docs-only/primarily docs |
| `tests` | Test infrastructure/coverage |
| `platform-windows` | Windows-specific behavior |
| `platform-macos` | macOS-specific behavior |
| `platform-linux` | Linux-specific behavior |
| `dependencies` | Dependency automation |
| `dotnet` | .NET/NuGet updates |
| `github-actions` | Workflow action updates |
| `release` | Release/versioning/artifact work |
| `good first issue` | Narrow contributor-friendly task |
| `help wanted` | Maintainer explicitly welcomes assistance |
| `blocked` | Waiting on an external prerequisite |

Issue forms already request `bug`, `enhancement`, and `needs-triage`; Dependabot configuration requests dependency labels. If a requested label does not exist, create it in repository settings so automation stays consistent.

## Milestones

Use milestones for real release coordination rather than every small task. Suggested milestones from the current source baseline:

- `v2.6.0.2 Release Candidate`
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

Do not ask users to publish full settings/log files when a minimal sanitized excerpt will do.

## Pull requests

The PR template requires formatting/build/test/security/privacy/accessibility review. Maintainers should additionally verify:

- the change respects the Core/Infrastructure/App dependency rule;
- persistent data changes include compatibility/migration thought;
- UI changes have keyboard/high-contrast review;
- platform-specific changes have safe fallbacks;
- new dependencies are justified;
- version-bearing changes keep `Version`, `PackageVersion`, `AssemblyVersion`, and `FileVersion` synchronized;
- documentation and changelog are synchronized.

Run `scripts/check-version.ps1` and `scripts/check-markdown-links.ps1` locally when PowerShell is available; CI also enforces both checks.

## Dependabot

`.github/dependabot.yml` updates:

- NuGet packages weekly;
- GitHub Actions weekly.

Dependency PRs should still pass normal CI/security checks. Avoid merging a major dependency update solely because automation opened it; inspect release notes/compatibility and run relevant UI smoke tests.

## CodeQL and dependency review

`CodeQL` runs on main pushes, pull requests, and a weekly schedule. `Dependency Review` runs on pull requests and rejects dependency changes at the configured severity/license policy.

Security tooling is defense in depth. A green automated scan does not replace source review of local process launching, startup integration, import parsing, filesystem writes, or external URI handling.

## Funding

`.github/FUNDING.yml` points to:

```text
https://buymeacoffee.com/sanskarIN
```

Funding must remain optional and non-intrusive. Do not gate issues, releases, features, or security response behind donations.

## Releases

ChronoDesk uses four-component versions: `MAJOR.MINOR.PATCH.REVISION`. The current source version is `2.6.0.2`.

Tags matching `v*.*.*.*` trigger `.github/workflows/release.yml`. The release workflow invokes `scripts/check-version.ps1 -Tag <tag>` and fails if the tag does not exactly match the application project version.

Follow `docs/release.md` before pushing a release tag. Do not tag merely to test whether the workflow compiles. Use a branch/PR for workflow changes; a public version tag should represent an intentional, verified release candidate/release.

Release packaging creates self-contained ZIPs for the supported runtime identifiers, bundles the project license/readme/changelog/privacy/security/support documents, and publishes `SHA256SUMS.txt`. Verify downloaded ZIP hashes before declaring the release complete.

## Repository About section

Recommended About metadata:

**Description:**

> A polished, offline-first cross-platform digital clock and world-clock dashboard built with C# .NET 9 and Avalonia UI.

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
desktop
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
- CodeQL/default code scanning consistent with the committed workflow.

Do not assume a feature is enabled solely because a workflow/configuration file exists; repository administrators should verify settings in GitHub.

## Audit cadence

At each release candidate:

- run the version/tag consistency verifier;
- review branch/ruleset check contexts;
- review stale labels/milestones;
- review dependency/security alerts;
- inspect Actions permissions;
- ensure old artifacts/workflow runs do not contain sensitive data;
- verify release checksum generation and artifact contents;
- confirm About description/topics still match the product;
- confirm README badges point at current workflow filenames.
