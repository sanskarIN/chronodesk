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

## Phase 2 — Complete core product

Status: **Implemented in source; platform validation pending**

- [x] Focus mode.
- [x] Mini always-on-top mode.
- [x] Configurable normal always-on-top.
- [x] Themes and layout controls.
- [x] Typography and spacing controls.
- [x] First-run onboarding.
- [x] Settings import/export.
- [x] Accessibility preferences.
- [x] Quiet-hours-aware chimes.
- [x] Startup preference.
- [x] Tray actions.
- [x] About/support/funding experience.
- [x] User-facing English strings externalized for future localization.
- [x] Timezone search empty/result-count feedback.
- [x] Undo for the most recently removed world clock.

## Phase 3 — Platform hardening

Status: **Source implementation present; manual validation required**

- [x] User-level Windows startup adapter.
- [x] User-level macOS LaunchAgent adapter.
- [x] User-level Linux XDG autostart adapter.
- [x] Deterministic startup-registration builders with path validation.
- [x] Atomic startup-file replacement on macOS/Linux.
- [x] Exact expected-registration checks before reporting startup enabled.
- [x] OS timezone database strategy.
- [x] Best-effort local system chime adapters.
- [x] Redacted structured logging.
- [x] Centralized safe external URI launching for fixed product/support links.
- [ ] Validate tray behavior on Windows 11.
- [ ] Validate tray behavior on current macOS Intel and Apple Silicon hardware/runners with a GUI session.
- [ ] Validate tray behavior on representative Linux GNOME/KDE sessions.
- [ ] Validate startup enable/disable manually on each target OS.
- [ ] Validate chime behavior with and without optional Linux sound utilities.

## Phase 4 — Automated quality depth

Status: **Implemented for domain/persistence/headless UI; native-desktop validation remains**

- [x] Clock formatting tests.
- [x] Quiet-hour boundary tests.
- [x] Chime cadence tests.
- [x] Settings normalization tests.
- [x] JSON persistence/import/export/corruption tests.
- [x] Timezone catalog tests.
- [x] Startup registration generation/escaping tests without modifying real startup locations.
- [x] Startup preference rollback/import-consistency tests.
- [x] World-clock undo and timezone-search feedback tests.
- [x] External URI allow-list policy tests.
- [x] Deterministic property-style tests for quiet hours/settings invariants.
- [x] Deterministic malformed-import fuzz coverage and oversized-input rejection.
- [x] Avalonia headless XUnit smoke tests for primary windows and focus/mini transitions.
- [x] Headless presence checks for world-clock undo/search-feedback controls.
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
- [x] CI-local Markdown target verification.
- [x] CI high-signal tracked-file secret verification.
- [ ] Replace placeholder with real verified screenshots from release builds.
- [ ] Complete clean-checkout manual verification on Windows, macOS, and Linux.
- [ ] Confirm CI and CodeQL are green for the release commit.
- [ ] Confirm repository branch protection uses the actual workflow check names.
- [ ] Tag first release candidate.

## Phase 6 — Final audit and stable release

Status: **Automated audit in progress; native GUI release gates remain**

- [ ] Run the complete release checklist in `docs/release.md` on real supported desktops.
- [ ] Validate accessibility checklist on each primary platform.
- [ ] Validate settings migration path after the first tagged preview creates a real prior-version fixture.
- [ ] Confirm the local-link verifier passes against the exact tagged tree.
- [ ] Confirm vulnerability scan has no unresolved moderate-or-higher dependency finding.
- [ ] Confirm tracked-file secret scan and GitHub security review show no real credentials/private data.
- [ ] Publish stable `v1.0.0` only when the above gates pass.

## Phase 7 — Release hardening continuation

Status: **Implementation complete on the continuation branch; automated verification pending**

- [x] Refactor startup registration strings/documents into deterministic pure builders.
- [x] Add startup path validation and escaping tests.
- [x] Add atomic startup registration file replacement.
- [x] Add world-clock removal undo.
- [x] Add visible timezone-search empty/count feedback.
- [x] Centralize external support/repository/funding links and URI policy.
- [x] Add deterministic documentation local-link CI gate.
- [x] Add high-signal tracked-file secret CI gate without printing matched values.
- [x] Add release archive checksums and integrity manifest.
- [x] Document release integrity verification.
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
- a cryptocurrency/mining client.

The main product remains a polished local desktop clock.
