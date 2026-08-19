## Summary

Describe the user-visible or engineering change and why it belongs in ChronoDesk.

## Verification

- [ ] `python3 scripts/check_markdown_links.py`
- [ ] `python3 scripts/check_documentation_inventory.py`
- [ ] `python3 scripts/check_repository_secrets.py`
- [ ] `python3 -m unittest discover -s scripts/tests -p 'test_*.py'`
- [ ] `dotnet format ChronoDesk.sln --verify-no-changes`
- [ ] `dotnet build ChronoDesk.sln -c Release`
- [ ] `dotnet test ChronoDesk.sln -c Release`
- [ ] Relevant manual UI/accessibility/platform behavior reviewed
- [ ] No secrets, tokens, private endpoints, or personal data added

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

Add captures for meaningful UI changes, or write `Not applicable`. Remove private notifications, usernames, filesystem details, tokens, or unrelated personal information before attaching a capture.

## Risk and rollback

Describe compatibility concerns, platform-specific effects, persistence changes, external OS integration changes, and how to revert safely.

## Documentation

- [ ] User/developer docs updated when behavior changed
- [ ] `CHANGELOG.md` updated when user-visible behavior changed
- [ ] `docs/test-catalog.md` updated when automated test-file responsibilities changed
- [ ] `docs/repository-reference.md` updated for every added/renamed/moved/deleted tracked file
- [ ] `PRIVACY.md` / `SECURITY.md` updated when data, network, permission, or trust boundaries changed
- [ ] ADR added or superseded when a durable architectural decision changed
