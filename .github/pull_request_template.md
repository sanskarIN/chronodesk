## Summary

Describe the user-visible or engineering change and why it belongs in ChronoDesk.

## Verification

- [ ] `./scripts/check-version.ps1`
- [ ] `./scripts/check-markdown-links.ps1`
- [ ] `dotnet format ChronoDesk.sln --verify-no-changes`
- [ ] `dotnet build ChronoDesk.sln -c Release`
- [ ] `dotnet test ChronoDesk.sln -c Release`
- [ ] Relevant manual UI/accessibility behavior reviewed
- [ ] No secrets, tokens, private endpoints, or personal data added

## Version / release impact

- [ ] No product-version change
- [ ] Version change intentionally updates `Version`, `PackageVersion`, `AssemblyVersion`, and `FileVersion` together
- [ ] About still displays all four version components
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

## Screenshots / recordings

Add captures for meaningful UI changes, or write `Not applicable`.

## Risk and rollback

Describe compatibility concerns, platform-specific effects, persistence changes, and how to revert safely.

## Documentation

- [ ] User/developer docs updated when behavior changed
- [ ] `CHANGELOG.md` updated when user-visible behavior changed
- [ ] ADR added/updated when an architectural decision changed
