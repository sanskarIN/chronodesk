# ChronoDesk Release Guide

ChronoDesk releases are created only after the source tree, automated checks, desktop behavior, documentation, and packaged artifacts have been verified. A tag is a release decision, not a substitute for verification.

## Versioning

Use semantic version tags:

```text
vMAJOR.MINOR.PATCH
vMAJOR.MINOR.PATCH-PRERELEASE
```

Examples:

```text
v0.1.0
v0.1.0-rc.1
v0.2.0
v1.0.0
v1.0.1
```

The Release workflow validates the pushed tag, removes the leading `v`, and uses the resulting semantic version to stamp `Version`, assembly/file version, and informational version for the published application. This prevents a tagged binary from retaining the repository's normal `0.1.0-preview` development metadata.

Tags containing a prerelease suffix such as `-rc.1` are published as GitHub prereleases. Before `v1.0.0`, minor versions may contain breaking preview changes, but those changes must still be documented clearly.

## Release prerequisites

The release operator needs:

- push permission to `sanskarIN/chronodesk`;
- Git;
- Python 3 for repository/release-integrity scripts;
- .NET 9 SDK for local verification;
- access to supported Windows, macOS, and Linux desktop sessions for manual validation;
- no uncommitted changes in the release checkout.

No application API credentials are required.

## 1. Prepare release metadata

Update:

- `CHANGELOG.md` — move relevant Unreleased entries into the exact target version/date section;
- `ROADMAP.md` — reflect completed/replanned items;
- `what_changed.md` — record the release candidate state;
- `README.md` — replace the explicit screenshot placeholder with a verified release-build capture and update compatibility notes when needed;
- `PRIVACY.md` / `SECURITY.md` if release behavior changed their scope.

For a planned tag, run:

```bash
python3 scripts/check_release_metadata.py --tag vX.Y.Z
```

For a release candidate:

```bash
python3 scripts/check_release_metadata.py --tag vX.Y.Z-rc.1
```

This release-only validator intentionally fails while either of these is true:

- `CHANGELOG.md` has no `## [X.Y.Z]` / `## [X.Y.Z-PRERELEASE]` heading matching the tag;
- README still contains the explicit `docs/assets/screenshot-placeholder.svg` release screenshot placeholder.

The validator is unit-tested during normal CI. It is also executed by tag-time `Release preflight`, so pushing a tag before release metadata is ready cannot proceed to platform packaging.

Verify version-dependent About/Settings text still derives from assembly/package metadata rather than a stale hard-coded version. Normal development builds should show the preview semantic version; tagged publish builds receive their version from the release tag.

## 2. Clean-checkout verification

Use a disposable fresh clone.

```bash
git clone https://github.com/sanskarIN/chronodesk.git chronodesk-release
cd chronodesk-release
git checkout <release-commit>
python3 scripts/check_markdown_links.py
python3 scripts/check_repository_secrets.py
python3 -m unittest discover -s scripts/tests -p 'test_*.py'
dotnet --info
dotnet restore ChronoDesk.sln
dotnet format ChronoDesk.sln --verify-no-changes --no-restore
dotnet build ChronoDesk.sln -c Release --no-restore
dotnet test ChronoDesk.sln -c Release --no-build --collect:"XPlat Code Coverage"
dotnet list ChronoDesk.sln package --vulnerable --include-transitive
```

Then run `check_release_metadata.py` with the exact intended tag as shown above.

Do not proceed if documentation integrity, credential scanning, repository-script tests, release metadata, restore, formatting, build, tests, or vulnerability review reports an unresolved blocker.

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

The tag-triggered Release workflow repeats critical release-metadata, repository-integrity, formatting, Release build, tests, and NuGet vulnerability checks in a `Release preflight` job before any platform package is created. This is a second gate, not a replacement for pull-request/main validation.

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
- Settings Updates & About version display;
- Open GitHub Releases default-browser behavior and safe failure if no handler is available;
- About support/mail links;
- optional chime;
- keyboard-only navigation;
- screen-reader naming for Settings controls;
- high contrast and text scaling.

### macOS

Repeat the same product flows and specifically verify:

- x64/arm64 target appropriate to test hardware;
- tray/menu-bar behavior as Avalonia exposes it;
- LaunchAgent creation/removal;
- `afplay` chime fallback;
- browser/mail-handler external-link behavior;
- Gatekeeper behavior for the unsigned development artifact is documented accurately.

### Linux

Test at least one GNOME-family and, when practical, one KDE-family session. Record:

- distribution/version;
- desktop environment;
- tray/status notifier result;
- XDG autostart result;
- browser/mail-handler external-link result;
- which optional sound helper, if any, supplied chime playback.

The clock must remain usable when optional tray/sound integrations or external default handlers are absent.

## 5. Capture real screenshots

Replace `docs/assets/screenshot-placeholder.svg` references with real captures only after running the release candidate.

Screenshots must:

- come from an actual ChronoDesk build;
- contain no private notifications, usernames, file paths, tokens, or unrelated personal information;
- show a coherent theme and representative clock/world-clock state;
- include meaningful alt text in README/docs.

Do not present a design mock as a verified running-app screenshot. `check_release_metadata.py` deliberately makes the explicit placeholder a tag-time blocker.

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

The workflow publishes Windows as ZIP and Unix targets as `tar.gz`. Unix tarballs are used so executable permission bits are retained for the self-contained executable.

Launch the produced executable on the matching platform before tagging whenever possible. Confirm the version shown by Settings/About matches the version you intend to tag.

## 7. Create the tag

Only after the release commit is finalized **and** `check_release_metadata.py` passes for the exact intended tag:

```bash
git tag -a vX.Y.Z -m "ChronoDesk vX.Y.Z"
git push origin vX.Y.Z
```

For a release candidate:

```bash
git tag -a vX.Y.Z-rc.1 -m "ChronoDesk vX.Y.Z-rc.1"
git push origin vX.Y.Z-rc.1
```

The `Release` GitHub Actions workflow accepts the supported semantic tag form, validates release metadata, runs release preflight, stamps package metadata from the tag, builds self-contained platform artifacts, generates checksums, verifies the downloaded artifacts, and creates the GitHub Release only after those steps succeed.

## 8. Inspect and verify generated artifacts

Expected archive formats are:

- `chronodesk-vX.Y.Z-win-x64.zip`
- `chronodesk-vX.Y.Z-linux-x64.tar.gz`
- `chronodesk-vX.Y.Z-osx-x64.tar.gz`
- `chronodesk-vX.Y.Z-osx-arm64.tar.gz`

Prerelease filenames use the exact prerelease tag, for example `chronodesk-v0.1.0-rc.1-linux-x64.tar.gz`.

Each archive is accompanied by `<archive>.sha256`. The workflow verifies all four checksum/archive pairs after downloading the package artifacts and before creating the GitHub release.

For each published archive:

- download the archive and its `.sha256` file from the GitHub release;
- verify the SHA-256 digest locally;
- inspect the archive contents;
- extract to a fresh folder;
- verify the application launches;
- confirm Settings/About show the tag-derived version;
- repeat a clock/settings smoke test;
- check that no settings/log/test result files were packaged accidentally.

Example Linux/macOS checksum verification when `sha256sum` is available:

```bash
sha256sum -c chronodesk-vX.Y.Z-linux-x64.tar.gz.sha256
```

PowerShell example:

```powershell
Get-FileHash .\chronodesk-vX.Y.Z-win-x64.zip -Algorithm SHA256
```

Compare the displayed hash with the matching `.sha256` file.

## 9. Signing and notarization

The repository does not commit private signing keys. If code signing/notarization is added later:

- signing secrets must live in an approved secret store such as GitHub encrypted environments/secrets;
- fork pull requests must never receive signing credentials;
- unsigned PR builds and signed protected release builds should be separate jobs;
- documentation must identify which published artifacts are signed/notarized.

Never place a `.pfx`, private key, Apple certificate private material, or password in Git history.

Checksums are integrity data, not a substitute for code signing/notarization or publisher-identity verification.

## 10. Release notes

Every release note should cover:

- headline user-visible changes;
- fixes;
- accessibility changes;
- security/privacy changes when applicable;
- platform-specific known limitations;
- upgrade/settings migration notes;
- artifact list and checksum availability;
- support/security reporting links;
- license/funding credit without making funding intrusive.

Use `docs/release-notes-template.md` as the starting point.

## 11. Post-release verification

After publication:

1. verify the release page is public and all four archives plus four checksum files are downloadable;
2. download at least one archive and checksum again from the public release page and verify them independently;
3. install/extract at least one artifact from the release page rather than the local build directory;
4. confirm the running artifact displays the expected tag-derived version;
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

- repository-integrity checks pass;
- repository validation-script tests pass;
- release metadata validation passes for the exact intended tag;
- clean restore/build/test/format checks pass;
- dependency/security checks are reviewed;
- core user journeys are manually exercised on target desktops;
- accessibility basics are manually reviewed;
- startup/tray/chime/external-handler platform differences are documented accurately;
- real release screenshots contain no private data and replace the explicit placeholder;
- documentation matches the exact source tree;
- `CHANGELOG.md`, `ROADMAP.md`, and `what_changed.md` are current;
- no critical/blocker defect remains known.

A successful tag workflow still requires post-publication artifact launch checks before the release should be considered fully verified.
