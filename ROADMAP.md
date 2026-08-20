# ChronoDesk Roadmap

This roadmap keeps ChronoDesk focused on becoming a dependable, privacy-respecting clock across desktop, mobile/tablet, and browser environments rather than accumulating unrelated features. Items may move when testing, accessibility, platform reliability, or security work reveals a higher-priority need.

## Phase 0 — Repository and architecture baseline

Status: **Implemented; verification continues in CI**

- [x] .NET 10 shared solution and project boundaries.
- [x] Shared Avalonia application library.
- [x] Thin Desktop, Android, iOS/iPadOS, and Browser hosts.
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
- [x] Local settings persistence contract.
- [x] World-clock cards.
- [x] Timezone search.
- [x] Error-safe settings fallback, including unreadable local data.
- [x] Responsive single-view shell for phone, tablet, and browser hosts.

## Phase 2 — Complete core product

Status: **Implemented in source; platform validation pending**

Shared capabilities:

- [x] Main clock and world clocks.
- [x] Themes/high-contrast resources.
- [x] Accessible/touch-friendly shared controls.
- [x] Timezone search/add/remove flows.
- [x] Quiet-hours-aware chime policy.
- [x] About/support/funding experience.
- [x] User-facing English strings externalized for future localization.

Desktop capabilities:

- [x] Focus mode with previous window-state restoration.
- [x] Mini always-on-top mode.
- [x] Configurable normal always-on-top.
- [x] Full Settings/import/export workflow.
- [x] First-run onboarding window.
- [x] User-scoped startup preference.
- [x] Tray actions.
- [x] Best-effort native desktop chime playback.

## Phase 3 — Cross-platform hosts

Status: **Implemented in source; native/emulator/browser validation required**

### Desktop

- [x] Windows x64 host/release target.
- [x] Windows arm64 host/release target.
- [x] macOS x64 host/release target.
- [x] macOS arm64 host/release target.
- [x] Linux x64 host/release target.
- [x] Linux arm64 host/release target.
- [x] User-level Windows startup adapter.
- [x] User-level macOS LaunchAgent adapter.
- [x] User-level Linux XDG autostart adapter.

### Android

- [x] `net10.0-android` host.
- [x] Avalonia Android launcher activity.
- [x] Shared single-view application shell.
- [x] Application/version metadata.
- [ ] Validate on representative emulator and physical device.
- [ ] Validate protected Play-distribution signing workflow before store publication.

### iOS / iPadOS

- [x] `net10.0-ios` host.
- [x] Avalonia app delegate and entry point.
- [x] Shared single-view application shell.
- [x] iPhone/iPad orientation metadata.
- [x] Apple marketing/build version mapping for canonical `2.6.0.2`.
- [ ] Validate on iPhone simulator/device.
- [ ] Validate on iPad simulator/device.
- [ ] Validate protected Apple signing/provisioning workflow before store publication.

### Browser / WebAssembly

- [x] `net10.0-browser` host.
- [x] Avalonia WebAssembly bootstrap.
- [x] Responsive HTML/CSS safe-area shell.
- [x] Static-site publish/release packaging.
- [ ] Validate published site over HTTP(S) in representative browsers.
- [ ] Confirm/document browser storage persistence behavior on the chosen deployment host.

## Phase 4 — Platform hardening

Status: **Source implementation present; manual validation required**

- [x] OS/runtime timezone database strategy.
- [x] Desktop chime helper process execution avoids unconsumed redirected output streams.
- [x] Redacted structured logging.
- [x] Startup integration safely reports unsupported outside desktop runtimes.
- [x] Single-view timer stops when detached from the visual tree.
- [x] Desktop-only capabilities are isolated from mobile/browser lifetimes.
- [ ] Validate tray behavior on Windows 11.
- [ ] Validate tray behavior on current macOS Intel and Apple Silicon systems with a GUI session.
- [ ] Validate tray behavior on representative Linux GNOME/KDE sessions.
- [ ] Validate startup enable/disable manually on each desktop target OS.
- [ ] Validate chime behavior with and without optional Linux sound utilities.
- [ ] Validate Android lifecycle pause/resume/reopen behavior.
- [ ] Validate iOS/iPadOS lifecycle/orientation behavior.
- [ ] Validate WebAssembly reload/storage behavior.

## Phase 5 — Automated quality depth

Status: **Implemented for domain/persistence/headless UI and host compilation; native validation remains**

- [x] Clock formatting tests.
- [x] Quiet-hour boundary tests.
- [x] Chime cadence tests.
- [x] Settings normalization tests.
- [x] JSON persistence/import/export/corruption tests.
- [x] Transient settings-read failures preserve the original settings file and recover after the lock clears.
- [x] Timezone catalog tests.
- [x] Startup-persistence rollback and unreadable-settings initialization regression tests.
- [x] Deterministic property-style tests for quiet hours/settings invariants.
- [x] Deterministic malformed-import fuzz coverage and oversized-input rejection.
- [x] Avalonia headless XUnit smoke tests for desktop windows/focus/mini transitions.
- [x] Headless smoke coverage for the shared single-view shell.
- [x] Headless regression coverage for restoring the pre-focus window state.
- [x] Headless regression coverage for the complete four-part About version.
- [x] Imported world-clock uniqueness tests.
- [x] Desktop CI on Windows, macOS, and Linux.
- [x] Android build job with Android workload.
- [x] iOS/iPadOS simulator build job with iOS workload.
- [x] Browser/WebAssembly build job with `wasm-tools`.
- [x] Cross-platform version consistency verification in CI.
- [x] CodeQL aligned to .NET 10 and explicit shared/desktop build graph.
- [ ] Add startup-adapter tests through isolated fake filesystem/registry abstractions if platform regressions justify the extra abstraction.
- [ ] Add deeper headless interaction tests for file-picker-independent settings flows after the first complete cross-platform CI pass establishes a stable baseline.

## Phase 6 — Release readiness

Status: **Version `2.6.0.2` metadata prepared; native release validation still required**

- [x] Four-component tagged release workflow.
- [x] Six desktop self-contained RID packages.
- [x] Browser/WebAssembly static-site release ZIP.
- [x] Release documentation baseline.
- [x] README screenshot placeholder clearly identified as a placeholder.
- [x] Canonical application/package/assembly/file metadata set to `2.6.0.2`.
- [x] Android display/version-code mapping documented and verified by script.
- [x] Apple marketing/build-number mapping documented and verified by script.
- [x] About UI preserves all four canonical version components.
- [x] Repository-local version verifier integrated into CI/release workflows.
- [x] Release tags must match the canonical application version exactly.
- [x] Desktop release ZIPs include license/readme/changelog/privacy/security/support documents.
- [x] Release workflow generates `SHA256SUMS.txt` for packaged ZIPs.
- [x] Repository-local Markdown link verifier integrated into CI.
- [x] Final-audit verification record exists.
- [ ] Replace placeholder with real verified screenshots from representative release builds.
- [ ] Complete clean-checkout manual verification on Windows, macOS, Linux, Android, iOS/iPadOS, and Browser as applicable.
- [ ] Confirm CI, CodeQL, and Dependency Review are green for the exact release commit.
- [ ] Confirm repository branch protection uses actual workflow check names.
- [ ] Produce signed mobile distribution packages only from protected maintainer credentials.
- [ ] Tag `v2.6.0.2` only after all release gates pass.

## Phase 7 — Final cross-platform audit for 2.6.0.2

Status: **Source/repository migration implemented; workflow and native evidence gates remain**

- [x] Split platform-neutral App from executable hosts.
- [x] Upgrade shared SDK baseline to .NET 10.
- [x] Add Desktop/Android/iOS/Browser projects to the solution.
- [x] Add single-view mobile/browser UI.
- [x] Make startup integration safe on sandboxed/non-desktop runtimes.
- [x] Add host-specific CI workloads.
- [x] Align CodeQL with the new .NET 10 host-scoped build model.
- [x] Expand desktop release matrix to x64/arm64 where supported.
- [x] Add Browser/WebAssembly release packaging.
- [x] Synchronize README/setup/architecture/development/testing/release/contributor/roadmap documentation.
- [ ] Require final green workflows for the exact PR/release head.
- [ ] Perform documented native/emulator/browser validation.
- [ ] Audit documentation links against the exact tagged tree.
- [ ] Confirm vulnerability scan has no unresolved release-blocking dependency finding.
- [ ] Confirm no real credentials/private signing data are present in the release tag.
- [ ] Publish/tag `v2.6.0.2` only when the above gates pass.

## Post-2.6.0.2 candidates

These are candidates, not promises:

- user-editable world-clock card labels after adding a timezone;
- optional native mobile notification/chime integrations with explicit platform capability abstractions;
- stronger browser persistence using a reviewed browser-storage adapter rather than relying only on virtual-filesystem behavior;
- runtime language switching using the established resource catalog and a reliable live-refresh strategy;
- richer calendar detail options that remain offline;
- signed/notarized desktop installers and automated protected mobile store packaging when signing infrastructure is available;
- stronger automated accessibility checks where tooling is reliable.

## Non-goals

ChronoDesk does not currently plan to become:

- an advertising surface;
- a mandatory-account product;
- a social network;
- a cloud calendar service;
- an employee-monitoring/time-tracking product;
- a privileged system daemon;
- a cryptocurrency/mining client.

The main product remains a polished local/offline-first clock and world-clock experience across supported desktop, mobile/tablet, and browser hosts.
