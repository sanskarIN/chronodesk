# ChronoDesk Release Guide

ChronoDesk releases are created only after the source tree, automated checks, desktop behavior, documentation, and packaged artifacts have been verified. A tag is a release decision, not a substitute for verification.

## Versioning

Use semantic version tags:

```text
vMAJOR.MINOR.PATCH
```

Examples:

```text
v0.1.0
v0.2.0
v1.0.0
v1.0.1
```

Before `v1.0.0`, minor versions may contain breaking preview changes, but those changes must still be documented clearly.

## Release prerequisites

The release operator needs:

- push permission to `sanskarIN/chronodesk`;
- Git;
- Python 3 for repository-integrity scripts;
- .NET 9 SDK for local verification;
- access to supported Windows, macOS, and Linux desktop sessions for manual validation;
- no uncommitted changes in the release checkout.

No application API credentials are required.

## 1. Prepare release metadata

Update:

- `CHANGELOG.md` — move relevant Unreleased entries into the target version/date section;
- `ROADMAP.md` — reflect completed/replanned items;
- `what_changed.md` — record the release candidate state;
- `README.md` — update screenshots or compatibility notes when needed;
- `PRIVACY.md` / `SECURITY.md` if release behavior changed their scope.

Verify version-dependent About text still derives from assembly/package metadata rather than a stale hard-coded version.

## 2. Clean-checkout verification

Use a disposable fresh clone.

```bash
git clone https://github.com/sanskarIN/chronodesk.git chronodesk-release
cd chronodesk-release
git checkout <release-commit>
python3 scripts/check_markdown_links.py
python3 scripts/check_repository_secrets.py
dotnet --info
dotnet restore ChronoDesk.sln
dotnet format ChronoDesk.sln --verify-no-changes --no-restore
dotnet build ChronoDesk.sln -c Release --no-restore
dotnet test ChronoDesk.sln -c Release --no-build --collect:"XPlat Code Coverage"
dotnet list ChronoDesk.sln package --vulnerable --include-transitive
```

Do not proceed if documentation integrity, credential scanning, restore, formatting, build, tests, or vulnerability review reports an unresolved blocker.

The credential script intentionally uses high-confidence patterns and is not a substitute for reviewing staged/release files for private names, screenshots, certificates, database exports, or other sensitive material that may not resemble a token.

## 3. CI verification

For the exact release commit, confirm the repository checks that actually exist in `.github/workflows/` are green:

- CI / Repository integrity;
- CI on Ubuntu;
- CI on Windows;
- CI on macOS;
- CodeQL;
- dependency review where applicable to the release pull request.

Do not add a README badge for a workflow that does not exist. If branch protection requires named checks, use the exact check names shown on the release-candidate pull request rather than guessing them from workflow filenames.

## 4. Manual desktop verification

Perform the relevant checklist from `docs/testing.md` and the full accessibility checklist from `docs/accessibility.md`.

Minimum release-candidate coverage:

### Windows

- launch and onboarding;
- main clock and world clocks;
- focus/mini modes;
- tray hide/show/quit;
- startup enable/disable;
- settings import/export;
- optional chime;
- keyboard-only navigation;
- high contrast and text scaling.

### macOS

Repeat the same product flows and specifically verify:

- x64/arm64 target appropriate to test hardware;
- tray/menu-bar behavior as Avalonia exposes it;
- LaunchAgent creation/removal;
- `afplay` chime fallback;
- Gatekeeper behavior for the unsigned development artifact is documented accurately.

### Linux

Test at least one GNOME-family and, when practical, one KDE-family session. Record:

- distribution/version;
- desktop environment;
- tray/status notifier result;
- XDG autostart result;
- which optional sound helper, if any, supplied chime playback.

The clock must remain usable when optional tray/sound integrations are absent.

## 5. Capture real screenshots

Replace `docs/assets/screenshot-placeholder.svg` references with real captures only after running the release candidate.

Screenshots must:

- come from an actual ChronoDesk build;
- contain no private notifications, usernames, file paths, tokens, or unrelated personal information;
- show a coherent theme and representative clock/world-clock state;
- include meaningful alt text in README/docs.

Do not present a design mock as a verified running-app screenshot.

## 6. Validate release packaging locally when practical

Example Windows x64:

```bash
dotnet publish src/ChronoDesk.App/ChronoDesk.App.csproj \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -p:DebugType=None
```

Equivalent release-workflow RIDs:

- `win-x64`
- `linux-x64`
- `osx-x64`
- `osx-arm64`

Launch the produced executable on the matching platform before tagging whenever possible.

## 7. Create the tag

Only after the release commit is finalized:

```bash
git tag -a vX.Y.Z -m "ChronoDesk vX.Y.Z"
git push origin vX.Y.Z
```

The `Release` GitHub Actions workflow is configured to build self-contained ZIP packages and create the GitHub Release for tags matching `v*.*.*`.

## 8. Inspect generated artifacts

For each release ZIP:

- download it from the GitHub release/artifact output;
- inspect the archive contents;
- extract to a fresh folder;
- verify the application launches;
- repeat a clock/settings smoke test;
- check that no settings/log/test result files were packaged accidentally.

Record SHA-256 checksums in release notes if checksums are introduced as a formal release artifact.

## 9. Signing and notarization

The repository does not commit private signing keys. If code signing/notarization is added later:

- signing secrets must live in an approved secret store such as GitHub encrypted environments/secrets;
- fork pull requests must never receive signing credentials;
- unsigned PR builds and signed protected release builds should be separate jobs;
- documentation must identify which published artifacts are signed/notarized.

Never place a `.pfx`, private key, Apple certificate private material, or password in Git history.

## 10. Release notes

Every release note should cover:

- headline user-visible changes;
- fixes;
- accessibility changes;
- security/privacy changes when applicable;
- platform-specific known limitations;
- upgrade/settings migration notes;
- artifact list;
- support/security reporting links;
- license/funding credit without making funding intrusive.

Use `docs/release-notes-template.md` as the starting point.

## 11. Post-release verification

After publication:

1. verify the release page is public and artifacts are downloadable;
2. install/extract at least one artifact from the release page rather than the local build directory;
3. confirm README download/release references remain valid;
4. update `what_changed.md` with the tag, release URL, verified platforms, and any follow-up task;
5. open focused issues for non-blocking follow-up defects discovered after release.

## Rollback

If a severe regression is discovered:

- do not rewrite the published Git tag silently;
- document the affected version;
- prepare a forward fix in a new patch version where possible;
- remove/mark an artifact only when continued distribution creates meaningful risk;
- for a security issue, coordinate through `SECURITY.md` rather than publishing exploit details prematurely.

## Definition of release-ready

A release candidate is not ready to tag until:

- repository-integrity checks pass;
- clean restore/build/test/format checks pass;
- dependency/security checks are reviewed;
- core user journeys are manually exercised on target desktops;
- accessibility basics are manually reviewed;
- startup/tray/chime platform differences are documented accurately;
- real release screenshots contain no private data;
- documentation matches the exact source tree;
- `CHANGELOG.md`, `ROADMAP.md`, and `what_changed.md` are current;
- no critical/blocker defect remains known.
