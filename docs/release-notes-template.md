# ChronoDesk vX.Y.Z

Release date: YYYY-MM-DD

## Highlights

- Describe the most important user-visible improvement.
- Describe another meaningful improvement.

## Added

- New features and capabilities.

## Changed

- Behavior, UX, performance, or compatibility changes.

## Fixed

- User-visible defects and regressions fixed in this release.

## Accessibility

- Keyboard, screen-reader, contrast, text-scaling, reduced-motion, or other accessibility improvements.

## Security and privacy

- Security hardening or privacy-impacting changes. If there are none, state that no intentional privacy model change was introduced.
- Do not publish unpatched exploit detail.

## Platform notes

### Windows

- Verified versions/architectures and any limitations.

### macOS

- Verified architectures and signing/notarization status.

### Linux

- Verified distributions/desktops and tray/chime limitations.

## Upgrade and settings compatibility

- State the settings schema/version impact.
- Document any required migration or reset steps.

## Downloads

Release workflow targets:

- `chronodesk-vX.Y.Z-win-x64.zip`
- `chronodesk-vX.Y.Z-linux-x64.zip`
- `chronodesk-vX.Y.Z-osx-x64.zip`
- `chronodesk-vX.Y.Z-osx-arm64.zip`

Each ZIP should have a sibling `.sha256` file. The release should also include:

- `release-manifest.json`
- `release-manifest.json.sha256`

Only list artifacts that were actually produced and verified for the release.

## Integrity verification

- ZIP checksum sidecars: PASS
- Release manifest checksum: PASS
- Manifest `version` matches tag: PASS
- Manifest `commit` matches tagged commit: PASS
- Expected runtime archives listed: PASS

Checksums verify artifact bytes against the published expected values; they do not replace code signing/notarization or prove publisher identity.

## Verification

- CI: PASS / link in GitHub release context
- CodeQL: PASS
- Dependency review: PASS / N/A with reason
- Documentation local-link gate: PASS
- Tracked-file secret gate: PASS
- NuGet vulnerability review: PASS
- Clean checkout: PASS
- Windows manual smoke test: PASS / details
- macOS manual smoke test: PASS / details
- Linux manual smoke test: PASS / details
- Accessibility checklist: PASS / documented exceptions

## Known limitations

- List real non-blocking limitations. Do not hide known release-impacting behavior.

## Support

- Support: supportramsandesh@gmail.com
- Business: sanskarin@outlook.in
- Business: sanskarin.business@gmail.com
- GitHub: https://github.com/sanskarIN/chronodesk

[![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-sanskarIN-FFDD00?logo=buy-me-a-coffee&logoColor=000000)](https://buymeacoffee.com/sanskarIN)

Funding is optional and does not unlock product functionality.

**Made by the Sanskar**
