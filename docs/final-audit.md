# ChronoDesk Final Audit

This document records the release-oriented audit criteria for ChronoDesk version `2.6.0.2`. It separates source/repository checks enforced by automation from native/emulator/browser validation that must be performed on real target environments so the project does not claim evidence it does not have.

## Scope

The final audit covers:

- shared source structure and dependency boundaries;
- Desktop, Android, iOS/iPadOS, and Browser host configuration;
- exact canonical/platform version metadata;
- settings persistence/import safety and browser sandbox limits;
- clock/world-clock behavior;
- desktop chime/focus/mini/startup/tray capabilities;
- single-view mobile/tablet/browser lifecycle and responsiveness;
- automated tests and platform-aware CI gates;
- local documentation integrity;
- dependency/static-security checks;
- desktop/browser release packaging and artifact integrity;
- protected mobile signing expectations;
- remaining native/emulator/browser validation.

## Current source version

Canonical shared version:

```text
2.6.0.2
```

`src/ChronoDesk.App/ChronoDesk.App.csproj` declares:

- `Version` = `2.6.0.2`;
- `PackageVersion` = `2.6.0.2`;
- `AssemblyVersion` = `2.6.0.2`;
- `FileVersion` = `2.6.0.2`.

Platform mappings:

- Desktop package/assembly/file version: `2.6.0.2`.
- Android display version: `2.6.0.2`; numeric version code: `2602`.
- iOS/iPadOS marketing version: `2.6.0`; build number: `2602`.
- In-app About version: `2.6.0.2`.

`scripts/check-version.ps1` enforces canonical equality, assembly component bounds, desktop equality, Android metadata rules, Apple mapping, and exact release-tag matching when `-Tag` is supplied.

## Cross-platform architecture audit

The solution now contains:

- `ChronoDesk.Core` — framework-independent product models/rules/interfaces.
- `ChronoDesk.Infrastructure` — guarded environment adapters.
- `ChronoDesk.App` — platform-neutral Avalonia application/presentation library.
- `ChronoDesk.Desktop` — Windows/macOS/Linux executable host.
- `ChronoDesk.Android` — Android host.
- `ChronoDesk.iOS` — iPhone/iPad host.
- `ChronoDesk.Browser` — WebAssembly host.
- `ChronoDesk.Tests` — domain/persistence/view-model/headless UI coverage.

`ChronoDesk.App` supports both Avalonia lifetime families used by the hosts:

- classic desktop lifetime → `MainWindow`;
- single-view lifetime → `MainView`.

Desktop-only capabilities are not pretended to exist on mobile/browser. Unsupported startup/tray/window/process functionality must degrade safely instead of crashing application initialization.

## Repository-level automated gates

### Desktop/shared matrix

The `CI` workflow runs shared/desktop validation on:

- Ubuntu;
- Windows;
- macOS.

Each desktop job performs:

1. cross-platform version metadata verification;
2. .NET 10 setup;
3. host-scoped restore for Desktop and Tests;
4. formatting verification;
5. repository-local Markdown link verification;
6. Desktop Release build;
7. xUnit/Avalonia headless test suite with coverage collection;
8. NuGet vulnerability inspection;
9. test-result artifact upload.

### Android gate

Ubuntu CI:

1. configures JDK 17;
2. installs .NET 10;
3. installs the Android workload;
4. restores `ChronoDesk.Android`;
5. builds Android Release configuration.

### iOS / iPadOS gate

macOS CI:

1. installs .NET 10;
2. installs the iOS workload;
3. restores `ChronoDesk.iOS`;
4. builds an iOS simulator target appropriate to runner architecture.

### Browser gate

Ubuntu CI:

1. installs .NET 10;
2. installs `wasm-tools`;
3. restores `ChronoDesk.Browser`;
4. builds Browser/WebAssembly Release configuration.

### Security workflows

- CodeQL installs .NET 10 and explicitly builds the shared/Desktop graph under manual C# build mode.
- Dependency Review runs on pull requests.
- Dependabot monitors NuGet and GitHub Actions dependencies.

A configured workflow is not the same as a passing run. Before a release tag, inspect the **exact release commit** and require every applicable check to be green.

## Source audit findings addressed

The current source/repository audits added or strengthened:

- unreadable local settings fallback while still populating clock/world-clock/timezone UI;
- non-destructive handling of transient settings I/O failures;
- focus-mode restoration of the previous desktop window state;
- case-insensitive imported world-clock/timezone deduplication;
- non-blocking chime helper process handling;
- complete four-component About version rendering;
- repository-local Markdown link verification;
- canonical/platform version verification;
- .NET 10 SDK migration for current mobile host support;
- thin Desktop/Android/iOS/Browser host separation;
- responsive single-view `MainView` for Android/iOS/iPadOS/Browser;
- single-view timer start/stop tied to visual-tree lifecycle;
- desktop startup adapter tolerance of missing `Environment.ProcessPath` on sandboxed/non-desktop runtimes;
- host-specific CI workloads instead of unsafe full-solution autobuild assumptions;
- CodeQL migration from stale .NET 9 autobuild to .NET 10 explicit shared/Desktop build;
- ARM64 desktop release targets for Windows/Linux plus existing macOS ARM64;
- Browser/WebAssembly static release packaging;
- documentation of protected mobile signing rather than committing signing material.

Regression/headless tests accompany relevant settings/window/version/single-view behavior.

## Release artifact hardening

Tagged release automation:

- accepts four-component tags (`v*.*.*.*`);
- rejects a tag that does not exactly equal `v` + canonical version;
- produces self-contained Desktop ZIPs for:
  - `win-x64`
  - `win-arm64`
  - `linux-x64`
  - `linux-arm64`
  - `osx-x64`
  - `osx-arm64`
- produces a Browser/WebAssembly static-site ZIP;
- bundles release/policy documentation into desktop packages;
- includes license/privacy documentation with the Browser site package;
- generates `SHA256SUMS.txt` for release ZIPs;
- publishes checksums with the GitHub Release.

Checksum evidence is not a substitute for publisher identity through platform code signing/notarization.

Android/iOS/iPadOS are build-validated in CI but production store packages require private maintainer signing/provisioning credentials and are intentionally not signed from committed/public secrets.

## Security and privacy review

Relevant controls include:

- no required ChronoDesk network account/backend for clock operation;
- user-scoped desktop startup integration;
- unsupported desktop startup integration returns safely on mobile/browser;
- bounded/schema-validated settings imports;
- string-only enum deserialization;
- bounded/single-line imported text;
- atomic native settings writes where supported;
- invalid-settings preservation where supported;
- non-destructive transient-I/O fallback;
- startup-preference protection during portable import;
- redacted structured logging;
- external-link scheme restrictions;
- browser sandbox boundaries documented/enforced by avoiding desktop-only calls;
- no committed Android/Apple production signing secrets;
- canonical/platform release version validation;
- SHA-256 release package checksums;
- CodeQL, Dependency Review, Dependabot, and vulnerability checks.

Before every public release, inspect the tagged tree and workflow configuration for accidental credentials, private data, generated local settings, signing material, provisioning data, copied logs, or private screenshots.

## Documentation audit

The repository documentation includes project, contribution, support, security, privacy, release, architecture, setup, development, testing, accessibility, performance, troubleshooting, roadmap, ADR, GitHub-maintenance, release-note-template, final-audit, and handoff documents.

Cross-platform documentation must consistently describe:

- .NET 10;
- shared App + thin hosts;
- host-scoped workload installation/build commands;
- Windows/macOS/Linux x64/arm64 package matrix;
- Android and iOS/iPadOS build requirements;
- Browser/WebAssembly deployment model;
- desktop-only versus single-view capabilities;
- Apple/Android version mapping;
- mobile signing secrecy;
- browser storage/security limitations.

The local link verifier checks repository-relative Markdown destinations. It intentionally does not require external websites to be reachable because external availability is nondeterministic and should not make an otherwise reproducible offline check fail.

## Native/emulator/browser gates still requiring evidence

These checks require real target environments and must remain release gates rather than being marked complete by source inspection alone.

### Windows

- x64 and arm64 package launch where corresponding hardware/emulation is available;
- tray/minimize-to-tray;
- startup enable/disable;
- chime playback;
- keyboard/screen-reader/high-contrast/text-scaling review.

### macOS

- Intel and Apple Silicon package launch;
- tray/menu integration;
- LaunchAgent startup;
- chime playback;
- keyboard/VoiceOver/text scaling;
- lifecycle/Gatekeeper behavior as documented.

### Linux

- x64 and arm64 package coverage where available;
- representative GNOME/KDE tray behavior;
- XDG autostart;
- optional chime helpers;
- keyboard/accessibility tooling.

### Android

- emulator and representative physical-device launch;
- portrait/landscape UI;
- lifecycle resume/reopen without duplicate timers;
- timezone add/remove flows;
- no desktop-only capability invocation;
- protected signing/package verification before store publication.

### iOS / iPadOS

- iPhone simulator/device launch;
- iPad simulator/device launch;
- supported orientations;
- lifecycle behavior;
- marketing version `2.6.0` / build `2602` verification;
- protected signing/provisioning validation before distribution.

### Browser / WebAssembly

- published static site loads over HTTP(S);
- no runtime/console startup errors;
- narrow/wide responsive layout;
- keyboard/accessibility review;
- reload/storage behavior matches documentation;
- no `file://` or unrestricted desktop API assumption.

### Common release evidence

- real release-build screenshots without private data;
- clean-checkout publish validation for every advertised automated package target;
- final branch-protection/status-check verification on GitHub;
- successful CI/CodeQL/Dependency Review for the exact release commit;
- downloaded release ZIP checksum verification;
- exact version/package metadata verification.

See `docs/release.md`, `docs/testing.md`, `docs/accessibility.md`, `ROADMAP.md`, and `docs/github-maintenance.md` for detailed checklists.

## Repository-settings gate

Files in Git cannot prove that GitHub repository settings are enabled. Before tagging, an administrator must confirm actual `main` ruleset/branch protection, required check contexts, security features, Actions permissions, and any protected signing environments used for production distribution.

## Release decision rule

Do not publish/tag `v2.6.0.2` solely because the source audit is complete. Release requires green automated checks for the exact release commit plus the documented native/emulator/browser/repository-settings gates. Any failed gate must be fixed before release or explicitly re-scoped and documented without falsely advertising unsupported verification.
