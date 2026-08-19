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
dotnet --info
dotnet restore ChronoDesk.sln
dotnet format ChronoDesk.sln --verify-no-changes --no-restore
dotnet build ChronoDesk.sln -c Release --no-restore
dotnet test ChronoDesk.sln -c Release --no-build --collect:"XPlat Code Coverage"
dotnet list ChronoDesk.sln package --vulnerable --include-transitive
```

Do not proceed if restore, formatting, build, tests, or vulnerability review reports an unresolved blocker.

## 3. CI verification

For the exact release commit, confirm the repository checks that actually exist in `.github/workflows/` are green:

- CI on Ubuntu;
- CI on Windows;
- CI on macOS;
- CodeQL;
- dependency review where applicable to the release pull request.

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

Launch the produced executable on the matching platform before tagging whenever possible.

## 7. Create the tag

Only after the release commit is finalized:

```bash
git tag -a vX.Y.Z -m "ChronoDesk vX.Y.Z"
git push origin vX.Y.Z
```

The `Release` GitHub Actions workflow is configured to build self-contained ZIP packages and create the GitHub Release for tags matching `v*.*.*`.

For every runtime ZIP, the workflow also creates a sibling `.sha256` file. Before publication, the release job recalculates every archive hash and refuses to create the GitHub Release if a checksum is missing or does not match.

The release job also creates:

- `release-manifest.json` — product, tag/version, source commit, generation timestamp, archive names, sizes, and SHA-256 hashes;
- `release-manifest.json.sha256` — SHA-256 checksum for the manifest itself.

The manifest is metadata for integrity/auditing; it is not a code-signing substitute.

## 8. Inspect and verify generated artifacts

For each release ZIP:

- download the ZIP and its sibling `.sha256` file from the GitHub release;
- verify the checksum before extraction;
- inspect the archive contents;
- extract to a fresh folder;
- verify the application launches;
- repeat a clock/settings smoke test;
- check that no settings/log/test-result files were packaged accidentally.

### Verify on PowerShell

```powershell
$archive = "chronodesk-vX.Y.Z-win-x64.zip"
$expected = ((Get-Content -Raw "$archive.sha256").Trim() -split '\s+')[0].ToLowerInvariant()
$actual = (Get-FileHash -Algorithm SHA256 $archive).Hash.ToLowerInvariant()
if ($actual -ne $expected) { throw "Checksum mismatch" }
```

### Verify on Linux/macOS

If `sha256sum` is available:

```bash
sha256sum -c chronodesk-vX.Y.Z-linux-x64.zip.sha256
```

On macOS systems using `shasum` instead:

```bash
expected="$(awk '{print $1}' chronodesk-vX.Y.Z-osx-arm64.zip.sha256)"
actual="$(shasum -a 256 chronodesk-vX.Y.Z-osx-arm64.zip | awk '{print $1}')"
test "$expected" = "$actual"
```

Also inspect `release-manifest.json` and confirm:

- `version` equals the Git tag;
- `commit` equals the tagged source commit;
- exactly four expected runtime archives are listed for the current matrix;
- each listed `sizeBytes` is non-zero;
- each listed SHA-256 matches its archive;
- the manifest's own `.sha256` file verifies before relying on its contents.

## 9. Signing and notarization

The repository does not commit private signing keys. If code signing/notarization is added later:

- signing secrets must live in an approved secret store such as GitHub encrypted environments/secrets;
- fork pull requests must never receive signing credentials;
- unsigned PR builds and signed protected release builds should be separate jobs;
- documentation must identify which published artifacts are signed/notarized.

Never place a `.pfx`, private key, Apple certificate private material, or password in Git history.

Checksums detect accidental or malicious byte changes after the expected hash is known, but they do not establish publisher identity. Code signing/notarization remains a separate future capability.

## 10. Release notes

Every release note should cover:

- headline user-visible changes;
- fixes;
- accessibility changes;
- security/privacy changes when applicable;
- platform-specific known limitations;
- upgrade/settings migration notes;
- artifact list;
- checksum/manifest verification note;
- support/security reporting links;
- license/funding credit without making funding intrusive.

Use `docs/release-notes-template.md` as the starting point.

## 11. Post-release verification

After publication:

1. verify the release page is public and artifacts are downloadable;
2. verify at least one downloaded ZIP against its published `.sha256` file;
3. verify `release-manifest.json.sha256` and inspect the manifest commit/tag values;
4. install/extract at least one artifact from the release page rather than the local build directory;
5. confirm README download/release references remain valid;
6. update `what_changed.md` with the tag, release URL, verified platforms, and any follow-up task;
7. open focused issues for non-blocking follow-up defects discovered after release.

## Rollback

If a severe regression is discovered:

- do not rewrite the published Git tag silently;
- document the affected version;
- prepare a forward fix in a new patch version where possible;
- remove/mark an artifact only when continued distribution creates meaningful risk;
- for a security issue, coordinate through `SECURITY.md` rather than publishing exploit details prematurely.

## Definition of release-ready

A release candidate is not ready to tag until:

- clean restore/build/test/format checks pass;
- dependency/security checks are reviewed;
- core user journeys are manually exercised on target desktops;
- accessibility basics are manually reviewed;
- startup/tray/chime platform differences are documented accurately;
- real release screenshots contain no private data;
- documentation matches the exact source tree;
- generated ZIP/checksum pairs and the release integrity manifest verify;
- `CHANGELOG.md`, `ROADMAP.md`, and `what_changed.md` are current;
- no critical/blocker defect remains known.
