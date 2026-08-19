# Contributing to ChronoDesk

Thank you for helping improve ChronoDesk. Contributions should preserve the project's focus: a reliable, accessible, privacy-respecting cross-platform desktop clock.

## Before you start

- Read `README.md`, `ROADMAP.md`, `SECURITY.md`, and the relevant architecture decision records.
- Search existing issues before opening a duplicate.
- Use the bug or feature issue form when a change benefits from discussion first.
- For security vulnerabilities, follow `SECURITY.md` instead of opening a public issue.

## Development prerequisites

- Git
- .NET 9 SDK
- PowerShell 7 for repository verification scripts
- A supported desktop OS for UI testing

Clone and verify:

```bash
git clone https://github.com/sanskarIN/chronodesk.git
cd chronodesk
dotnet restore ChronoDesk.sln
dotnet format ChronoDesk.sln --verify-no-changes --no-restore
dotnet build ChronoDesk.sln -c Release --no-restore
dotnet test ChronoDesk.sln -c Release --no-build
```

Repository-specific verification:

```powershell
./scripts/check-version.ps1
./scripts/check-markdown-links.ps1
```

For local development details, see `docs/development.md`.

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

If a change creates a durable architecture decision, add or update an ADR in `docs/adr/`.

## Code quality

- Nullable reference types stay enabled.
- Warnings are treated as errors.
- Keep methods cohesive and error handling user-safe.
- Avoid logging private settings values, paths containing sensitive data, tokens, email addresses, or arbitrary imported content.
- Validate untrusted imported data before use.
- Keep user-facing defaults non-intrusive.
- Do not add remote telemetry or sign-in requirements to core clock functionality.

## Versioning

ChronoDesk uses four numeric application/release version components:

```text
MAJOR.MINOR.PATCH.REVISION
```

The current source version is `2.6.0.2`. The application project keeps `Version`, `PackageVersion`, `AssemblyVersion`, and `FileVersion` synchronized, and CI checks that they remain equal.

If your pull request intentionally changes the product version:

1. update all four project properties together;
2. update the version-bearing README/roadmap/changelog/release documentation in the same pull request;
3. run `./scripts/check-version.ps1`;
4. do not create/push a release tag merely to test workflow changes.

The About screen must continue to display all four version components.

## Tests

Every bug fix should include a regression test when the defect is testable below the UI layer. New domain behavior should have unit tests. Persistence/platform changes should include integration-oriented tests where they can be deterministic.

Before opening a pull request:

```bash
dotnet format ChronoDesk.sln --verify-no-changes
dotnet build ChronoDesk.sln -c Release
dotnet test ChronoDesk.sln -c Release
```

Also run:

```powershell
./scripts/check-version.ps1
./scripts/check-markdown-links.ps1
```

Also manually exercise relevant UI behavior when the change affects Avalonia views, keyboard navigation, focus/mini mode, tray behavior, startup, chimes, file pickers, or accessibility.

## Accessibility expectations

A UI contribution must not rely on color alone. Preserve keyboard reachability, visible focus, usable target sizes, text scaling, and descriptive labels. Avoid decorative motion that ignores the reduced-motion preference.

See `docs/accessibility.md`.

## Security and privacy expectations

- Never commit real secrets, tokens, signing keys, user data, private production endpoints, or generated credentials.
- Keep imports bounded and validated.
- Use fixed/validated URI schemes before opening external links.
- Prefer user-level rather than machine-level OS integration.
- Do not weaken security workflows to make CI green.
- Preserve the distinction between malformed settings data and temporary I/O/read failures; do not quarantine potentially valid user settings because of a transient read problem.

## Documentation

Update documentation in the same pull request when behavior changes. User-visible changes normally require `CHANGELOG.md`; architecture changes may require an ADR; release/process changes require the appropriate file in `docs/`.

## Pull requests

A strong pull request explains:

- what changed;
- why it changed;
- how it was verified;
- platform-specific behavior;
- accessibility/security/privacy impact;
- version/release impact when applicable;
- rollback considerations.

The repository pull request template contains the required checklist.

## Contact

- Business: sanskarin@outlook.in
- Business: sanskarin.business@gmail.com
- Support: supportramsandesh@gmail.com
- GitHub: https://github.com/sanskarIN
- Funding: https://buymeacoffee.com/sanskarIN

By contributing, you agree to follow `CODE_OF_CONDUCT.md` and license your contribution under the repository's MIT License.
