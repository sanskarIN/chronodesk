# ChronoDesk GitHub Maintenance

This document describes repository settings that complement the files committed under `.github/`. Some settings live in GitHub rather than Git, so they must be configured by a repository administrator and kept aligned with the workflows that actually exist.

## Default branch

Default branch: `main`.

Do not force-push or rewrite published release history on `main` except for an exceptional recovery procedure with a documented reason.

## Recommended branch protection / ruleset

Apply a GitHub ruleset to `main` after a successful workflow run has established the exact check names.

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

- `CI`;
- `CodeQL`;
- `Dependency Review` on pull requests.

The `CI` workflow includes a `Repository integrity` job and separate .NET matrix jobs. Repository integrity currently validates:

- repository-local Markdown links/images;
- complete `git ls-files` coverage in `docs/repository-reference.md`;
- high-confidence committed credential patterns;
- Python repository-validator unit tests.

The matrix CI job produces separate OS check runs; configure protection from the exact contexts visible in GitHub.

## Merge strategy

The repository currently permits merge, squash, and rebase. For a clean public history:

- prefer squash for noisy external PR iteration when preserving individual commits adds little value;
- prefer rebase/merge when a contributor intentionally prepared multiple atomic meaningful commits worth retaining;
- preserve deliberately granular maintainer phase histories when they are useful for regression isolation/auditability;
- never squash merely to hide security-sensitive review context that should remain documented elsewhere.

Maintainer direct commits may be appropriate during repository bootstrap, but normal post-bootstrap feature work should increasingly use reviewable branches/PRs.

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
| `good first issue` | Narrow contributor-friendly task |
| `help wanted` | Maintainer explicitly welcomes assistance |
| `blocked` | Waiting on an external prerequisite |

Issue forms already request `bug`, `enhancement`, and `needs-triage`; Dependabot configuration requests dependency labels. If a requested label does not exist, create it in repository settings so automation stays consistent.

## Milestones

Use milestones for real release coordination rather than every small task. Suggested milestones:

- `v0.1.0 Preview`;
- `v0.2.0 Platform Validation`;
- `v1.0.0 Stable`.

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

The PR template requires repository validators plus formatting/build/test/security/privacy/accessibility review. Maintainers should additionally verify:

- the change respects the Core/Infrastructure/App dependency rule;
- persistent data changes include compatibility/migration thought;
- UI changes have keyboard/high-contrast review;
- platform-specific changes have safe fallbacks;
- new dependencies are justified;
- documentation and changelog are synchronized;
- every added/renamed/moved/deleted tracked file has the matching `docs/repository-reference.md` update;
- test-file responsibility changes are reflected in `docs/test-catalog.md`.

If the documentation-inventory job fails, treat the reported missing/stale path as a normal PR defect rather than bypassing/removing the gate.

## Documentation governance

`docs/README.md` is the technical documentation hub and `docs/repository-reference.md` is the exhaustive tracked-file responsibility inventory.

The canonical inventory is intentionally machine enforced so small files are not exempt. GitHub templates/workflows, assets, XAML, resource catalogs, manifests, scripts, fakes, and policy docs are all part of the maintained repository and must remain represented.

When GitHub-only settings change materially (rulesets, required checks, security settings, release permissions), update this file even though the setting itself cannot be captured as a normal repository file.

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

Tags matching `v*.*.*` trigger `.github/workflows/release.yml`. Follow `docs/release.md` before pushing a release tag.

Release preflight repeats repository documentation integrity before packaging. A tag with an undocumented tracked file should fail before any platform package is created.

Do not tag merely to test whether the workflow compiles. Use a branch/PR or workflow-safe development method first; a public semantic version tag should represent an intentional release candidate/release.

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

- review branch/ruleset check contexts;
- confirm Repository integrity includes documentation inventory and passes on the exact release commit;
- review stale labels/milestones;
- review dependency/security alerts;
- inspect Actions permissions;
- ensure old artifacts/workflow runs do not contain sensitive data;
- confirm About description/topics still match the product;
- confirm README badges point at current workflow filenames;
- confirm `docs/repository-reference.md` has no missing/stale tracked-file entries.
