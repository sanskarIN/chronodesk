## Summary

Describe the user-visible or engineering change and why it belongs in ChronoDesk. State which host/platforms are affected.

## Verification

Repository checks:

- [ ] `./scripts/check-version.ps1`
- [ ] `./scripts/check-markdown-links.ps1`

Shared/Desktop gate when applicable:

- [ ] `dotnet restore src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj`
- [ ] `dotnet restore tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj`
- [ ] `dotnet format src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj --verify-no-changes --no-restore`
- [ ] `dotnet format tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj --verify-no-changes --no-restore`
- [ ] `dotnet build src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj -c Release --no-restore`
- [ ] `dotnet test tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj -c Release --no-restore`

Affected host checks:

- [ ] Android host built when Android/shared behavior changed
- [ ] iOS/iPadOS host built when Apple/shared behavior changed
- [ ] Browser/WebAssembly host built when Browser/shared behavior changed
- [ ] Relevant native/emulator/browser UI/accessibility behavior reviewed
- [ ] Desktop-only versus single-view capability behavior considered
- [ ] No secrets, signing material, tokens, private endpoints, device identifiers, or personal data added

## Version / release impact

- [ ] No product-version change
- [ ] Canonical version change intentionally updates shared/desktop package metadata together
- [ ] Android display/version-code metadata updated consistently when version changes
- [ ] Apple marketing/build metadata updated consistently when version changes
- [ ] About still displays all four canonical version components
- [ ] Release/tag documentation updated when applicable

## Change type

- [ ] Feature
- [ ] Bug fix
- [ ] Refactor
- [ ] Performance
- [ ] Accessibility
- [ ] Security/privacy
- [ ] Documentation
- [ ] Build/CI/release
- [ ] Platform host / packaging

## Platform scope

- [ ] Shared / all hosts
- [ ] Windows
- [ ] macOS
- [ ] Linux
- [ ] Android
- [ ] iOS / iPhone
- [ ] iPadOS / iPad
- [ ] Browser / WebAssembly

## Screenshots / recordings

Add sanitized captures for meaningful UI changes, or write `Not applicable`. Do not include private notifications, signing identities, tokens, account data, or sensitive paths.

## Risk and rollback

Describe compatibility concerns, host-specific effects, persistence/browser-storage changes, mobile lifecycle/orientation impact, signing/release impact, and how to revert safely.

## Security / privacy / signing

- [ ] Browser/mobile code respects sandbox boundaries
- [ ] No production Android/Apple signing credentials are committed or exposed to untrusted PRs
- [ ] New external/network behavior is documented and justified
- [ ] Imported/untrusted data remains bounded and validated when affected

## Documentation

- [ ] User/developer docs updated when behavior changed
- [ ] `CHANGELOG.md` updated when user-visible behavior changed
- [ ] Support/platform matrix updated when capability changed
- [ ] ADR added/updated when an architectural decision changed
