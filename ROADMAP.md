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
- [x] Windows Run-key command generation/detection uses a canonical exact command rather than substring matching.
- [x] Linux XDG `Exec` generation follows freedesktop quoting/field-code rules for executable paths.
- [x] macOS LaunchAgent XML generation isolated from filesystem mutation.
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

Status: **Implemented for domain/persistence/headless UI and startup-artifact formatting; native-desktop validation remains**

- [x] Clock formatting tests.
- [x] Quiet-hour boundary tests.
- [x] Chime cadence tests.
- [x] Settings normalization tests.
- [x] JSON persistence/import/export/corruption tests.
- [x] Transient settings-read failures preserve the original settings file and recover after the lock clears.
- [x] Timezone catalog tests.
- [x] Startup-persistence rollback and unreadable-settings initialization regression tests.
- [x] Windows startup command generation/matching tests.
- [x] Linux XDG autostart `Exec` quoting/escaping tests.
- [x] macOS LaunchAgent plist generation tests.
- [x] Deterministic property-style tests for quiet hours/settings invariants.
- [x] Deterministic malformed-import fuzz coverage and oversized-input rejection.
- [x] Avalonia headless XUnit smoke tests for primary windows and focus/mini transitions.
- [x] Headless regression coverage for restoring the pre-focus window state.
- [x] Headless regression coverage for the complete four-part About version.
- [x] Imported world-clock uniqueness tests.
- [x] Multi-OS CI.
- [x] Four-part version consistency verification in CI.
- [ ] Add isolated Windows registry startup-adapter mutation tests if a platform regression justifies the extra abstraction.
- [ ] Add deeper headless interaction tests for file-picker-independent settings flows after the first full CI pass establishes stable baseline behavior.

## Phase 5 — Release readiness

Status: **Version `2.6.0.2` metadata prepared; native release validation still required**

- [x] Tagged release workflow.
- [x] Self-contained x64/arm64 artifact matrix for Windows, macOS, and Linux.
- [x] Release documentation baseline.
- [x] README screenshot placeholder clearly identified as a placeholder.
- [x] Application, package, assembly, and file metadata set to `2.6.0.2`.
- [x] About UI preserves all four version components.
- [x] Repository-local version verifier added and integrated into CI.
- [x] Release tags are required to match the application version exactly.
- [x] Release workflow expects four-component `v*.*.*.*` tags.
- [x] Release ZIPs include license/readme/changelog/privacy/security/support documents.
- [x] Release workflow generates `SHA256SUMS.txt` for packaged ZIPs.
- [x] Repository-local Markdown link verifier integrated into CI.
- [x] Final-audit verification record added.
- [ ] Replace placeholder with real verified screenshots from release builds.
- [ ] Complete clean-checkout manual verification on Windows, macOS, and Linux.
- [ ] Confirm CI and CodeQL are green for the exact release commit.
- [ ] Confirm repository branch protection uses the actual workflow check names.
- [ ] Tag `v2.6.0.2` only after all release gates pass.

## Phase 6 — Final audit for 2.6.0.2

Status: **Source/repository hardening complete for this pass; native GUI release gates remain**

- [x] Perform source-level final audit for core clock/settings/window/chime failure paths.
- [x] Add regression coverage for defects found during the final source audit.
- [x] Prevent temporary settings read failures from quarantining potentially valid data.
- [x] Add deterministic local-documentation link verification to CI.
- [x] Add deterministic four-part version verification to CI/release workflows.
- [x] Harden release artifacts with bundled policy/support docs and SHA-256 checksums.
- [x] Record automated versus manual release evidence in `docs/final-audit.md`.
- [ ] Run the complete release checklist in `docs/release.md` on real supported desktops.
- [ ] Validate accessibility checklist on each primary platform.
- [ ] Validate settings migration path after a real prior tagged version fixture exists.
- [ ] Audit documentation links against the exact tagged tree.
- [ ] Confirm vulnerability scan has no unresolved moderate-or-higher dependency finding for the release commit.
- [ ] Confirm no real credentials/private data are present in the release tag.
- [ ] Publish/tag `v2.6.0.2` only when the above gates pass.

## Post-2.6.0.2 candidates

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
