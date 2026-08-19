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
- [x] Error-safe settings fallback, including unreadable local data.

## Phase 2 — Complete core product

Status: **Implemented in source; platform validation pending**

- [x] Focus mode with previous window-state restoration.
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

## Phase 3 — Platform hardening

Status: **Source implementation present; manual validation required**

- [x] User-level Windows startup adapter.
- [x] User-level macOS LaunchAgent adapter.
- [x] User-level Linux XDG autostart adapter.
- [x] OS timezone database strategy.
- [x] Best-effort local system chime adapters.
- [x] Chime helper process execution avoids unconsumed redirected output streams.
- [x] Redacted structured logging.
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
- [x] Startup-persistence rollback and unreadable-settings initialization regression tests.
- [x] Deterministic property-style tests for quiet hours/settings invariants.
- [x] Deterministic malformed-import fuzz coverage and oversized-input rejection.
- [x] Avalonia headless XUnit smoke tests for primary windows and focus/mini transitions.
- [x] Headless regression coverage for restoring the pre-focus window state.
- [x] Imported world-clock uniqueness tests.
- [x] Multi-OS CI.
- [ ] Add startup-adapter tests through isolated fake filesystem/registry abstractions if platform regressions justify the extra abstraction.
- [ ] Add deeper headless interaction tests for file-picker-independent settings flows after the first full CI pass establishes stable baseline behavior.

## Phase 5 — Release readiness

Status: **Infrastructure implemented; release candidate not declared**

- [x] Tagged release workflow.
- [x] Self-contained artifact matrix.
- [x] Release documentation baseline.
- [x] README screenshot placeholder clearly identified as a placeholder.
- [x] Preview assembly/package metadata established (`0.1.0-preview`).
- [x] Repository-local Markdown link verifier integrated into CI.
- [x] Final-audit verification record added.
- [ ] Replace placeholder with real verified screenshots from release builds.
- [ ] Complete clean-checkout manual verification on Windows, macOS, and Linux.
- [ ] Confirm CI and CodeQL are green for the release commit.
- [ ] Confirm repository branch protection uses the actual workflow check names.
- [ ] Tag first release candidate.

## Phase 6 — Final audit and stable release

Status: **Source/repository hardening complete for this pass; native GUI release gates remain**

- [x] Perform source-level final audit for core clock/settings/window/chime failure paths.
- [x] Add regression coverage for defects found during the final source audit.
- [x] Add deterministic local-documentation link verification to CI.
- [x] Record automated versus manual release evidence in `docs/final-audit.md`.
- [ ] Run the complete release checklist in `docs/release.md` on real supported desktops.
- [ ] Validate accessibility checklist on each primary platform.
- [ ] Validate settings migration path after the first tagged preview creates a real prior-version fixture.
- [ ] Audit documentation links against the exact tagged tree.
- [ ] Confirm vulnerability scan has no unresolved moderate-or-higher dependency finding for the release commit.
- [ ] Confirm no real credentials/private data are present in the release tag.
- [ ] Publish stable `v1.0.0` only when the above gates pass.

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
