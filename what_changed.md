# ChronoDesk — Work Handoff

## Current milestone

**Phase 7 — release hardening implementation complete; automated and native release verification still gated.**

Date: **2026-08-19**

Repository: `https://github.com/sanskarIN/chronodesk`

Continuation branch: `phase-7-release-hardening`

Pull request: `https://github.com/sanskarIN/chronodesk/pull/15`

Phase 7 base commit on `main`: `8695efc3ba81b3e408630691a3da7b8093954ad9`

Latest branch commit immediately before this handoff update: `fc845252242584cb9485ff7ca01bd3549986de70`

Repository policy: **PUBLIC / OPEN SOURCE / MIT**

Required visible credit: **Made by the Sanskar**

Requested Git author/contact email: `sanskarin@outlook.in`

The connected GitHub contents/write API does not expose an author-email field for normal file commits. Commits created through that API use the authenticated GitHub identity. Local contributor setup continues to document the requested Git email.

---

## Source of truth

ChronoDesk continues to be implemented against the uploaded `10_chronodesk_master_prompt.md` and the actual repository state. The implementation is not being replaced with unrelated features merely to increase commit count.

Phase 7 specifically closes prompt-level gaps around:

- reversible/destructive world-clock UX;
- explicit empty/search feedback;
- deterministic startup integration generation;
- startup path/input hardening;
- settings schema migration infrastructure;
- release artifact integrity metadata;
- documentation-link verification;
- high-signal tracked-secret verification;
- centralized safe external-link launching;
- deeper automated tests;
- GitHub Actions queue/concurrency behavior during granular development.

---

## Application scope implemented

ChronoDesk now contains a production-oriented C#/.NET 9 + Avalonia desktop clock application for Windows, macOS, and Linux with:

- live local digital time;
- 12-hour and 24-hour modes;
- seconds toggle;
- date and weekday display;
- ISO week number and optional calendar/UTC-offset details;
- multiple offline world-clock cards;
- OS-provided timezone discovery/search;
- localized timezone-search result count and empty-state feedback;
- undo for the most recently removed world-clock card;
- local settings persistence;
- explicit settings schema versioning and legacy migration pipeline;
- bounded and normalized settings import/export;
- first-run onboarding;
- full-screen focus clock;
- mini always-on-top mode;
- normal always-on-top preference;
- system/light/dark/high-contrast presentation choices;
- typography, clock-size, spacing, and layout controls;
- reduced-motion preference;
- hourly, half-hourly, and quarter-hourly chime policies;
- quiet hours including ranges that cross midnight;
- current-user startup integration on Windows, macOS, and Linux;
- deterministic/testable startup registration builders;
- system tray actions where supported by the desktop environment;
- keyboard shortcuts;
- local structured logs with redaction and rotation;
- About/support/funding UI;
- centralized approved external destinations with `https`/`mailto` launch policy;
- English-first `.resx` localization architecture;
- no required ChronoDesk account, telemetry service, analytics endpoint, advertising SDK, cloud database, or product secret.

---

## Architecture implemented

Solution projects:

- `src/ChronoDesk.Core` — domain models, abstractions, clock formatting, quiet hours, chime policy;
- `src/ChronoDesk.Infrastructure` — persistence, migrations, logging, timezone catalog, startup integration, platform chime integration;
- `src/ChronoDesk.App` — Avalonia application, views, localization resources, UI services, view models, composition root;
- `tests/ChronoDesk.Tests` — deterministic domain/integration/persistence/security/headless UI tests.

The design remains a modular desktop monolith. Platform side effects are kept behind abstractions or deterministic helper functions where practical so business and formatting behavior can be tested without a real GUI/session startup directory.

---

# Phase 7 completed work

## 1. Deterministic startup registration generation

Added:

- `src/ChronoDesk.Infrastructure/Platform/StartupRegistrationDocuments.cs`

Changed:

- `src/ChronoDesk.Infrastructure/Platform/PlatformStartupManager.cs`

Implemented:

- deterministic Windows Run-command generation;
- deterministic macOS LaunchAgent XML generation;
- deterministic Linux XDG desktop-entry generation;
- fixed `--background` startup argument;
- trimming/normalization of executable paths;
- rejection of executable paths containing control characters;
- rejection of embedded double quotes for Windows Run commands;
- XML escaping for macOS executable paths;
- escaping for Linux desktop-entry `Exec` path characters including backslashes, quotes, backticks, and dollar signs;
- exact expected-registration comparison when checking whether startup is enabled;
- atomic temporary-file + move replacement for macOS/Linux startup files;
- cleanup of temporary registration files on failure;
- current-user integration only.

Why this changed:

Previously startup generation was embedded in `PlatformStartupManager`, which made escaping and expected-registration behavior harder to test without touching real platform state. The builder layer now keeps string/document generation deterministic and testable.

## 2. Startup registration tests

Added:

- `tests/ChronoDesk.Tests/StartupRegistrationDocumentsTests.cs`

Coverage includes:

- quoted Windows executable command;
- required background argument;
- embedded Windows quote rejection;
- valid macOS plist XML;
- macOS XML escaping;
- Linux desktop-entry generation;
- Linux executable escaping;
- control-character rejection;
- outer-whitespace normalization.

A follow-up compatibility fix removed use of a newer/uncertain xUnit `Assert.Contains` overload and keeps assertions compatible with the pinned xUnit baseline.

## 3. World-clock undo and clearer search state

Changed:

- `src/ChronoDesk.App/ViewModels/MainWindowViewModel.cs`
- `src/ChronoDesk.App/Views/MainWindow.axaml`
- `src/ChronoDesk.App/Views/MainWindow.axaml.cs`
- `src/ChronoDesk.App/Localization/Strings.resx`
- `src/ChronoDesk.App/Localization/Strings.cs`

Implemented:

- capture of the most recently removed world-clock record and its previous index;
- `CanUndoWorldClockRemoval` state;
- `UndoWorldClockRemovalAsync`;
- restoration at the previous dashboard position;
- duplicate-timezone protection during undo;
- undo candidate cleared after settings import/reset;
- localized `Undo` action;
- localized `World clock restored` status;
- localized timezone-search empty state;
- localized timezone-search result-count format;
- visible search feedback text below the timezone search field;
- visible undo button only when a reversible removal exists.

This addresses the master prompt requirement for undo on destructive operations where practical and stronger empty/status states.

## 4. World-clock and search regression tests

Changed:

- `tests/ChronoDesk.Tests/MainWindowViewModelTests.cs`
- `tests/ChronoDesk.Tests/HeadlessUiSmokeTests.cs`

Added coverage for:

- remove + undo round trip;
- restoration to original ordering position;
- undo state clearing;
- removal/restoration status strings;
- populated timezone-search feedback;
- empty timezone-search feedback;
- presence of `UndoWorldClockButton` in headless XAML;
- presence of `TimeZoneSearchStatusText` in headless XAML.

## 5. Centralized approved application links

Added:

- `src/ChronoDesk.App/Services/AppLinks.cs`
- `src/ChronoDesk.App/Services/ExternalUriLauncher.cs`

Changed:

- `src/ChronoDesk.App/Views/AboutWindow.axaml.cs`

Implemented:

- one centralized source for repository, release, funding, business, and support destinations;
- reusable URI validation;
- `https` only for browser destinations;
- `mailto` allowed for mail destinations;
- rejection of `http`, `file`, script schemes, relative paths, malformed destinations, and HTTPS user-info/credential URLs;
- safe failure when no browser/mail handler is available.

Approved product destinations remain fixed constants; user settings/import files do not supply executable external destinations.

## 6. External URI policy tests

Added:

- `tests/ChronoDesk.Tests/ExternalUriLauncherTests.cs`

Coverage includes:

- GitHub HTTPS accepted;
- BMC HTTPS accepted;
- business mail accepted;
- insecure HTTP rejected;
- local file URI rejected;
- script URI rejected;
- credential-bearing HTTPS rejected;
- relative path rejected;
- empty/whitespace rejected;
- every `AppLinks` destination remains allowed by the launcher policy.

Tests validate policy only and do not open a real browser/mail client.

## 7. Settings schema migration pipeline

Added:

- `src/ChronoDesk.Infrastructure/Persistence/SettingsMigrationPipeline.cs`

Changed:

- `src/ChronoDesk.Infrastructure/Persistence/JsonSettingsStore.cs`

Implemented:

- JSON root validation before treating content as settings;
- source schema extraction from raw JSON;
- missing `schemaVersion` treated explicitly as legacy schema `0`;
- explicit schema `0 -> 1` migration;
- negative schema versions rejected;
- future schema versions rejected;
- stepwise migration loop with an explicit migration switch;
- failure when no intermediate migration is known instead of guessing;
- migration before final `Normalize()`;
- current saves/exports still emit current normalized schema.

The current `0 -> 1` migration is intentionally data-preserving. Pre-versioned development documents used the same preference semantics; Phase 7 makes the transition explicit/testable without inventing a fake incompatible change.

## 8. Settings migration tests

Changed:

- `tests/ChronoDesk.Tests/JsonSettingsStoreTests.cs`

Added coverage for:

- missing-schema legacy document migration;
- explicit schema-0 migration;
- preference preservation during migration;
- negative schema rejection;
- future schema rejection;
- current schema result after migration.

Existing coverage for numeric enum rejection, corrupt JSON recovery, persistence round trip, and import/export remains.

## 9. Settings migration architecture documentation

Added:

- `docs/adr/0007-stepwise-settings-schema-migrations.md`

Changed:

- `docs/adr/0002-json-settings-persistence.md`
- `docs/testing.md`
- `ROADMAP.md`
- `CHANGELOG.md`

ADR 0007 records:

- missing schema = legacy schema `0`;
- one-version-at-a-time migration;
- future/negative version rejection;
- migration before normalization;
- required regression-test policy for future schema increments;
- why a database migration framework is unnecessary for this bounded JSON preference document.

ADR 0002 now links directly to ADR 0007.

## 10. Release archive integrity

Changed:

- `.github/workflows/release.yml`
- `docs/release.md`
- `docs/release-notes-template.md`

Implemented in the release workflow:

- SHA-256 sidecar for every runtime ZIP;
- publication job verifies every archive against its sidecar before creating a release;
- expected current artifact count checked;
- generated `release-manifest.json` containing:
  - schema version;
  - product;
  - Git tag/version;
  - source commit;
  - generation timestamp;
  - archive names;
  - archive sizes;
  - archive SHA-256 values;
- `release-manifest.json.sha256`;
- release uploads include ZIPs, sidecars, and manifest files.

Documentation now explains verification on PowerShell, Linux, and macOS and explicitly states that checksums are integrity metadata, not a replacement for publisher code signing/notarization.

## 11. Local documentation-link verification

Added:

- `scripts/verify-doc-links.ps1`

Changed:

- `.github/workflows/ci.yml`
- `docs/testing.md`

Implemented:

- recursive Markdown scan;
- repository/file-relative local target resolution;
- fragment/query stripping for local paths;
- URI decoding;
- external schemes and same-document anchors ignored;
- build/generated directories excluded;
- CI failure for missing local targets;
- script runs on all three CI operating systems.

This is intentionally a deterministic local-link gate and not an internet crawler.

## 12. High-signal tracked-file secret verification

Added:

- `scripts/verify-no-secrets.ps1`

Changed:

- `.github/workflows/ci.yml`
- `SECURITY.md`
- `docs/testing.md`

Implemented:

- enumerates Git-tracked files with `git ls-files`;
- skips known binary/archive extensions;
- skips files above the scanner inspection limit;
- checks selected high-signal patterns including private-key headers and common token families;
- excludes the scanner's own pattern source to avoid self-detection;
- reports only file + detector rule;
- never prints the matched secret value;
- runs in CI on Ubuntu, Windows, and macOS.

This is a defense-in-depth check, not a claim that every possible secret format can be detected.

## 13. GitHub Actions queue/concurrency hardening

Changed:

- `.github/workflows/codeql.yml`
- `.github/workflows/dependency-review.yml`

The main CI workflow already cancelled superseded same-ref runs. Phase 7 adds equivalent same-ref cancellation to:

- CodeQL;
- Dependency Review.

Reason: granular commits across multiple concurrent project sessions created many obsolete queued workflow runs. Newer workflow definitions now cancel superseded runs for the same ref, reducing future queue pressure while preserving the final branch-head verification run.

Older runs created before these concurrency definitions may remain in GitHub's queue until GitHub processes/cancels them; the available connected GitHub action API does not expose a general workflow-run cancellation operation in this environment.

---

# Repository engineering state

The repository contains:

- solution/project configuration;
- central package management;
- strict nullable/analyzer/compiler settings;
- editor/build settings;
- `.gitignore`, `.gitattributes`, and safe `.env.example`;
- MIT `LICENSE`;
- `README.md`;
- `CHANGELOG.md`;
- `ROADMAP.md`;
- `SECURITY.md`;
- `PRIVACY.md`;
- `SUPPORT.md`;
- `CONTRIBUTING.md`;
- `CODE_OF_CONDUCT.md`;
- architecture/development/setup/testing/release/troubleshooting/accessibility/performance/GitHub-maintenance documentation;
- seven ADRs including persistence, timezone, startup, logging, localization, and schema migration decisions;
- GitHub issue forms and pull-request template;
- Buy Me a Coffee funding configuration;
- Dependabot configuration;
- multi-OS CI;
- local documentation-link verification;
- high-signal tracked-secret verification;
- CodeQL;
- dependency review;
- tagged release packaging workflow;
- release archive checksums and integrity manifest.

---

# Security and resilience state

Implemented protections include:

- maximum settings-import size;
- JSON object-root validation;
- explicit source schema detection;
- schema-version migration pipeline;
- negative/future schema rejection;
- JSON string-enum parsing with numeric enum values rejected;
- normalization of invalid runtime enum values;
- normalization of nullable nested settings values;
- bounded font/world-clock/timezone text;
- control-character flattening for imported display text;
- maximum world-clock count;
- atomic settings writes;
- preservation of corrupt primary settings documents;
- safe fallback to defaults after unreadable primary settings;
- imported backup files cannot silently modify OS startup registration;
- best-effort startup rollback if startup registration changes but settings persistence then fails;
- deterministic generated startup registration;
- startup executable control-character rejection;
- Windows embedded-quote rejection;
- macOS XML escaping;
- Linux desktop-entry path escaping;
- atomic macOS/Linux startup-file replacement;
- exact startup-registration matching;
- centralized `https`/`mailto` external URI policy;
- rejection of credential-bearing HTTPS URLs;
- no imported executable/shell-command/credential fields;
- argument-list/fixed-path process launching for optional Unix chime helpers;
- redacted local logging;
- tracked-file high-signal secret scan;
- NuGet vulnerability inspection;
- dependency review;
- CodeQL;
- Dependabot.

---

# Automated test inventory

Current automated coverage includes:

- clock formatting;
- 12/24-hour behavior;
- seconds rendering;
- ISO week/calendar details;
- quiet-hour boundaries;
- overnight quiet-hour ranges;
- chime cadence and duplicate suppression;
- settings normalization and invariants;
- JSON save/load round trips;
- portable export/import;
- corrupt JSON recovery;
- unsupported/numeric enum rejection;
- oversized import rejection;
- deterministic malformed-import fuzz corpus;
- missing-schema/schema-0 migration;
- negative/future schema rejection;
- system timezone discovery/search/fallback;
- deterministic property-style domain coverage;
- startup registration generation/escaping;
- startup-preference persistence consistency through the main view model;
- startup rollback behavior when persistence fails;
- safe import behavior preserving the device startup preference;
- world-clock remove/undo ordering;
- timezone-search feedback state;
- external URI allow-list policy;
- Avalonia headless smoke tests for primary windows;
- mini/focus window-mode transitions;
- undo/search-feedback control presence;
- localized resource loading in primary windows.

---

# Commands/checks actually run in this continuation

## Local execution environment

Checked command availability from the connected execution environment:

```text
dotnet: not installed / not on PATH
pwsh: not installed / not on PATH
git: /usr/bin/git
git version 2.47.3
```

Therefore this continuation does **not** claim a local `dotnet restore`, `dotnet build`, `dotnet test`, `dotnet format`, PowerShell script execution, native GUI launch, or native package launch.

Earlier network clone attempts from this execution environment also encountered DNS/network access limitations, so GitHub repository inspection/writes and GitHub Actions remain the authoritative remote verification mechanism available here.

## GitHub Actions status observed before this handoff commit

For branch commit:

`fc845252242584cb9485ff7ca01bd3549986de70`

GitHub registered:

- Dependency Review run `32216403945` — **queued** at observation time;
- CI run `32216403971` — **queued** at observation time;
- CodeQL run `32216403949` — **queued** at observation time.

This status is intentionally recorded as queued, not PASS.

The queue is affected by many superseded runs created during granular development. Phase 7 added concurrency cancellation to CodeQL and Dependency Review; the CI workflow already had cancellation. The connected GitHub tooling does not expose a general action for cancelling all historic queued workflow runs, so old queue entries cannot be truthfully cleared from this session.

This handoff update creates another documentation-only branch commit, so GitHub may register a newer set of branch-head checks after this file is committed. The final release/merge gate remains: use the latest branch-head CI/CodeQL/dependency-review results, not the older run IDs above.

---

# Errors/bugs found and fixed during Phase 7

## C# startup path validation compile/API issue

Initial startup builder code used a `string.Contains(char, StringComparison)` shape that is not a valid API form for the target baseline.

Fixed by using the valid character overload:

```csharp
path.Contains('"')
```

Regression-test coverage remains.

## Startup document assertions

Startup tests initially used `Assert.Contains(..., StringComparison.Ordinal)` overloads whose compatibility with the pinned xUnit baseline was unnecessary to depend on.

Fixed by using baseline-compatible `Assert.Contains(string, string)` assertions.

## Secret-scan Git enumeration portability

The first scanner version used null-delimited `git ls-files -z` output through PowerShell string handling. To avoid platform/host differences, it was simplified to normal line-based `git ls-files`, whose tracked paths are safe for this repository's file naming policy.

## GitHub contents stale-SHA write conflict

One sequential edit of `scripts/verify-no-secrets.ps1` received an HTTP 409 because the supplied blob SHA had already changed.

Resolution:

1. fetched the branch file again;
2. used the returned current blob SHA;
3. applied the intended fix successfully.

No content was lost.

## Workflow queue accumulation

Many atomic commits caused old CodeQL/dependency-review runs to accumulate because those workflows did not originally have same-ref concurrency cancellation.

Fixed for future runs by adding `cancel-in-progress: true` concurrency groups to both workflows.

---

# Migration notes

Current settings schema: **1**.

Migration policy now implemented:

- no `schemaVersion` field -> interpret as legacy schema `0`;
- schema `0` -> migrate to schema `1`;
- schema `< 0` -> reject;
- schema `> CurrentSchemaVersion` -> reject;
- every future migration must advance exactly one version at a time;
- a missing migration step must fail rather than guess;
- final migrated settings pass through normal bounded/default-safe normalization;
- importing settings still preserves the current device startup preference and cannot silently change operating-system startup registration.

The current `0 -> 1` migration does not rename/remove fields because pre-versioned development settings used the same semantics. It establishes the real migration mechanism and tests without manufacturing a fake breaking change.

For the first future schema increment:

1. increment `AppSettings.CurrentSchemaVersion`;
2. add a `1 -> 2` migration case;
3. retain all `0 -> 1` tests;
4. add a real previous-version fixture;
5. test a multi-step `0 -> 1 -> 2` upgrade;
6. update ADR 0007, CHANGELOG, release notes, and this file.

---

# Release notes draft for Phase 7 changes

## Added

- world-clock removal undo;
- timezone-search count/empty feedback;
- deterministic startup registration builders and tests;
- explicit settings schema migration pipeline;
- migration compatibility tests;
- centralized external URI policy and tests;
- documentation local-link CI gate;
- tracked-file high-signal secret CI gate;
- release archive SHA-256 sidecars;
- release integrity manifest and manifest checksum.

## Changed

- startup enabled-state detection now requires the exact expected registration;
- macOS/Linux startup files use atomic replacement;
- settings reads identify and migrate source schema before normalization;
- About external destinations use centralized approved constants/launcher policy;
- CodeQL/dependency-review workflows cancel superseded same-ref runs.

## Security

- startup executable paths reject control characters;
- Windows startup commands reject embedded quotes;
- macOS/Linux startup registration escapes path content appropriately;
- external launch policy rejects insecure/local/script/credential-bearing destinations;
- negative/future settings schemas fail closed;
- tracked-file scanner adds a non-echoing high-signal secret gate;
- release archives receive verified integrity digests.

## Release state

No stable release tag has been created by Phase 7. Native desktop verification and latest-head GitHub Actions checks remain release gates.

---

# Native validation still required before stable v1.0.0

These are release gates rather than unimplemented source features:

- Windows 11 tray behavior in an interactive desktop session;
- macOS tray/menu behavior on current Intel and Apple Silicon GUI environments;
- Linux tray behavior on representative GNOME/KDE sessions;
- startup enable/disable using real current-user OS integration;
- chime behavior with platform sound facilities present/absent;
- real native file-picker import/export flows;
- screen-reader and keyboard accessibility on each primary platform;
- high contrast with actual platform accessibility settings;
- display scaling and large-text behavior;
- real packaged application launch for each release RID;
- replacement of the README/documentation screenshot placeholder with screenshots from verified release builds;
- signed/notarized installers if signing infrastructure is later supplied.

These tasks cannot be truthfully marked complete from a repository-only API session.

---

# Open issues / remaining release gates

1. Latest branch-head CI must complete successfully on Ubuntu, Windows, and macOS.
2. Latest branch-head CodeQL must complete successfully.
3. Latest branch-head dependency review must complete successfully.
4. Any failure reported by those jobs must be fixed before merge/release.
5. PR #15 should be merged only after the automated branch-head verification is satisfactory.
6. Real desktop manual checks from `docs/testing.md`, `docs/accessibility.md`, and `docs/release.md` remain mandatory before stable `v1.0.0`.
7. Real screenshots must replace the placeholder only after a verified build is run.
8. Branch-protection required-check names should be confirmed against actual successful workflow check names before the release branch policy is declared final.
9. Signing/notarization remains optional future infrastructure and requires protected secrets/certificates not stored in Git.

---

# Next exact tasks

If resuming while PR #15 is still open:

1. read this file first;
2. inspect the current PR head;
3. fetch the latest CI/CodeQL/Dependency Review runs for that exact head;
4. if a job failed, fetch the failed job/log details and fix the concrete defect with a focused commit + regression test where applicable;
5. update this file with the actual result;
6. merge PR #15 only after automated verification is satisfactory;
7. after merge, confirm `main` contains the Phase 7 files and update this handoff only if the merge changes the final state materially.

If PR #15 has already been merged in a later session:

1. do not reimplement Phase 7;
2. proceed to real release-candidate desktop validation;
3. record OS/version/architecture/desktop details and pass/fail evidence in the release checklist;
4. replace screenshots only with real verified application captures;
5. prepare the first tagged preview/release candidate only when documented gates pass;
6. do not publish stable `v1.0.0` until all Definition-of-Done gates that require real execution are satisfied.

---

# Recent meaningful Phase 7 commits

- `fc845252242584cb9485ff7ca01bd3549986de70` — `docs: link persistence decision to migration pipeline`
- `25b7ea92d3b5b596bf295c05bfbd450d28016ee1` — `docs: document settings schema migration coverage`
- `0950e3220e149be5e2069dfcb3a241fb10e02321` — `docs: mark settings migration hardening implemented`
- `0f7f869db6c6edda689aa6776a62bba098b5de4b` — `docs: record settings migrations and workflow queue hardening`
- `7e15e0008a0767662951907a650ca6c0c7db699c` — `ci: cancel superseded dependency review runs`
- `1c32ba3897b930db99dfd7123dd17ea30b195799` — `ci: cancel superseded CodeQL runs`
- `85dbfbc8f24caee9c68a0366172d99f7d21ba6d3` — `docs: record stepwise settings migration architecture`
- `5354bce0f8da8eb99e198004aaa25521c462af31` — `test: cover legacy and unsupported settings schema versions`
- `866a1f0b82f1cc4619e94bfbccf2e96b3f1dfde7` — `refactor: route settings reads through schema migration pipeline`
- `517ea90f1480d67c3b4b2ba9ad142a4f72caffdb` — `feat: add versioned settings migration pipeline`
- `1afc13c64c37573b480d229e7d17682a0b44741f` — `docs: add release integrity fields to notes template`
- `138aec3a9cb9665c84f6af75186f1ca3803e61cb` — `docs: align testing guide with release hardening checks`
- `8b251bdc0e41a6d2775721cf3141d5f79fdde043` — `docs: document startup and tracked-secret hardening`
- `a1fcb5d6183bb2eac9584ff78d74d37ac2caa591` — `test: keep startup assertions compatible with baseline xunit API`
- `aade97f2bc4083388a229d3a1212a937388a0ded` — `docs: add phase 7 release hardening roadmap`
- `b83824821d178ef8144ad084b061d9816d1eb068` — `docs: record phase 7 release hardening changes`
- `a3d9d86c1c5a449eb7e6116ac3a05baa231347f9` — `ci: scan tracked files for high-signal secrets`
- `87a2ae98f6fbfecb7275e2c703cc06390c618126` — `fix: enumerate tracked files portably in secret scan`
- `38a72976c211efa767e66403a43e0104c68ac261` — `ci: add release checksums and integrity manifest`
- `a2c6516e7c5efbb71b138831c4cd79337717e77d` — `fix: use valid quote validation for Windows startup paths`
- `55476b18758dafa012023ea79552427868d6afd8` — `test: cover startup registration document generation`
- `d4954ca4e9dc1f5d6e950865db02cf31e9464a3a` — `refactor: centralize platform startup registration generation`
- `59073ce3cc146907786aca15aa79864519b9d3b9` — `security: add deterministic startup registration builders`
- `97778629ca5eb095cfbeffddcf36783479b9d17e` — `docs: refresh ChronoDesk implementation handoff`

Additional Phase 7 commits exist for localization, undo/search UI, URI services/tests, documentation scripts, and release documentation. Use the pull request commit list as the authoritative complete sequence.

---

# Handoff rule

For every later meaningful milestone, update this file with:

- current version/milestone;
- completed work;
- files/modules added or changed;
- tests added;
- commands/checks that actually ran and exact results;
- known limitations;
- open issues;
- next exact tasks;
- migration notes;
- release notes draft;
- latest meaningful commit hashes/messages.

Do not convert queued checks into PASS, do not claim native verification from headless tests, and do not call ChronoDesk stable-release complete until the actual Definition-of-Done gates in the master prompt and repository release documentation are satisfied.
