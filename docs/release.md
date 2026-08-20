# ChronoDesk Release Guide

ChronoDesk releases are created only after the exact source tree, automated checks, platform-host builds, documentation, and distributable artifacts have been verified. A tag is a release decision, not a substitute for verification.

## Versioning

ChronoDesk uses a canonical four-component version:

```text
MAJOR.MINOR.PATCH.REVISION
```

Current canonical version:

```text
2.6.0.2
```

Release tag:

```text
v2.6.0.2
```

`src/ChronoDesk.App/ChronoDesk.App.csproj` remains the canonical source for `Version`, `PackageVersion`, `AssemblyVersion`, and `FileVersion`.

Cross-platform mappings:

- Desktop package/assembly/file version: `2.6.0.2`.
- Android `ApplicationDisplayVersion`: `2.6.0.2`.
- Android numeric `ApplicationVersion`: `2602`.
- iOS/iPadOS marketing `ApplicationDisplayVersion`: `2.6.0`.
- iOS/iPadOS build `ApplicationVersion`: `2602`.
- In-app About version: `2.6.0.2`.

Apple uses a three-component marketing version; the fourth canonical ChronoDesk component is represented through the Apple build number. `scripts/check-version.ps1` enforces the mapping and exact tag equality.

## Release prerequisites

Release engineering may require multiple environments because not all targets can be built/signed everywhere.

Common:

- push permission to `sanskarIN/chronodesk`;
- Git;
- .NET 10 SDK;
- PowerShell 7 for repository scripts;
- a clean checkout of the exact release commit.

Platform verification:

- Windows session for Windows behavior/package smoke tests;
- macOS for macOS and iOS/iPadOS builds/tests;
- Linux session for Linux behavior/package smoke tests;
- Android SDK/JDK 17 + emulator/device for Android manual validation;
- modern browser + HTTP(S) static host for WebAssembly validation.

Production mobile distribution additionally requires maintainer-controlled signing/provisioning credentials. Those secrets must not be committed.

No application API credentials are required.

## 1. Prepare release metadata

Update as applicable:

- `src/ChronoDesk.App/ChronoDesk.App.csproj` — canonical four-part version;
- `src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj` — matching four-part package/assembly/file values;
- `src/ChronoDesk.Android/ChronoDesk.Android.csproj` — display version + monotonic numeric version code;
- `src/ChronoDesk.iOS/ChronoDesk.iOS.csproj` — Apple marketing version + monotonic build number;
- `CHANGELOG.md`;
- `ROADMAP.md`;
- `what_changed.md`;
- `README.md`;
- privacy/security/support documentation if behavior changed.

Verify before builds:

```powershell
./scripts/check-version.ps1
```

For the current release candidate, expected output must confirm canonical `2.6.0.2` and Apple mapping `2.6.0` / build `2602`.

## 2. Shared and desktop clean-checkout verification

Use a disposable fresh clone:

```bash
git clone https://github.com/sanskarIN/chronodesk.git chronodesk-release
cd chronodesk-release
git checkout <release-commit>
dotnet --info
dotnet restore src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj
dotnet restore tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj
dotnet format src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj --verify-no-changes --no-restore
dotnet format tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj --verify-no-changes --no-restore
dotnet build src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj -c Release --no-restore
dotnet test tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj -c Release --no-restore --collect:"XPlat Code Coverage"
dotnet list src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj package --vulnerable --include-transitive
```

Repository checks:

```powershell
./scripts/check-version.ps1
./scripts/check-markdown-links.ps1
```

Do not substitute `dotnet restore/build ChronoDesk.sln` on a machine without all mobile/browser workloads; the solution intentionally contains workload-specific platform projects.

## 3. Platform-host verification

### Android

```bash
dotnet workload install android
dotnet restore src/ChronoDesk.Android/ChronoDesk.Android.csproj
dotnet build src/ChronoDesk.Android/ChronoDesk.Android.csproj -c Release --no-restore
```

Then deploy a debug/release-candidate build to a representative emulator/device and complete the Android checklist in `docs/testing.md`.

### iOS / iPadOS

On macOS with compatible Xcode:

```bash
dotnet workload install ios
dotnet restore src/ChronoDesk.iOS/ChronoDesk.iOS.csproj
dotnet build src/ChronoDesk.iOS/ChronoDesk.iOS.csproj -c Release --no-restore
```

Validate on an iPhone/iPad simulator and, before store distribution, a provisioned physical device where available.

### Browser / WebAssembly

```bash
dotnet workload install wasm-tools
dotnet restore src/ChronoDesk.Browser/ChronoDesk.Browser.csproj
dotnet build src/ChronoDesk.Browser/ChronoDesk.Browser.csproj -c Release --no-restore
dotnet publish src/ChronoDesk.Browser/ChronoDesk.Browser.csproj -c Release -o publish/browser
```

Serve `publish/browser/wwwroot` through HTTP(S) and complete the browser checklist in `docs/testing.md`.

## 4. GitHub CI/security verification

For the exact release commit, confirm green checks for:

- Desktop / Ubuntu / .NET 10;
- Desktop / Windows / .NET 10;
- Desktop / macOS / .NET 10;
- Android / .NET 10;
- iOS and iPadOS / .NET 10;
- Browser / WebAssembly / .NET 10;
- CodeQL;
- Dependency Review where applicable.

Do not tag from an older green commit after new changes have landed. Checks must correspond to the exact commit being tagged.

## 5. Manual desktop verification

Use `docs/testing.md` and `docs/accessibility.md`.

### Windows

Verify:

- launch/onboarding;
- main clock/world clocks;
- focus and mini modes;
- tray hide/show/quit;
- current-user startup enable/disable;
- settings import/export;
- optional chime;
- keyboard-only navigation;
- high contrast/text scaling;
- both x64 and arm64 release artifacts when matching hardware/emulation is available.

### macOS

Verify:

- Intel/Apple Silicon target appropriate to test hardware;
- tray/menu-bar behavior exposed by Avalonia;
- LaunchAgent creation/removal;
- `afplay` chime fallback;
- expected Gatekeeper behavior for unsigned development artifacts;
- both release RIDs are generated.

### Linux

Test at least one common desktop environment and, when practical, a second family. Record:

- distro/version;
- desktop environment;
- tray/status-notifier result;
- XDG autostart result;
- optional sound-helper behavior;
- x64/arm64 artifact coverage available for the release.

The clock must remain usable when optional tray/sound facilities are absent.

## 6. Manual mobile/tablet verification

### Android

Verify:

- app installs and launches;
- single-view UI works in portrait/landscape;
- clock format/seconds controls work;
- timezone search/add/remove works;
- lifecycle resume does not create duplicate timers;
- no desktop startup/tray/window capability is invoked;
- application ID and version metadata are correct;
- signed production package is generated only through protected signing material.

### iOS/iPadOS

Verify:

- simulator/device launch;
- iPhone and iPad layouts/orientations;
- timezone/clock controls;
- lifecycle behavior;
- package marketing version `2.6.0` and build `2602`;
- provisioning/signing remains external to source control.

## 7. Browser verification

Verify published static site:

- loads only through HTTP(S), not `file://`;
- starts without console/runtime errors;
- clock/timezone interactions work;
- narrow/wide viewport layouts remain usable;
- browser sandbox limitations match documentation;
- storage/reload behavior is documented accurately;
- no desktop-only process/registry/tray API is assumed.

## 8. Capture real screenshots

Replace `docs/assets/screenshot-placeholder.svg` only with verified runtime captures.

Screenshots must:

- come from an actual ChronoDesk build;
- contain no private notifications, usernames, paths, tokens, signing identities, or unrelated personal information;
- show representative platform state;
- include meaningful alt text.

Do not present a design mock as a verified running-app screenshot.

## 9. Validate desktop packaging locally when practical

Example Windows x64:

```bash
dotnet publish src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -p:DebugType=None
```

Release workflow desktop RIDs:

```text
win-x64
win-arm64
linux-x64
linux-arm64
osx-x64
osx-arm64
```

Each desktop ZIP includes:

- application files;
- `LICENSE`;
- `README.md`;
- `CHANGELOG.md`;
- `PRIVACY.md`;
- `SECURITY.md`;
- `SUPPORT.md`.

Launch/extract the matching platform package before tagging whenever possible.

## 10. Validate browser packaging

The release workflow runs:

```bash
dotnet publish src/ChronoDesk.Browser/ChronoDesk.Browser.csproj --configuration Release --output publish/browser
```

It packages the generated `publish/browser/wwwroot` as:

```text
chronodesk-v2.6.0.2-browser-wasm.zip
```

The archive also contains `LICENSE` and `PRIVACY.md`.

Extract and serve the archive from a clean HTTP(S) static host before release approval.

## 11. Mobile signing and store packaging

CI intentionally verifies **unsigned/source buildability**, not production signing.

### Android

Production APK/AAB signing credentials must be stored in an approved protected secret store/release environment. Never commit:

- `.jks`/`.keystore` files;
- keystore passwords;
- key passwords;
- private signing material.

### Apple

Apple device/App Store distribution requires protected:

- signing identity/private key;
- certificate material as required by the release system;
- provisioning/profile configuration;
- App Store Connect credentials/tokens if automated later.

Fork pull requests must never receive production signing secrets.

## 12. Create the tag

Verify exact intended tag:

```powershell
./scripts/check-version.ps1 -Tag "v2.6.0.2"
```

Then create/push the annotated tag:

```bash
git tag -a v2.6.0.2 -m "ChronoDesk v2.6.0.2"
git push origin v2.6.0.2
```

The GitHub Release workflow accepts four-component tags matching `v*.*.*.*` and independently rejects a tag that does not match canonical source metadata.

## 13. Inspect release artifacts and checksums

The release workflow generates:

- six desktop ZIPs;
- one Browser/WebAssembly ZIP;
- `SHA256SUMS.txt`.

For every ZIP:

1. download from the published release rather than trusting a local build folder;
2. verify SHA-256 against `SHA256SUMS.txt`;
3. inspect archive contents;
4. extract to a fresh location;
5. launch/serve on the matching platform;
6. verify expected version and a basic clock/world-clock flow;
7. confirm no settings/log/test-result/signing-secret files were packaged accidentally.

Treat any checksum mismatch as a blocker.

## 14. Release notes

Every release note should cover:

- exact canonical four-part version;
- supported platform matrix;
- user-visible changes/fixes;
- accessibility changes;
- security/privacy changes when applicable;
- desktop/mobile/browser-specific limitations;
- settings/schema migration notes;
- desktop/browser artifact list + checksum file;
- mobile signing/store availability status;
- support/security reporting links.

Use `docs/release-notes-template.md` as the starting point.

## 15. Post-release verification

After publication:

1. verify release page/artifacts are public and downloadable;
2. verify checksums against downloaded ZIPs;
3. launch at least one downloaded desktop artifact;
4. serve the downloaded browser artifact;
5. confirm README/release references remain valid;
6. update `what_changed.md` with tag, release URL, verified platforms, and follow-ups;
7. create focused issues for non-blocking defects;
8. when signed mobile builds are distributed, record their exact version/build identifiers without exposing secrets.

## Rollback

If a severe regression is discovered:

- do not silently rewrite a published tag;
- document the affected version/platforms;
- prepare a forward fix in a new four-part version where possible;
- remove/mark an artifact only when continued distribution creates meaningful risk;
- coordinate security issues through `SECURITY.md` rather than publishing sensitive exploitation detail prematurely.

## Definition of release-ready

A candidate is not release-ready until:

- canonical and platform version metadata is consistent;
- intended tag matches exactly;
- desktop formatting/build/tests pass;
- Android host build passes;
- iOS/iPadOS simulator build passes;
- Browser/WebAssembly build and static publish pass;
- CodeQL/dependency checks are reviewed;
- representative manual desktop/mobile/browser journeys are exercised;
- accessibility basics are reviewed on applicable interaction modes;
- desktop-only feature boundaries are documented accurately;
- mobile production signing material remains protected;
- screenshots contain no private data;
- packaged desktop/browser ZIPs and checksums are verified;
- `CHANGELOG.md`, `ROADMAP.md`, `README.md`, docs, and `what_changed.md` match the exact source tree;
- no critical/blocker defect remains known.
