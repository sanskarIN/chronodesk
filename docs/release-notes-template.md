# ChronoDesk vMAJOR.MINOR.PATCH.REVISION

Release date: YYYY-MM-DD

Current target example: `v2.6.0.2`

## Highlights

- Describe the most important user-visible improvement.
- Describe another meaningful improvement.
- Call out cross-platform availability/host changes when relevant.

## Added

- New features, host support, or capabilities.

## Changed

- Behavior, UX, performance, compatibility, packaging, or platform changes.

## Fixed

- User-visible defects and regressions fixed in this release.

## Accessibility

- Keyboard, touch, screen-reader, contrast, text-scaling/browser-zoom, reduced-motion, orientation, or other accessibility improvements.
- State which platform/assistive-technology checks were actually performed.

## Security and privacy

- Security hardening or privacy-impacting changes. If none, state that no intentional privacy-model change was introduced.
- Mention mobile permission/signing or browser storage/network-hosting changes when applicable.
- Do not publish unpatched exploit detail or signing secrets.

## Platform notes

### Windows

- Verified versions/architectures (`x64`, `arm64`) and limitations.
- Tray/startup/chime/accessibility validation status.

### macOS

- Verified Intel/Apple Silicon coverage and signing/notarization status.
- Tray/LaunchAgent/chime/VoiceOver validation status.

### Linux

- Verified architectures/distributions/desktops and tray/autostart/chime limitations.

### Android

- Verified Android versions/API levels/device/emulator/ABI as applicable.
- Lifecycle/orientation/accessibility validation.
- Signed Play-distribution artifact status, if one was actually produced through protected credentials.

### iOS / iPadOS

- Verified iPhone/iPad OS/device/simulator coverage.
- Lifecycle/orientation/VoiceOver validation.
- Marketing/build version and signing/provisioning/App Store status.

### Browser / WebAssembly

- Verified browsers/versions and hosting environment.
- Responsive/accessibility/storage limitations.
- State whether the published static ZIP/site was actually served and tested over HTTP(S).

## Capability differences

Document intentional host differences clearly, for example:

- tray/focus/mini/start-with-system are desktop capabilities;
- current process-based native chime playback is desktop-specific;
- Android/iOS/iPadOS/Browser use the shared single-view shell;
- browser storage uses sandboxed WebAssembly runtime semantics.

Do not describe intentional platform limitations as if they were silently missing features.

## Upgrade and settings compatibility

- State settings schema/version impact.
- Document migration/reset steps if required.
- State browser persistence implications if behavior changed.
- State whether portable imports preserve desktop startup preference.

## Version mapping

Canonical/in-app version:

```text
MAJOR.MINOR.PATCH.REVISION
```

For `2.6.0.2`, current platform mapping is:

```text
Desktop: 2.6.0.2
Android display version: 2.6.0.2
Android numeric version code: 2602
iOS/iPadOS marketing version: 2.6.0
iOS/iPadOS build number: 2602
```

Update this section with actual values for the release rather than copying stale numbers.

## Automated downloads

Current GitHub Release workflow targets:

- `chronodesk-vMAJOR.MINOR.PATCH.REVISION-win-x64.zip`
- `chronodesk-vMAJOR.MINOR.PATCH.REVISION-win-arm64.zip`
- `chronodesk-vMAJOR.MINOR.PATCH.REVISION-linux-x64.zip`
- `chronodesk-vMAJOR.MINOR.PATCH.REVISION-linux-arm64.zip`
- `chronodesk-vMAJOR.MINOR.PATCH.REVISION-osx-x64.zip`
- `chronodesk-vMAJOR.MINOR.PATCH.REVISION-osx-arm64.zip`
- `chronodesk-vMAJOR.MINOR.PATCH.REVISION-browser-wasm.zip`
- `SHA256SUMS.txt`

Desktop ZIPs are expected to include the application plus `LICENSE`, `README.md`, `CHANGELOG.md`, `PRIVACY.md`, `SECURITY.md`, and `SUPPORT.md`.

The Browser ZIP contains the static WebAssembly site plus the release-added license/privacy documents.

Android/iOS/iPadOS source/build support is validated in CI, but production signed store packages are not automatically listed here unless protected signing/release infrastructure actually produced and verified them.

Only list artifacts that were actually produced and verified.

## Version verification

- Canonical shared `Version`: PASS / exact value
- Shared `PackageVersion`: PASS / exact value
- Shared `AssemblyVersion`: PASS / exact value
- Shared `FileVersion`: PASS / exact value
- Desktop metadata match: PASS / details
- Android display/version-code mapping: PASS / details
- Apple marketing/build mapping: PASS / details
- About displays all four canonical components: PASS
- Release tag exactly matches `v` + canonical version: PASS

## Artifact verification

- `SHA256SUMS.txt` present: PASS
- Downloaded automated ZIP checksums match: PASS / details
- Bundled legal/privacy/security/support documents present where expected: PASS
- Windows x64 package launch: PASS / not verified
- Windows arm64 package launch: PASS / not verified
- Linux x64 package launch: PASS / not verified
- Linux arm64 package launch: PASS / not verified
- macOS x64 package launch: PASS / not verified
- macOS arm64 package launch: PASS / not verified
- Browser static ZIP served over HTTP(S): PASS / not verified
- Signed Android distribution package: PASS / not produced / details
- Signed iOS/iPadOS distribution package: PASS / not produced / details

Do not mark an artifact PASS merely because the workflow definition exists.

## Verification

- CI exact release commit: PASS / details
  - Desktop Ubuntu: PASS
  - Desktop Windows: PASS
  - Desktop macOS: PASS
  - Android: PASS
  - iOS/iPadOS: PASS
  - Browser/WebAssembly: PASS
- CodeQL exact release commit: PASS
- Dependency Review/security review: PASS / details
- Clean checkout: PASS
- Windows manual smoke/accessibility test: PASS / details
- macOS manual smoke/accessibility test: PASS / details
- Linux manual smoke/accessibility test: PASS / details
- Android emulator/device smoke/accessibility test: PASS / details
- iPhone/iPad simulator/device smoke/accessibility test: PASS / details
- Browser published-site smoke/accessibility test: PASS / details
- Repository branch/ruleset/security settings reviewed: PASS / details

Replace `PASS` placeholders with real evidence or `Not performed`; do not leave template status text that could be mistaken for completed verification.

## Known limitations

- List real non-blocking limitations.
- Separate intentional capability differences from defects.
- Do not hide known release-impacting behavior.

## Support

- Support: supportramsandesh@gmail.com
- Business: sanskarin@outlook.in
- Business: sanskarin.business@gmail.com
- GitHub: https://github.com/sanskarIN/chronodesk

[![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-sanskarIN-FFDD00?logo=buy-me-a-coffee&logoColor=000000)](https://buymeacoffee.com/sanskarIN)

Funding is optional and does not unlock product functionality.

**Made by the Sanskar**
