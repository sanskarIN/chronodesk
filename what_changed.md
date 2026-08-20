# ChronoDesk — Work Handoff

## Current milestone

**Phase 8 — full cross-platform migration and final repository audit for version `2.6.0.2`, in pull request #19.**

ChronoDesk has been restructured from a desktop-only Avalonia executable into a shared cross-platform Avalonia application with dedicated Desktop, Android, iOS/iPadOS, and Browser/WebAssembly hosts.

The canonical product version remains **`2.6.0.2`**.

## Source of truth

- Repository: `https://github.com/sanskarIN/chronodesk`
- Default branch: `main`
- Migration branch: `feature/full-cross-platform-2.6.0.2`
- Pull request: `#19` — `feat: make ChronoDesk fully cross-platform`
- Canonical product version source: `src/ChronoDesk.App/ChronoDesk.App.csproj`
- Required canonical version: `2.6.0.2`
- SDK family: .NET 10
- Avalonia baseline: 11.3.18

## Platform matrix implemented

| Platform | Host project | Target / release coverage |
|---|---|---|
| Windows | `src/ChronoDesk.Desktop` | `win-x64`, `win-arm64` |
| macOS | `src/ChronoDesk.Desktop` | `osx-x64`, `osx-arm64` |
| Linux | `src/ChronoDesk.Desktop` | `linux-x64`, `linux-arm64` |
| Android | `src/ChronoDesk.Android` | `net10.0-android` |
| iOS | `src/ChronoDesk.iOS` | `net10.0-ios` |
| iPadOS | `src/ChronoDesk.iOS` | `net10.0-ios` |
| Browser | `src/ChronoDesk.Browser` | `net10.0-browser` / WebAssembly |

## Architecture changes

### Shared app

`src/ChronoDesk.App` is now a reusable Avalonia library rather than the desktop executable.

It owns:

- shared `AppServices` composition;
- `MainWindowViewModel` and domain-facing UI state;
- localization/resources/themes/assets;
- desktop `MainWindow`, Settings, onboarding, and About views;
- responsive `MainView` used by Android, iOS/iPadOS, and Browser.

`App.OnFrameworkInitializationCompleted` now supports:

- `IClassicDesktopStyleApplicationLifetime` → desktop `MainWindow`;
- `ISingleViewApplicationLifetime` → responsive `MainView`.

### Desktop host

`src/ChronoDesk.Desktop`:

- owns `Program.Main`;
- owns the desktop application manifest;
- references `Avalonia.Desktop` and shared App;
- keeps release executable assembly name `ChronoDesk`.

The old desktop `Program.cs` and `app.manifest` were moved out of the shared app project.

### Android host

`src/ChronoDesk.Android`:

- targets `net10.0-android`;
- uses `Avalonia.Android`;
- owns the launcher `MainActivity`;
- application ID `com.sanskar.chronodesk`;
- display version `2.6.0.2`;
- numeric version code `2602`.

### iOS / iPadOS host

`src/ChronoDesk.iOS`:

- targets `net10.0-ios`;
- uses `Avalonia.iOS`;
- owns `AppDelegate`, native entry point, and `Info.plist`;
- supports iPhone/iPad orientation metadata;
- application ID `com.sanskar.chronodesk`;
- Apple marketing version `2.6.0`;
- Apple build number `2602`;
- shared/in-app canonical version remains `2.6.0.2`.

### Browser host

`src/ChronoDesk.Browser`:

- targets `net10.0-browser`;
- uses `Microsoft.NET.Sdk.WebAssembly` and `Avalonia.Browser`;
- starts the shared App inside HTML element `#out`;
- includes browser bootstrap JavaScript;
- includes responsive CSS/safe-area layout;
- includes runtime globalization configuration;
- publishes a static `wwwroot` site.

## Shared single-view UI

Added:

- `src/ChronoDesk.App/Views/MainView.axaml`
- `src/ChronoDesk.App/Views/MainView.axaml.cs`

The single-view shell includes:

- current clock/date/week/timezone display;
- format toggle;
- seconds toggle;
- world-clock cards;
- timezone search;
- add/remove world clocks;
- responsive scroll layout;
- touch-friendly controls;
- shared status/credit display.

Lifecycle/reliability behavior:

- shared view model initializes on visual-tree attachment;
- first-run state completes without a desktop modal onboarding window;
- 250 ms clock timer starts when attached;
- timer stops when detached;
- overlapping tick execution is suppressed;
- if first-run persistence fails after initialization, the failure is logged but the clock timer still starts;
- format/seconds/world-clock async button actions catch/log persistence failures instead of leaking unhandled event-handler exceptions.

## Platform-safety fixes

### Startup integration

`PlatformStartupManager` previously required `Environment.ProcessPath` during construction. Sandboxed/mobile runtimes may not provide that value.

It now:

- tolerates a missing process path;
- reports startup integration supported only when a desktop OS and usable executable path are present;
- safely returns disabled on unsupported platforms;
- throws `PlatformNotSupportedException` only if a caller explicitly attempts to mutate unsupported startup integration.

This prevents Browser/mobile service construction from failing simply because desktop startup registration is unavailable.

### Single-view persistence failure handling

`MainView` initialization now separates shared model initialization from first-run persistence. A failed onboarding/settings write can no longer suppress the active clock after initialization succeeded. User-triggered single-view persistence actions are also contained/logged rather than escaping from `async void` event handlers.

## SDK/package changes

- `Directory.Build.props` default target moved from `net9.0` to `net10.0`.
- `global.json` moved from SDK `9.0.100` to `10.0.100` with `latestFeature` roll-forward.
- Added centrally managed packages:
  - `Avalonia.Android` 11.3.18
  - `Avalonia.iOS` 11.3.18
  - `Avalonia.Browser` 11.3.18
- Existing Avalonia packages remain aligned on 11.3.18.

## Solution structure

`ChronoDesk.sln` now registers:

- `ChronoDesk.Core`
- `ChronoDesk.Infrastructure`
- `ChronoDesk.App`
- `ChronoDesk.Desktop`
- `ChronoDesk.Android`
- `ChronoDesk.iOS`
- `ChronoDesk.Browser`
- `ChronoDesk.Tests`

A full-solution restore requires workload-specific SDK packs. Normal development is documented as host-scoped restore/build instead.

## CI changes

`.github/workflows/ci.yml` is platform-aware.

### Desktop matrix

Runs on Ubuntu, Windows, and macOS and performs:

- cross-platform version verification;
- Desktop/Test restore;
- formatting checks;
- local Markdown-link verification;
- Desktop Release build;
- shared/headless tests with coverage;
- NuGet vulnerability inspection.

### Browser job

- installs `wasm-tools`;
- restores `ChronoDesk.Browser`;
- builds WebAssembly Release configuration.

### Android job

- configures JDK 17;
- installs .NET Android workload;
- restores/builds Android Release configuration.

### iOS/iPadOS job

- runs on macOS;
- installs .NET iOS workload;
- restores Apple host;
- selects simulator RID based on runner architecture;
- builds Release configuration.

### CodeQL correction found during final audit

The separate CodeQL workflow was still pinned to .NET 9 and `autobuild` after the initial platform migration. That stale configuration was fixed.

CodeQL now:

- installs .NET 10;
- initializes C# CodeQL with `build-mode: manual`;
- restores `ChronoDesk.Desktop`;
- explicitly builds the shared/Desktop graph in Release mode;
- avoids trying to autobuild the full workload-specific solution on one runner.

Dependency Review remains independent on pull requests.

## Release automation changes

`.github/workflows/release.yml` targets .NET 10 and packages:

- `win-x64`
- `win-arm64`
- `linux-x64`
- `linux-arm64`
- `osx-x64`
- `osx-arm64`
- Browser/WebAssembly static-site ZIP

Desktop ZIPs include repository release documents. Browser ZIP includes the static site plus license/privacy documents. All ZIPs are included in generated `SHA256SUMS.txt`.

Android/iOS production signing is intentionally **not** performed with committed credentials. CI validates buildability; protected maintainer signing/provisioning remains a release step.

## Version verification changes

`scripts/check-version.ps1` now verifies:

- canonical shared `Version`, `PackageVersion`, `AssemblyVersion`, `FileVersion`;
- exact Desktop version match;
- Android display version equals canonical `2.6.0.2`;
- positive Android version code;
- Apple marketing version equals canonical first three components (`2.6.0`);
- positive Apple build number;
- exact `v2.6.0.2` tag match when requested.

## Tests added/updated

`HeadlessUiSmokeTests` now also constructs `MainView` and verifies:

- supplied shared view model remains the DataContext;
- timezone search control loads;
- timezone results control loads.

Existing desktop headless focus/mini/Settings/About coverage remains.

## Documentation and repository UX updated

The final migration/audit updated:

- `README.md`
- `CHANGELOG.md`
- `CONTRIBUTING.md`
- `PRIVACY.md`
- `SECURITY.md`
- `SUPPORT.md`
- `ROADMAP.md`
- `docs/setup.md`
- `docs/architecture.md`
- `docs/development.md`
- `docs/testing.md`
- `docs/release.md`
- `docs/troubleshooting.md`
- `docs/accessibility.md`
- `docs/performance.md`
- `docs/github-maintenance.md`
- `docs/release-notes-template.md`
- `docs/final-audit.md`
- `.github/pull_request_template.md`
- `.github/ISSUE_TEMPLATE/bug_report.yml`
- `.github/ISSUE_TEMPLATE/feature_request.yml`
- `what_changed.md`

These now consistently describe:

- .NET 10;
- shared App + thin host architecture;
- host-scoped workload installation/build commands;
- Windows/macOS/Linux x64 + arm64 release targets;
- Android/iOS/iPadOS host requirements;
- Browser/WebAssembly deployment and sandbox behavior;
- desktop-only versus single-view capabilities;
- Apple/Android version mapping;
- protected mobile signing;
- mobile lifecycle/orientation/accessibility requirements;
- browser zoom/storage/main-thread/performance considerations;
- cross-platform issue reporting and PR verification.

## Pull request state

PR #19 is open from `feature/full-cross-platform-2.6.0.2` to `main`.

GitHub previously reported it mergeable after branch synchronization with `main`. Every new branch commit correctly starts/restarts the PR workflow set.

Workflow families to require on the **exact final stable head**:

- CI;
- CodeQL;
- Dependency Review.

Do not record a queued/in-progress run as passing. Any actual compiler/test/security failure must be fixed rather than weakening the check.

## Current verification boundary

Source/repository work completed by inspection includes:

- platform host structure;
- solution registration;
- shared/single-view lifetime split;
- single-view persistence-failure containment;
- startup-manager sandbox safety;
- .NET 10 SDK/package alignment;
- platform-aware CI configuration;
- .NET 10 host-scoped CodeQL configuration;
- expanded desktop/browser release packaging;
- version mapping/verification;
- comprehensive cross-platform documentation/repository templates;
- single-view headless smoke test;
- PR creation/mergeability review.

Still evidence-gated until GitHub/native environments confirm it:

- final green CI for Desktop, Android, iOS/iPadOS, Browser;
- final green CodeQL;
- final green Dependency Review;
- real Android emulator/device launch;
- real iPhone/iPad simulator/device launch;
- real Browser published-site launch;
- native Windows/macOS/Linux x64/arm64 package launch coverage;
- mobile production signing/store submission;
- real platform screenshots;
- tagged release artifact/checksum verification.

These must not be fabricated from source inspection.

## Important capability distinction

ChronoDesk is now **platform-supportable** across desktop, mobile/tablet, and browser, but not every desktop feature has a meaningful equivalent everywhere.

Desktop-only capabilities remain desktop-only by design:

- tray icon/menu;
- always-on-top/mini mode;
- classic desktop full-screen focus window mode;
- current-user desktop startup registration;
- process-based native desktop chime helper.

Android/iOS/iPadOS/Browser use the shared clock/world-clock single-view shell and degrade safely when a desktop-only capability is unavailable.

## Next exact tasks

1. Freeze the final branch head and allow the exact PR #19 workflow set to run.
2. Inspect CI/CodeQL/Dependency Review results for that exact head.
3. If a platform host fails, inspect the failing job steps/logs and fix the real compiler/workload issue.
4. Repeat until the final stable head is green or record an external runner/blocking condition accurately.
5. Merge PR #19 only after repository verification is satisfactory.
6. Perform the manual native/emulator/browser release evidence in `docs/testing.md` / `docs/release.md` before tagging `v2.6.0.2`.
7. Do not tag/publish `v2.6.0.2` until the evidence-gated release checklist is complete.
