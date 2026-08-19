# ChronoDesk — Work Handoff

## Current milestone

**Phase 7 — release-hardening source implementation complete; latest-head automated verification and real-desktop release validation remain gated.**

Date: **2026-08-19**

Repository: `https://github.com/sanskarIN/chronodesk`

Continuation branch: `phase-7-release-hardening`

Pull request: `https://github.com/sanskarIN/chronodesk/pull/15`

Phase 7 base on `main`: `8695efc3ba81b3e408630691a3da7b8093954ad9`

Latest code/docs head observed **before this handoff commit**: `0a1fc4b3021b9fd9bc8f7ff648960e4518317874`

PR state at that observation:

- OPEN
- mergeable: true
- draft: false
- 101 commits
- 55 changed files
- 3199 additions
- 392 deletions

Repository policy: **PUBLIC / OPEN SOURCE / MIT**

Required visible credit: **Made by the Sanskar**

Requested Git author/contact email: `sanskarin@outlook.in`

Connected GitHub file writes use the authenticated GitHub identity. Earlier raw commit inspection for this repository showed the requested email in Git author metadata; the contents API itself does not provide an author-email override field per write. Local contributor documentation continues to set `sanskarin@outlook.in` explicitly.

---

# Source of truth

ChronoDesk is implemented against the uploaded `10_chronodesk_master_prompt.md` and the actual repository source tree.

This continuation did **not** add unrelated functionality to inflate commit count. Commits remain small and concern-oriented across feature, fix, security, test, documentation, CI, and refactor changes.

The current implementation is a C#/.NET 9 + Avalonia desktop clock for Windows, macOS, and Linux with an offline-first/local-data design.

---

# Product capabilities implemented

## Clock and calendar

- live local digital clock;
- 12-hour and 24-hour modes;
- seconds toggle;
- date and weekday;
- ISO week number;
- optional calendar details/day-of-year/UTC offset;
- configurable font family;
- configurable clock size;
- configurable spacing;
- Centered/Compact/Dashboard layouts.

## World clocks

- OS/.NET `TimeZoneInfo` catalog;
- timezone search by ID/display/search text;
- up to `AppSettings.MaximumWorldClockCount` = 24 saved cards;
- visible search result count;
- explicit no-results state;
- duplicate timezone add protection;
- duplicate timezone normalization during imported/local settings normalization;
- case-insensitive timezone uniqueness;
- duplicate clock-ID normalization;
- remove action;
- last-removal Undo;
- Undo restores the previous card position;
- last remaining clock cannot be removed;
- Windows/IANA mapping attempts through .NET;
- UTC fallback when a saved timezone cannot be resolved on the current platform.

## Desktop modes

- F11 full-screen focus clock;
- Esc exits focus;
- Ctrl+M mini mode;
- mini mode always topmost;
- normal always-on-top setting;
- mini-mode exit uses the **current saved** always-on-top preference rather than stale pre-mini state;
- previous mini-mode dimensions/position are restored;
- tray Show/Focus/Mini/Quit actions where reliable tray-menu integration exists;
- close/background hiding only when a reliable tray restoration route exists;
- when tray integration is unavailable, ChronoDesk stays visible/closeable instead of intentionally creating an unreachable hidden process.

## Themes and accessibility

- System theme;
- Light;
- Dark;
- High Contrast;
- separate high-contrast preference;
- reduced-motion preference;
- no decorative fake-loading animation/delay;
- localized loading/status/error/empty feedback;
- system palette follows Avalonia `ActualThemeVariant`;
- custom palette refreshes when the OS theme changes at runtime while System mode is selected;
- initial system palette is applied before creating the first desktop window;
- keyboard-first shortcuts and native control navigation;
- semantic automation naming on key controls;
- scalable typography;
- accessibility release checklist in `docs/accessibility.md`.

## Settings

Settings now contains the required functional sections:

- Clock;
- Appearance;
- Accessibility;
- Behavior;
- Data & Privacy;
- Updates;
- About.

The Updates section:

- displays the exact application informational version;
- preserves prerelease labels such as `0.1.0-preview`;
- strips `+build-metadata` from user-facing display text;
- performs **no background update polling**;
- opens only the fixed official GitHub Releases HTTPS destination after explicit user activation;
- keeps all normal clock behavior offline.

The Settings About section includes:

- product description;
- version + MIT License text;
- privacy/license summary;
- GitHub repository action;
- Buy Me a Coffee action;
- primary business email;
- secondary business email;
- support email;
- visible **Made by the Sanskar** credit.

A standalone About window remains available from the main window and uses the same version helper and safe external link policy.

## Chimes

- disabled by default;
- hourly/30-minute/15-minute cadence;
- quiet hours;
- overnight quiet-hour ranges;
- duplicate suppression for the same cadence minute;
- Windows local beep path;
- fixed-path macOS/Linux helpers when present;
- no shell command construction;
- user text is not inserted into helper arguments;
- unused stdout/stderr streams are no longer redirected, avoiding unnecessary pipe buffering/wait risks.

## Startup integration

- opt-in only;
- current-user scope;
- Windows Run key;
- macOS LaunchAgent;
- Linux XDG autostart;
- fixed `--background` argument;
- deterministic/testable registration document builders;
- executable path validation;
- Windows embedded-quote rejection;
- macOS XML escaping;
- Linux desktop-entry escaping;
- exact expected-registration comparison;
- atomic macOS/Linux registration-file replacement;
- imported settings cannot silently alter the local startup setting;
- if an explicit startup change succeeds but settings persistence fails, ChronoDesk attempts to restore the previous startup state;
- rollback uses `CancellationToken.None`, so cancellation of the failed save does not itself prevent startup-state restoration.

---

# Persistence and migration implementation

Primary implementation:

- `src/ChronoDesk.Infrastructure/Persistence/JsonSettingsStore.cs`
- `src/ChronoDesk.Infrastructure/Persistence/SettingsMigrationPipeline.cs`
- `src/ChronoDesk.Core/Models/AppSettings.cs`

Current settings schema: **1**.

Implemented rules:

- settings document maximum = 2 MiB;
- input size is checked from the **opened FileStream** before JSON parsing;
- JSON root must be an object;
- missing `schemaVersion` is interpreted as legacy schema 0;
- schema 0 -> schema 1 migration;
- negative schema rejected;
- future schema rejected;
- migration advances one schema version at a time;
- missing intermediate migration fails instead of guessing;
- string-enum parsing rejects numeric enum values;
- invalid runtime enum values normalize to safe defaults;
- null nested settings values normalize safely;
- imported font/world-clock/timezone text is bounded and flattened to a single line;
- duplicate clock IDs are removed;
- duplicate timezone IDs are removed case-insensitively;
- world-clock count is capped by the single domain constant;
- at least one world clock remains;
- writes use unique temporary files followed by replacement;
- corrupt primary settings are preserved when possible;
- corrupt recovery names now use millisecond timestamp precision plus a GUID, preventing rapid repeated recovery filename collisions;
- import cannot change current device startup registration implicitly.

Migration decision: `docs/adr/0007-stepwise-settings-schema-migrations.md`.

Persistence decision: `docs/adr/0002-json-settings-persistence.md`.

---

# External destinations and privacy

Centralized destinations:

- `src/ChronoDesk.App/Services/AppLinks.cs`

Safe launcher:

- `src/ChronoDesk.App/Services/ExternalUriLauncher.cs`

Allowed schemes:

- HTTPS;
- mailto.

Rejected examples/policies:

- HTTP;
- file URI;
- script URI;
- relative path;
- malformed URI;
- credential-bearing HTTPS user-info.

Fixed product destinations include:

- repository;
- GitHub Releases;
- Buy Me a Coffee;
- primary business email;
- secondary business email;
- support email.

The application contains no required account, telemetry SDK, advertising SDK, analytics endpoint, cloud database, background update tracker, API key, or product credential.

`PRIVACY.md` now explicitly documents:

- no background update polling;
- user-initiated Releases navigation only;
- local informational version reading;
- opened-stream import size validation;
- object/schema validation;
- duplicate timezone normalization;
- collision-resistant corrupt recovery files;
- cancellation-independent startup rollback;
- tray-unavailable behavior;
- local chime helper behavior.

---

# Loading and state quality

Added:

- `src/ChronoDesk.App/Localization/StateStrings.resx`
- `src/ChronoDesk.App/Localization/StateStrings.cs`

Main-window state now begins with a real localized:

`Loading local clock data…`

There is no fake delay.

`MainWindowViewModel` exposes:

- `IsInitialized`;
- `IsLoading`;
- localized status text;
- localized singular/plural world-clock count.

World-clock count no longer lowercases an English section heading, which improves localization correctness.

---

# Version/update implementation

Added:

- `src/ChronoDesk.App/Services/AppVersionInfo.cs`
- `src/ChronoDesk.App/Localization/UpdateStrings.resx`
- `src/ChronoDesk.App/Localization/UpdateStrings.cs`

`AppVersionInfo.GetDisplayVersion()`:

1. reads `AssemblyInformationalVersionAttribute`;
2. uses it when present;
3. strips build metadata after `+`;
4. preserves prerelease identifiers;
5. falls back to assembly version/development only when informational version is unavailable.

A static review caught and fixed an invalid `IndexOf(char, StringComparison)` call in the first version; the final implementation uses the valid character overload.

Updates section behavior:

- version only from local assembly metadata;
- no feed/API call;
- no timer/background worker;
- explicit user click opens `AppLinks.Releases` through `ExternalUriLauncher`;
- safe failure status if no system handler is available.

---

# Theme implementation follow-through

Added:

- `src/ChronoDesk.App/Services/ThemePaletteSelector.cs`

`ThemePaletteSelector` centralizes fixed palette selection for:

- Light;
- Dark;
- High Contrast;
- System using Avalonia's current actual theme variant.

`App.axaml.cs` now:

- stores active normalized settings used for theme selection;
- subscribes to `ActualThemeVariantChanged`;
- applies the current system palette before first desktop-window construction;
- applies the requested explicit/System variant when settings change;
- recomputes the five custom brushes when the actual OS theme changes.

Official Avalonia source/API was checked during this work to verify the relevant theme and tray properties/events rather than relying on stale memory.

Real visual behavior still needs native Windows/macOS/Linux validation.

---

# Tray safety follow-through

Primary files:

- `src/ChronoDesk.App/Services/TrayVisibilityPolicy.cs`
- `src/ChronoDesk.App/App.axaml.cs`
- `src/ChronoDesk.App/Views/MainWindow.axaml.cs`

`App.IsTrayIntegrationAvailable` starts false and is set true only when the created Avalonia tray icon exposes a native menu exporter.

Official Avalonia source was inspected and confirms `TrayIcon.NativeMenuExporter` maps to the platform implementation's menu exporter.

Close-to-tray requires:

- not explicit application quit;
- MinimizeToTray enabled;
- reliable tray restoration available.

Background startup hiding requires:

- `--background` requested;
- MinimizeToTray enabled;
- reliable tray restoration available.

This protects Linux/other desktop environments where tray support can be absent or incomplete.

---

# Release and repository quality

Repository includes:

- MIT `LICENSE`;
- README;
- CHANGELOG;
- ROADMAP;
- SECURITY;
- PRIVACY;
- SUPPORT;
- CONTRIBUTING;
- CODE_OF_CONDUCT;
- architecture/setup/development/testing/release/troubleshooting/accessibility/performance/GitHub-maintenance docs;
- seven ADRs;
- issue forms;
- PR template;
- funding config;
- Dependabot;
- CI;
- CodeQL;
- Dependency Review;
- tagged release workflow;
- documentation local-link verifier;
- high-signal tracked-secret verifier.

Release workflow creates self-contained ZIPs for:

- win-x64;
- linux-x64;
- osx-x64;
- osx-arm64.

Release integrity output:

- one `.sha256` per ZIP;
- checksum verification before publication;
- `release-manifest.json` containing version/tag/source commit/archive name/archive size/archive SHA-256;
- `release-manifest.json.sha256`.

Checksums are documented as integrity metadata, not a replacement for code signing/notarization.

CodeQL and Dependency Review now use same-ref concurrency cancellation to reduce obsolete workflow queue buildup from granular development.

---

# Automated test inventory

## Domain/core

- `ClockFormatterTests`
- `QuietHoursTests`
- `ChimePolicyTests`
- `AppSettingsTests`
- `DomainPropertyTests`

Coverage includes:

- 12/24-hour formatting;
- seconds;
- date/week/calendar details;
- quiet-hour boundaries and overnight logic;
- chime cadence and duplicate suppression;
- visual-setting bounds;
- null/invalid enum normalization;
- imported text bounds;
- single-world-clock invariant;
- maximum-world-clock invariant;
- duplicate clock IDs;
- duplicate timezone IDs case-insensitively;
- deterministic property-style idempotence;
- deterministic timezone uniqueness property assertions.

## Persistence/import

- `JsonSettingsStoreTests`
- `SettingsImportFuzzTests`

Coverage includes:

- save/load;
- export/import;
- corrupt JSON;
- corrupt backup preservation;
- repeated corrupt recovery creates unique files;
- non-object JSON root rejection;
- missing-schema migration;
- schema-0 migration;
- negative/future schema rejection;
- numeric enum rejection;
- oversized import rejection;
- deterministic malformed binary/JSON fuzz corpus;
- primary settings remain untouched by failed import.

## Timezones

- `SystemTimeZoneCatalogTests`

Coverage includes catalog availability, UTC, invalid fallback, bounded/case-insensitive search.

## Startup/platform policies

- `StartupRegistrationDocumentsTests`
- `MainWindowViewModelTests`
- `StartupRollbackCancellationTests`
- `TrayVisibilityPolicyTests`

Coverage includes:

- platform registration document/command generation;
- escaping/path validation;
- startup rollback after normal persistence failure;
- imported startup isolation;
- explicit startup change application;
- startup rollback even when the failed save cancels its caller token;
- close/background tray safety truth tables.

## App policies/services

- `ExternalUriLauncherTests`
- `AppVersionInfoTests`
- `ThemePaletteSelectorTests`
- `MainWindowStateTests`

Coverage includes:

- safe URI policy;
- approved product destinations;
- display-version prerelease/build-metadata behavior;
- System/Light/Dark/High Contrast palette selection;
- loading -> initialized/ready transition;
- localized singular/plural world-clock count.

## Headless Avalonia UI

- `AvaloniaTestSetup`
- `HeadlessUiSmokeTests`

Coverage includes:

- main-window XAML/resources;
- named clock/search/search-status/undo controls;
- mini dimensions and topmost restoration;
- focus-mode chrome hide/restore;
- primary Settings controls;
- Settings Updates controls/version;
- Settings About version;
- onboarding;
- standalone About.

Headless tests do not prove native tray, startup, sound, browser/mail handlers, file pickers, screen readers, display scaling, or OS-theme visual integration.

---

# Current documentation aligned in this continuation

Updated:

- `README.md`
- `CHANGELOG.md`
- `ROADMAP.md`
- `PRIVACY.md`
- `SECURITY.md`
- `docs/testing.md`
- `docs/accessibility.md`
- `docs/performance.md`
- `what_changed.md`

Documentation now reflects:

- offline-safe Updates section;
- Settings About section;
- informational version display;
- loading/count states;
- runtime System-theme refresh;
- duplicate timezone normalization;
- cancellation-independent startup rollback;
- opened-stream settings size validation;
- collision-resistant corrupt recovery;
- tray safety;
- non-redirected chime helper output;
- current expanded tests;
- exact verification limitations.

---

# Errors/bugs found and fixed in the latest continuation

## App version helper compile/API issue

First implementation used:

`informationalVersion.IndexOf('+', StringComparison.Ordinal)`

That char + `StringComparison` overload is not the correct API form for the target.

Fixed to:

`informationalVersion.IndexOf('+')`

## Startup rollback cancellation consistency

Problem:

If startup integration was changed, then settings persistence cancelled the same caller token and failed, rollback reused that cancelled token and could be prevented from restoring the previous OS startup state.

Fix:

Rollback now calls `SetEnabledAsync(previousValue, CancellationToken.None)` and has dedicated regression coverage.

## Duplicate imported timezone invariant

Problem:

Normal UI prevented adding the same timezone twice, but imported/in-memory normalized settings only deduplicated clock IDs. Two different IDs could reference the same timezone.

Fix:

Normalization now also applies case-insensitive `TimeZoneId` uniqueness. Existing capacity/property tests were updated so they continue exercising real list bounds rather than collapsing all synthetic clocks to `UTC`.

## Settings input size observation

Improvement:

The import/load size limit previously used `FileInfo.Length` before the actual file open. Validation now occurs from `stream.Length` on the opened stream immediately before parsing, keeping the bound tied to the actual read handle.

## Corrupt recovery filename collision

Problem:

Corrupt backups originally used only second-resolution timestamp suffixes. Two failures in the same second could collide and prevent preservation.

Fix:

Recovery name now includes milliseconds plus a GUID. A repeated-corruption regression test verifies two distinct backups.

## Unix chime output redirection

Problem/risk:

Optional system helper processes redirected stdout/stderr but ChronoDesk never read those streams. A sufficiently chatty helper can block on a full pipe while the app awaits process exit.

Fix:

Unused redirects were removed. Fixed executable paths and `ArgumentList` remain.

## System theme custom-palette staleness

Problem:

Requested System theme could let Avalonia's underlying Fluent theme follow the OS while ChronoDesk's custom brushes stayed at the palette computed during the last settings application.

Fix:

Centralized palette selection + `ActualThemeVariantChanged` subscription updates custom brushes when the OS theme changes.

## Settings baseline gaps

The master prompt expects Settings sections for Updates and About. These are now implemented rather than deferred to the standalone About window or documentation.

---

# Verification actually observed

## Local execution environment

Earlier checks in this implementation session established:

```text
dotnet: not installed / not on PATH
pwsh: not installed / not on PATH
git: /usr/bin/git
git version 2.47.3
```

The connected execution environment therefore cannot truthfully claim local:

- `dotnet restore`;
- `dotnet format`;
- `dotnet build`;
- `dotnet test`;
- PowerShell verification scripts;
- Avalonia GUI launch;
- native publish/package launch.

Earlier direct clone/network attempts also hit DNS/network restrictions, so connected GitHub repository operations plus GitHub Actions are the authoritative remote verification route available in this session.

## PR state observed before this handoff commit

PR #15 at source head:

`0a1fc4b3021b9fd9bc8f7ff648960e4518317874`

was:

- open;
- not merged;
- mergeable;
- 101 commits;
- 55 changed files.

## GitHub Actions state observed for that exact head

- CodeQL run `32223266917` — **queued**;
- Dependency Review run `32223266931` — **queued**;
- CI run `32223266921` — **queued**.

These are recorded as queued, **not PASS**.

This handoff update creates a newer documentation-only branch commit, so GitHub may register a newer workflow set. Any future merge/release decision must use checks from the actual latest branch head, not the run IDs above.

---

# Stable-release gates that are still open

These are execution/verification gates, not missing source features:

1. latest exact branch-head CI must finish successfully on Ubuntu, Windows, and macOS;
2. latest exact branch-head CodeQL must finish successfully;
3. latest exact branch-head Dependency Review must finish successfully;
4. any CI/compiler/format/test/security failure must be fixed before merge;
5. PR #15 should remain open until latest-head automated verification is satisfactory;
6. real Windows 11 GUI/tray/startup/chime/file-picker/theme/accessibility validation;
7. real macOS x64/arm64 GUI/tray/LaunchAgent/chime/file-picker/theme/accessibility validation;
8. representative Linux GNOME/KDE GUI/tray/XDG/chime/file-picker/theme/accessibility validation;
9. verify tray-unavailable environments keep the app reachable;
10. verify System theme switches live with the OS;
11. verify browser/mail handler behavior and safe failure state;
12. verify idle Updates UI performs no ChronoDesk background network activity;
13. verify display scaling and large text;
14. capture real screenshots from verified release builds and replace the explicit placeholder;
15. publish only artifacts that pass checksum/manifest inspection;
16. signing/notarization remains optional future infrastructure and requires protected signing credentials not stored in Git;
17. do not tag stable `v1.0.0` before the repository Definition-of-Done/release checklist gates are actually satisfied.

---

# Migration notes for future schema changes

Current schema: 1.

For a future schema 2:

1. increment `AppSettings.CurrentSchemaVersion`;
2. add explicit `1 -> 2` migration;
3. retain `0 -> 1` coverage;
4. add a real schema-1 fixture from an actual released version;
5. test direct `1 -> 2` and multi-step `0 -> 1 -> 2`;
6. verify import still cannot alter startup registration implicitly;
7. verify normalized timezone/capacity/text invariants after migration;
8. update ADR 0007;
9. update CHANGELOG/release notes/this handoff.

---

# Release notes draft — current Phase 7 branch

## Added

- deterministic cross-platform startup registration builders;
- world-clock remove Undo;
- timezone search count/no-results feedback;
- world-clock capacity feedback;
- explicit loading state and localized world-clock count text;
- Settings Updates section;
- Settings About section;
- informational version display helper;
- runtime System-theme palette selection helper;
- stepwise settings migration pipeline;
- release ZIP SHA-256 files and integrity manifest;
- local documentation-link verifier;
- non-echoing high-signal tracked-secret verifier;
- expanded unit/property/fuzz/headless regression suite.

## Changed

- macOS/Linux startup registration uses atomic replacement;
- startup enabled-state check requires exact expected registration;
- settings imports normalize duplicate timezone cards;
- settings file size is checked from the opened stream;
- corrupt recovery filenames are collision-resistant;
- system theme custom brushes refresh when OS theme changes;
- optional Unix chime helpers do not redirect unused output;
- external product/release/support/funding links are centralized and scheme-restricted;
- CodeQL/dependency-review cancel superseded same-ref runs;
- README/security/privacy/testing/accessibility/performance docs match current behavior.

## Fixed

- false success at the 24-world-clock limit;
- stale always-on-top state after mini mode;
- unreachable hidden-process risk when tray restoration is unavailable;
- startup rollback after a cancelled settings save;
- displayed prerelease version losing informational-version suffix;
- potential corrupt recovery filename collision;
- custom System-theme palette remaining stale after OS theme change;
- invalid char/StringComparison API usage found during static review.

## Security/privacy

- bounded object-root settings input;
- explicit schema migration/rejection rules;
- duplicate timezone normalization;
- safe startup document generation and rollback;
- fixed safe external destinations;
- no background update polling;
- high-signal tracked-secret CI gate;
- release artifact integrity metadata.

No stable release tag has been created from this branch.

---

# Next exact continuation

If PR #15 is still open:

1. read this file first;
2. fetch PR #15 and record its exact current head SHA;
3. fetch CI/CodeQL/Dependency Review for **that exact SHA**;
4. if any latest-head job failed, inspect the failed job/log and fix the concrete failure with a focused commit plus regression test where practical;
5. update this file with the real result;
6. merge only after latest-head automated verification is satisfactory;
7. after merge, verify `main` contains the Phase 7 files.

If PR #15 has already been merged in a later continuation:

1. do not reimplement this branch;
2. move to real release-candidate desktop validation;
3. record OS/version/architecture/desktop environment and actual pass/fail evidence;
4. replace screenshot placeholders only with real verified captures;
5. prepare the first preview/release-candidate tag only after the release checklist passes;
6. keep stable `v1.0.0` blocked until all required real-execution gates are complete.

---

# Recent meaningful continuation commits

Latest observed sequence before this handoff includes:

- `0a1fc4b3021b9fd9bc8f7ff648960e4518317874` — `test: cover timezone uniqueness in property-style normalization`
- `eccab09920dc6cd499e3d875b73517d90d2eae46` — `docs: add update and theme performance constraints`
- `d40b9c1ce069e5fb6e2dbbb47731f989f2bbc1a7` — `docs: add update About and live-theme accessibility checks`
- `3586fa32a3790a0f76ec125e2029be7073439734` — `docs: mark update About theme and recovery hardening complete`
- `c9d830edbc91bb6b8bb5c8e56d056348855cc91c` — `docs: align testing guide with final hardening coverage`
- `cdf5e37483b280a3f8318976da0d0cd3e3623fbb` — `docs: align security policy with latest hardening`
- `1a7ed33bc531d2afebf79f9583e4c579b6ab8e29` — `docs: record final settings and reliability hardening`
- `150ddf09f8d610fdf82bcadabb98796d90f87c35` — `docs: document offline update and recovery privacy behavior`
- `ab70197f30016c183b859fa826261666f41625af` — `docs: align README with release hardening behavior`
- `59ff2dbbf3d6bf9e48920b94318a9ffd9dbdff1f` — `test: cover settings root validation and corrupt backup uniqueness`
- `f36c2bf9f32dea2ad9eedf96cb16722bb606bcde` — `fix: avoid unread redirected output in chime helpers`
- `bed4b4cde26a214f97bcb7683e4e2f3c85e97b8c` — `security: validate settings size from opened stream`
- `6d6dfe0f96b572cd2ee1ec2f4c1d58fec2c278d4` — `fix: use valid build metadata separator lookup`
- `58388addf2ddfd3b6ecd2ae2eb21f43af0d46d93` — `fix: apply system palette before first desktop window`
- `d82149cdaa2b910a01967836a57b923be0d257ad` — `test: cover system and explicit theme palette selection`
- `6ec93f61ce472ded5571adb30e440941d5343062` — `fix: refresh custom palette when system theme changes`
- `8257eaecf12316145557e3118736eb7aab6ca23b` — `refactor: centralize theme palette selection`
- `f4002e4c22778259f81d4cbd53d64d3e02531b80` — `test: cover localized loading and world clock count state`
- `d126aaefd7022e3fae90e79c4cb0c2c1df0ab6d4` — `feat: expose localized loading and world clock count state`
- `33648f3be00ce43382f3cf88426364d391efe945` — `feat: expose localized loading and count resources`
- `74c6ddcbdf95660c5966a1448def3e6e1e725565` — `feat: add localized loading and count resources`
- `afbeb600a24f4391b4182dd54e7efd84f96ccf05` — `test: align world clock normalization invariants`
- `a69c16ca5c2772e0fd5dbfa6311c12c420d53973` — `fix: normalize duplicate timezone cards consistently`
- `82a0fda228c6d6770a2ebfa0c4f8566e1bd0cc16` — `test: restore startup state after cancelled settings save`
- `8febb263a47d441f05afe0bf670c19548baf084c` — `fix: make startup rollback independent of cancelled save token`
- `455cbe1be7fc2efeb4c8741dfa218ff5179e178c` — `test: cover update settings controls in headless UI`
- `7df0fe780557d0c37fc93284d9d50c99dce7d4e2` — `test: cover application display version metadata`
- `1e0dc939f9fe982452ad91f4c117a5dfca92d101` — `feat: wire settings release information action`
- `296907f0fc847d0cb1cc5b6ddbd623aa85f245bd` — `feat: add offline-safe updates section to settings`
- `aa9f9809f1f97ae2ac835314328b677009fc9c7d` — `feat: expose localized update settings resources`
- `ac4451ce93843595ad66ed03526dd61399fb061f` — `feat: add localized update settings resources`
- `0fc27c571c55f924256c1a632091eab9796d6464` — `refactor: use centralized version metadata in About`
- `46e4a38f1166fbb4812b220e68fbc6eb6e180ca3` — `feat: centralize application version metadata`

Additional commits between these milestones implement Settings About, generic safe external-handler feedback, headless About coverage, and related review fixes. PR #15's commit list is the authoritative complete sequence because this file itself adds one newer commit.

---

# Handoff rule

Every future continuation must keep this file current with:

- milestone/version;
- exact repository branch/PR/head;
- files/features changed;
- tests added/changed;
- commands/checks actually run and actual result;
- bugs/errors found and fixes;
- known limitations;
- open issues/release gates;
- migration notes;
- release notes draft;
- exact next tasks;
- meaningful commit hashes/messages.

Never convert queued/pending checks into PASS. Never treat headless coverage as proof of native desktop behavior. Never claim a stable ChronoDesk release until the master-prompt Definition-of-Done and repository release checklist are actually satisfied.
