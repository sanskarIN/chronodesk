# ChronoDesk — Work Handoff

## Current milestone

Phase 7 — automated platform, product-scope, accessibility, repository-integrity, release, and exhaustive documentation hardening — 2026-08-19.

The current preview product scope is implemented in source. Phase 7 closes the remaining automatable gaps found in the roadmap and original ChronoDesk master prompt, adds permanent tracked-file documentation enforcement, and keeps native desktop behavior/accessibility/screenshots/real release-candidate verification as evidence-based release gates rather than claiming success from source inspection alone.

## Repository state

Repository: `sanskarIN/chronodesk`

Default branch: `main`

Active branch: `phase-7-automation-hardening`

Active pull request: `#16` — `Phase 7: automate startup adapter and release hardening`

Merge policy: preserve the intentionally granular Phase 7 history with a normal merge commit. Do not squash/rebase the phase away.

Immediately before this handoff commit, GitHub compare reported:

- branch status: ahead;
- ahead by: **100 commits**;
- behind by: **0 commits**;
- this handoff is the next meaningful commit, so the branch should become **101 commits ahead** if no concurrent external branch update occurs.

The branch must remain draft/unmerged until the newest exact head receives completed green CI, CodeQL, and Dependency Review checks.

## Complete documentation system

ChronoDesk now has a layered documentation system instead of a README plus scattered notes.

### Canonical documentation hub

`docs/README.md` is the technical documentation entry point and defines source-of-truth precedence, maintenance responsibilities, and validation commands.

It links the complete documentation set:

- user guide;
- setup;
- architecture;
- production source-code reference;
- runtime behavior;
- settings schema/reference;
- build/configuration reference;
- platform integration;
- localization;
- development;
- testing and test catalog;
- CI/CD;
- release procedure;
- troubleshooting;
- accessibility;
- performance;
- GitHub maintenance;
- repository file reference;
- architecture decision records.

### End-user guide

Added `docs/user-guide.md`.

It documents normal product use in detail:

- first launch/onboarding;
- main-window structure;
- 12/24-hour format and seconds;
- date/weekday/week/calendar details;
- timezone search;
- adding/removing world clocks;
- portable timezone IDs and UTC fallback;
- Focus mode;
- Mini mode;
- always-on-top behavior;
- themes/appearance;
- accessibility preferences;
- chime cadence and quiet hours;
- platform sound behavior;
- Start with system;
- Minimize to tray;
- tray menu;
- all current keyboard shortcuts;
- Settings Save/Cancel/Reset;
- backup/export;
- restore/import and import safety;
- local data/log behavior;
- Updates & About;
- offline/network behavior;
- troubleshooting pointers for links/timezones/startup/chimes/tray;
- privacy before sharing diagnostics/screenshots;
- release archive checksum verification;
- support/security routes.

### Production source-code reference

Added `docs/source-code-reference.md`.

It documents the production code at namespace/type/method-contract level:

#### Core

- all five abstractions (`IAppLogger`, `IChimePlayer`, `ISettingsStore`, `IStartupManager`, `ITimeZoneCatalog`);
- `AppSettings` invariants/schema/normalization responsibility;
- chime/settings/time/layout/theme/timezone/world-clock models;
- `ClockFormatter` deterministic formatting contract;
- `ChimePolicy` decision-only boundary.

#### Infrastructure

- `AppPaths`;
- `SafeFileLogger`;
- `JsonSettingsStore`;
- startup platform detector;
- startup filesystem and Registry testability seams;
- production filesystem/Registry adapters;
- `PlatformStartupManager` platform behavior;
- `SystemChimePlayer` fixed-helper behavior;
- `SystemTimeZoneCatalog` discovery/search/ID-conversion/fallback behavior.

#### App

- process `Program`;
- `AppServices` composition root;
- `AppVersionProvider`;
- `ExternalLinkLauncher`;
- application lifecycle/theme/tray `App`;
- localization facades;
- `ObservableObject`;
- `WorldClockCardViewModel`;
- `MainWindowViewModel` orchestration/transaction/import/tick contracts;
- MainWindow XAML/code-behind responsibilities;
- Settings XAML/code-behind responsibilities;
- onboarding/about responsibilities;
- shared design system;
- logo/icon/manifest/test-visibility files.

It also documents:

- typical persistent-setting/platform/text/new-file change paths;
- public API philosophy;
- cancellation/concurrency principles;
- optional-feature vs explicit-user-mutation failure handling;
- security review hotspots;
- testability review hotspots;
- source documentation maintenance rules.

### Runtime behavior reference

`docs/runtime-behavior.md` documents:

- process entry and `--background`;
- explicit service composition;
- app/main-window initialization;
- tray best-effort behavior;
- 250 ms non-overlapping clock ticks;
- one-instant coherent local/world-clock updates;
- chime evaluation/playback;
- settings startup/persistence transaction order;
- startup rollback on settings persistence failure;
- Settings save/reset/import/export behavior;
- imported startup preference protection;
- Focus/Mini/keyboard behavior;
- close-to-tray/Quit;
- theme/high-contrast application;
- optional facility error containment;
- user-initiated-only external navigation;
- shutdown/disposal.

### Complete settings reference

`docs/settings-reference.md` documents every persistent setting and rule:

- schema version;
- defaults;
- clock format/display toggles;
- theme/layout;
- font/clock size/spacing bounds;
- reduced motion/high contrast;
- always-on-top/startup/tray;
- chime interval;
- quiet-hours same-day/overnight/equal-bound semantics;
- world-clock IDs/labels/timezone IDs;
- at least one / max 24 clock rules;
- string sanitization/bounds;
- camel-case/string-enum JSON;
- 2 MiB import bound;
- schema validation;
- atomic persistence;
- corrupt settings preservation;
- import startup-preference protection;
- Settings UI mapping.

### Build/configuration reference

`docs/configuration-reference.md` documents:

- `global.json`;
- `Directory.Build.props`;
- central NuGet package management/current versions;
- solution/project dependency direction;
- every project file's role;
- development preview vs tag-stamped release versions;
- `CHRONODESK_DATA_DIR`;
- `.env.example`;
- `.editorconfig`;
- `.gitattributes`;
- `.gitignore`;
- Windows manifest;
- internal test visibility;
- Debug vs Release;
- verification commands.

### Platform integration reference

`docs/platform-integration.md` documents:

- runtime startup platform detection;
- Windows HKCU Run value;
- macOS per-user LaunchAgent;
- Linux XDG autostart desktop entry;
- quoting/XML escaping/XDG fallback;
- startup transaction consistency;
- background startup;
- tray behavior;
- Windows/macOS/Linux sound behavior;
- OS timezone database and IANA/Windows conversion;
- local data paths;
- HTTPS/mailto external-link policy;
- native file picker boundary;
- Windows manifest/icon behavior;
- release RIDs/archive formats;
- exact native validation still required.

### Localization reference

`docs/localization.md` documents:

- `Strings.resx` / `Strings.cs`;
- `SettingsExtras.resx` / `SettingsExtras.cs`;
- `CurrentUICulture` vs `CurrentCulture`;
- XAML `x:Static` usage;
- primary resource categories;
- dynamic/formatted strings;
- date/time localization boundary;
- stable serialized enum values vs translated labels;
- quiet-hour input contract;
- accessibility localization;
- privacy/security wording requirements;
- adding resource keys/translations;
- long-string/layout validation.

### CI/CD reference

`docs/ci-cd.md` documents:

- CI triggers/permissions/concurrency;
- Repository integrity job;
- three-OS .NET matrix;
- formatting/build/test/coverage/NuGet vulnerability gates;
- CodeQL schedule/permissions;
- Dependency Review severity/license policy;
- Dependabot NuGet/Actions cadence;
- all repository validator scripts;
- validator tests;
- release preflight;
- semantic tag/version outputs;
- four-RID package matrix;
- Windows ZIP vs Unix tar.gz;
- tag-derived version stamping;
- SHA-256 sidecars;
- downloaded artifact re-verification;
- least-privilege final release permission;
- prerelease publication;
- branch protection/check-name guidance;
- workflow security/diagnosis rules.

### Exhaustive test catalog

`docs/test-catalog.md` maps every current test/test-support file to its product contract:

- settings normalization;
- deterministic property-style invariants;
- quiet hours;
- chime policy;
- clock formatting;
- real temp-filesystem settings persistence/recovery;
- malformed import fuzz/oversized import;
- timezone catalog;
- view-model startup/persistence transaction behavior;
- startup artifact generation across all supported platforms;
- external URI allowlist;
- semantic version display;
- headless main/settings/onboarding/about UI;
- every shared fake;
- Python repository validator tests;
- manual native boundaries that automated tests do not replace.

## Every tracked file is documented

`docs/repository-reference.md` is the canonical file-by-file repository inventory.

The previous full audit contained 140 tracked-file responsibility entries. Two additional permanent documentation files were then added:

- `docs/user-guide.md`;
- `docs/source-code-reference.md`.

The reference was updated in the same phase, so the canonical inventory now contains **142 tracked-file responsibility entries** at this checkpoint.

It includes all known tracked files across:

- repository root build/config/policy/product/handoff files;
- `.github` funding/templates/Dependabot/workflows;
- all documentation/ADRs/documentation assets;
- all repository validation scripts and script tests;
- every Core abstraction/model/service/project file;
- every Infrastructure persistence/logging/platform/timezone/project/test-visibility file;
- every App project/composition/version/link/localization/asset/style/view-model/view/XAML/code-behind/manifest file;
- every .NET test;
- every shared test fake.

No tracked file category is intentionally exempt because it is “small.”

## Documentation completeness is permanently machine enforced

Added `scripts/check_documentation_inventory.py`.

It:

- reads Git's authoritative tracked set with `git ls-files -z`;
- parses canonical `- `path` — description` entries in `docs/repository-reference.md`;
- removes fenced code blocks before parsing so syntax examples are not false tracked entries;
- reports tracked files missing documentation;
- reports stale documentation entries for deleted/untracked files;
- fails nonzero on mismatch.

Added `scripts/tests/test_check_documentation_inventory.py`.

It verifies:

- canonical inventory parsing;
- ordinary inline/noncanonical examples ignored;
- fenced backtick examples ignored;
- fenced tilde examples ignored;
- missing path detection;
- stale path detection;
- exact match success.

### CI enforcement

`.github/workflows/ci.yml` Repository integrity now runs:

1. `check_markdown_links.py`;
2. `check_documentation_inventory.py`;
3. `check_repository_secrets.py`;
4. Python validator unit tests.

### Release enforcement

`.github/workflows/release.yml` Release preflight runs the documentation inventory gate before build/package work.

Therefore a future tracked source/test/asset/resource/workflow/template/script/doc file cannot be added without a corresponding canonical responsibility entry and still pass the intended CI/release gates.

## Governance documentation synchronized

Updated:

- root `README.md` — complete documentation discovery and validator commands;
- `CONTRIBUTING.md` — documentation as implementation, inventory/update rules, validator commands;
- `.github/pull_request_template.md` — inventory/test-catalog/privacy-security/ADR checklist;
- `docs/development.md` — settings schema, localization, platform/process/logging/testing/documentation workflows;
- `docs/testing.md` — inventory gate and complete test strategy;
- `docs/release.md` — inventory as clean-checkout/preflight/release-ready gate;
- `docs/github-maintenance.md` — repository ruleset/review/release audit expectations;
- `CHANGELOG.md` — documentation/inventory automation recorded;
- `ROADMAP.md` — exhaustive documentation infrastructure recorded complete;
- `docs/README.md` — user/source-code references linked and maintenance rules extended.

## Phase 7 engineering work retained under the documentation layer

The documentation audit does not replace or remove earlier Phase 7 engineering work.

### Startup integration testability

- internal startup platform model/detector;
- startup filesystem boundary/system adapter;
- startup current-user Registry boundary/Windows adapter;
- `PlatformStartupManager` injectable platform/filesystem/registry/profile/XDG inputs;
- deterministic Windows/macOS/Linux/unsupported/cancellation tests;
- tests do not alter CI runner login startup state.

### Settings deterministic interaction

- awaitable internal save/reset operations preserving UI handlers;
- shared in-memory settings/startup/timezone/chime/logger test doubles;
- headless save mapping/startup tests;
- invalid quiet-hour no-persist/no-startup test;
- reset/default/startup-disable/control-reload test.

### Settings Updates & About

- new Settings Updates & About surface;
- semantic current version shown;
- explicit Open GitHub Releases;
- explicit Open About;
- no background update polling/fetch/downloader;
- companion localized resource catalog;
- headless controls/version coverage.

### External navigation hardening

- centralized `ExternalLinkLauncher`;
- absolute HTTPS/mailto only;
- HTTP/file/script/relative/empty rejected;
- About/Settings share same policy;
- tests exercise URI policy without launching handlers.

### Accessibility

- explicit automation names on Settings controls whose visual labels are adjacent text;
- accessibility checklist expanded for Settings, external handlers, scaling, and screen readers.

### Repository integrity

- offline local Markdown target validator;
- high-confidence committed credential scanner without printing matches;
- tracked-file documentation inventory validator;
- Python validator tests;
- dedicated Repository integrity CI job.

### Release identity and packaging

- semantic display-version provider;
- user-visible build metadata stripped;
- semantic tag stamps Version/AssemblyVersion/FileVersion/InformationalVersion;
- prerelease detection/publication;
- release preflight;
- least-privilege workflow permissions;
- Windows ZIP, Linux/macOS tar.gz;
- SHA-256 sidecars;
- checksum re-verification after artifact download;
- tag-time changelog/screenshot readiness validation and tests.

## Current release status

ChronoDesk is **not** declared release-ready yet.

Intentional release blockers remain:

- README still uses the explicit screenshot placeholder;
- changelog remains `[Unreleased]` instead of an exact intended release heading;
- native/manual desktop validation has not been evidenced in this connected environment;
- the final newest branch head must complete green CI/CodeQL/Dependency Review after the last documentation commit.

The release workflow is intentionally expected to reject a premature semantic release tag until metadata/readiness gates are satisfied.

## Remaining evidence-based native release gates

### Windows 11

- real tray Show/Focus/Mini/Quit and close-to-tray;
- real HKCU startup enable/disable/login launch;
- real chime;
- native import/export file pickers;
- default HTTPS/mailto handlers;
- packaged win-x64 archive launch;
- keyboard/screen-reader/high-contrast/scaling verification.

### macOS

- tray/menu behavior;
- x64/arm64 package validation as available;
- real LaunchAgent creation/removal/login launch;
- `afplay` behavior;
- native pickers/default handlers;
- executable permissions after tar extraction;
- accessibility/Gatekeeper/signing-state documentation verification.

### Linux

- representative GNOME/KDE sessions;
- tray/status-notifier behavior;
- real XDG autostart;
- optional sound helper success/fallback;
- native pickers/default handlers;
- executable permissions after tar extraction;
- accessibility/scaling verification.

### Release candidate

- replace screenshot placeholder with real sanitized release-build captures;
- move changelog content into exact target version heading;
- run `check_release_metadata.py --tag <exact-tag>`;
- complete clean-checkout release procedure;
- confirm final CI/CodeQL/Dependency Review;
- confirm branch protection uses actual current check contexts;
- manually review private data/artifacts in addition to scanners;
- create first release-candidate tag only after those gates;
- validate migration after a real prior tagged preview fixture exists;
- publish stable `v1.0.0` only after stable gates pass.

## Connected-environment verification boundary

Repository edits are made through the connected GitHub API. The available execution environment previously could not clone GitHub directly and is not the authoritative local .NET build environment for this repo, so GitHub Actions remains the authoritative compiler/analyzer/test runner for connected commits.

CI uses `cancel-in-progress`. Because Phase 7 intentionally uses many granular commits, older runs are cancelled as newer commits arrive. Merge decisions must use the newest frozen head only.

The final documentation head must prove:

- local Markdown validator passes;
- documentation inventory validator reports zero missing/stale files;
- credential scanner passes;
- Python validator tests pass;
- Ubuntu .NET 9 format/build/test/vulnerability checks pass;
- Windows .NET 9 format/build/test/vulnerability checks pass;
- macOS .NET 9 format/build/test/vulnerability checks pass;
- CodeQL passes;
- Dependency Review passes.

Queued or structurally mergeable is not equivalent to verified.

## Commit identity

For local maintainer work use:

```bash
git config user.name "Sanskar"
git config user.email "sanskarin@outlook.in"
```

Connected GitHub writes use the authenticated integration/GitHub commit behavior. Do not rewrite otherwise-valid history merely to change attribution.

## Next exact tasks

1. Freeze the branch after this handoff commit.
2. Verify the branch becomes **101 commits ahead of `main` and 0 behind** unless a concurrent external update occurred.
3. Update PR #16 metadata to the newest head/count without creating source churn.
4. Inspect final CI/CodeQL/Dependency Review runs for this exact head.
5. Inspect Repository integrity first because it validates the 142-file documentation contract plus Markdown/secrets/Python tests.
6. If a check fails, use the exact job log and add only a focused fix/regression commit.
7. Keep PR #16 draft while any required final check is queued/incomplete/failing.
8. When final checks are green, mark PR ready and merge normally to preserve granular history.
9. Re-check `main` after merge.
10. Do not mark native release gates complete or push a release tag without real desktop/build evidence.

## Permanent documentation rule

For every future tracked file addition/move/rename/deletion:

- update `docs/repository-reference.md` in the same change;
- update `docs/source-code-reference.md` when a production type's responsibility/contract changes;
- update `docs/user-guide.md` when user operation changes;
- update `docs/test-catalog.md` when test responsibility changes;
- update the closest specialized technical/operational document;
- run Markdown and documentation inventory validators;
- let PR CI enforce the same invariant;
- let Release preflight enforce it again before packaging.

This turns “complete documentation without skipping files” into a permanent repository control rather than a one-time documentation pass.
