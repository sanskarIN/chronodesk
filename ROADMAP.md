# ChronoDesk Roadmap

This roadmap keeps ChronoDesk focused on becoming a dependable desktop clock rather than accumulating unrelated features. Items may move when testing, accessibility, platform reliability, or security work reveals a higher-priority need.

## Phase 0 — Repository and architecture baseline

Status: **Implemented; verification continues in CI**

- [x] .NET 9 solution and project boundaries.
- [x] Coding/editor standards.
- [x] MIT license and repository policy files.
- [x] GitHub issue/PR templates.
- [x] CI, CodeQL, dependency review, Dependabot.
- [x] Architecture decision records and handoff file structure.
- [x] English-first `.resx` localization architecture.

## Phase 1 — End-to-end clock MVP

Status: **Implemented; release verification pending**

- [x] Local digital clock.
- [x] 12/24-hour formats.
- [x] Seconds toggle.
- [x] Date and weekday.
- [x] ISO week number/calendar details.
- [x] Local settings persistence.
- [x] World-clock cards.
- [x] Timezone search.
- [x] Error-safe settings fallback.
- [x] Explicit localized loading state.

## Phase 2 — Complete core product

Status: **Implemented in source; platform validation pending**

- [x] Focus mode.
- [x] Mini always-on-top mode.
- [x] Configurable normal always-on-top.
- [x] Themes and layout controls.
- [x] Runtime System-theme light/dark palette refresh.
- [x] Typography and spacing controls.
- [x] First-run onboarding.
- [x] Settings import/export.
- [x] Accessibility preferences.
- [x] Quiet-hours-aware chimes.
- [x] Startup preference.
- [x] Tray actions.
- [x] Standalone About/support/funding experience.
- [x] Settings About section with version/license/support/funding/credit.
- [x] Settings Updates section with local version and user-initiated official Releases link.
- [x] User-facing English strings externalized for future localization.
- [x] Timezone search empty/result-count feedback.
- [x] Localized singular/plural world-clock counts.
- [x] Undo for the most recently removed world clock.
- [x] Explicit world-clock capacity feedback before persistence.

## Phase 3 — Platform hardening

Status: **Source implementation present; manual validation required**

- [x] User-level Windows startup adapter.
- [x] User-level macOS LaunchAgent adapter.
- [x] User-level Linux XDG autostart adapter.
- [x] Deterministic startup-registration builders with path validation.
- [x] Atomic startup-file replacement on macOS/Linux.
- [x] Exact expected-registration checks before reporting startup enabled.
- [x] Startup rollback independent of a cancelled failed-save token.
- [x] OS timezone database strategy.
- [x] Best-effort local system chime adapters without unread redirected output.
- [x] Redacted structured logging.
- [x] Centralized safe external URI launching for fixed product/support/funding/release links.
- [x] Explicit stepwise settings migration pipeline for supported legacy schemas.
- [x] Opened-stream settings size validation and object-root validation.
- [x] Collision-resistant corrupt-settings recovery names.
- [x] Duplicate timezone normalization during import/settings normalization.
- [x] Close/background hiding requires verified tray-menu restoration availability.
- [ ] Validate tray behavior on Windows 11.
- [ ] Validate tray behavior on current macOS Intel and Apple Silicon hardware/runners with a GUI session.
- [ ] Validate tray behavior on representative Linux GNOME/KDE sessions.
- [ ] Validate startup enable/disable manually on each target OS.
- [ ] Validate chime behavior with and without optional Linux sound utilities.
- [ ] Validate live System-theme switching on each primary desktop.
- [ ] Validate browser/mail handlers for Settings/About links.

## Phase 4 — Automated quality depth

Status: **Implemented for domain/persistence/headless UI; native-desktop validation remains**

- [x] Clock formatting tests.
- [x] Quiet-hour boundary tests.
- [x] Chime cadence tests.
- [x] Settings normalization tests.
- [x] Duplicate timezone normalization test.
- [x] JSON persistence/import/export/corruption tests.
- [x] Repeated corrupt-recovery uniqueness test.
- [x] JSON non-object-root rejection test.
- [x] Missing-schema/schema-0 migration tests and invalid/future schema rejection tests.
- [x] Timezone catalog tests.
- [x] Startup registration generation/escaping tests without modifying real startup locations.
- [x] Startup preference rollback/import-consistency tests.
- [x] Startup rollback-after-cancellation test.
- [x] World-clock capacity, undo, and timezone-search feedback tests.
- [x] Localized loading/world-clock count state tests.
- [x] Tray visibility policy tests.
- [x] External URI allow-list policy tests.
- [x] Display-version metadata tests.
- [x] Theme palette selection tests.
- [x] Deterministic property-style tests for quiet hours/settings invariants.
- [x] Deterministic malformed-import fuzz coverage and oversized-input rejection.
- [x] Avalonia headless XUnit smoke tests for primary windows and focus/mini transitions.
- [x] Headless mini-mode topmost restoration regression coverage.
- [x] Headless presence checks for world-clock undo/search-feedback controls.
- [x] Headless Settings Updates and About version coverage.
- [x] Multi-OS CI.
- [ ] Add direct OS registry/session integration tests only if a reliable isolated runner strategy can avoid touching user startup state.
- [ ] Add deeper headless interaction tests for native-file-picker-independent settings flows when they improve confidence beyond view-model coverage.

## Phase 5 — Release readiness

Status: **Infrastructure implemented; release candidate not declared**

- [x] Tagged release workflow.
- [x] Self-contained artifact matrix.
- [x] Release ZIP SHA-256 sidecars.
- [x] Release integrity manifest with archive hashes/sizes/source commit.
- [x] Release workflow checksum verification before publication.
- [x] Release documentation baseline and checksum verification instructions.
- [x] README screenshot placeholder clearly identified as a placeholder.
- [x] Preview assembly/package metadata established (`0.1.0-preview`).
- [x] Display version preserves prerelease metadata.
- [x] Offline-safe Settings Updates section; no background update polling.
- [x] CI-local Markdown target verification.
- [x] CI high-signal tracked-file secret verification.
- [x] CI/CodeQL/dependency-review concurrency cancellation for superseded same-ref runs.
- [ ] Replace placeholder with real verified screenshots from release builds.
- [ ] Complete clean-checkout manual verification on Windows, macOS, and Linux.
- [ ] Confirm CI and CodeQL are green for the release commit.
- [ ] Confirm dependency review is green for the release PR.
- [ ] Confirm repository branch protection uses the actual workflow check names.
- [ ] Tag first release candidate.

## Phase 6 — Final audit and stable release

Status: **Automated audit in progress; native GUI release gates remain**

- [ ] Run the complete release checklist in `docs/release.md` on real supported desktops.
- [ ] Validate accessibility checklist on each primary platform.
- [x] Establish an explicit migration path for pre-versioned/schema-0 settings and regression tests.
- [ ] Validate migration from the first real tagged preview if a future schema increment changes persisted semantics.
- [ ] Confirm the local-link verifier passes against the exact tagged tree.
- [ ] Confirm vulnerability scan has no unresolved moderate-or-higher dependency finding.
- [ ] Confirm tracked-file secret scan and GitHub security review show no real credentials/private data.
- [ ] Publish stable `v1.0.0` only when the above gates pass.

## Phase 7 — Release hardening continuation

Status: **Source implementation complete on the continuation branch; latest-head automated verification pending**

- [x] Refactor startup registration strings/documents into deterministic pure builders.
- [x] Add startup path validation and escaping tests.
- [x] Add atomic startup registration file replacement.
- [x] Make startup rollback resilient to a cancelled save token.
- [x] Add world-clock removal undo.
- [x] Add visible timezone-search empty/count feedback.
- [x] Enforce world-clock capacity before save with localized feedback.
- [x] Normalize duplicate imported timezone cards.
- [x] Add explicit localized loading and singular/plural count states.
- [x] Restore the current always-on-top preference after mini mode.
- [x] Prevent close/background hiding when reliable tray restoration is unavailable.
- [x] Refresh custom palette when System theme changes at runtime.
- [x] Centralize external support/repository/funding/release links and URI policy.
- [x] Add offline-safe Settings Updates section.
- [x] Add Settings About section.
- [x] Preserve prerelease labels in displayed version metadata.
- [x] Validate settings input size from the opened stream.
- [x] Make corrupt-settings recovery filenames collision-resistant.
- [x] Avoid unused redirected output in optional Unix chime helpers.
- [x] Add deterministic documentation local-link CI gate.
- [x] Add high-signal tracked-file secret CI gate without printing matched values.
- [x] Add release archive checksums and integrity manifest.
- [x] Document release integrity verification.
- [x] Add versioned settings migration pipeline and legacy schema tests.
- [x] Record the migration design in ADR 0007.
- [x] Add same-ref cancellation to CodeQL and dependency-review workflows to limit superseded run buildup.
- [ ] Obtain green CI, CodeQL, and dependency-review results for the final continuation commit.
- [ ] Merge the continuation pull request after automated verification.

## Post-1.0 candidates

These are candidates, not promises:

- user-editable world-clock card labels after adding a timezone;
- optional additional bundled chime tones that can be licensed and played reliably cross-platform;
- runtime language switching using the established resource catalog and a reliable live-refresh strategy;
- richer calendar detail options that remain offline;
- signed/notarized installers when signing infrastructure is available;
- stronger automated desktop accessibility checks where tooling is reliable.

## Non-goals

ChronoDesk does not currently plan to become:

- an advertising surface;
- a mandatory-account product;
- a social network;
- a cloud calendar service;
- an employee-monitoring/time-tracking product;
- a privileged system daemon;
- a cryptocurrency/mining client;
- a background update-tracking service.

The main product remains a polished local desktop clock.
