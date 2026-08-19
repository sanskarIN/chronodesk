# ChronoDesk Release Guide

ChronoDesk releases are created only after the source tree, automated checks, desktop behavior, documentation, and packaged artifacts have been verified. A tag is a release decision, not a substitute for verification.

## Versioning

ChronoDesk uses four numeric version components:

```text
MAJOR.MINOR.PATCH.REVISION
```

The repository's current application version is:

```text
2.6.0.2
```

Release tags add a leading `v`:

```text
vMAJOR.MINOR.PATCH.REVISION
```

Example:

```text
v2.6.0.2
```

`src/ChronoDesk.App/ChronoDesk.App.csproj` is the canonical source for `Version`, `PackageVersion`, `AssemblyVersion`, and `FileVersion`. Keep all four values identical. `scripts/check-version.ps1` enforces that rule and can also verify that a release tag matches the application version exactly.

## Release prerequisites

The release operator needs:

- push permission to `sanskarIN/chronodesk`;
- Git;
- .NET 9 SDK for local verification;
- PowerShell 7 for repository verification scripts;
- access to supported Windows, macOS, and Linux desktop sessions for manual validation;
- no uncommitted changes in the release checkout.

No application API credentials are required.

## 1. Prepare release metadata

Update:

- `src/ChronoDesk.App/ChronoDesk.App.csproj` — set the intended four-part version consistently;
- `CHANGELOG.md` — move relevant Unreleased entries into the target version/date section only when the release is actually ready;
- `ROADMAP.md` — reflect completed/replanned items;
- `what_changed.md` — record the release candidate state;
- `README.md` — update screenshots or compatibility notes when needed;
- `PRIVACY.md` / `SECURITY.md` if release behavior changed their scope.

Verify the version metadata before any build:

```powershell
./scripts/check-version.ps1
```

The About window must derive its displayed version from assembly metadata and must preserve all four version components.

## 2. Clean-checkout verification

Use a disposable fresh clone.

```bash
git clone https://github.com/sanskarIN/chronodesk.git chronodesk-release
cd chronodesk-release
git checkout <release-commit>
dotnet --info
dotnet restore ChronoDesk.sln
dotnet format ChronoDesk.sln --verify-no-changes --no-restore
dotnet build ChronoDesk.sln -c Release --no-restore
dotnet test ChronoDesk.sln -c Release --no-build --collect:"XPlat Code Coverage"
dotnet list ChronoDesk.sln package --vulnerable --include-transitive
```

Also run:

```powershell
./scripts/check-version.ps1
./scripts/check-markdown-links.ps1
```

Do not proceed if version validation, documentation validation, restore, formatting, build, tests, or vulnerability review reports an unresolved blocker.

## 3. CI verification

For the exact release commit, confirm the repository checks that actually exist in `.github/workflows/` are green:

- CI on Ubuntu;
- CI on Windows;
- CI on macOS;
- CodeQL;
- dependency review where applicable to the release pull request.

The CI matrix validates the four-part version metadata before restore/build/test work.

Do not add a README badge for a workflow that does not exist.

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

The release workflow also copies `LICENSE`, `README.md`, `CHANGELOG.md`, `PRIVACY.md`, `SECURITY.md`, and `SUPPORT.md` into every ZIP so distributed artifacts remain self-describing.

Launch the produced executable on the matching platform before tagging whenever possible.

## 7. Create the tag

Only after the release commit is finalized, verify the exact intended tag against the project metadata:

```powershell
./scripts/check-version.ps1 -Tag "v2.6.0.2"
```

Then create and push the annotated tag:

```bash
git tag -a v2.6.0.2 -m "ChronoDesk v2.6.0.2"
git push origin v2.6.0.2
```

For future versions, substitute the new four-part value consistently. The `Release` GitHub Actions workflow is configured for four-component tags matching `v*.*.*.*` and independently rejects a tag that does not exactly match the application version.

## 8. Inspect generated artifacts and checksums

The release workflow generates one ZIP per runtime identifier plus `SHA256SUMS.txt`.

For each release ZIP:

- download it from the GitHub release/artifact output;
- verify its SHA-256 value against `SHA256SUMS.txt`;
- inspect the archive contents;
- confirm the bundled license/privacy/security/support documents are present;
- extract to a fresh folder;
- verify the application launches;
- confirm About displays the exact four-part version;
- repeat a clock/settings smoke test;
- check that no settings/log/test-result files were packaged accidentally.

Treat a checksum mismatch as a release blocker until the cause is understood.

## 9. Signing and notarization

The repository does not commit private signing keys. If code signing/notarization is added later:

- signing secrets must live in an approved secret store such as GitHub encrypted environments/secrets;
- fork pull requests must never receive signing credentials;
- unsigned PR builds and signed protected release builds should be separate jobs;
- documentation must identify which published artifacts are signed/notarized.

Never place a `.pfx`, private key, Apple certificate private material, or password in Git history.

## 10. Release notes

Every release note should cover:

- exact four-part version;
- headline user-visible changes;
- fixes;
- accessibility changes;
- security/privacy changes when applicable;
- platform-specific known limitations;
- upgrade/settings migration notes;
- artifact list and checksum file;
- support/security reporting links;
- license/funding credit without making funding intrusive.

Use `docs/release-notes-template.md` as the starting point.

## 11. Post-release verification

After publication:

1. verify the release page is public and artifacts are downloadable;
2. verify `SHA256SUMS.txt` against downloaded ZIPs;
3. install/extract at least one artifact from the release page rather than the local build directory;
4. confirm README download/release references remain valid;
5. update `what_changed.md` with the tag, release URL, verified platforms, and any follow-up task;
6. open focused issues for non-blocking follow-up defects discovered after release.

## Rollback

If a severe regression is discovered:

- do not rewrite the published Git tag silently;
- document the affected version;
- prepare a forward fix in a new four-part version where possible;
- remove/mark an artifact only when continued distribution creates meaningful risk;
- for a security issue, coordinate through `SECURITY.md` rather than publishing exploit details prematurely.

## Definition of release-ready

A release candidate is not ready to tag until:

- four-part project/version metadata is internally consistent;
- the intended tag matches that version exactly;
- clean restore/build/test/format checks pass;
- dependency/security checks are reviewed;
- core user journeys are manually exercised on target desktops;
- accessibility basics are manually reviewed;
- startup/tray/chime platform differences are documented accurately;
- real release screenshots contain no private data;
- documentation matches the exact source tree;
- `CHANGELOG.md`, `ROADMAP.md`, and `what_changed.md` are current;
- no critical/blocker defect remains known.
