<div align="center">
  <img src="src/ChronoDesk.App/Assets/chronodesk-logo.svg" width="128" alt="ChronoDesk logo" />

# ChronoDesk

**A focused, private-by-default clock and world-clock dashboard for Windows, macOS, Linux, Android, iOS/iPadOS, and WebAssembly browsers.**

[![CI](https://github.com/sanskarIN/chronodesk/actions/workflows/ci.yml/badge.svg)](https://github.com/sanskarIN/chronodesk/actions/workflows/ci.yml)
[![CodeQL](https://github.com/sanskarIN/chronodesk/actions/workflows/codeql.yml/badge.svg)](https://github.com/sanskarIN/chronodesk/actions/workflows/codeql.yml)
[![Version](https://img.shields.io/badge/version-2.6.0.2-512BD4)](src/ChronoDesk.App/ChronoDesk.App.csproj)
[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)

[![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-sanskarIN-FFDD00?logo=buy-me-a-coffee&logoColor=000000)](https://buymeacoffee.com/sanskarIN)

**Made by the Sanskar**
</div>

---

## Why ChronoDesk?

ChronoDesk is intentionally more than a classroom clock demo. The application separates reusable clock/timezone logic, infrastructure, shared Avalonia UI, and platform hosts so one codebase can run across desktop, mobile, tablet, and browser environments. It supports multiple world clocks, local preferences, accessible/touch-friendly layouts, optional quiet-hours-aware desktop chimes, and desktop focus/mini modes while keeping platform-specific capabilities isolated from the shared core.

The application does not require sign-in, analytics, a cloud database, or a network connection for its clock features. Timezone data comes from the operating system/.NET runtime, application data stays local to the host, and the product remains usable without funding or donations.

## Current source version

The canonical application, package, assembly, and file version is **`2.6.0.2`**.

ChronoDesk uses four numeric components:

```text
MAJOR.MINOR.PATCH.REVISION
```

`scripts/check-version.ps1` verifies the shared app, desktop package, Android package metadata, and Apple package metadata. A release tag must match the canonical application version exactly, so this source version corresponds to `v2.6.0.2` only after the release checklist has passed.

Apple requires a three-component marketing version, so the iOS/iPadOS host maps `2.6.0.2` to marketing version `2.6.0` with build number `2602`; the shared/in-app ChronoDesk version remains `2.6.0.2`.

## Screenshot

> This image is an explicit placeholder until verified release builds are captured. The repository does not pretend that an unverified mockup is a real running-app screenshot.

![ChronoDesk screenshot placeholder](docs/assets/screenshot-placeholder.svg)

## Features

### Clock

- 12-hour and 24-hour formats.
- Optional seconds display.
- Date and weekday display.
- ISO week number.
- Optional calendar detail line with day-of-year, ISO week, and UTC offset.
- Configurable clock font family, font size, spacing, theme, and layout in the desktop settings experience.
- Responsive single-view clock shell for phone, tablet, and browser hosts.

### World clocks

- Up to 24 local world-clock cards.
- Search the timezone database available through `TimeZoneInfo` on the host runtime.
- Portable IANA/Windows timezone-ID conversion fallback where .NET can map an ID.
- Graceful UTC fallback when a persisted timezone is unavailable on the current platform.
- No remote timezone API is required.

### Desktop modes

These features are intentionally desktop-only because they depend on desktop window/session concepts:

- **Focus mode:** `F11` full-screen clock.
- **Mini mode:** `Ctrl+M` compact always-on-top clock.
- Configurable normal always-on-top behavior.
- System tray menu with Show, Focus, Mini, and Quit actions where the desktop environment supports tray integration.
- Optional minimize-to-tray behavior.
- User-scoped start-with-system integration on Windows, macOS, and Linux.

### Mobile, tablet, and browser shell

- Avalonia single-view lifetime for Android, iPhone, iPad, and WebAssembly.
- Touch-friendly clock-format and seconds controls.
- Responsive world-clock cards.
- Timezone search, add, and remove flows.
- Safe degradation of desktop-only startup/tray/window features.
- Orientation-aware mobile host configuration.

### Chimes and quiet hours

- Chimes are opt-in and disabled by default.
- Hourly, half-hourly, and quarter-hourly cadence options.
- Quiet hours can span midnight.
- Duplicate chimes within the same minute are suppressed.
- Native system chime playback is currently a desktop integration; non-desktop hosts safely no-op rather than invoking unsupported process APIs.

### Accessibility

- Keyboard-first desktop operation and shortcuts.
- Touch-friendly mobile controls.
- High-contrast palette.
- Reduced-motion preference; the application intentionally avoids decorative motion by default.
- Visible native focus behavior from Avalonia/Fluent controls.
- Semantic automation names on key clock/search controls.
- Scalable clock typography.
- Non-color-only status text.
- Responsive browser viewport and safe-area handling.

### Privacy and reliability

- Local settings; no cloud account is required.
- Atomic temporary-file writes before settings replacement on filesystem-backed hosts.
- Malformed settings are preserved for manual recovery instead of silently destroyed where the host filesystem permits it.
- Temporary I/O/read failures fall back safely without renaming a potentially valid settings file as corrupt.
- Import/export is size-bounded and schema-validated in the desktop settings workflow.
- Structured JSONL logging with common email/secret-pattern redaction on filesystem-backed hosts.
- No credentials or API keys are required.
- Startup behavior is opt-in, user-scoped, and desktop-only.

## Supported platforms

ChronoDesk uses a shared Avalonia UI/application layer with dedicated platform hosts:

| Platform | Project / target | Support notes |
|---|---|---|
| Windows | `ChronoDesk.Desktop` / x64, arm64 | Desktop shell, tray, startup integration, focus/mini modes. |
| macOS | `ChronoDesk.Desktop` / x64, arm64 | Desktop shell, LaunchAgent startup integration, focus/mini modes. |
| Linux | `ChronoDesk.Desktop` / x64, arm64 | Desktop shell and XDG autostart; tray/chime behavior may vary by desktop environment. |
| Android | `ChronoDesk.Android` / `net10.0-android` | Phone/tablet single-view shell; requires the .NET Android workload to build. |
| iOS | `ChronoDesk.iOS` / `net10.0-ios` | iPhone single-view shell; build/sign on macOS with Apple tooling. |
| iPadOS | `ChronoDesk.iOS` / `net10.0-ios` | iPad orientations supported through the same Apple host. |
| Web browser | `ChronoDesk.Browser` / `net10.0-browser` | WebAssembly single-view shell for modern WASM-capable browsers. |

Tagged release automation produces self-contained ZIP artifacts for `win-x64`, `win-arm64`, `linux-x64`, `linux-arm64`, `osx-x64`, and `osx-arm64`, plus a deployable `browser-wasm` site ZIP. Android and iOS/iPadOS are continuously built in CI; store/distribution packages require developer signing credentials and should be produced through the documented platform signing process rather than committed secrets.

## Technology stack

- C#
- .NET 10
- Avalonia UI 11.3.x
- Avalonia Desktop, Android, iOS, and Browser hosts
- Fluent theme
- `System.Text.Json`
- xUnit + Avalonia Headless
- GitHub Actions
- GitHub CodeQL
- Dependabot

No database server, web service, authentication provider, telemetry SDK, or cloud account is required.

## Repository layout

```text
chronodesk/
├─ .github/                     # CI, CodeQL, release, Dependabot, templates
├─ docs/                        # Architecture, setup, testing, ADRs, operations
├─ scripts/                     # Deterministic repository verification helpers
├─ src/
│  ├─ ChronoDesk.Core/          # Domain models, formatting, chime policy, contracts
│  ├─ ChronoDesk.Infrastructure/# Persistence, timezone/startup/chime/log adapters
│  ├─ ChronoDesk.App/           # Shared Avalonia app, views, view models, assets
│  ├─ ChronoDesk.Desktop/       # Windows/macOS/Linux entry point
│  ├─ ChronoDesk.Android/       # Android host
│  ├─ ChronoDesk.iOS/           # iPhone/iPad host
│  └─ ChronoDesk.Browser/       # WebAssembly host and wwwroot
├─ tests/
│  └─ ChronoDesk.Tests/         # Shared logic and Avalonia headless tests
├─ ChronoDesk.sln
├─ global.json                  # .NET 10 SDK family
├─ Directory.Build.props
├─ Directory.Packages.props
└─ what_changed.md              # Primary cross-chat / cross-session handoff
```

See [docs/architecture.md](docs/architecture.md) for dependency rules and runtime flow.

## Quick start

### Common prerequisites

- Git
- .NET 10 SDK

Clone once:

```bash
git clone https://github.com/sanskarIN/chronodesk.git
cd chronodesk
```

Do not restore the entire solution unless all mobile/browser workloads are installed. Restore the host you are developing instead.

### Windows, macOS, or Linux

```bash
dotnet restore src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj
dotnet run --project src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj
```

The first desktop run shows the onboarding window. No account is created and no remote service is contacted by ChronoDesk itself.

### Android

Install the workload once:

```bash
dotnet workload install android
dotnet restore src/ChronoDesk.Android/ChronoDesk.Android.csproj
dotnet build src/ChronoDesk.Android/ChronoDesk.Android.csproj -c Debug
```

Use Android Studio/ADB or the .NET Android tooling on your development machine to select an emulator/device and deploy the generated app.

### iOS / iPadOS

Apple targets require macOS with a compatible Xcode installation:

```bash
dotnet workload install ios
dotnet restore src/ChronoDesk.iOS/ChronoDesk.iOS.csproj
dotnet build src/ChronoDesk.iOS/ChronoDesk.iOS.csproj -c Debug
```

Use an iOS/iPadOS simulator for unsigned development builds. Device/App Store distribution requires your Apple signing identity and provisioning configuration.

### WebAssembly browser

```bash
dotnet workload install wasm-tools
dotnet restore src/ChronoDesk.Browser/ChronoDesk.Browser.csproj
dotnet run --project src/ChronoDesk.Browser/ChronoDesk.Browser.csproj
```

For a static deployment bundle:

```bash
dotnet publish src/ChronoDesk.Browser/ChronoDesk.Browser.csproj -c Release -o publish/browser
```

Serve the generated `publish/browser/wwwroot` over HTTP(S); do not open `index.html` directly from `file://` because the WebAssembly runtime loads module/runtime files through web requests.

For platform-specific prerequisites and packaging notes, read [docs/setup.md](docs/setup.md).

## Development setup

Desktop/shared development does not require mobile workloads:

```bash
dotnet --info
dotnet restore src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj
dotnet restore tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj
dotnet format src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj --verify-no-changes --no-restore
dotnet build src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj --configuration Release --no-restore
dotnet test tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj --configuration Release --no-restore
```

Repository checks:

```powershell
./scripts/check-version.ps1
./scripts/check-markdown-links.ps1
```

Optional development data isolation for filesystem-backed hosts:

```bash
# PowerShell
$env:CHRONODESK_DATA_DIR = "$PWD/.local-data"

# bash/zsh
export CHRONODESK_DATA_DIR="$PWD/.local-data"
```

`CHRONODESK_DATA_DIR` is not a secret. Browser builds use the WebAssembly runtime filesystem model and should not rely on a desktop absolute path.

More detail: [docs/development.md](docs/development.md).

## Testing

The automated suite covers:

- 12/24-hour and seconds formatting.
- ISO week/calendar details.
- overnight quiet-hour boundaries.
- chime cadence and duplicate suppression.
- settings normalization invariants.
- JSON settings round-trip, backup/export/import, malformed-data recovery, and transient read failure behavior.
- timezone catalog discovery/search/fallback behavior.
- startup preference rollback/import behavior.
- malformed import fuzzing and settings property invariants.
- Avalonia headless desktop-window smoke coverage including focus/mini transitions.
- shared app rendering and exact `2.6.0.2` About version rendering.

Run shared tests:

```bash
dotnet test tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj -c Release --collect:"XPlat Code Coverage"
```

CI additionally builds Windows/macOS/Linux shared/desktop code, Android, iOS/iPadOS simulator code, and Browser/WebAssembly with their required workloads. See [docs/testing.md](docs/testing.md).

## Build and publish

Framework-dependent desktop publish:

```bash
dotnet publish src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj -c Release
```

Example self-contained Windows x64 publish:

```bash
dotnet publish src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true
```

Desktop release RIDs are:

```text
win-x64
win-arm64
linux-x64
linux-arm64
osx-x64
osx-arm64
```

Browser release packaging publishes `src/ChronoDesk.Browser` and archives its static `wwwroot` site. Mobile CI verifies that the Android and Apple hosts compile, while signed APK/AAB/IPA/App Store packages must be generated with the maintainer's private signing credentials.

Tagged desktop ZIPs include the application plus `LICENSE`, `README.md`, `CHANGELOG.md`, `PRIVACY.md`, `SECURITY.md`, and `SUPPORT.md`. Release automation publishes `SHA256SUMS.txt` for integrity verification.

Read [docs/release.md](docs/release.md) before creating a release tag.

## Keyboard shortcuts

Desktop-only shortcuts:

| Shortcut | Action |
|---|---|
| `F11` | Toggle full-screen focus clock |
| `Ctrl+M` | Toggle mini mode |
| `Ctrl+K` | Focus timezone search |
| `Ctrl+,` | Open Settings |
| `Ctrl+Shift+T` | Toggle normal always-on-top setting |
| `Esc` | Leave focus or mini mode |

## Timezone update strategy

ChronoDesk deliberately does not ship a private or silently downloaded timezone database. `TimeZoneInfo` reads timezone information supplied by the operating system/.NET runtime. This keeps timezone updates aligned with platform maintenance updates and allows clock features to remain offline.

After the host timezone database/runtime data is updated, restart ChronoDesk to rebuild its in-memory timezone catalog. Imported settings can contain Windows or IANA IDs; ChronoDesk attempts the platform mappings exposed by .NET before falling back to UTC for an unavailable ID.

See [docs/architecture.md](docs/architecture.md) and ADR 0003 for the design decision.

## Local data

On filesystem-backed desktop/mobile hosts, settings and logs are resolved beneath the current user's application-data location in a `ChronoDesk` directory unless an explicit development override is configured.

Typical contents:

```text
ChronoDesk/
├─ settings.json
└─ logs/
   └─ chronodesk.log.jsonl
```

WebAssembly runs inside the browser sandbox and uses the runtime's virtual filesystem semantics. Browser storage lifetime therefore depends on the hosting/runtime persistence configuration; ChronoDesk does not claim a desktop path or unrestricted filesystem access in the browser.

If a settings document is malformed, filesystem-backed hosts return to safe defaults and preserve/quarantine the malformed document when possible. Temporary read/I/O failures return safe defaults without quarantining the original file, so a later read can recover once the transient problem clears.

For the complete data policy, see [PRIVACY.md](PRIVACY.md).

## Security

ChronoDesk is offline-first, but local application software still has a security boundary. The repository therefore uses:

- user-scoped desktop startup integration;
- bounded settings imports;
- safe JSON parsing;
- atomic settings writes where supported;
- restricted external-link schemes;
- redacted structured logs;
- exact release-tag/version matching;
- SHA-256 release checksums;
- CodeQL;
- dependency review;
- Dependabot;
- CI vulnerability inspection;
- no committed production signing credentials or API secrets.

Do not report vulnerabilities in a public issue. Follow [SECURITY.md](SECURITY.md).

## Accessibility

Accessibility is a release criterion rather than a post-release extra. Before a tagged release, manually review keyboard-only desktop use, touch use, visible focus, screen-reader naming, contrast, text scaling, reduced-motion behavior, mobile orientations, browser viewport scaling, and desktop focus/mini transitions on the applicable target platforms.

See [docs/accessibility.md](docs/accessibility.md).

## Architecture

ChronoDesk is a shared cross-platform application with thin platform hosts:

```text
ChronoDesk.Desktop ─┐
ChronoDesk.Android ─┤
ChronoDesk.iOS ─────┼──> ChronoDesk.App ───> ChronoDesk.Core
ChronoDesk.Browser ─┘          │
                               └──> ChronoDesk.Infrastructure ───> ChronoDesk.Core

ChronoDesk.Core ──X──> Avalonia / OS APIs / filesystem
```

`ChronoDesk.Core` owns models and business rules. `ChronoDesk.Infrastructure` implements persistence and guarded platform boundaries. `ChronoDesk.App` owns reusable Avalonia composition, desktop window views, the single-view mobile/browser shell, and view models. Platform hosts contain only the entry point and platform packaging configuration needed by each runtime.

Architecture decisions live under [docs/adr](docs/adr/).

## Contributing

Contributions are welcome when they keep ChronoDesk focused, testable, accessible, private by default, and maintainable.

Start with:

1. [CONTRIBUTING.md](CONTRIBUTING.md)
2. [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md)
3. [ROADMAP.md](ROADMAP.md)
4. [docs/development.md](docs/development.md)
5. [docs/testing.md](docs/testing.md)

Please keep commits atomic and use Conventional Commits where practical.

Local Git identity requested for this project:

```bash
git config user.name "Sanskar"
git config user.email "sanskarin@outlook.in"
```

## GitHub repository maintenance

The repository includes issue forms, a pull request checklist, dependency automation, platform-aware CI, CodeQL, dependency review, release packaging, version verification, and checksum generation. Recommended branch-protection rules are documented in `docs/github-maintenance.md` so repository settings can match the checks actually present in source control.

## Roadmap

See [ROADMAP.md](ROADMAP.md). The immediate release path for `2.6.0.2` is:

- require green desktop, Android, iOS/iPadOS, Browser, CodeQL, and dependency-security checks for the exact release commit;
- capture real screenshots on representative desktop/mobile/browser targets;
- validate tray/startup/chime behavior on desktops and touch/orientation behavior on mobile/tablet;
- validate browser hosting from an HTTP(S) static server;
- verify packaged desktop/browser ZIPs and SHA-256 checksums;
- generate signed mobile distribution packages only from protected maintainer signing credentials;
- tag `v2.6.0.2` only after the clean-checkout release checklist passes.

## License

ChronoDesk is licensed under the [MIT License](LICENSE).

## Contact and support

- Business: **sanskarin@outlook.in**
- Business: **sanskarin.business@gmail.com**
- Support: **supportramsandesh@gmail.com**
- GitHub: **https://github.com/sanskarIN**
- Repository: **https://github.com/sanskarIN/chronodesk**
- Buy Me a Coffee: **https://buymeacoffee.com/sanskarIN**

For support expectations, read [SUPPORT.md](SUPPORT.md).

---

<div align="center">

[![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-sanskarIN-FFDD00?logo=buy-me-a-coffee&logoColor=000000)](https://buymeacoffee.com/sanskarIN)

**Made by the Sanskar**

</div>
