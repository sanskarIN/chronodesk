<div align="center">
  <img src="src/ChronoDesk.App/Assets/chronodesk-logo.svg" width="128" alt="ChronoDesk logo" />

# ChronoDesk

**A focused, private-by-default digital clock and world-clock dashboard for Windows, macOS, and Linux.**

[![CI](https://github.com/sanskarIN/chronodesk/actions/workflows/ci.yml/badge.svg)](https://github.com/sanskarIN/chronodesk/actions/workflows/ci.yml)
[![CodeQL](https://github.com/sanskarIN/chronodesk/actions/workflows/codeql.yml/badge.svg)](https://github.com/sanskarIN/chronodesk/actions/workflows/codeql.yml)
[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)

[![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-sanskarIN-FFDD00?logo=buy-me-a-coffee&logoColor=000000)](https://buymeacoffee.com/sanskarIN)

**Made by the Sanskar**
</div>

---

## Why ChronoDesk?

ChronoDesk is intentionally more than a classroom clock demo. It separates clock/timezone logic from UI and platform integration, persists preferences safely, supports multiple world clocks, offers focus and mini modes, respects accessibility preferences, provides optional quiet-hours-aware chimes, and includes the repository quality expected from a serious open-source desktop project.

The application does not require sign-in, analytics, a cloud database, or a network connection for its clock features. Timezone data comes from the operating system, settings stay local, and the product remains fully usable without funding or donations.

## Screenshot

> This image is an explicit placeholder until a release build is captured on a supported desktop. The repository does not pretend that an unverified mockup is a real running-app screenshot. The tagged release workflow refuses to package a release while this placeholder remains in README.

![ChronoDesk screenshot placeholder](docs/assets/screenshot-placeholder.svg)

## Features

### Clock

- 12-hour and 24-hour formats.
- Optional seconds display.
- Date and weekday display.
- ISO week number.
- Optional calendar detail line with day-of-year, ISO week, and UTC offset.
- Configurable clock font family, font size, spacing, theme, and layout.

### World clocks

- Up to 24 local world-clock cards.
- Search the timezone database available through `TimeZoneInfo` on the host OS.
- Portable IANA/Windows timezone-ID conversion fallback where .NET can map an ID.
- Graceful UTC fallback when a persisted timezone is unavailable on the current OS.
- No remote timezone API is required.

### Desktop modes

- **Focus mode:** `F11` full-screen clock.
- **Mini mode:** `Ctrl+M` compact always-on-top clock.
- Configurable normal always-on-top behavior.
- System tray menu with Show, Focus, Mini, and Quit actions where the platform tray implementation is available.
- Optional minimize-to-tray behavior.

### Chimes and quiet hours

- Chimes are opt-in and disabled by default.
- Hourly, half-hourly, and quarter-hourly cadence options.
- Quiet hours can span midnight.
- Duplicate chimes within the same minute are suppressed.
- Playback uses OS-appropriate best-effort system facilities without a remote dependency.

### Settings, updates, and About

- Clock, appearance, accessibility, behavior, privacy/data, and Updates & About settings areas.
- Import/export and reset-to-defaults controls.
- Current semantic version shown in both Settings and About.
- **Open GitHub Releases** is explicitly user-initiated; ChronoDesk does not poll an update server in the background.
- Full About dialog includes project, MIT license, privacy, support contacts, GitHub, funding information, and **Made by the Sanskar** credit.
- External navigation is restricted to absolute HTTPS and mailto destinations before it is passed to the operating system.

### Accessibility

- Keyboard-first operation and shortcuts.
- High-contrast palette.
- Reduced-motion preference; the application intentionally avoids decorative motion by default.
- Visible native focus behavior from Avalonia/Fluent controls.
- Semantic automation names on key clock/search controls and visually adjacent Settings labels.
- Scalable clock typography and touch-friendly control sizing.
- Non-color-only status text.

### Privacy and reliability

- Local JSON settings only.
- Atomic temporary-file writes before settings replacement.
- Corrupt settings are preserved for manual recovery instead of silently destroyed.
- Import/export is size-bounded and schema-validated.
- Structured JSONL logging with common email/secret-pattern redaction.
- No credentials or API keys are required.
- Startup behavior is opt-in and user-scoped.
- No telemetry, advertising, background update checker, or app-controlled update download service.

## Supported platforms

ChronoDesk targets desktop systems supported by Avalonia and .NET 9:

| Platform | Target | Release archive | Notes |
|---|---|---|---|
| Windows | x64 | `.zip` | User-level startup registration through the current-user Run key. |
| macOS | x64 / arm64 | `.tar.gz` | User LaunchAgent startup integration; tarball preserves executable mode bits. |
| Linux | x64 | `.tar.gz` | XDG autostart integration; tray/chime behavior can vary by desktop environment; tarball preserves executable mode bits. |

Release automation produces self-contained artifacts for `win-x64`, `linux-x64`, `osx-x64`, and `osx-arm64` when a supported semantic version tag is pushed. Every archive has a sibling `.sha256` checksum file, and the publication job verifies all four archive/checksum pairs before creating the GitHub Release.

## Technology stack

- C#
- .NET 9
- Avalonia UI 11
- Fluent theme
- `System.Text.Json`
- xUnit
- Python 3 repository/release validation scripts
- GitHub Actions
- GitHub CodeQL
- Dependabot

No database server, web service, authentication provider, telemetry SDK, or cloud account is required.

## Repository layout

```text
chronodesk/
├─ .github/                     # CI, CodeQL, release, Dependabot, templates
├─ docs/                        # Architecture, setup, testing, ADRs, operations
├─ scripts/                     # Repository/release integrity validators + tests
├─ src/
│  ├─ ChronoDesk.Core/          # Domain models, formatting, chime policy, contracts
│  ├─ ChronoDesk.Infrastructure/# JSON persistence, timezone/startup/chime/log adapters
│  └─ ChronoDesk.App/           # Avalonia shell, views, view models, assets
├─ tests/
│  └─ ChronoDesk.Tests/         # Unit/integration/headless automated tests
├─ ChronoDesk.sln
├─ Directory.Build.props
├─ Directory.Packages.props
└─ what_changed.md              # Primary cross-chat / cross-session handoff
```

For a deep, navigable technical map start at **[docs/README.md](docs/README.md)**. For the purpose of **every tracked file**, use **[docs/repository-reference.md](docs/repository-reference.md)**. See [docs/architecture.md](docs/architecture.md) for dependency rules and the system architecture.

## Documentation

ChronoDesk maintains product, technical, operational, architecture-decision, and file-inventory documentation as part of the implementation.

Important entry points:

- [Documentation hub](docs/README.md)
- [Architecture](docs/architecture.md)
- [Runtime behavior](docs/runtime-behavior.md)
- [Complete settings reference](docs/settings-reference.md)
- [Build/configuration reference](docs/configuration-reference.md)
- [Platform integration](docs/platform-integration.md)
- [Localization guide](docs/localization.md)
- [Testing guide](docs/testing.md)
- [Exhaustive test catalog](docs/test-catalog.md)
- [CI/CD reference](docs/ci-cd.md)
- [Release procedure](docs/release.md)
- [Accessibility checklist](docs/accessibility.md)
- [Troubleshooting](docs/troubleshooting.md)
- [Repository file reference](docs/repository-reference.md)

Documentation completeness is enforced in CI: every tracked file must have a canonical entry in `docs/repository-reference.md`.

## Quick start

### Prerequisites

- Git
- .NET 9 SDK
- A supported Windows, macOS, or Linux desktop

### Clone and run

```bash
git clone https://github.com/sanskarIN/chronodesk.git
cd chronodesk
dotnet restore ChronoDesk.sln
dotnet run --project src/ChronoDesk.App/ChronoDesk.App.csproj
```

The first run shows a short onboarding window. No account is created and no remote service is contacted by ChronoDesk itself.

For platform-specific prerequisites and packaging notes, read [docs/setup.md](docs/setup.md).

## Development setup

```bash
dotnet --info
python3 scripts/check_markdown_links.py
python3 scripts/check_documentation_inventory.py
python3 scripts/check_repository_secrets.py
python3 -m unittest discover -s scripts/tests -p 'test_*.py'
dotnet restore ChronoDesk.sln
dotnet format ChronoDesk.sln --verify-no-changes --no-restore
dotnet build ChronoDesk.sln --configuration Release --no-restore
dotnet test ChronoDesk.sln --configuration Release --no-build
```

Optional development data isolation:

```bash
# PowerShell
$env:CHRONODESK_DATA_DIR = "$PWD/.local-data"

# bash/zsh
export CHRONODESK_DATA_DIR="$PWD/.local-data"
```

`CHRONODESK_DATA_DIR` is the only application-specific environment variable. It is not a secret.

More detail: [docs/development.md](docs/development.md).

## Testing

The current automated suite covers:

- 12/24-hour and seconds formatting;
- ISO week/calendar details;
- overnight quiet-hour boundaries;
- chime cadence and duplicate suppression;
- settings normalization invariants;
- JSON settings round-trip, backup/export/import, and corrupt-file recovery;
- timezone catalog discovery/search/fallback behavior;
- startup artifact generation/cleanup across Windows, macOS, and Linux through isolated adapters;
- deterministic malformed-import fuzz/property-style robustness cases;
- external-link scheme allowlisting;
- semantic version display normalization;
- Avalonia headless main/settings/onboarding/About smoke and interaction flows;
- repository validation-script unit tests;
- repository-local Markdown link integrity;
- exhaustive tracked-file documentation inventory coverage;
- high-confidence committed credential pattern scanning.

Run:

```bash
dotnet test ChronoDesk.sln -c Release --collect:"XPlat Code Coverage"
python3 -m unittest discover -s scripts/tests -p 'test_*.py'
```

CI runs repository integrity plus formatting, build, tests, and NuGet vulnerability checks across Ubuntu, Windows, and macOS. See [docs/testing.md](docs/testing.md) and [docs/test-catalog.md](docs/test-catalog.md).

## Build and publish

Framework-dependent local publish:

```bash
dotnet publish src/ChronoDesk.App/ChronoDesk.App.csproj -c Release
```

Example self-contained Windows x64 publish:

```bash
dotnet publish src/ChronoDesk.App/ChronoDesk.App.csproj \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true
```

Equivalent RIDs used by release automation are `linux-x64`, `osx-x64`, and `osx-arm64`.

Read [docs/release.md](docs/release.md) before creating a release tag. The release workflow rejects a tag if its matching changelog heading is missing, this README still references the explicit screenshot placeholder, local Markdown is broken, the tracked-file documentation inventory is incomplete, a high-confidence committed credential pattern is detected, formatting/build/tests fail, or NuGet vulnerability inspection reports a vulnerable package.

## Keyboard shortcuts

| Shortcut | Action |
|---|---|
| `F11` | Toggle full-screen focus clock |
| `Ctrl+M` | Toggle mini mode |
| `Ctrl+K` | Focus timezone search |
| `Ctrl+,` | Open Settings |
| `Ctrl+Shift+T` | Toggle normal always-on-top setting |
| `Esc` | Leave focus or mini mode |

## Timezone update strategy

ChronoDesk deliberately does not ship a private or silently downloaded timezone database. `TimeZoneInfo` reads timezone information supplied by the operating system/.NET runtime. This keeps timezone updates aligned with system security/maintenance updates and allows clock features to remain offline.

After the OS timezone database is updated, restart ChronoDesk to rebuild its in-memory timezone catalog. Imported settings can contain Windows or IANA IDs; ChronoDesk attempts the platform mappings exposed by .NET before falling back to UTC for an unavailable ID.

See [docs/architecture.md](docs/architecture.md) and ADR 0003 for the design decision.

## Application update strategy

ChronoDesk does not run a background update checker. Settings shows the current application version and provides **Open GitHub Releases** so the user can deliberately open the repository's public release page in the default browser. ChronoDesk itself does not download or execute update packages.

This keeps update awareness compatible with the project's offline-first/privacy model while still making verified release artifacts easy to find.

## Local data

By default, settings and logs are placed under the user's application-data folder in a `ChronoDesk` directory. The exact base path is resolved through .NET's `Environment.SpecialFolder.ApplicationData` for the current user.

Typical contents:

```text
ChronoDesk/
├─ settings.json
└─ logs/
   └─ chronodesk.log.jsonl
```

If a settings document is malformed, ChronoDesk returns to safe defaults and renames the malformed document with a timestamped `.corrupt-...json` suffix when possible.

For the complete data policy, see [PRIVACY.md](PRIVACY.md) and [docs/settings-reference.md](docs/settings-reference.md).

## Security

ChronoDesk is an offline-first clock, but local desktop software still has a security boundary. The repository therefore uses:

- user-scoped startup integration;
- bounded settings imports;
- safe JSON parsing;
- atomic settings writes;
- centralized HTTPS/mailto external-link allowlisting;
- redacted structured logs;
- repository-local Markdown, tracked-file documentation, and high-confidence credential scans;
- CodeQL;
- dependency review;
- Dependabot;
- CI and release-preflight vulnerability inspection;
- tag-time changelog/screenshot readiness checks;
- release SHA-256 sidecars verified before publication;
- no committed production credentials.

Do not report vulnerabilities in a public issue. Follow [SECURITY.md](SECURITY.md).

## Accessibility

Accessibility is a release criterion rather than a post-release extra. Before a tagged release, manually review keyboard-only use, visible focus, screen-reader naming, contrast, text scaling, reduced-motion behavior, and focus/mini window transitions on each primary platform.

Settings controls with separate visual labels use explicit automation names on the interactive element rather than relying on visual adjacency alone.

See [docs/accessibility.md](docs/accessibility.md).

## Architecture

ChronoDesk is a modular desktop monolith:

```text
ChronoDesk.App
   ├──> ChronoDesk.Core
   └──> ChronoDesk.Infrastructure ──> ChronoDesk.Core

ChronoDesk.Core ──X──> Avalonia / OS APIs / filesystem
```

`ChronoDesk.Core` owns models and business rules. `ChronoDesk.Infrastructure` implements persistence and platform boundaries. `ChronoDesk.App` owns Avalonia composition and UI behavior. This keeps time/chime/settings logic testable without a desktop session.

Architecture decisions live under [docs/adr](docs/adr/). The detailed runtime sequence is in [docs/runtime-behavior.md](docs/runtime-behavior.md).

## Contributing

Contributions are welcome when they keep ChronoDesk focused, testable, accessible, private by default, and maintainable.

Start with:

1. [CONTRIBUTING.md](CONTRIBUTING.md)
2. [docs/README.md](docs/README.md)
3. [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md)
4. [ROADMAP.md](ROADMAP.md)
5. [docs/development.md](docs/development.md)
6. [docs/testing.md](docs/testing.md)
7. [docs/repository-reference.md](docs/repository-reference.md)

Please keep commits atomic and use Conventional Commits where practical. Any new tracked file must be documented in the repository reference in the same change.

Local Git identity requested for this project:

```bash
git config user.name "Sanskar"
git config user.email "sanskarin@outlook.in"
```

## GitHub repository maintenance

The repository includes issue forms, a pull request checklist, dependency automation, CI, CodeQL, dependency review, repository-integrity automation, and hardened release packaging. Recommended branch-protection rules are documented in `docs/github-maintenance.md` so repository settings can match the checks actually present in source control.

For the complete workflow/release automation map, see [docs/ci-cd.md](docs/ci-cd.md).

## Roadmap

See [ROADMAP.md](ROADMAP.md). The immediate release path is:

- stabilize first public preview;
- capture real per-platform screenshots and replace the explicit placeholder;
- validate tray and startup behavior on supported desktops;
- complete native accessibility validation;
- move `CHANGELOG.md` from Unreleased into the intended release version;
- tag the first verified release only after the clean-checkout release checklist passes.

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
