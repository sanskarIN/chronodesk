# ChronoDesk — Work Handoff

## Current milestone

Phase 7 — automated platform, product-scope, accessibility, repository-integrity, and release hardening — 2026-08-19.

The current preview product scope is implemented in source. This phase closes the remaining automatable gaps found in the roadmap and the original ChronoDesk master prompt, while keeping native desktop behavior, accessibility, screenshots, and real release-candidate verification as explicit evidence-based release gates.

## Source of truth

Repository: `sanskarIN/chronodesk`

Default branch: `main`

Active phase branch: `phase-7-automation-hardening`

Active pull request: `#16` — `Phase 7: automate startup adapter and release hardening`

Pull request state during this handoff: **draft** until the final Phase 7 head receives completed green automated validation.

Phase base on `main`: `8695efc3ba81b3e408630691a3da7b8093954ad9`.

Immediately before this handoff commit, GitHub compare reported the phase branch **70 commits ahead of `main`, 0 behind**. This handoff update is intentionally the next granular commit, so the resulting branch head is expected to be 71 commits ahead unless another concurrent repository write occurs.

The repository is being implemented from the ChronoDesk master prompt supplied for this project. This file is the primary cross-chat/cross-session handoff and must remain more authoritative than chat summaries.

## Phase 7 completed work

### 1. Startup integration testability

- Added an internal `StartupPlatform` model and platform detector for deterministic platform selection.
- Added `IStartupFileSystem` plus the production `SystemStartupFileSystem` adapter.
- Added `IStartupRegistry` plus the production Windows `SystemStartupRegistry` adapter.
- Refactored `PlatformStartupManager` so production composition still uses the real environment while tests can inject executable path, platform, filesystem, registry, user-profile path, and XDG config home.
- Preserved the public `IStartupManager` contract consumed by the app.
- Added test-only internal visibility for the infrastructure seams required by deterministic tests.

### 2. Startup adapter automated coverage

`PlatformStartupManagerTests` now verifies without modifying a CI runner's real login configuration:

- Windows startup enable writes the quoted ChronoDesk executable plus `--background`.
- Windows startup disable removes the current-user Run value.
- Windows enabled-state detection requires the configured ChronoDesk executable path.
- macOS LaunchAgent path construction uses the supplied user profile.
- macOS XML-sensitive executable characters are escaped.
- macOS disable removes an existing LaunchAgent.
- Linux honors `XDG_CONFIG_HOME`.
- Linux falls back to `~/.config/autostart` when XDG config home is absent.
- Linux desktop-entry executable paths containing spaces are quoted.
- Linux disable removes an existing desktop entry.
- Unsupported platforms report unsupported and reject startup writes.
- Startup operations honor pre-cancelled cancellation tokens.

Dedicated fake filesystem and registry implementations keep these tests isolated from the host machine.

### 3. Settings-window deterministic interaction coverage

- Added test visibility for the application internals required by headless tests.
- Refactored Settings save handling into awaitable `SaveChangesAsync` while retaining the normal Avalonia click handler.
- Refactored reset handling into awaitable `ResetDefaultsAsync` while retaining the normal Avalonia click handler.
- Added reusable test doubles:
  - `MemorySettingsStore`;
  - `RecordingStartupManager`;
  - `UtcTimeZoneCatalog`;
  - `NullChimePlayer`;
  - `NullAppLogger`.
- Consolidated `MainWindowViewModelTests` on the shared doubles.
- Added headless tests for control-to-settings mapping and persistence.
- Added headless invalid quiet-hours validation coverage proving invalid text does not persist settings or alter startup integration.
- Added headless reset coverage proving defaults are persisted, startup is disabled when necessary, and controls are reloaded.

Native file-picker behavior remains a real desktop boundary. Import/export persistence and safety logic beneath the picker is independently automated.

### 4. Settings Updates & About master-prompt completion

The original product prompt expected update/About access inside Settings. Phase 7 closes that gap without introducing a background updater.

- Added a localized **Updates & About** Settings tab.
- Added current application version text in Settings.
- Added **Open GitHub Releases** pointing to the fixed public repository Releases page.
- Added **Open About** to display the existing full About dialog.
- Added privacy text making clear that opening Releases is user-initiated and leaves the offline app for the default browser.
- Added localized error text when the OS cannot open the external handler.
- Added creator credit in the new Settings surface.
- Added headless checks for the new buttons, localized tab text, and preview version display.
- No update polling, release API fetch, background network request, package download, or in-process update execution was added.

### 5. Centralized external-link security boundary

- Added `ExternalLinkLauncher` as the application-wide external-navigation boundary.
- Only absolute `https` and `mailto` URIs are accepted.
- Plain `http`, `file:`, script-style URIs, relative paths, malformed targets, and empty targets are rejected.
- About now uses the same launcher instead of duplicating process-launch logic.
- Settings Releases access uses the same launcher.
- Missing/unsupported OS handlers are non-fatal.
- Added deterministic URI-policy tests that do not launch a real browser/mail client.

### 6. User-facing semantic version identity

- Added `AppVersionProvider`.
- User-facing version text prefers `AssemblyInformationalVersion` / semantic version metadata.
- `+build` metadata is removed from the visible version while prerelease identifiers are preserved.
- Three-part assembly-version fallback remains available.
- A final `development` fallback exists when metadata is unavailable.
- About and Settings both use the same version provider.
- Added regression tests for preview, stable, prerelease, build-metadata, assembly fallback, and development fallback cases.
- Headless UI tests verify the ordinary development build shows `0.1.0-preview`.

### 7. Release tag version stamping

The tagged release workflow no longer risks publishing binaries that still identify as the repository's fixed preview version.

- Release tags are validated as supported semantic versions.
- The leading `v` is removed for application/package metadata.
- Tag version now drives:
  - `Version`;
  - `AssemblyVersion`;
  - `FileVersion`;
  - `InformationalVersion`.
- Prerelease suffixes such as `-rc.1` are preserved in visible/package identity.
- Prerelease tags are published as GitHub prereleases.
- Version resolution is centralized in release preflight and passed to platform packaging jobs.

### 8. Release workflow preflight and permission hardening

The tagged release workflow now has an explicit `Release preflight` stage before packaging.

Preflight performs:

- semantic tag resolution;
- release-metadata validation;
- repository-local Markdown link validation;
- committed-credential scanning;
- .NET restore;
- formatting verification;
- Release build;
- tests;
- NuGet vulnerable-package inspection.

Additional workflow hardening:

- default workflow permission is `contents: read`;
- only the final GitHub release publication job receives `contents: write`;
- platform package jobs depend on successful preflight;
- final publication depends on all platform packages.

### 9. Platform-appropriate release archives

- Windows remains a `.zip` archive.
- Linux uses `.tar.gz`.
- macOS x64 uses `.tar.gz`.
- macOS arm64 uses `.tar.gz`.
- Unix tarballs are intentional so executable permission bits survive extraction.

Expected stable release artifact forms:

- `chronodesk-vX.Y.Z-win-x64.zip`
- `chronodesk-vX.Y.Z-linux-x64.tar.gz`
- `chronodesk-vX.Y.Z-osx-x64.tar.gz`
- `chronodesk-vX.Y.Z-osx-arm64.tar.gz`

Prerelease artifact names contain the exact prerelease tag.

### 10. Release checksum integrity

- Every release archive receives a sibling `.sha256` sidecar.
- SHA-256 is generated after archive creation.
- The final publication job downloads the packages from Actions artifact storage rather than trusting the package job's local filesystem.
- Publication requires exactly four archives and four checksum files.
- Each checksum file is parsed and validated.
- Every referenced archive must exist.
- Every downloaded archive is re-hashed and compared with its sidecar before `gh release create` is allowed to run.
- Checksums are documented accurately as integrity data, not code-signing/publisher-authentication equivalents.

### 11. Repository-local Markdown integrity automation

Added `scripts/check_markdown_links.py`:

- scans repository Markdown without network access;
- validates local inline/reference link and image targets;
- rejects targets that escape the repository;
- reports missing repository-local targets;
- intentionally excludes external URL availability from deterministic CI.

It runs in the CI repository-integrity job and in release preflight.

### 12. High-confidence committed-credential scan

Added `scripts/check_repository_secrets.py`:

- scans committed text files for high-confidence private-key and common token/credential patterns;
- includes common AWS, GitHub, Google, Slack, OpenAI-style, Stripe, and private-key patterns;
- reports only file, line, and finding class rather than printing a matched secret;
- skips expected build/test output directories and oversized/binary files;
- runs in CI and release preflight;
- is explicitly documented as a tripwire rather than proof that every possible sensitive datum is absent.

Human review of source, screenshots, generated archives, signing material, and release artifacts remains mandatory.

### 13. Tag-time release metadata gate

Added `scripts/check_release_metadata.py` and standard-library unit tests.

For an intended tag, the validator requires:

- a supported stable or prerelease semantic tag;
- a matching `CHANGELOG.md` heading such as `## [1.0.0]` or `## [0.1.0-rc.1]`;
- removal of the explicit README release screenshot placeholder.

The validator intentionally fails when README still contains `docs/assets/screenshot-placeholder.svg` / the explicit placeholder label.

Normal CI executes the validator's unit tests. The tag-triggered release preflight executes the validator with the actual Git tag, which prevents packaging when release metadata is not ready.

### 14. Repository validation-script tests

CI now executes:

```bash
python3 -m unittest discover -s scripts/tests -p 'test_*.py'
```

Current release-metadata test cases cover:

- valid stable release metadata;
- valid prerelease metadata;
- missing matching changelog release heading;
- remaining screenshot placeholder;
- unsupported/non-semantic release tag.

### 15. Settings accessibility hardening

Settings controls with separate visual labels now expose explicit automation names on the actual interactive element rather than assuming visual adjacency is sufficient for assistive technology.

Covered controls include:

- clock format;
- seconds/date/weekday/week-number/calendar-detail toggles;
- chime interval;
- quiet-hours enabled/start/end;
- theme;
- layout;
- font family;
- clock font size;
- content spacing;
- Settings window itself.

The source-level semantic hardening does **not** replace native screen-reader verification.

### 16. Documentation synchronization

Updated documentation now matches the Phase 7 source instead of describing aspirational behavior.

Updated files include:

- `README.md`;
- `CHANGELOG.md`;
- `ROADMAP.md`;
- `PRIVACY.md`;
- `SECURITY.md`;
- `docs/accessibility.md`;
- `docs/architecture.md`;
- `docs/testing.md`;
- `docs/release.md`;
- `docs/release-notes-template.md`;
- this `what_changed.md` handoff.

Documentation now covers:

- startup testability boundaries;
- Settings deterministic headless flows;
- Updates & About behavior;
- no-background-update privacy model;
- centralized external-link allowlisting;
- semantic version display;
- tag-derived release identity;
- release preflight;
- Windows ZIP vs Unix tarball packaging;
- checksum generation and independent publication-time verification;
- release permission scoping;
- tag-time changelog/screenshot readiness checks;
- accessibility semantics and remaining native validation;
- Python repository-validation tests.

## Settings schema status

Phase 7 did **not** add or reinterpret a persisted settings field and did not intentionally change `AppSettings` schema compatibility. The Updates & About UI is informational/navigation behavior, not persisted update configuration.

This matters for release migration planning: a real prior-version fixture is still needed after the first tagged preview exists before the migration-path release gate can be validated honestly.

## Current intentional release blockers

The branch is **not** being presented as a verified release candidate yet.

Two visible repository states are intentionally still blockers:

1. `README.md` still uses the explicit screenshot placeholder because no verified real release-build screenshot has been supplied/captured in this connected source-editing environment.
2. `CHANGELOG.md` remains under `[Unreleased]` because no release candidate version has been declared and verified yet.

The new tag-time metadata validator is expected to reject a release tag while either condition remains. Do not weaken or bypass that gate just to make a release workflow run.

## Current automated verification model

Pull-request automation covers:

- `CI / Repository integrity`;
- repository Python validation-script unit tests;
- Ubuntu .NET 9 format/build/test/vulnerability inspection;
- Windows .NET 9 format/build/test/vulnerability inspection;
- macOS .NET 9 format/build/test/vulnerability inspection;
- CodeQL;
- Dependency Review where the workflow applies.

Because Phase 7 deliberately used many small commits, CI `cancel-in-progress` behavior cancelled older CI runs whenever a newer branch head was pushed. Only the final frozen Phase 7 head should be used to decide whether the pull request is automation-green.

The connected GitHub editing environment does not provide a local .NET checkout/build execution path. GitHub Actions is therefore the authoritative compiler, XAML compiler, analyzer, and automated-test runner for connector-created source changes.

## Remaining evidence-based release gates

These items stay unchecked until they are actually performed in the required environment.

### Native Windows validation

- Launch from a clean release artifact on Windows 11.
- Validate tray Show/Focus/Mini/Quit behavior.
- Validate minimize-to-tray close behavior.
- Validate current-user Run startup enable/disable and background launch.
- Validate optional chime behavior.
- Validate native file pickers for settings import/export.
- Validate default browser and mail-handler behavior for Releases/About links.
- Validate keyboard-only behavior, focus visibility, high contrast, scaling, large text, and screen-reader output.

### Native macOS validation

- Validate both applicable x64 and arm64 release targets with GUI sessions/hardware/runners.
- Validate tray/menu-bar behavior.
- Validate LaunchAgent creation/removal and background launch.
- Validate `afplay` fallback.
- Validate native file pickers.
- Validate browser/mail-handler behavior.
- Validate accessibility behavior with the platform stack.
- Document unsigned/Gatekeeper behavior accurately.

### Native Linux validation

- Validate at least a representative GNOME-family session and, when practical, KDE-family session.
- Validate tray/status-notifier behavior.
- Validate XDG autostart.
- Validate operation with and without optional sound helpers.
- Validate native file pickers.
- Validate browser/mail-handler behavior.
- Validate accessibility behavior with the selected desktop/accessibility stack.

### Release candidate preparation

- Capture a real screenshot from a verified running release build and replace the README placeholder.
- Decide the exact release-candidate semantic version.
- Move the intended changes from `[Unreleased]` into a changelog heading matching that exact tag.
- Run `python3 scripts/check_release_metadata.py --tag <exact-tag>` successfully.
- Complete the clean-checkout verification in `docs/release.md`.
- Confirm the final release commit's CI, CodeQL, dependency/security, and repository-integrity checks are green.
- Confirm branch-protection required checks use the exact check names actually reported by GitHub.
- Manually review the final tree and generated release artifacts for private data in addition to automated credential scanning.
- Run the complete accessibility checklist on real supported desktops.
- Tag only after all release-candidate blockers are resolved.

### Post-publication validation

Even a successful tag workflow is not the end of verification:

- download public release archives and checksum files again from the release page;
- independently verify checksums;
- extract/launch at least representative public-release artifacts;
- confirm the running application shows the tag-derived version;
- perform smoke checks against downloaded artifacts rather than only build-directory outputs;
- update this handoff with the tag, release URL, platforms actually verified, and any follow-up defects.

### Stable v1.0.0 gate

- Validate migration behavior using a real previous tagged preview fixture.
- Resolve all stable-release blockers.
- Publish stable `v1.0.0` only after the documented stable gates are genuinely satisfied.

## Git commit identity note

The connected GitHub contents/write API does not expose commit author/committer email fields. Commits created through this integration therefore use the authenticated GitHub identity rather than allowing an explicit `sanskarin@outlook.in` email override.

For local Git work, configure:

```bash
git config user.name "Sanskar"
git config user.email "sanskarin@outlook.in"
```

Do not rewrite otherwise useful connected commit history solely to alter attribution unless there is a separate repository-history reason.

## Next exact tasks

1. Treat this handoff commit as the intended final Phase 7 source/documentation head unless automated validation finds a defect.
2. Inspect the newest PR #16 CI, CodeQL, and Dependency Review results for this exact head.
3. If any check fails, inspect the specific failed job/log and make the smallest focused fix/regression commit required; then revalidate the new head.
4. Keep PR #16 draft while any required available automated check is failing or still unresolved.
5. If all available automated checks for the final head are green, mark PR #16 ready for review.
6. Merge with a **normal merge commit** rather than squash/rebase so the intentionally granular Phase 7 history is preserved.
7. Re-check `main` after merge for the post-merge workflow state.
8. Do not mark native desktop/release gates complete from source inspection alone.
9. Continue release-candidate work only when real native GUI/build evidence and a selected release version are available.

## Phase 7 commit strategy

Phase 7 deliberately uses a large number of small, meaningful commits rather than one monolithic change. The 70-commit pre-handoff branch history separates abstractions, production adapters, fakes, tests, UI changes, localization, release pipeline stages, security gates, accessibility hardening, integrity scripts, README/roadmap/release/security/privacy/testing/architecture updates, and other independently reviewable concerns.

This handoff refresh is intentionally the next commit. Preserve the history when merging PR #16 so future regressions can be bisected and individual design decisions remain auditable.
