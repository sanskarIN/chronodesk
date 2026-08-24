# ChronoDesk 2.7.0.0 Development Handoff

## Purpose

This document tracks work prepared for the next ChronoDesk development line without disturbing the `2.6.0.2` release candidate on `main`.

- Development branch: `next-version-2.7.0.0`
- Target development version: `2.7.0.0`
- Baseline branch: `main`
- Baseline commit at branch creation: `d56f21e17cf4b4723bce62b1d942947ed0660ebb`
- Canonical version source: `src/ChronoDesk.App/ChronoDesk.App.csproj`
- Draft pull request: `#21`

The branch must remain separate from `main` until the `2.6.0.2` release decision is complete. The draft pull request is for CI and review and must not be merged merely to obtain validation results.

## Implemented in this next-version slice

### Version preparation

- Set `Version`, `PackageVersion`, `AssemblyVersion`, and `FileVersion` to `2.7.0.0` on the next-version branch only.
- Preserved the four-component version policy and existing repository version verifier.
- Replaced the About headless smoke test's hardcoded `2.6.0.2` expectation with an assertion derived from the application assembly version, preventing the same stale-test failure on future version bumps.

### Editable world-clock labels

World-clock cards now expose an inline local label editor while keeping the saved clock identity and timezone ID unchanged.

Behavior:

- each saved world clock shows an editable label field;
- `Save changes` persists the label through the existing settings store;
- Enter saves the current label from the keyboard;
- Escape cancels the edit and restores the persisted label;
- surrounding whitespace is trimmed;
- existing settings normalization enforces the 160-character single-line label limit and removes control characters;
- blank submissions restore/retain the existing label instead of causing a settings write;
- user-visible rename status is generated from the normalized persisted label rather than raw editor input;
- renaming does not change the world-clock ID or timezone ID;
- settings remain local and offline, consistent with the existing privacy model.

### Regression coverage

`MainWindowViewModelTests` now covers:

- persistence of a normalized edited label;
- preservation of world-clock ID/timezone identity during rename;
- in-memory world-clock refresh after persistence;
- normalized user-visible rename status;
- rejection of blank labels without a settings write.

The existing About smoke test now derives its expected four-part version from the application assembly instead of embedding a release-specific literal.

## Validation status

Repository/source review has been performed for the changed files. This chat environment does not provide the .NET SDK, so it cannot truthfully claim a local build or test pass.

Draft PR #21 is mergeable. GitHub CI, CodeQL, and Dependency Review are the authoritative automated gates for the current branch head. A queued or in-progress workflow is not considered passing evidence.

The branch should continue to satisfy the existing CI commands:

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

1. add deeper headless UI coverage for the inline world-clock editor when reliable templated-control interaction is established in the headless harness;
2. improve world-clock card responsiveness at narrow desktop widths;
3. continue runtime-localization work only after a reliable live-refresh strategy is defined;
4. consider richer offline calendar details without adding accounts, tracking, or mandatory network services;
5. evaluate installer signing/notarization only when real signing infrastructure is available.

## Release relationship

`2.7.0.0` development does not replace the outstanding `2.6.0.2` release evidence. The `2.6.0.2` tag remains gated on native Windows/macOS/Linux validation, accessibility checks, exact release-CI/security evidence, screenshots, clean-checkout packaging/launch verification, repository ruleset configuration, and checksum verification.
