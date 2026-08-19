# Contributing to ChronoDesk

Thank you for helping improve ChronoDesk. Contributions should preserve the project's focus: a reliable, accessible, privacy-respecting cross-platform desktop clock.

## Before you start

- Read `README.md`, `docs/README.md`, `ROADMAP.md`, `SECURITY.md`, and the relevant architecture decision records.
- Use `docs/repository-reference.md` to understand the responsibility of any tracked file you plan to change.
- Search existing issues before opening a duplicate.
- Use the bug or feature issue form when a change benefits from discussion first.
- For security vulnerabilities, follow `SECURITY.md` instead of opening a public issue.

## Development prerequisites

- Git
- Python 3 for deterministic repository validators
- .NET 9 SDK
- A supported desktop OS for UI testing

Clone and verify:

```bash
git clone https://github.com/sanskarIN/chronodesk.git
cd chronodesk
python3 scripts/check_markdown_links.py
python3 scripts/check_documentation_inventory.py
python3 scripts/check_repository_secrets.py
python3 -m unittest discover -s scripts/tests -p 'test_*.py'
dotnet restore ChronoDesk.sln
dotnet format ChronoDesk.sln --verify-no-changes --no-restore
dotnet build ChronoDesk.sln -c Release --no-restore
dotnet test ChronoDesk.sln -c Release --no-build
```

For local development details, see `docs/development.md`. For the complete documentation map, see `docs/README.md`.

## Git identity

For commits authored for this project, the requested local email is:

```bash
git config user.name "Sanskar"
git config user.email "sanskarin@outlook.in"
```

Contributors may of course use their own valid Git identity for their own contributions.

## Branches

Use a short descriptive branch name, for example:

- `feat/world-clock-labels`
- `fix/quiet-hours-boundary`
- `docs/linux-tray-notes`
- `test/settings-corruption`

Do not mix unrelated work into one pull request.

## Commit style

Prefer small, atomic, meaningful commits and Conventional Commits:

- `feat: add ...`
- `fix: handle ...`
- `test: cover ...`
- `docs: document ...`
- `refactor: simplify ...`
- `perf: optimize ...`
- `build: configure ...`
- `ci: verify ...`
- `chore: maintain ...`

Do not create empty commits or meaningless churn to increase commit count.

## Architecture rules

1. `ChronoDesk.Core` must not depend on Avalonia, filesystem implementation details, registry APIs, process launching, or other UI/platform infrastructure.
2. `ChronoDesk.Infrastructure` may implement interfaces from Core and use OS/filesystem APIs.
3. `ChronoDesk.App` owns Avalonia composition and user interaction.
4. Business rules should remain deterministic and directly testable where possible.
5. Prefer explicit dependency wiring over hidden global state.
6. New platform-specific code must have a clear guard and a documented fallback.

If a change creates a durable architecture decision, add or supersede an ADR in `docs/adr/` rather than silently changing the historical rationale.

## Code quality

- Nullable reference types stay enabled.
- Warnings are treated as errors.
- Keep methods cohesive and error handling user-safe.
- Avoid logging private settings values, paths containing sensitive data, tokens, email addresses, or arbitrary imported content.
- Validate untrusted imported data before use.
- Keep user-facing defaults non-intrusive.
- Do not add remote telemetry or sign-in requirements to core clock functionality.
- Keep platform integrations user-scoped and deterministic below the native execution boundary when practical.

## Tests

Every bug fix should include a regression test when the defect is testable below the UI layer. New domain behavior should have unit tests. Persistence/platform changes should include integration-oriented tests where they can be deterministic.

The exhaustive mapping of current test files and their contracts is in `docs/test-catalog.md`.

Before opening a pull request:

```bash
python3 scripts/check_markdown_links.py
python3 scripts/check_documentation_inventory.py
python3 scripts/check_repository_secrets.py
python3 -m unittest discover -s scripts/tests -p 'test_*.py'
dotnet restore ChronoDesk.sln
dotnet format ChronoDesk.sln --verify-no-changes --no-restore
dotnet build ChronoDesk.sln -c Release --no-restore
dotnet test ChronoDesk.sln -c Release --no-build
```

Also manually exercise relevant UI behavior when the change affects Avalonia views, keyboard navigation, focus/mini mode, tray behavior, startup, chimes, file pickers, external default handlers, or accessibility.

## Accessibility expectations

A UI contribution must not rely on color alone. Preserve keyboard reachability, visible focus, usable target sizes, text scaling, and descriptive labels. Avoid decorative motion that ignores the reduced-motion preference.

See `docs/accessibility.md`.

## Security and privacy expectations

- Never commit real secrets, tokens, signing keys, user data, private production endpoints, or generated credentials.
- Keep imports bounded and validated.
- Use fixed/validated URI schemes before opening external links.
- Prefer user-level rather than machine-level OS integration.
- Do not weaken security workflows to make CI green.
- Update `PRIVACY.md` and/or `SECURITY.md` whenever a data category, network behavior, external process, permission, or trust boundary changes.

## Documentation

Documentation is part of the implementation, not a follow-up task.

For every behavior change:

- update the closest specialized guide from `docs/README.md`;
- update `README.md` when public features, prerequisites, support, or artifacts change;
- update `CHANGELOG.md` for user-visible behavior;
- update `docs/testing.md` and `docs/test-catalog.md` when coverage changes;
- update `docs/release.md` for release/process changes;
- update `PRIVACY.md`/`SECURITY.md` for relevant trust/data changes;
- add/supersede an ADR for durable architecture decisions;
- update `docs/repository-reference.md` whenever a tracked file is added, renamed, moved, or deleted.

The repository inventory rule is machine enforced. Every tracked path must have exactly one canonical entry in `docs/repository-reference.md`. Run:

```bash
python3 scripts/check_documentation_inventory.py
```

A new source, test, asset, resource, workflow, template, script, or documentation file is incomplete until its inventory entry is committed too.

## Pull requests

A strong pull request explains:

- what changed;
- why it changed;
- how it was verified;
- platform-specific behavior;
- accessibility/security/privacy impact;
- documentation impact;
- rollback considerations.

The repository pull request template contains the required checklist.

## Contact

- Business: sanskarin@outlook.in
- Business: sanskarin.business@gmail.com
- Support: supportramsandesh@gmail.com
- GitHub: https://github.com/sanskarIN
- Funding: https://buymeacoffee.com/sanskarIN

By contributing, you agree to follow `CODE_OF_CONDUCT.md` and license your contribution under the repository's MIT License.
