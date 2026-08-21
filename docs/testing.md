# ChronoDesk Testing Guide

## Quality gates

The intended release gate is:

```bash
dotnet restore ChronoDesk.sln
dotnet format ChronoDesk.sln --verify-no-changes --no-restore
dotnet build ChronoDesk.sln -c Release --no-restore
dotnet test ChronoDesk.sln -c Release --no-build --collect:"XPlat Code Coverage"
```

Also verify version metadata and repository-local Markdown links with PowerShell:

```powershell
./scripts/check-version.ps1
./scripts/check-markdown-links.ps1
```

For a tagged release candidate, also bind the tag to the application version:

```powershell
./scripts/check-version.ps1 -Tag "v2.6.0.2"
```

CI executes equivalent version/documentation/formatting/build/test work on Ubuntu, Windows, and macOS and also inspects NuGet dependencies for reported vulnerabilities.

## Automated test areas

### Clock formatting

`ClockFormatterTests` verifies:

- 24-hour time with seconds;
- 12-hour time without seconds;
- ISO week number;
- UTC offset/calendar detail rendering.

Use explicit `DateTimeOffset`, timezone, and culture inputs so tests do not depend on the machine's current wall clock.

### Quiet hours

`QuietHoursTests` verifies:

- overnight intervals such as 22:00 → 07:00;
- inclusive start/exclusive end behavior;
- disabled quiet hours;
- equal start/end meaning no quiet interval.

### Chime policy

`ChimePolicyTests` verifies:

- hourly cadence;
- quarter-hour cadence boundaries;
- duplicate suppression within the same minute;
- quiet-hour suppression.

New cadence behavior must remain independent of actual sound playback.

### Settings model

`AppSettingsTests` verifies:

- visual range normalization;
- default font fallback;
- invalid clock removal;
- case-insensitive duplicate clock/timezone removal;
- at least one clock invariant;
- 24-card limit.

### Persistence

`JsonSettingsStoreTests` uses isolated temporary directories and verifies:

- settings save/load round-trip;
- portable export/import;
- malformed JSON fallback;
- corrupt-file preservation;
- transient read failures fall back safely without renaming a potentially valid settings file;
- normal loading resumes after the transient file lock is released.

Tests must not read or write the developer's real ChronoDesk data folder.

### View-model reliability

`MainWindowViewModelTests` verifies:

- explicit startup changes are applied once;
- startup integration is rolled back when persistence fails;
- portable imports cannot silently change the machine startup preference;
- unreadable settings fall back to defaults while the clock, world clocks, and timezone search still initialize.

### Platform startup artifact serialization

`WindowsStartupCommandTests` verifies deterministic Run-key command generation/matching without mutating the registry. Coverage includes:

- quoting executable paths containing spaces;
- keeping `--background` as the canonical argument;
- case-insensitive canonical command matching with harmless outer whitespace;
- rejecting substring lookalikes and commands with unexpected extra arguments;
- rejecting executable path strings containing quotes, carriage returns, or newlines.

`LinuxDesktopEntryTests` verifies deterministic XDG autostart `Exec` generation without touching a real desktop session. Coverage includes:

- executable paths containing spaces;
- escaping for backslashes, double quotes, dollar signs, and backticks inside a quoted `Exec` token;
- literal percent escaping as `%%` so a path cannot accidentally create a desktop-entry field code;
- rejection of executable paths containing `=`, carriage returns, or newlines where a safe executable token cannot be emitted.

`MacLaunchAgentPlistTests` verifies deterministic LaunchAgent generation without touching a real `~/Library/LaunchAgents` directory. Coverage includes:

- XML-safe preservation of an executable path containing `&`, `<`, and `>`;
- the stable `com.sanskar.chronodesk` label;
- `--background` as a distinct `ProgramArguments` entry;
- `RunAtLoad` enabled.

The plist tests parse generated XML with DTD processing disabled. These serializer tests reduce platform-regression risk, but they intentionally do **not** claim that Registry, LaunchAgent, or XDG startup enable/disable behavior has been validated in a native desktop session.

### Timezone catalog

`SystemTimeZoneCatalogTests` verifies:

- system timezone discovery;
- UTC availability;
- invalid-ID fallback;
- bounded case-insensitive search.

### Property-style robustness tests

`DomainPropertyTests` runs deterministic seeded randomized cases against reference invariants. It verifies thousands of quiet-hour combinations and checks that settings normalization is idempotent, bounded, and produces unique clock IDs.

This test style gives broad edge coverage while staying deterministic in CI. A failure can always be reproduced from the committed seed.

### Import fuzz tests

`SettingsImportFuzzTests` feeds a deterministic corpus of malformed binary/JSON inputs into the settings importer and verifies that the primary settings document is not changed. It also verifies rejection of files above the configured size limit.

Fuzz inputs are generated locally inside the test and never contain production/user data.

### Headless Avalonia UI smoke tests

`AvaloniaTestSetup` and `HeadlessUiSmokeTests` use `Avalonia.Headless.XUnit` with the same Avalonia 11.3 maintenance baseline as the application. Current smoke coverage verifies:

- main-window XAML/resource loading;
- key named controls exist;
- mini mode can enter and restore normal dimensions;
- focus mode hides/restores application chrome;
- focus mode restores the prior normal/maximized window state;
- Settings loads primary preference controls;
- onboarding and About windows load localized resources;
- About displays the complete four-part `2.6.0.2` assembly version rather than truncating the revision component.

Headless UI tests strengthen cross-platform CI but do **not** replace real desktop testing for system tray, startup registration, sound playback, accessibility APIs, display scaling, or native file pickers.

## Version verification

`scripts/check-version.ps1` reads the application project and enforces:

- a four-component numeric `Version` (`MAJOR.MINOR.PATCH.REVISION`);
- matching `PackageVersion`, `AssemblyVersion`, and `FileVersion` values;
- no conflicting `VersionPrefix`/`VersionSuffix` values;
- assembly-version component bounds;
- exact tag equality when `-Tag` is supplied.

The current required source version is `2.6.0.2`.

## Documentation-link verification

`scripts/check-markdown-links.ps1` recursively inspects Markdown documents and validates repository-local file and directory targets. It ignores external URLs and same-document anchors so transient network failures do not make offline verification nondeterministic.

The check rejects missing local targets and relative links that escape the repository root. Repository paths containing spaces should be percent-encoded or use Markdown's angle-bracket destination form.

## Manual UI checklist

Automated Core/headless tests are not a substitute for desktop validation. Before a tagged release, test each supported primary platform.

### Launch/onboarding

- [ ] Fresh data directory opens onboarding.
- [ ] Onboarding explains offline/private behavior accurately.
- [ ] Completing onboarding persists and it does not reappear next launch.
- [ ] An unreadable settings location produces a warning while the clock remains usable.
- [ ] A temporary settings-file lock does not rename/delete the valid settings file.

### Main clock

- [ ] Time updates smoothly without visible duplicate/reordered text.
- [ ] 12/24-hour toggle is immediate.
- [ ] Seconds toggle is immediate.
- [ ] Date/weekday/week/calendar settings match visible output.
- [ ] Large font sizes remain usable at the documented minimum window size.

### World clocks

- [ ] Search accepts city/region/timezone-ID fragments.
- [ ] A selected timezone can be added.
- [ ] Duplicate timezone add is rejected with status text.
- [ ] A card can be removed.
- [ ] The last remaining card cannot be removed.
- [ ] Restart preserves the list.
- [ ] Imported duplicate IDs/timezone IDs normalize to one card per unique value.

### Focus mode

- [ ] `F11` enters full screen.
- [ ] `F11` exits full screen.
- [ ] `Esc` exits focus mode.
- [ ] Header, world clocks, add section, and footer hide during focus mode.
- [ ] A maximized window returns to maximized after leaving focus mode.

### Mini mode

- [ ] `Ctrl+M` enters a compact window.
- [ ] Mini mode is always on top.
- [ ] `Esc` exits mini mode.
- [ ] Previous window dimensions/position are restored reasonably.
- [ ] Normal always-on-top preference remains correct after leaving mini mode.

### Tray

- [ ] Tray icon appears where the OS/desktop supports tray icons.
- [ ] Show restores and activates the window.
- [ ] Focus toggles focus mode.
- [ ] Mini toggles mini mode.
- [ ] Quit exits the process.
- [ ] Closing the main window hides it when minimize-to-tray is enabled.
- [ ] Closing exits normally when minimize-to-tray is disabled.

### Settings

- [ ] Theme changes apply.
- [ ] High contrast applies without unreadable controls.
- [ ] Layout changes visibly affect the hero clock.
- [ ] Font/size/spacing values persist.
- [ ] Startup setting writes/removes the correct current-user integration.
- [ ] Import/export round-trips a settings file.
- [ ] Invalid import displays a safe error and does not replace good settings.
- [ ] Reset returns to defaults.

### About/version

- [ ] About displays `2.6.0.2` exactly.
- [ ] File/application metadata on packaged binaries reports `2.6.0.2` where the platform exposes it.

### Chime

- [ ] Disabled chime is silent.
- [ ] Enabled cadence fires once on a boundary.
- [ ] Quiet hours suppress playback.
- [ ] Clock continues normally when OS sound helpers are missing.

### Accessibility

Use the detailed checklist in `docs/accessibility.md`.

## Regression test rule

When fixing a bug:

1. reproduce it;
2. add the smallest failing automated test when the defect is below the UI/platform boundary;
3. apply the fix;
4. confirm the new test passes with the complete suite;
5. document manual reproduction/verification for UI-only defects.

## Coverage philosophy

Coverage percentage is not a release target by itself. Prefer tests that protect invariants and failure paths. A high line percentage that does not exercise timezone boundaries, persistence corruption, chime suppression, malformed import handling, or window-mode transitions is less useful than focused behavioral coverage.

## Performance testing

Clock ticks should not perform settings I/O or network I/O. If a future change adds heavy work to the tick path, capture CPU/allocation measurements and update `docs/performance.md`.
