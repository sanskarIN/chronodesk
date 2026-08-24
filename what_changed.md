# ChronoDesk — Work Handoff

## Current continuation — 2026-08-24

**Phase 7 — version `2.7.0.0` next-version development is active in draft PR #21.**

This continuation deliberately keeps the unreleased `2.6.0.2` release candidate on `main` intact while next-version work proceeds on a separate development branch.

### Current source of truth

- Repository: `https://github.com/sanskarIN/chronodesk`
- Default/release-candidate branch: `main`
- Next-version branch: `next-version-2.7.0.0`
- Next-version baseline: `d56f21e17cf4b4723bce62b1d942947ed0660ebb`
- Draft pull request: `#21` — `feat: prepare ChronoDesk 2.7.0.0 development`
- PR state when last inspected before this handoff update: **open, draft, mergeable**
- PR head before this handoff update: `af231b78e83d96062f64e73d28c269e425201450`
- PR snapshot before this handoff update: 21 commits, 10 changed files, 294 additions, 17 deletions
- Canonical version source: `src/ChronoDesk.App/ChronoDesk.App.csproj`
- Next-version metadata on the development branch: `2.7.0.0`

PR #21 must remain unmerged until the `2.6.0.2` release/tag decision is complete. The branch exists so next-version implementation and CI review can continue without overwriting the release-candidate state on `main`.

## Work completed in this continuation

### Prepared the `2.7.0.0` development line

- Created `next-version-2.7.0.0` from the current `main` baseline.
- Set `Version`, `PackageVersion`, `AssemblyVersion`, and `FileVersion` to `2.7.0.0` on that branch only.
- Opened draft PR #21 against `main` for review and automated validation.
- Added `docs/next-version.md` as the dedicated next-version development handoff.
- Added a separate `2.7.0.0` section to `CHANGELOG.md` while retaining the `2.6.0.2` release-candidate history.
- Added Phase 7 to `ROADMAP.md` so implemented and remaining next-version work is explicit.

### Added editable world-clock labels

Saved world-clock cards now support local label editing without changing the clock identity or timezone association.

Implemented behavior:

- each world-clock card has an editable label field;
- `Save changes` persists through the existing settings store;
- Enter saves from the keyboard;
- Escape cancels and restores the persisted label;
- blank submissions restore/retain the prior value and do not cause a settings write;
- leading/trailing whitespace is trimmed;
- existing `AppSettings.Normalize()` safeguards continue to enforce single-line, length-bounded labels;
- world-clock IDs and timezone IDs remain unchanged during rename;
- user-visible rename status is generated from the normalized persisted value rather than raw editor input;
- Enter is marked handled before asynchronous persistence begins so the keystroke does not bubble during the save;
- editor and action controls were split into separate rows with bounded card widths to improve narrow desktop layout behavior;
- the label editor carries an automation name derived from the saved clock label.

### Added and hardened regression coverage

`MainWindowViewModelTests` now verifies:

- normalized label persistence;
- unchanged world-clock identity/timezone during rename;
- rebuilt in-memory world-clock state after persistence;
- normalized rename status text;
- blank-label rejection without persistence.

The About headless smoke test no longer embeds the previous release literal `2.6.0.2`. It derives the expected four-part version from the application assembly, so ordinary version bumps do not create a stale-test failure.

### Source review defects caught during the continuation

The following issues were found and fixed before the branch was treated as ready for CI evidence:

1. The About smoke test still hardcoded `2.6.0.2` after the project version moved to `2.7.0.0`.
2. A blank label could leave an empty editor value even though persistence correctly rejected it.
3. Rename status initially used pre-normalized editor text instead of the saved normalized label.
4. Enter was initially marked handled only after awaiting persistence, allowing the key event to remain unhandled during the asynchronous gap.
5. The first inline editor layout forced the text field and both action buttons into one narrow row; the card was restructured for better responsive behavior.

## Current automated validation state

For PR head `af231b78e83d96062f64e73d28c269e425201450`, GitHub created these pull-request workflow runs immediately before this handoff update:

- CI run `419` / run id `32732716632` — **queued** when observed;
- CodeQL run `418` / run id `32732716841` — **queued** when observed;
- Dependency Review run `352` / run id `32732716731` — **queued** when observed.

Queued is not passing evidence. This `what_changed.md` commit changes the branch head again, so final automated evidence must be taken from the newest PR head after the documentation commit, not from these superseded runs.

This chat environment still does not provide the .NET SDK, so no local build/test PASS is invented. The repository CI remains the authoritative automated build/test/security evidence.

Expected validation remains:

```text
./scripts/check-version.ps1
./scripts/check-markdown-links.ps1
dotnet restore ChronoDesk.sln
dotnet format ChronoDesk.sln --verify-no-changes --no-restore
dotnet build ChronoDesk.sln --configuration Release --no-restore
dotnet test ChronoDesk.sln --configuration Release --no-build --collect:"XPlat Code Coverage"
dotnet list ChronoDesk.sln package --vulnerable --include-transitive
```

## Commits created for the current `2.7.0.0` continuation before this handoff update

- `2fc0f58` — `build: prepare ChronoDesk version 2.7.0.0`
- `614284d` — `feat: add editable world clock labels`
- `382e39c` — `feat: persist world clock label changes`
- `2b8495b` — `feat: add inline world clock label editor`
- `b049276` — `feat: wire world clock label save action`
- `cc359bd` — `test: cover world clock label editing`
- `2c213ee` — `fix: restore blank world clock labels in editor`
- `302fe2d` — `docs: add 2.7.0.0 development handoff`
- `283a8b8` — `test: derive About version from app assembly`
- `afa3419` — `fix: report normalized world clock labels`
- `0249423` — `test: verify normalized world clock rename status`
- `aaa8c01` — `feat: add keyboard handling to world clock labels`
- `1259a44` — `feat: save or cancel world clock labels by keyboard`
- `c7edd06` — `docs: update 2.7.0.0 feature and validation handoff`
- `8ee8c19` — `docs: separate 2.7.0.0 development changelog`
- `5cb2ce5` — `docs: add 2.7.0.0 development roadmap phase`
- `d6f174b` — `fix: handle Enter before asynchronous label save`
- `714ec61` — `ui: improve responsive world clock editor cards`
- `0813ce1` — `docs: mark responsive world clock cards complete`
- `c51eecf` — `docs: record responsive editor and keyboard hardening`
- `af231b7` — `docs: record responsive world clock editor changes`

## Next exact `2.7.0.0` work

The next-version source slice is substantially prepared. Remaining next-version work should proceed in this order:

1. obtain green CI, CodeQL, and Dependency Review evidence for the final branch head;
2. add deeper headless interaction coverage for templated world-clock editing only if the Avalonia headless harness can exercise the template reliably without brittle implementation coupling;
3. continue runtime-language switching only after a reliable live-resource refresh architecture is defined;
4. consider richer offline calendar details without introducing accounts, tracking, or mandatory network dependencies;
5. strengthen automated accessibility checks where tooling produces stable cross-platform evidence;
6. evaluate signed/notarized installers only when real signing infrastructure exists;
7. keep PR #21 draft/unmerged until the `2.6.0.2` release/tag decision is complete.

## `2.6.0.2` release gates remain unchanged

Starting `2.7.0.0` development does **not** satisfy or remove the `2.6.0.2` release gates. The release candidate still requires:

- green CI/CodeQL/dependency-security evidence for the exact release commit;
- Windows 11 tray/minimize/startup/chime/keyboard/accessibility validation;
- macOS Intel/Apple Silicon tray/startup/chime/VoiceOver/lifecycle validation;
- Linux GNOME/KDE tray/XDG-autostart/chime/accessibility validation;
- real screenshots from verified release builds;
- clean-checkout publish/launch validation for every advertised RID;
- actual GitHub `main` branch protection/ruleset and required-status-check configuration;
- exact tagged-tree secret/private-data/documentation review;
- downloaded ZIP checksum verification against `SHA256SUMS.txt`;
- packaged About/file metadata confirmation;
- a real prior-version migration fixture when one exists.

The `v2.6.0.2` tag remains intentionally uncreated until those gates have real evidence.

---

# Historical `2.6.0.2` Handoff — retained for release continuity

## Current milestone

**Phase 7 — version `2.6.0.2` final source/repository/release hardening, merged 2026-08-19.**

The product/source baseline is explicitly versioned as `2.6.0.2`. The final source, reliability, release-automation, test, security/privacy, and open-source documentation hardening pass has been merged into `main`. Native desktop validation and repository settings remain release-evidence gates; they are not fabricated by source inspection.

## Source of truth

- Repository: `https://github.com/sanskarIN/chronodesk`
- Default branch: `main`
- Version-hardening branch: `release-version-2.6.0.2`
- Pull request: `#18` — `release: finalize ChronoDesk version 2.6.0.2 hardening`
- `main` baseline before this pass: `acadb0e3861721bf72d90bdbb2c0282ef96b847d`
- Final PR head merged: `011711503703c7cdc64120cafe9dbb5fdc11e0f5`
- Merge commit: `d8179bdcac162059968c1700711e09e6ce904f63`
- Canonical product version source: `src/ChronoDesk.App/ChronoDesk.App.csproj`
- Required version: `2.6.0.2`
- Product requirements: `10_chronodesk_master_prompt.md` supplied for the project plus the checked-in repository documentation.

## Merge result

PR #18 was reviewed and merged successfully into `main` using a normal merge commit, preserving the 24 atomic commits from the version-hardening branch. GitHub reports the PR as closed/merged with 21 changed files.

The merge commit is GitHub-verified and records the author as **Sanskar `<sanskarin@outlook.in>`**.

The `v2.6.0.2` tag was intentionally **not** created. A release tag is still gated on the release evidence listed below.

## Version state

The application project declares all of these as exactly `2.6.0.2`:

- `Version`
- `PackageVersion`
- `AssemblyVersion`
- `FileVersion`

The old `0.1.0-preview` metadata and three-component release guidance have been removed from the active release path.

The About window renders all four assembly-version components. A headless Avalonia regression test requires `2.6.0.2` to be present so the revision component cannot silently disappear again.

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

The verifier enforces:

- exactly four numeric version components (`MAJOR.MINOR.PATCH.REVISION`);
- matching `Version`, `PackageVersion`, `AssemblyVersion`, and `FileVersion`;
- valid assembly-version component bounds;
- no conflicting `VersionPrefix` / `VersionSuffix` values;
- exact `v<version>` tag matching when `-Tag` is supplied.

### CI

The three-platform CI matrix runs `scripts/check-version.ps1` before restore/build/test work, in addition to formatting, Markdown-link verification, tests, coverage collection, and vulnerability inspection.

### Tagged release workflow

Release workflow hardening includes:

- four-component tag trigger: `v*.*.*.*`;
- exact tag/project-version verification before packaging;
- self-contained packages for `win-x64`, `linux-x64`, `osx-x64`, and `osx-arm64`;
- release ZIP copies of `LICENSE`, `README.md`, `CHANGELOG.md`, `PRIVACY.md`, `SECURITY.md`, and `SUPPORT.md`;
- generated `SHA256SUMS.txt` for all release ZIPs;
- checksum publication with the GitHub Release.

## Documentation synchronized in this pass

The following now consistently describe `2.6.0.2`, four-component release tags, the version verifier, persistence behavior, bundled release documents, and checksums:

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

## Files changed by PR #18

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
- PR #18 mergeability before merge: **PASS**.
- PR #18 merge to `main`: **PASS**.
- Merge commit present at `main`: **PASS**, `d8179bdcac162059968c1700711e09e6ce904f63` before this post-merge handoff commit.

### Automated workflow state observed for the final PR head

For final PR head `011711503703c7cdc64120cafe9dbb5fdc11e0f5`, GitHub created these pull-request workflow runs:

- CI run `333` / run id `32252935771` — **queued** when last observed before merge;
- CodeQL run `332` / run id `32252936297` — **queued** when last observed before merge;
- Dependency Review run `270` / run id `32252935476` — **queued** when last observed before merge.

Queued is not passing evidence. These conclusions must not be rewritten as successful unless GitHub later reports success.

### Repository settings observed

The actual GitHub `main` branch was observed as **not protected** both before and immediately after PR #18 was merged (`protected: false`). Branch protection/rulesets are GitHub repository settings rather than files in the source tree.

The available GitHub connector in this pass exposes branch/ref operations but does not expose a branch-protection/ruleset mutation action. Therefore the source documentation is prepared, but an administrator must enable/verify the desired `main` ruleset in GitHub settings before release.

### Local execution limitation

This chat execution environment did not provide `dotnet` or `pwsh` for an authoritative local build/test/script run. Therefore no local PASS claim is invented.

The expected automated verification for the exact release commit is:

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

## Commits created in the `2.6.0.2` branch pass

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
- `0117115` — `docs: record final 2.6.0.2 pull request review state`
- `d8179bd` — `merge: finalize ChronoDesk 2.6.0.2 source hardening`
- post-merge handoff: historical commit on `main`.

## Historical next exact tasks

The remaining `2.6.0.2` work is release evidence and GitHub repository configuration:

1. require green CI/CodeQL/dependency-security results for the exact release candidate;
2. enable/verify the intended `main` branch protection/ruleset and exact required status-check contexts in GitHub settings;
3. perform the documented Windows/macOS/Linux and accessibility checks;
4. capture verified release screenshots;
5. perform clean-checkout publish/launch validation for every advertised RID;
6. create `v2.6.0.2` only after those gates pass;
7. verify the generated release ZIPs and `SHA256SUMS.txt` after publication.
