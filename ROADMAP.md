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

## Phase 3 — Platform hardening

Status: **Source implementation present; manual validation required**

- [x] User-level Windows startup adapter.
- [x] User-level macOS LaunchAgent adapter.
- [x] User-level Linux XDG autostart adapter.
- [x] OS timezone database strategy.
- [x] Best-effort local system chime adapters.
- [x] Redacted structured logging.
- [ ] Validate tray behavior on Windows 11.
- [ ] Validate tray behavior on current macOS Intel and Apple Silicon hardware/runners with a GUI session.
- [ ] Validate tray behavior on representative Linux GNOME/KDE sessions.
- [ ] Validate startup enable/disable manually on each target OS.
- [ ] Validate chime behavior with and without optional Linux sound utilities.

## Phase 4 — Automated quality depth

Status: **Core tests implemented; UI depth remains**

- [x] Clock formatting tests.
- [x] Quiet-hour boundary tests.
- [x] Chime cadence tests.
- [x] Settings normalization tests.
- [x] JSON persistence/import/export/corruption tests.
- [x] Timezone catalog tests.
- [x] Multi-OS CI.
- [ ] Add deterministic headless Avalonia smoke tests after the first clean CI compile establishes the UI-test package baseline.
- [ ] Add property-based testing for broader quiet-hour/timezone boundary generation if it provides value beyond table-driven coverage.
- [ ] Add startup-adapter tests through isolated fake filesystem/registry abstractions if platform regressions justify the extra abstraction.

## Phase 5 — Release readiness

Status: **Infrastructure implemented; release candidate not declared**

- [x] Tagged release workflow.
- [x] Self-contained artifact matrix.
- [x] Release documentation baseline.
- [x] README screenshot placeholder clearly identified as a placeholder.
- [ ] Replace placeholder with real verified screenshots from release builds.
- [ ] Complete clean-checkout manual verification on Windows, macOS, and Linux.
- [ ] Confirm CI and CodeQL are green for the release commit.
- [ ] Confirm repository branch protection uses the actual workflow check names.
- [ ] Tag first release candidate.

## Phase 6 — Final audit and stable release

Status: **Pending release candidate**

- [ ] Run the complete release checklist in `docs/release.md`.
- [ ] Validate accessibility checklist on each primary platform.
- [ ] Validate settings migration path from the first tagged preview.
- [ ] Audit documentation links against the tagged tree.
- [ ] Confirm vulnerability scan has no unresolved moderate-or-higher dependency finding.
- [ ] Confirm no real credentials/private data are present.
- [ ] Publish stable `v1.0.0` only when the above gates pass.

## Post-1.0 candidates

These are candidates, not promises:

- user-editable world-clock card labels after adding a timezone;
- optional additional bundled chime tones that can be licensed and played reliably cross-platform;
- runtime language switching after all user-facing strings are migrated to a finalized localization catalog;
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
