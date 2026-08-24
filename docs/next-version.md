# ChronoDesk 2.7.0.0 Development Handoff

## Purpose

This document tracks work prepared for the next ChronoDesk development line without disturbing the `2.6.0.2` release candidate on `main`.

- Development branch: `next-version-2.7.0.0`
- Target development version: `2.7.0.0`
- Baseline branch: `main`
- Baseline commit at branch creation: `d56f21e17cf4b4723bce62b1d942947ed0660ebb`
- Canonical version source: `src/ChronoDesk.App/ChronoDesk.App.csproj`

The branch must remain separate from `main` until the `2.6.0.2` release decision is complete. A draft pull request is appropriate for CI and review, but it should not be merged merely to obtain validation results.

## Implemented in this next-version slice

### Version preparation

- Set `Version`, `PackageVersion`, `AssemblyVersion`, and `FileVersion` to `2.7.0.0` on the next-version branch only.
- Preserved the four-component version policy and existing repository version verifier.

### Editable world-clock labels

World-clock cards now expose an inline local label editor while keeping the saved clock identity and timezone ID unchanged.

Behavior:

- each saved world clock shows an editable label field;
- `Save changes` persists the label through the existing settings store;
- surrounding whitespace is trimmed;
- existing settings normalization still enforces the 160-character single-line label limit;
- blank editor submissions restore the existing label instead of saving an empty value;
- renaming does not change the world-clock ID or timezone ID;
- settings remain local and offline, consistent with the existing privacy model.

### Regression coverage

`MainWindowViewModelTests` now covers:

- persistence of a trimmed edited label;
- preservation of world-clock ID/timezone identity during rename;
- in-memory world-clock refresh after persistence;
- rejection of blank labels without a settings write.

## Validation status

Repository/source review has been performed for the changed files. This chat environment does not provide the .NET SDK, so it cannot truthfully claim a local build or test pass.

The branch should be validated through the existing CI gates:

```text
./scripts/check-version.ps1
./scripts/check-markdown-links.ps1
dotnet restore ChronoDesk.sln
dotnet format ChronoDesk.sln --verify-no-changes --no-restore
dotnet build ChronoDesk.sln --configuration Release --no-restore
dotnet test ChronoDesk.sln --configuration Release --no-build --collect:"XPlat Code Coverage"
dotnet list ChronoDesk.sln package --vulnerable --include-transitive
```

## Next 2.7.0.0 work candidates

Priority order for the next slice:

1. add headless UI coverage for the inline world-clock editor and save button;
2. add keyboard-friendly save/cancel behavior for edited world-clock labels;
3. improve world-clock card responsiveness at narrow desktop widths;
4. continue runtime-localization work only after a reliable live-refresh strategy is defined;
5. consider richer offline calendar details without adding accounts, tracking, or mandatory network services;
6. evaluate installer signing/notarization only when real signing infrastructure is available.

## Release relationship

`2.7.0.0` development does not replace the outstanding `2.6.0.2` release evidence. The `2.6.0.2` tag remains gated on native Windows/macOS/Linux validation, accessibility checks, exact release-CI/security evidence, screenshots, clean-checkout packaging/launch verification, repository ruleset configuration, and checksum verification.
