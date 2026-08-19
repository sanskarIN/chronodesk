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
- [x] Settings Updates & About section with current version and explicit GitHub Releases access.
- [x] User-initiated external navigation restricted to HTTPS/mailto.
- [x] User-facing English strings externalized for future localization.

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
- [ ] Validate default browser/mail-handler launch behavior from Settings/About on each target OS.

## Phase 4 — Automated quality depth

Status: **Implemented for domain/persistence/startup/headless UI; native-desktop validation remains**

- [x] Clock formatting tests.
- [x] Quiet-hour boundary tests.
- [x] Chime cadence tests.
- [x] Settings normalization tests.
- [x] JSON persistence/import/export/corruption tests.
- [x] Timezone catalog tests.
- [x] Deterministic property-style tests for quiet hours/settings invariants.
- [x] Deterministic malformed-import fuzz coverage and oversized-input rejection.
- [x] Avalonia headless XUnit smoke tests for primary windows and focus/mini transitions.
- [x] Startup-adapter tests through isolated fake filesystem/registry abstractions.
- [x] Deeper headless interaction tests for file-picker-independent settings save/validation/reset flows.
- [x] Headless coverage for Settings Updates/About controls and version display.
- [x] External-link allowlist regression tests.
- [x] Semantic version display regression tests.
- [x] Repository validation-script unit tests.
- [x] Multi-OS CI.

## Phase 5 — Release readiness

Status: **Infrastructure implemented; release candidate not declared**

- [x] Tagged release workflow.
- [x] Self-contained artifact matrix.
- [x] Windows ZIP and Unix `tar.gz` packaging appropriate to platform permission semantics.
- [x] Tag-derived package/assembly/file/informational version stamping.
- [x] Release preflight for repository integrity, formatting, Release build/tests, and NuGet vulnerability inspection.
- [x] SHA-256 sidecars plus pre-publication checksum verification for all release archives.
- [x] Prerelease tag recognition/publication.
- [x] Release workflow write permission scoped to the publication job.
- [x] Release documentation baseline.
- [x] README screenshot placeholder clearly identified as a placeholder.
- [x] Preview assembly/package metadata established (`0.1.0-preview`).
- [x] Repository-local Markdown link validation in CI.
- [x] High-confidence committed-credential scan in CI/release preflight.
- [x] Tag-time release metadata validator requires a matching changelog release heading and removal of the explicit screenshot placeholder.
- [ ] Replace placeholder with real verified screenshots from release builds.
- [ ] Move the intended release changes from `[Unreleased]` into the exact release-version changelog heading.
- [ ] Complete clean-checkout manual verification on Windows, macOS, and Linux.
- [ ] Confirm CI and CodeQL are green for the release commit.
- [ ] Confirm repository branch protection uses the actual workflow check names.
- [ ] Tag first release candidate.

## Phase 6 — Final audit and stable release

Status: **Automated audit substantially implemented; native GUI release gates remain**

- [ ] Run the complete release checklist in `docs/release.md` on real supported desktops.
- [ ] Validate accessibility checklist on each primary platform.
- [ ] Validate settings migration path after the first tagged preview creates a real prior-version fixture.
- [ ] Audit documentation links against the tagged tree (local link validation is automated; tagged/external review remains).
- [ ] Confirm vulnerability scan has no unresolved release-blocking dependency finding.
- [ ] Confirm no real credentials/private data are present after both automated scanning and human artifact review.
- [ ] Verify each published checksum from a separately downloaded release archive.
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
- a background auto-update/downloader service;
- a privileged system daemon;
- a cryptocurrency/mining client.

The main product remains a polished local desktop clock.
