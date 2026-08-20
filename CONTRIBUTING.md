# Contributing to ChronoDesk

Thank you for helping improve ChronoDesk. Contributions should preserve the project's focus: a reliable, accessible, privacy-respecting cross-platform clock and world-clock experience for desktop, mobile/tablet, and browser hosts.

## Before you start

- Read `README.md`, `ROADMAP.md`, `SECURITY.md`, and the relevant architecture decision records.
- Search existing issues before opening a duplicate.
- Use the bug or feature issue form when a change benefits from discussion first.
- For security vulnerabilities, follow `SECURITY.md` instead of opening a public issue.

## Development prerequisites

Common:

- Git
- .NET 10 SDK
- PowerShell 7 for repository verification scripts

Additional tooling depends on the host you are changing:

- Desktop: Windows, macOS, or Linux graphical session for native UI testing.
- Android: .NET Android workload, JDK 17, Android SDK, emulator/device for deployment validation.
- iOS/iPadOS: macOS, compatible Xcode, .NET iOS workload, simulator/device.
- Browser: `wasm-tools` workload and a modern WebAssembly-capable browser.

Clone:

```bash
git clone https://github.com/sanskarIN/chronodesk.git
cd chronodesk
```

The solution contains workload-specific Android, iOS, and Browser projects. Do not use full-solution restore/build as the default workflow unless all corresponding workloads are installed.

For shared/desktop development:

```bash
dotnet restore src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj
dotnet restore tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj
dotnet format src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj --verify-no-changes --no-restore
dotnet format tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj --verify-no-changes --no-restore
dotnet build src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj -c Release --no-restore
dotnet test tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj -c Release --no-restore
```

Repository-specific verification:

```powershell
./scripts/check-version.ps1
./scripts/check-markdown-links.ps1
```

For host-specific workload/build commands, see `docs/setup.md` and `docs/development.md`.

## Git identity

For commits authored for maintainer work on this project, the requested local identity is:

```bash
git config user.name "Sanskar"
git config user.email "sanskarin@outlook.in"
```

Other contributors should use their own valid Git identity.

## Branches

Use a short descriptive branch name, for example:

- `feat/world-clock-labels`
- `fix/android-lifecycle`
- `fix/browser-storage`
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

Do not create empty commits or meaningless churn solely to increase commit count.

## Architecture rules

1. `ChronoDesk.Core` must not depend on Avalonia, filesystem implementation details, registry APIs, process launching, or other UI/platform infrastructure.
2. `ChronoDesk.Infrastructure` may implement interfaces from Core and use guarded OS/filesystem APIs.
3. `ChronoDesk.App` is the platform-neutral Avalonia application/presentation library; it contains no executable entry point.
4. `ChronoDesk.Desktop`, `ChronoDesk.Android`, `ChronoDesk.iOS`, and `ChronoDesk.Browser` are thin hosts that own platform entry points and packaging configuration.
5. Reusable clock/world-clock behavior belongs in Core/App/Infrastructure, not duplicated in individual hosts.
6. Business rules should remain deterministic and directly testable where possible.
7. Prefer explicit dependency wiring over hidden global state.
8. New platform-specific code must have a clear runtime/build boundary and a documented fallback.
9. Do not fake desktop feature parity on mobile/browser when the underlying platform concept does not exist.

If a change creates a durable architecture decision, add or update an ADR in `docs/adr/`.

## Application lifetimes

The shared App supports two Avalonia lifetime families:

- Classic desktop lifetime → `MainWindow` plus optional desktop tray/window integrations.
- Single-view lifetime → `MainView` for Android, iOS/iPadOS, and Browser.

Do not open desktop-only modal/window features from the single-view path. Shared features should normally be exposed through the view model/service layer and rendered appropriately by each shell.

## Code quality

- Nullable reference types stay enabled.
- Warnings are treated as errors.
- Keep methods cohesive and error handling user-safe.
- Avoid logging private settings values, sensitive paths, tokens, email addresses, signing material, or arbitrary imported content.
- Validate untrusted imported data before use.
- Keep user-facing defaults non-intrusive.
- Do not add remote telemetry or sign-in requirements to core clock functionality.
- Browser/mobile code must not assume unrestricted process, registry, tray, desktop-window, or filesystem access.

## Versioning

ChronoDesk uses four numeric canonical application/release components:

```text
MAJOR.MINOR.PATCH.REVISION
```

Current canonical source version: `2.6.0.2`.

`scripts/check-version.ps1` verifies:

- shared `Version`, `PackageVersion`, `AssemblyVersion`, and `FileVersion` equality;
- matching desktop package/assembly/file metadata;
- Android display version and positive numeric version code;
- Apple three-component marketing-version mapping plus positive build number;
- exact four-component release-tag match.

Current mobile mappings are:

```text
Canonical / in-app: 2.6.0.2
Android version name: 2.6.0.2
Android version code: 2602
iOS/iPadOS marketing version: 2.6.0
iOS/iPadOS build number: 2602
```

If a pull request intentionally changes the product version:

1. update canonical and platform package metadata consistently;
2. keep Android/Apple numeric build identifiers monotonic for distribution;
3. update version-bearing README/roadmap/changelog/release documentation;
4. run `./scripts/check-version.ps1`;
5. do not create/push a release tag merely to test workflow changes.

The About screen must continue to display the full canonical four-component version.

## Tests and host builds

Every bug fix should include a regression test when the defect is testable below the native UI/platform boundary. New domain behavior should have unit tests. Persistence/platform changes should include integration-oriented tests when deterministic.

Before opening a pull request for shared/desktop changes, run the shared/desktop gate shown above plus repository scripts.

If your change affects another host, also build that host with its workload:

### Android

```bash
dotnet workload install android
dotnet restore src/ChronoDesk.Android/ChronoDesk.Android.csproj
dotnet build src/ChronoDesk.Android/ChronoDesk.Android.csproj -c Release --no-restore
```

### iOS / iPadOS

On macOS:

```bash
dotnet workload install ios
dotnet restore src/ChronoDesk.iOS/ChronoDesk.iOS.csproj
dotnet build src/ChronoDesk.iOS/ChronoDesk.iOS.csproj -c Release --no-restore
```

### Browser

```bash
dotnet workload install wasm-tools
dotnet restore src/ChronoDesk.Browser/ChronoDesk.Browser.csproj
dotnet build src/ChronoDesk.Browser/ChronoDesk.Browser.csproj -c Release --no-restore
```

CI performs all host-build gates independently so contributors working on one platform are not required to install every workload locally.

Also manually exercise relevant behavior when a change affects native UI, keyboard/touch navigation, focus/mini mode, tray, startup, chimes, file pickers, mobile lifecycle/orientation, browser hosting/storage, or accessibility.

## Accessibility expectations

A UI contribution must not rely on color alone. Preserve:

- keyboard reachability on desktop/browser;
- visible focus;
- usable touch targets on phone/tablet;
- text scaling;
- descriptive labels/automation names;
- high-contrast behavior;
- orientation/narrow-width usability;
- reduced-motion expectations.

See `docs/accessibility.md`.

## Security and privacy expectations

- Never commit real secrets, tokens, Android keystores, Apple private keys/certificates, provisioning secrets, user data, private production endpoints, or generated credentials.
- Keep imports bounded and validated.
- Use fixed/validated URI schemes before opening external links.
- Prefer user-level rather than machine-level desktop integration.
- Respect mobile/browser sandbox boundaries.
- Do not weaken security workflows to make CI green.
- Preserve the distinction between malformed settings data and temporary I/O/read failures; do not quarantine potentially valid user settings because of a transient read problem.
- Production mobile signing must run only with protected maintainer credentials/release environments.

## Documentation

Update documentation in the same pull request when behavior changes. User-visible changes normally require `CHANGELOG.md`; architecture changes may require an ADR; release/process changes require the appropriate file in `docs/`; platform capability changes must update the support matrix rather than silently changing behavior.

## Pull requests

A strong pull request explains:

- what changed;
- why it changed;
- how it was verified;
- which host/platforms are affected;
- desktop versus single-view capability impact;
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
