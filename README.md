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

> This image is an explicit placeholder until a release build is captured on a supported desktop. The repository does not pretend that an unverified mockup is a real running-app screenshot.

![ChronoDesk screenshot placeholder](docs/assets/screenshot-placeholder.svg)

## Features

### Clock

- 12-hour and 24-hour formats.
- Optional seconds display.
- Date and weekday display.
- ISO week number.
- Optional calendar detail line with day-of-year, ISO week, and UTC offset.
- Configurable clock font family, font size, spacing, theme, and layout.
- A localized local-loading state before initialization completes.
- System theme mode tracks runtime operating-system light/dark changes instead of requiring a settings restart.

### World clocks

- Up to 24 local world-clock cards with one domain-level capacity rule.
- Search the timezone database available through `TimeZoneInfo` on the host OS.
- Visible result-count and no-results feedback.
- Duplicate timezone cards are rejected by both normal UI adds and settings normalization/import.
- Removing a clock offers undo for the most recently removed card and restores its previous position.
- Portable IANA/Windows timezone-ID conversion fallback where .NET can map an ID.
- Graceful UTC fallback when a persisted timezone is unavailable on the current OS.
- No remote timezone API is required.

### Desktop modes

- **Focus mode:** `F11` full-screen clock.
- **Mini mode:** `Ctrl+M` compact always-on-top clock.
- Configurable normal always-on-top behavior; leaving mini mode restores the current saved preference.
- System tray menu with Show, Focus, Mini, and Quit actions where the platform tray implementation exposes reliable menu restoration.
- Optional minimize-to-tray behavior.
- If reliable tray restoration is unavailable, closing/background startup does not hide the only application window and leave an unreachable process.

### Chimes and quiet hours

- Chimes are opt-in and disabled by default.
- Hourly, half-hourly, and quarter-hourly cadence options.
- Quiet hours can span midnight.
- Duplicate chimes within the same minute are suppressed.
- Playback uses OS-appropriate best-effort system facilities without a remote dependency.
- Unix helper processes use fixed executable paths and argument lists without shell-command construction or unused redirected output streams.

### Updates and releases

- Settings includes an **Updates** section with the exact application informational version.
- ChronoDesk performs no background update polling or tracking.
- The only update-related network action is user initiated: **Open official releases** launches the repository's HTTPS GitHub Releases page through the same restricted external-URI policy used by About links.
- The core clock remains fully usable offline.

### Accessibility

- Keyboard-first operation and shortcuts.
- High-contrast palette.
- Reduced-motion preference; the application intentionally avoids decorative motion by default.
- Visible native focus behavior from Avalonia/Fluent controls.
- Semantic automation names on key clock/search controls.
- Scalable clock typography and touch-friendly control sizing.
- Non-color-only loading, success, warning, empty, and error status text.

### Privacy and reliability

- Local JSON settings only.
- Atomic temporary-file writes before settings replacement.
- Settings files are size-checked from the opened stream before parsing.
- Corrupt settings are preserved with collision-resistant recovery names instead of silently destroyed.
- Import/export is size-bounded, root-validated, schema-versioned, and migrated explicitly.
- Missing legacy schema is treated as schema `0`; negative/future schemas are rejected.
- Imported settings cannot silently change the current machine's startup registration.
- Startup changes are best-effort rolled back if persistence fails, including when the caller's save operation was cancelled.
- Structured JSONL logging with common email/secret-pattern redaction.
- No credentials or API keys are required.
- Startup behavior is opt-in and user-scoped.

## Supported platforms

ChronoDesk targets desktop systems supported by Avalonia and .NET 9:

| Platform | Target | Notes |
|---|---|---|
| Windows | x64 | User-level startup registration through the current-user Run key. |
| macOS | x64 / arm64 | User LaunchAgent startup integration. |
| Linux | x64 | XDG autostart integration; tray/chime behavior can vary by desktop environment. |

Release automation produces self-contained ZIP artifacts for `win-x64`, `linux-x64`, `osx-x64`, and `osx-arm64` when a semantic version tag is pushed. Every release ZIP receives a SHA-256 sidecar and the release workflow creates a source-commit/file-size/hash integrity manifest.

## Technology stack

- C#
- .NET 9
- Avalonia UI 11
- Fluent theme
- `System.Text.Json`
- xUnit
- GitHub Actions
- GitHub CodeQL
- Dependabot

No database server, web service, authentication provider, telemetry SDK, or cloud account is required.

## Repository layout

```text
chronodesk/
├─ .github/                     # CI, CodeQL, release, Dependabot, templates
├─ docs/                        # Architecture, setup, testing, ADRs, operations
├─ scripts/                     # Deterministic repository verification scripts
├─ src/
│  ├─ ChronoDesk.Core/          # Domain models, formatting, chime policy, contracts
│  ├─ ChronoDesk.Infrastructure/# JSON persistence, timezone/startup/chime/log adapters
│  └─ ChronoDesk.App/           # Avalonia shell, views, view models, assets
├─ tests/
│  └─ ChronoDesk.Tests/         # Unit/integration/headless UI regression tests
├─ ChronoDesk.sln
├─ Directory.Build.props
├─ Directory.Packages.props
└─ what_changed.md              # Primary cross-chat / cross-session handoff
```

See [docs/architecture.md](docs/architecture.md) for the dependency rules and runtime flow.

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
pwsh ./scripts/verify-doc-links.ps1
pwsh ./scripts/verify-no-secrets.ps1
dotnet --info
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

The automated suite includes coverage for:

- 12/24-hour, seconds, ISO week, calendar, and UTC-offset formatting;
- overnight quiet-hour boundaries, chime cadence, and duplicate suppression;
- settings normalization, capacity, duplicate timezone invariants, and text bounds;
- JSON round trips, import/export, corrupt-file recovery, unique corrupt backups, root/schema validation, and explicit schema migration;
- malformed/oversized import fuzz cases and deterministic property-style invariants;
- timezone catalog discovery/search/fallback behavior;
- startup registration generation/escaping and startup-setting rollback/import consistency;
- rollback after a settings-save cancellation;
- external URI allow-list behavior;
- tray visibility safety policy;
- application informational-version display behavior;
- system/light/dark/high-contrast palette selection;
- localized loading/world-clock-count states;
- Avalonia headless loading of main/settings/onboarding/About, update controls, and focus/mini transitions.

Run:

```bash
dotnet test ChronoDesk.sln -c Release --collect:"XPlat Code Coverage"
```

CI runs documentation links, tracked-file secret checks, formatting, build, tests, and NuGet vulnerability checks across Ubuntu, Windows, and macOS. See [docs/testing.md](docs/testing.md).

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

Read [docs/release.md](docs/release.md) before creating a release tag.

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

## Local data

By default, settings and logs are placed under the user's application-data folder in a `ChronoDesk` directory. The exact base path is resolved through .NET's `Environment.SpecialFolder.ApplicationData` for the current user.

Typical contents:

```text
ChronoDesk/
├─ settings.json
└─ logs/
   └─ chronodesk.log.jsonl
```

If a settings document is malformed, ChronoDesk returns to safe defaults and renames the malformed document with a collision-resistant `.corrupt-...json` suffix when possible.

For the complete data policy, see [PRIVACY.md](PRIVACY.md).

## Security

ChronoDesk is an offline-first clock, but local desktop software still has a security boundary. The repository therefore uses:

- user-scoped startup integration;
- deterministic/escaped startup registration documents;
- bounded, schema-versioned settings imports;
- safe JSON parsing and explicit migrations;
- atomic settings writes;
- restricted HTTPS/mailto external-link schemes;
- redacted structured logs;
- a non-echoing high-signal tracked-file secret scanner;
- CodeQL;
- dependency review;
- Dependabot;
- CI vulnerability inspection;
- no committed production credentials.

Do not report vulnerabilities in a public issue. Follow [SECURITY.md](SECURITY.md).

## Accessibility

Accessibility is a release criterion rather than a post-release extra. Before a tagged release, manually review keyboard-only use, visible focus, screen-reader naming, contrast, text scaling, reduced-motion behavior, update/settings navigation, and focus/mini window transitions on each primary platform.

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

The repository includes issue forms, a pull request checklist, dependency automation, CI, CodeQL, dependency review, and release packaging. Recommended branch-protection rules are documented in `docs/github-maintenance.md` so repository settings can match the checks actually present in source control.

## Roadmap

See [ROADMAP.md](ROADMAP.md). The immediate release path is:

- obtain green latest-head CI, CodeQL, and dependency review;
- capture real per-platform screenshots from verified builds;
- validate tray, startup, chime, file-picker, theme, and accessibility behavior on supported desktops;
- tag the first verified preview/release candidate only after the clean-checkout release checklist passes.

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
