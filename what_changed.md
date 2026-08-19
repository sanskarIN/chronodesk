# ChronoDesk — Work Handoff

## Current milestone

**Phase 7 — version `2.6.0.2` final source/repository/release hardening, 2026-08-19.**

The product/source baseline is now explicitly versioned as `2.6.0.2`. This pass fixes the remaining version-display and persistence edge cases, strengthens CI/release automation, expands release artifact integrity controls, and synchronizes the open-source documentation. Native desktop validation remains a release gate and is not fabricated by source inspection.

## Source of truth

- Repository: `https://github.com/sanskarIN/chronodesk`
- Default branch: `main`
- Version-hardening branch: `release-version-2.6.0.2`
- Pull request: `#18` — `release: finalize ChronoDesk version 2.6.0.2 hardening`
- `main` baseline before this pass: `acadb0e3861721bf72d90bdbb2c0282ef96b847d`
- PR head before this final review-state handoff update: `b6d35c103235b57f187d79523e0554128ab565e2`
- Canonical product version source: `src/ChronoDesk.App/ChronoDesk.App.csproj`
- Required version: `2.6.0.2`
- Product requirements: `10_chronodesk_master_prompt.md` supplied for the project plus the checked-in repository documentation.

## Version state

The application project now declares all of these as exactly `2.6.0.2`:

- `Version`
- `PackageVersion`
- `AssemblyVersion`
- `FileVersion`

The old `0.1.0-preview` metadata and three-component release guidance have been removed from the active release path.

The About window now renders all four assembly-version components. A headless Avalonia regression test requires `2.6.0.2` to be present so the revision component cannot silently disappear again.

## Final code/reliability fixes in this pass

### Full four-part About version

`AboutWindow` previously used `Version.ToString(3)`, which would display `2.6.0` even when assembly metadata was `2.6.0.2`. It now uses all four components and the UI smoke test verifies the exact value. The final assertion uses the basic xUnit string-containment overload to minimize test-framework compatibility risk.

### Non-destructive transient settings fallback

`JsonSettingsStore.LoadAsync` previously grouped `IOException` with malformed/schema-invalid settings and then attempted corrupt-file quarantine. That could convert a temporary availability/read problem into an unnecessary rename attempt against potentially valid data.

The loader now separates the cases:

- malformed/schema-invalid settings: safe defaults plus timestamped corrupt-file preservation where possible;
- transient `IOException`: safe defaults without renaming/deleting the original settings file;
- permission failures remain application-level local-data availability errors and are not bypassed.

A regression test locks a valid settings file, verifies safe fallback without a `.corrupt-*` rename, releases the lock, and verifies normal settings loading resumes.

## Release/version tooling added

### `scripts/check-version.ps1`

The new verifier enforces:

- exactly four numeric version components (`MAJOR.MINOR.PATCH.REVISION`);
- matching `Version`, `PackageVersion`, `AssemblyVersion`, and `FileVersion`;
- valid assembly-version component bounds;
- no conflicting `VersionPrefix` / `VersionSuffix` values;
- exact `v<version>` tag matching when `-Tag` is supplied.

### CI

The three-platform CI matrix now runs `scripts/check-version.ps1` before restore/build/test work, in addition to formatting, Markdown-link verification, tests, coverage collection, and vulnerability inspection.

### Tagged release workflow

Release workflow hardening now includes:

- four-component tag trigger: `v*.*.*.*`;
- exact tag/project-version verification before packaging;
- self-contained packages for `win-x64`, `linux-x64`, `osx-x64`, and `osx-arm64`;
- release ZIP copies of `LICENSE`, `README.md`, `CHANGELOG.md`, `PRIVACY.md`, `SECURITY.md`, and `SUPPORT.md`;
- generated `SHA256SUMS.txt` for all release ZIPs;
- checksum publication with the GitHub Release.

The release tag `v2.6.0.2` has **not** been created. It remains blocked on the documented clean-checkout, CI/security, native desktop, accessibility, screenshot, branch/ruleset, and packaged-artifact verification gates.

## Documentation synchronized in this pass

Updated documentation now consistently describes `2.6.0.2`, four-component release tags, the version verifier, persistence behavior, bundled release documents, and checksums:

- `README.md`
- `CHANGELOG.md`
- `ROADMAP.md`
- `CONTRIBUTING.md`
- `PRIVACY.md`
- `SECURITY.md`
- `docs/testing.md`
- `docs/release.md`
- `docs/final-audit.md`
- `docs/release-notes-template.md`
- `docs/github-maintenance.md`
- `.github/pull_request_template.md`
- this handoff file

## Files changed in this pass

PR #18 reports exactly 21 changed files:

- `.github/pull_request_template.md`
- `.github/workflows/ci.yml`
- `.github/workflows/release.yml`
- `CHANGELOG.md`
- `CONTRIBUTING.md`
- `PRIVACY.md`
- `README.md`
- `ROADMAP.md`
- `SECURITY.md`
- `docs/final-audit.md`
- `docs/github-maintenance.md`
- `docs/release-notes-template.md`
- `docs/release.md`
- `docs/testing.md`
- `scripts/check-version.ps1`
- `src/ChronoDesk.App/ChronoDesk.App.csproj`
- `src/ChronoDesk.App/Views/AboutWindow.axaml.cs`
- `src/ChronoDesk.Infrastructure/Persistence/JsonSettingsStore.cs`
- `tests/ChronoDesk.Tests/HeadlessUiSmokeTests.cs`
- `tests/ChronoDesk.Tests/JsonSettingsStoreTests.cs`
- `what_changed.md`

## Verification status

### Completed by repository/source inspection

- Required version metadata changed to `2.6.0.2`: **completed**.
- About four-component rendering defect identified and fixed: **completed**.
- Regression coverage for About version rendering added: **completed**.
- Transient settings-read quarantine risk identified and fixed: **completed**.
- Regression coverage for locked valid settings added: **completed**.
- Version consistency/tag verifier added: **completed**.
- CI integration for version verification added: **completed**.
- Four-component release tag policy implemented: **completed**.
- Release ZIP policy/support docs bundling added: **completed**.
- SHA-256 checksum generation/publishing added: **completed**.
- Release/testing/security/privacy/contributor/maintenance documentation synchronized: **completed**.
- Complete PR #18 changed-file list reviewed: **completed**.
- Complete PR #18 unified diff reviewed for version drift, workflow/script issues, test compile risk, persistence behavior, and documentation contradictions: **completed**.
- GitHub currently reports PR #18 as **mergeable**.
- Commit author/committer metadata observed on this branch: **Sanskar `<sanskarin@outlook.in>`**.

### Automated workflow state observed for the reviewed PR head

For PR head `b6d35c103235b57f187d79523e0554128ab565e2`, GitHub created these pull-request workflow runs:

- CI run `332` / run id `32252847982` — **queued** when observed;
- CodeQL run `331` / run id `32252847871` — **queued** when observed;
- Dependency Review run `269` / run id `32252848578` — **queued** when observed.

Queued is not passing evidence. These conclusions must not be rewritten as successful unless GitHub later reports success.

### Repository settings observed

The actual GitHub `main` branch was observed as **not protected** at commit `acadb0e3861721bf72d90bdbb2c0282ef96b847d` (`protected: false`). Branch protection/rulesets are GitHub repository settings rather than files in the source tree.

The available GitHub connector in this pass exposes branch/ref operations but does not expose a branch-protection/ruleset mutation action. Therefore the source documentation is prepared, but an administrator must enable/verify the desired `main` ruleset in GitHub settings before release.

### Local execution limitation

This chat execution environment did not provide `dotnet` or `pwsh` for an authoritative local build/test/script run. Therefore no local PASS claim is invented.

The expected automated verification for the exact PR/release commit is:

```text
./scripts/check-version.ps1
./scripts/check-markdown-links.ps1
dotnet restore ChronoDesk.sln
dotnet format ChronoDesk.sln --verify-no-changes --no-restore
dotnet build ChronoDesk.sln --configuration Release --no-restore
dotnet test ChronoDesk.sln --configuration Release --no-build --collect:"XPlat Code Coverage"
dotnet list ChronoDesk.sln package --vulnerable --include-transitive
```

For the actual tag, additionally:

```text
./scripts/check-version.ps1 -Tag "v2.6.0.2"
```

## Remaining release evidence (not source-code omissions)

- Green CI/CodeQL/dependency-security results for the exact release commit.
- Windows 11 tray/minimize/startup/chime/keyboard/accessibility validation.
- macOS Intel/Apple Silicon tray/startup/chime/VoiceOver/lifecycle validation.
- Linux GNOME/KDE tray/XDG-autostart/chime/accessibility validation.
- Real screenshots from verified release builds.
- Clean-checkout publish/launch validation for every advertised RID.
- Actual GitHub `main` branch ruleset/protection and required-status-check configuration.
- Exact tagged-tree secret/private-data/documentation review.
- Downloaded ZIP SHA-256 verification against `SHA256SUMS.txt`.
- Packaged About/file metadata confirmation of `2.6.0.2`.
- A real prior-version migration fixture when a prior tagged build exists.

These are deliberately left open until evidence exists.

## Commits created in the `2.6.0.2` pass before this final review-state handoff

- `b117e95` — `build: set ChronoDesk version to 2.6.0.2`
- `7e068de` — `fix: display full four-part application version`
- `2ffa7bd` — `test: verify full four-part About version`
- `a574306` — `chore: add release version consistency verifier`
- `e8c3319` — `ci: verify four-part version metadata`
- `0c24b92` — `ci: harden four-part release packaging`
- `5cad868` — `fix: preserve settings on transient read failures`
- `2c59f27` — `test: preserve valid settings across transient read failures`
- `41c71cf` — `test: make About version assertion nullable-safe`
- `4b5d305` — `docs: adopt ChronoDesk 2.6.0.2 release versioning`
- `ccd3d53` — `docs: document version and persistence regression gates`
- `2a158f2` — `docs: align roadmap with version 2.6.0.2`
- `5e4c1a4` — `docs: record 2.6.0.2 final hardening changes`
- `8ab45bb` — `docs: publish 2.6.0.2 source version guidance`
- `f132c80` — `docs: clarify transient settings read privacy behavior`
- `ef2638e` — `docs: finalize 2.6.0.2 audit criteria`
- `ec53bdc` — `docs: update release notes for four-part versions`
- `54dde6d` — `docs: align GitHub maintenance with 2.6.0.2 releases`
- `aed97cc` — `docs: add version verification to contributor workflow`
- `8e121ec` — `docs: add version checks to pull request template`
- `7dafcf5` — `docs: align security policy with 2.6.0.2 hardening`
- `2b2b2cc` — `docs: record 2.6.0.2 final release hardening handoff`
- `b6d35c1` — `test: simplify About version assertion`
- final PR review-state handoff: this commit.

## Next exact tasks

1. Merge PR #18 with normal merge history if its head remains unchanged and mergeable.
2. Re-check `main` after merge and record the merge commit in this handoff.
3. Observe any available workflow state without inventing a successful conclusion.
4. Do **not** create `v2.6.0.2` until the remaining release-evidence gates above are actually satisfied.
