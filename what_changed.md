# ChronoDesk — Work Handoff

## Current milestone

Phase 7 — automated platform, product-scope, accessibility, repository-integrity, release, and exhaustive documentation hardening — 2026-08-19.

The current preview product scope is implemented in source. Phase 7 closes the remaining automatable gaps found in the roadmap and original ChronoDesk master prompt, adds enforceable repository documentation completeness, and keeps native desktop behavior/accessibility/screenshots/real release-candidate verification as explicit evidence-based release gates rather than claiming them from source inspection.

## Source of truth

Repository: `sanskarIN/chronodesk`

Default branch: `main`

Active phase branch: `phase-7-automation-hardening`

Active pull request: `#16` — `Phase 7: automate startup adapter and release hardening`

PR policy: keep draft until the final head receives completed green CI/CodeQL/Dependency Review validation. Preserve the granular history with a normal merge commit rather than squashing the phase.

Immediately before this handoff update, GitHub compare reported:

- branch status: ahead of `main`;
- ahead by: **95 commits**;
- behind by: **0 commits**;
- changed files against `main`: approximately 60 Phase 7 files at that point;
- this `what_changed.md` refresh is the next intentional granular commit, so the branch should become **96 commits ahead** if no concurrent branch change occurs.

## Exhaustive documentation audit completed

The repository now has a canonical documentation architecture instead of relying only on root README plus scattered specialized documents.

### Documentation hub

Added `docs/README.md` as the canonical technical documentation entry point.

It maps users/maintainers to:

- setup;
- architecture;
- runtime behavior;
- complete settings schema;
- build/configuration;
- platform integration;
- localization;
- development;
- testing/test catalog;
- CI/CD;
- release procedure;
- troubleshooting;
- accessibility;
- performance;
- GitHub maintenance;
- ADRs;
- exhaustive repository file reference.

It also defines documentation source-of-truth precedence and maintenance rules so behavior does not become documented in only one transient handoff file.

### Runtime behavior reference

Added `docs/runtime-behavior.md`.

Documented in detail:

- process entry and `--background` behavior;
- explicit composition through `AppServices`;
- application initialization and tray best-effort behavior;
- main-window opening sequence;
- 250 ms non-overlapping clock tick model;
- ClockFormatter usage;
- coherent world-clock refresh from one instant;
- timezone unavailable → UTC fallback;
- chime decision/playback behavior;
- settings startup/persistence transaction ordering;
- best-effort startup rollback on persistence failure;
- Settings save/reset behavior;
- import/export safety behavior;
- imported settings preserving local startup preference;
- focus/mini state transitions and keyboard shortcuts;
- close-to-tray and explicit Quit semantics;
- theme/high-contrast behavior;
- optional facility error containment;
- user-initiated-only external network/navigation behavior;
- shutdown/disposal boundaries.

### Complete settings reference

Added `docs/settings-reference.md`.

Documented every persistent field, default, enum, bound, normalization rule, and UI mapping, including:

- schema version `1`;
- first-run state;
- 12/24-hour format;
- seconds/date/weekday/week/calendar visibility;
- theme/layout;
- font name/size/content spacing;
- reduced motion/high contrast;
- always on top/start with system/minimize to tray;
- chime enabled/cadence;
- quiet-hours start/end semantics including overnight and equal-bound behavior;
- world-clock ID/display/timezone fields;
- at most 24 clocks and at least one valid clock;
- string normalization/length constraints;
- camel-case/string-enum JSON policy;
- 2 MiB settings import bound;
- current/newer schema handling;
- atomic temp-file replacement;
- corrupt settings preservation;
- imported startup-preference protection;
- export/privacy considerations.

### Build/configuration reference

Added `docs/configuration-reference.md`.

Documented:

- `global.json` .NET 9 SDK selection/roll-forward;
- `Directory.Build.props` shared target/analyzer/warnings-as-errors/deterministic policy;
- `Directory.Packages.props` central package management and current package versions;
- solution/project dependency direction;
- Core/Infrastructure/App/Test `.csproj` responsibilities;
- development version vs tag-stamped release version;
- `CHRONODESK_DATA_DIR`;
- `.env.example`;
- `.editorconfig`;
- `.gitattributes`;
- `.gitignore`;
- Windows `app.manifest` role;
- internal test-visibility AssemblyInfo files;
- Debug vs Release behavior;
- local verification commands.

### Platform integration reference

Added `docs/platform-integration.md`.

Documented:

- startup platform detection;
- Windows HKCU Run key/value behavior;
- macOS LaunchAgent path/plist/escaping;
- Linux XDG autostart path/desktop-entry quoting;
- startup transaction consistency;
- `--background` behavior;
- tray lifecycle/close-to-hide semantics;
- Windows beep and fixed macOS/Linux sound helper behavior;
- OS timezone discovery and IANA/Windows conversion fallback;
- local application data paths;
- HTTPS/mailto external URI allowlist;
- native file-picker boundary;
- Windows manifest/icon behavior;
- release RIDs/archive formats;
- unsupported-platform policy;
- exact real-platform validation still required before release claims.

### Localization reference

Added `docs/localization.md`.

Documented:

- `Strings.resx`/`Strings.cs` ResourceManager architecture;
- `SettingsExtras.resx`/`SettingsExtras.cs` companion catalog;
- `CurrentUICulture` vs `CurrentCulture` responsibilities;
- XAML static resource usage;
- primary resource categories;
- dynamic/formatted strings;
- date/time localization boundaries;
- persisted enum identifiers vs translated labels;
- quiet-hour input format contract;
- accessibility localization requirements;
- security/privacy wording requirements;
- adding resource keys;
- adding future translations;
- layout/testing review for expanded translated strings.

### CI/CD reference

Added `docs/ci-cd.md`.

Documented every automation layer:

- CI triggers, permissions, concurrency, Repository integrity job;
- three-OS .NET 9 matrix;
- formatting/build/test/coverage/NuGet vulnerability behavior;
- CodeQL schedule/permissions;
- Dependency Review severity/license policy;
- Dependabot NuGet/Actions schedules;
- Markdown, documentation inventory, credential, and release metadata validators;
- validator unit tests;
- release preflight;
- semantic tag/version outputs;
- four-RID package matrix;
- Windows ZIP vs Unix tar.gz;
- tag-derived metadata stamping;
- SHA-256 sidecars;
- downloaded artifact re-verification;
- final release job permission escalation only;
- prerelease publication behavior;
- expected branch-protection check families;
- workflow security rules;
- CI diagnosis rules.

### Exhaustive test catalog

Added `docs/test-catalog.md`.

It maps every tracked .NET test/test-support file and Python validator test to the product contract it protects, including:

- AppSettings normalization tests;
- deterministic domain property tests;
- QuietHours tests;
- ChimePolicy tests;
- ClockFormatter tests;
- JsonSettingsStore real-temp-filesystem tests;
- malformed-import fuzz tests;
- SystemTimeZoneCatalog tests;
- MainWindowViewModel startup/persistence transaction tests;
- PlatformStartupManager fake-registry/filesystem tests;
- ExternalLinkLauncher URI-policy tests;
- AppVersionProvider SemVer display tests;
- Avalonia headless smoke tests;
- SettingsWindow headless interaction tests;
- every shared fake under `tests/ChronoDesk.Tests/Fakes/`;
- Avalonia headless bootstrap;
- test project responsibilities;
- Python release metadata validator tests;
- Python documentation inventory validator tests;
- manual native behavior not replaced by automated tests.

### Exhaustive tracked-file reference

Added `docs/repository-reference.md`.

This is the canonical file-by-file inventory for the complete tracked repository. It includes a responsibility entry for every known tracked file across:

- all root build/config/policy/product files;
- all `.github` funding/templates/dependency/workflow files;
- all documentation/ADRs/assets;
- all repository/release scripts and script tests;
- every Core abstraction/model/service/project file;
- every Infrastructure persistence/logging/platform/timezone/project/test-visibility file;
- every App project/composition/version/link/resource/asset/style/view-model/view/XAML/code-behind/manifest file;
- every .NET test file;
- every shared test fake.

At this audit point, the canonical inventory contains **140 tracked-file responsibility entries**. This count includes the new documentation inventory script/test/reference files themselves.

## Documentation completeness is now machine enforced

Added `scripts/check_documentation_inventory.py`.

Behavior:

- obtains Git's authoritative file set with `git ls-files -z`;
- parses canonical entries from `docs/repository-reference.md`;
- ignores fenced code examples;
- reports tracked files missing documentation;
- reports stale inventory entries for files no longer tracked;
- returns nonzero on any mismatch;
- does not rely on a manually maintained count alone.

Added `scripts/tests/test_check_documentation_inventory.py` covering:

- canonical entry parsing;
- fenced backtick examples ignored;
- fenced tilde examples ignored;
- noncanonical inline/example text ignored;
- missing path detection;
- stale path detection;
- exact-match success.

### CI integration

Updated `.github/workflows/ci.yml` so `Repository integrity` now runs:

1. local Markdown link validation;
2. tracked-file documentation inventory validation;
3. high-confidence committed credential scanning;
4. Python repository validator unit tests.

### Release integration

Updated `.github/workflows/release.yml` so tag-time `Release preflight` also rejects an incomplete/stale tracked-file documentation inventory before .NET restore/build/test/package work can proceed.

This makes “no skipped tracked files in documentation” an enforceable repository invariant for both pull requests and releases.

## Documentation discovery/governance synchronized

Updated `README.md`:

- exposes the documentation hub;
- links runtime/settings/configuration/platform/localization/testing/test-catalog/CI-CD/release/accessibility/troubleshooting/repository-reference documents;
- includes the documentation inventory validator in development commands;
- documents repository-integrity coverage;
- explains the release-preflight documentation gate;
- directs contributors to the exhaustive file reference.

Updated `CONTRIBUTING.md`:

- adds Python/repository validation prerequisites/commands;
- makes documentation part of implementation completion;
- requires repository-reference updates for every added/renamed/moved/deleted tracked file;
- points to settings/localization/platform/test documentation contracts;
- requires privacy/security documentation changes when trust/data boundaries change.

Updated `.github/pull_request_template.md`:

- adds all repository validator commands to verification;
- explicitly requires `docs/repository-reference.md` updates for tracked-file changes;
- requires test-catalog updates when test responsibilities change;
- expands privacy/security/ADR documentation checks.

Updated `docs/development.md`:

- adds repository validators to daily/pre-commit flow;
- deepens project/test/script placement rules;
- documents settings transaction/schema change workflow;
- documents localization/platform/external-process/logging/test/documentation rules;
- explicitly requires inventory updates for source/tests/assets/XAML/resources/workflows/templates/scripts/docs.

Updated `docs/testing.md`:

- adds the documentation inventory and Python tests to quality gates;
- points to the exhaustive test catalog;
- documents view-model transaction coverage;
- documents tracked-file inventory behavior and fenced-example handling;
- clarifies automated vs manual/native boundaries.

Updated `docs/release.md`:

- requires the documentation inventory in clean-checkout verification;
- requires `docs/repository-reference.md` release metadata preparation;
- documents the Repository integrity job contents;
- records release preflight documentation enforcement;
- includes exact documentation completeness in the definition of release-ready.

Updated `docs/github-maintenance.md`:

- documents Repository integrity subchecks;
- requires file-reference synchronization during PR review;
- adds documentation governance;
- records release-preflight documentation enforcement;
- adds inventory verification to release-candidate repository audit cadence.

Updated `CHANGELOG.md` and `ROADMAP.md` to record the complete documentation/inventory infrastructure as implemented.

## Phase 7 engineering work already present on this branch

This documentation pass sits on top of the earlier Phase 7 implementation and does not remove/rewrite that work.

### Startup integration testability

- startup platform detector/model;
- startup filesystem abstraction/system adapter;
- startup registry abstraction/Windows adapter;
- injectable `PlatformStartupManager` platform/filesystem/registry/profile/XDG inputs;
- deterministic Windows/macOS/Linux/unsupported/cancellation tests;
- no real CI runner startup configuration modified by those tests.

### Settings deterministic interaction tests

- awaitable internal Settings save/reset operations preserving UI handlers;
- shared settings/startup/timezone/chime/logger test doubles;
- headless save mapping/startup tests;
- invalid quiet-hour no-persist/no-startup tests;
- reset/default/startup disable/control reload tests.

### Settings Updates & About scope

- new Settings Updates & About tab;
- semantic version displayed in Settings;
- explicit Open GitHub Releases action;
- explicit Open About action;
- no background release polling/network update client;
- companion resource catalog;
- headless controls/version coverage.

### External navigation hardening

- centralized `ExternalLinkLauncher`;
- absolute HTTPS/mailto only;
- HTTP/file/script/relative/empty rejection;
- About/Settings reuse the same boundary;
- URI regression tests do not open external programs.

### Accessibility hardening

- explicit automation names on Settings controls whose visible labels are adjacent text;
- accessibility checklist expanded for Settings/external handlers/scaling/screen reader behavior.

### Repository integrity

- offline local Markdown target validator;
- high-confidence credential-pattern scanner that does not print matched values;
- documentation inventory validator;
- Python validator tests;
- dedicated CI Repository integrity job.

### Release identity and packaging

- semantic display version provider;
- user-visible `+build` metadata removal;
- tag-derived publish Version/AssemblyVersion/FileVersion/InformationalVersion;
- prerelease detection/publication;
- release preflight gates;
- least privilege workflow permissions;
- Windows ZIP and Unix tar.gz packaging;
- SHA-256 sidecars;
- post-download checksum verification before GitHub Release creation;
- tag-time changelog/screenshot readiness validator/tests.

## Current release status

The repository is **not** declared release-ready yet.

Intentional release blockers remain:

- README still uses the explicit screenshot placeholder;
- changelog is still `[Unreleased]` rather than an exact intended tag heading;
- native/manual desktop validation has not been evidenced in this connected environment;
- final branch-head CI/CodeQL/Dependency Review must complete successfully after the documentation handoff commit.

The release workflow is intentionally expected to reject premature tags until release metadata/native readiness work is completed.

## Remaining evidence-based release gates

### Native desktop behavior

Windows 11:

- tray show/focus/mini/quit and close-to-tray behavior;
- real HKCU startup enable/disable and login launch;
- real chime behavior;
- native import/export pickers;
- default HTTPS/mailto handler behavior;
- packaged win-x64 archive launch.

macOS:

- real tray/menu-bar behavior;
- x64/arm64 release artifacts as available;
- real LaunchAgent creation/removal/login launch;
- real `afplay` behavior;
- native pickers/default handlers;
- Gatekeeper/signing state documented accurately;
- executable permissions retained after tar extraction.

Linux:

- representative GNOME and KDE sessions;
- tray/status-notifier behavior;
- real XDG autostart behavior;
- optional sound helper success/fallback;
- native pickers/default handlers;
- executable permissions after tar extraction.

### Accessibility

Run `docs/accessibility.md` on primary platforms, including:

- keyboard-only traversal;
- visible focus;
- screen-reader naming;
- high contrast;
- OS scaling/text-size behavior;
- focus/mini mode transitions;
- Settings label semantics;
- external handler focus handoff.

### Release candidate

- replace README screenshot placeholder with verified release-build captures containing no private data;
- move intended changelog content into the exact release tag heading;
- run `scripts/check_release_metadata.py --tag <exact-tag>`;
- run clean-checkout validation from `docs/release.md`;
- confirm final CI/CodeQL/Dependency Review on the release commit;
- confirm branch protection uses the actual current check contexts;
- perform manual private-data/artifact review in addition to automated scanners;
- tag first release candidate only after these gates pass;
- validate settings migration after a real prior tagged preview fixture exists;
- publish stable `v1.0.0` only after the stable-release gates pass.

## Verification behavior in this connected environment

Repository writes are made through the connected GitHub API. The available execution container previously could not resolve GitHub for a local clone and does not provide the repository's authoritative local .NET build environment, so GitHub Actions remains the authoritative compiler/analyzer/test runner for connected commits.

Because CI uses `cancel-in-progress`, the deliberately granular Phase 7 commit sequence cancels older PR CI runs whenever a newer commit arrives. Only the newest frozen branch head should be used for the merge decision.

The final documentation head must specifically prove:

- `scripts/check_markdown_links.py` passes;
- `scripts/check_documentation_inventory.py` reports no missing/stale tracked files;
- `scripts/check_repository_secrets.py` passes;
- Python validator unit tests pass;
- Ubuntu .NET 9 format/build/test/vulnerability checks pass;
- Windows .NET 9 format/build/test/vulnerability checks pass;
- macOS .NET 9 format/build/test/vulnerability checks pass;
- CodeQL completes successfully;
- Dependency Review completes successfully.

Do not merge solely because the PR is structurally mergeable while these checks are queued or incomplete.

## Commit identity note

Connected GitHub writes are performed through the authenticated integration. Current commits on this phase show author identity `Sanskar <sanskarin@outlook.in>` where GitHub reports that metadata, but the connector API should not be treated as a generic local `git config` interface.

For local maintainer work use:

```bash
git config user.name "Sanskar"
git config user.email "sanskarin@outlook.in"
```

Do not rewrite otherwise-valid connected history solely for attribution changes.

## Next exact tasks

1. Freeze the branch after this handoff commit.
2. Confirm the branch is expected to be 96 commits ahead of `main` and 0 behind, unless a concurrent external change occurred.
3. Inspect the newest PR #16 CI/CodeQL/Dependency Review runs for this exact handoff SHA.
4. If Repository integrity fails, use its output to correct the exact missing/stale documentation path, Markdown target, credential finding, or Python validator test.
5. If a .NET/analyzer/test job fails, inspect the exact job log and add only a focused fix/regression commit.
6. Keep PR #16 draft until all available automated checks on the final head are green.
7. When the final head is green, mark PR #16 ready and merge using a normal merge commit so the granular phase history is preserved.
8. After merge, re-check `main` and do not mark native release gates complete without real platform evidence.
9. Continue release-candidate preparation only when verified screenshots/native GUI sessions/artifacts are available.

## Documentation maintenance rule for all future work

No tracked file may be treated as “too small to document.”

For every future file addition/move/rename/deletion:

- update `docs/repository-reference.md` in the same change;
- update the closest specialized documentation when responsibility/behavior changes;
- update `docs/test-catalog.md` for test-file responsibility changes;
- run local Markdown + documentation inventory validators;
- let CI enforce the same rule before merge;
- let Release preflight enforce the same rule before packaging.

This converts the user's “complete documentation without skipping any files” requirement into a permanent repository control rather than a one-time documentation pass.
