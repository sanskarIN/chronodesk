# ChronoDesk CI/CD Reference

This document describes every automated repository quality, security, dependency, and release workflow in ChronoDesk, including triggers, permissions, required behavior, artifact flow, and maintainer expectations.

## Automation files

Primary automation lives in:

```text
.github/workflows/ci.yml
.github/workflows/codeql.yml
.github/workflows/dependency-review.yml
.github/workflows/release.yml
.github/dependabot.yml
scripts/check_markdown_links.py
scripts/check_documentation_inventory.py
scripts/check_repository_secrets.py
scripts/check_release_metadata.py
scripts/tests/
```

## CI workflow

File: `.github/workflows/ci.yml`.

Workflow name: `CI`.

Triggers:

- push to `main`;
- pull request targeting `main`.

Default permission:

```text
contents: read
```

### Concurrency

CI uses a workflow/ref concurrency group with `cancel-in-progress: true`.

Implication: when multiple commits are pushed quickly to the same PR, earlier runs may show as cancelled even though the branch is healthy. Release/merge decisions must use the newest head commit's checks, not an older cancelled run.

### Repository integrity job

Expected job/check name:

```text
Repository integrity
```

Runner: `ubuntu-latest`.

Steps:

1. checkout;
2. validate local Markdown links;
3. validate tracked-file documentation inventory;
4. scan committed text for common high-confidence credential patterns;
5. run Python standard-library repository-script tests.

This job intentionally does not require the .NET SDK and can catch documentation/repository defects early.

### .NET build/test matrix

Matrix:

```text
ubuntu-latest / .NET 9
windows-latest / .NET 9
macos-latest / .NET 9
```

For each OS:

1. checkout;
2. install/cache .NET 9;
3. restore the solution;
4. verify `dotnet format` reports no changes;
5. build Release without restoring again;
6. run Release tests without rebuilding, collecting XPlat code coverage into `TestResults`;
7. inspect direct/transitive NuGet packages for known vulnerabilities;
8. upload test results even when an earlier step fails.

Test-result artifacts are retained for 14 days.

### Vulnerable-package gate

CI runs:

```text
dotnet list ChronoDesk.sln package --vulnerable --include-transitive
```

The workflow fails when the command itself fails or the output reports vulnerable packages.

This check complements, rather than replaces, Dependency Review and Dependabot.

## CodeQL workflow

File: `.github/workflows/codeql.yml`.

Workflow name: `CodeQL`.

Expected job/check name:

```text
CodeQL / C#
```

Triggers:

- push to `main`;
- pull request targeting `main`;
- scheduled every Tuesday at 04:23 UTC.

Permissions:

- contents read;
- security-events write;
- packages read.

Execution:

1. checkout;
2. install .NET 9;
3. initialize CodeQL for C# using autobuild;
4. analyze and upload the C# result category.

CodeQL is a static-analysis layer. It does not prove runtime platform safety or replace manual security review.

## Dependency Review workflow

File: `.github/workflows/dependency-review.yml`.

Workflow name: `Dependency Review`.

Trigger: pull requests targeting `main`.

Permission: contents read.

Policy:

- fail on dependency findings at `moderate` severity or higher;
- deny newly introduced GPL-3.0 and AGPL-3.0 licenses through the configured action policy.

Expected job/check name:

```text
dependency-review
```

This evaluates dependency changes in the PR. The NuGet vulnerability command in CI evaluates the resolved dependency graph independently.

## Dependabot

File: `.github/dependabot.yml`.

### NuGet updates

Schedule:

- weekly;
- Monday;
- 05:00 Asia/Kolkata.

Open PR limit: 10.

Labels:

- `dependencies`;
- `dotnet`.

Commit prefix:

```text
chore(deps)
```

### GitHub Actions updates

Schedule:

- weekly;
- Monday;
- 05:15 Asia/Kolkata.

Open PR limit: 5.

Labels:

- `dependencies`;
- `github-actions`.

Commit prefix:

```text
chore(actions)
```

NuGet and Actions are intentionally separate ecosystems so update PRs stay scoped and reviewable.

## Repository validation scripts

### Markdown link validator

File: `scripts/check_markdown_links.py`.

Purpose:

- scan repository Markdown;
- validate local inline/reference link and image targets;
- reject repository-escaping local paths;
- report missing targets;
- avoid network-dependent external link availability checks.

External websites can disappear or rate-limit CI, so deterministic local-link validation is preferred for required CI.

### Documentation inventory validator

File: `scripts/check_documentation_inventory.py`.

Purpose:

- obtain the authoritative tracked file list from Git;
- parse `docs/repository-reference.md` inventory entries;
- fail when a tracked file is missing from the reference;
- fail when the reference lists a file that is no longer tracked;
- make file-level documentation completeness an enforceable repository invariant.

When adding, moving, or deleting any tracked file, update the canonical repository reference in the same change.

### Credential-pattern scanner

File: `scripts/check_repository_secrets.py`.

Purpose:

- inspect committed text for common high-confidence private-key/token/credential formats;
- avoid printing the matched secret value;
- report file/line and finding type;
- skip binary/oversized/generated output categories.

This is a defense-in-depth check, not a complete secret-detection product. A real exposed credential must be revoked/rotated and repository history assessed even if it is subsequently deleted from the working tree.

### Release metadata validator

File: `scripts/check_release_metadata.py`.

Executed during tag preflight.

It validates:

- supported semantic release tag form;
- exact matching release heading in `CHANGELOG.md`;
- removal/replacement of the explicit README screenshot placeholder before release packaging.

The repository can therefore remain intentionally unreleasable while development documentation still says `[Unreleased]` and the screenshot placeholder is present.

## Python validator tests

Directory: `scripts/tests/`.

These tests use Python's standard `unittest` framework so repository-integrity checks do not require third-party Python dependencies.

At minimum the release metadata validator's stable/prerelease success and failure conditions are regression-tested. Add tests whenever validator parsing or policy becomes nontrivial.

## Release workflow

File: `.github/workflows/release.yml`.

Workflow name: `Release`.

Trigger:

```text
push tag v*.*.*
```

The tag glob starts the workflow, but preflight applies stricter semantic-version validation before packaging.

Default permission:

```text
contents: read
```

Only the final release publication job receives `contents: write`.

## Release preflight

Job name:

```text
Release preflight
```

Runner: Ubuntu.

### Version resolution

Supported tag shape is conceptually:

```text
vMAJOR.MINOR.PATCH
vMAJOR.MINOR.PATCH-prerelease.identifiers
```

Examples:

```text
v1.0.0
v1.0.0-rc.1
```

Preflight produces:

- semantic `version` without the leading `v`;
- four-part `assembly_version` ending in `.0`;
- boolean prerelease state.

These outputs are passed to every package job.

### Preflight gates

The tag must pass:

1. semantic version resolution;
2. release metadata validation;
3. Markdown local-link validation;
4. documentation inventory validation;
5. committed-credential scan;
6. .NET 9 setup;
7. restore;
8. formatting verification;
9. Release build;
10. tests;
11. direct/transitive NuGet vulnerability inspection.

Package jobs depend on successful preflight.

## Package matrix

Release package jobs target:

| Runner | RID | Archive |
|---|---|---|
| Windows | `win-x64` | `.zip` |
| Ubuntu | `linux-x64` | `.tar.gz` |
| macOS | `osx-x64` | `.tar.gz` |
| macOS | `osx-arm64` | `.tar.gz` |

Each job:

1. checks out source;
2. installs .NET 9;
3. restores;
4. runs Release tests;
5. publishes the App as self-contained single-file output;
6. stamps version properties from the release tag;
7. creates the platform archive;
8. generates a SHA-256 sidecar;
9. uploads archive + checksum as an Actions artifact retained for 14 days.

### Tagged version stamping

Package publish overrides:

- `Version`;
- `AssemblyVersion`;
- `FileVersion`;
- `InformationalVersion`.

This prevents development metadata such as `0.1.0-preview` from leaking into a tagged `v1.0.0` package.

### Archive choice

Windows uses ZIP.

Linux/macOS use tar.gz so executable permission bits can be preserved more reliably than in a cross-platform ZIP extraction flow.

## Release checksum publication

For every archive, the package job writes a sibling:

```text
<archive>.sha256
```

The line contains lowercase SHA-256 followed by two spaces and the archive filename.

The final release job downloads all four package artifacts and **re-verifies** the downloaded archives against their checksum sidecars before publication.

Expected totals:

- four archives;
- four checksum files.

The release job rejects malformed checksum files, missing referenced archives, count mismatches, and hash mismatches.

A checksum verifies integrity. It does not by itself authenticate a publisher like platform code signing/notarization would.

## GitHub Release publication

Final job:

```text
Publish GitHub release
```

Dependencies: release preflight + all package matrix jobs.

Permission escalation: `contents: write` only for this job.

Publication uses `gh release create` with:

- verified existing tag;
- generated release notes;
- title `ChronoDesk <tag>`;
- all archive/checksum assets;
- `--prerelease` when the semantic tag contains a prerelease suffix.

If package/checksum verification fails, the GitHub Release is not created.

## Release blockers outside automation

Automated success is necessary but not sufficient for a stable release.

Manual/evidence-based gates include:

- Windows tray/startup/chime/file picker;
- macOS x64/arm64 tray/menu/startup/chime/file picker;
- Linux GNOME/KDE tray/startup/chime/file picker;
- keyboard/focus/high-contrast/scaling/screen-reader checks;
- real packaged-app startup from extracted artifacts;
- clean-checkout release verification;
- screenshot captured from a verified release build;
- final manual private-data/artifact review;
- code-signing/notarization work if/when the project claims signed publisher identity.

See `release.md` and `accessibility.md`.

## Branch protection guidance

Branch protection should use the **actual check names shown by GitHub** after workflows run. Expected current checks include:

```text
CI / Repository integrity
CI / ubuntu-latest / .NET 9
CI / windows-latest / .NET 9
CI / macos-latest / .NET 9
CodeQL / CodeQL / C#
Dependency Review / dependency-review
```

GitHub may render workflow/job names differently in branch-protection UI. Confirm the names from a real successful PR rather than copying this list blindly.

Do not require a release workflow on ordinary PRs because it is tag-triggered.

## Workflow action version maintenance

GitHub Actions dependencies are maintained through Dependabot. Treat action-version PRs as executable supply-chain changes:

- review release notes and permissions;
- ensure major-version syntax/inputs remain compatible;
- avoid broad permission increases;
- merge only after normal PR checks are green.

## Workflow security rules

When editing workflows:

- keep default permissions minimal;
- grant write permissions only to the exact job that needs them;
- avoid executing untrusted PR text as shell code;
- pass dynamic values through supported environment/argument mechanisms;
- never echo credentials;
- prefer pinned major action versions maintained by Dependabot;
- keep release creation downstream of verification;
- preserve checksum verification after artifact download;
- document new secrets/permissions in `SECURITY.md` and release docs.

## Diagnosing CI

When a PR check fails:

1. confirm the failing run belongs to the current PR head SHA;
2. identify the failing job and step;
3. download/read logs or test artifacts;
4. reproduce locally where possible;
5. add a focused fix/regression test;
6. push a new commit;
7. remember that `cancel-in-progress` will cancel the older CI run;
8. require the new head's checks before merge.

Queued/cancelled is not equivalent to passed.

## Changing automation checklist

Any CI/CD change should update:

- this document;
- `docs/testing.md` if test commands/gates change;
- `docs/release.md` if release semantics change;
- `SECURITY.md` for permission/supply-chain changes;
- `README.md` if public build/artifact instructions change;
- `CHANGELOG.md` when user/release behavior changes;
- `docs/repository-reference.md` when files are added/moved/deleted.
