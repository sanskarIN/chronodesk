# ChronoDesk Testing Guide

## Quality gates

The intended release gate is:

```bash
pwsh ./scripts/verify-doc-links.ps1
pwsh ./scripts/verify-no-secrets.ps1
dotnet restore ChronoDesk.sln
dotnet format ChronoDesk.sln --verify-no-changes --no-restore
dotnet build ChronoDesk.sln -c Release --no-restore
dotnet test ChronoDesk.sln -c Release --no-build --collect:"XPlat Code Coverage"
```

CI executes equivalent documentation, tracked-secret, formatting, build, test, and NuGet vulnerability work on Ubuntu, Windows, and macOS.

## Repository verification scripts

### Local documentation links

`scripts/verify-doc-links.ps1` scans tracked project Markdown locations for relative links and image targets. It ignores external schemes and same-document anchors, resolves repository-relative and file-relative paths, and fails when a referenced local target does not exist.

This is intentionally a local-target verifier rather than a crawler. Network URLs can fail transiently or require authentication and should be reviewed separately when preparing a release.

### High-signal tracked-file secret scan

`scripts/verify-no-secrets.ps1` enumerates Git-tracked files and scans text-sized content for a deliberately small set of high-signal credential patterns including private-key headers and common token families.

The scanner:

- skips known binary/archive extensions and files larger than its inspection limit;
- scans tracked files rather than unrelated developer files;
- excludes its own pattern-definition source so the detector does not report itself;
- reports only the file and rule name;
- intentionally never echoes the matched value.

This supplements CodeQL, dependency review, GitHub security features, and human review. It is not a universal secret detector.

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
- invalid/duplicate clock removal;
- at least one clock invariant;
- the world-clock list is bounded by `AppSettings.MaximumWorldClockCount`.

### Persistence and schema migration

`JsonSettingsStoreTests` uses isolated temporary directories and verifies:

- settings save/load round-trip;
- portable export/import;
- malformed JSON fallback;
- corrupt-file preservation;
- legacy documents without `schemaVersion` are explicitly treated as schema `0` and migrated to the current schema;
- explicit schema `0` documents migrate to the current schema while preserving compatible preferences;
- negative schema versions are rejected;
- future/unsupported schema versions are rejected;
- numeric enum representations are rejected.

`SettingsMigrationPipeline` advances supported documents one schema version at a time before normal settings normalization. The current `0 -> 1` migration is data-preserving because pre-versioned development files used the same field semantics. Future schema changes must add an explicit step and regression coverage rather than silently reinterpreting old JSON.

Tests must not read or write the developer's real ChronoDesk data folder.

### Timezone catalog

`SystemTimeZoneCatalogTests` verifies:

- system timezone discovery;
- UTC availability;
- invalid-ID fallback;
- bounded case-insensitive search.

### Startup registration documents

`StartupRegistrationDocumentsTests` validates the pure platform-registration builders without modifying a real user startup location. It verifies:

- Windows Run-key command quoting and the `--background` argument;
- rejection of unsafe embedded Windows quote characters;
- valid macOS LaunchAgent XML generation;
- XML escaping for macOS executable paths;
- Linux XDG desktop-entry generation;
- Linux executable escaping for spaces, backslashes, dollar signs, and backticks;
- rejection of control characters in executable paths;
- normalization of harmless outer whitespace.

`PlatformStartupManager` uses these same builders when it reads/writes OS startup registration. This keeps generated content deterministic and allows most string/escaping logic to be tested without touching the registry, `~/Library/LaunchAgents`, or the XDG autostart directory.

Real platform startup enable/disable remains a manual release gate because registry permissions, LaunchServices/session behavior, Linux desktop-environment behavior, and filesystem permissions cannot be faithfully reproduced by pure unit tests.

### Main-window orchestration

`MainWindowViewModelTests` verifies behavior below the native-window boundary, including:

- startup integration rollback when persistence fails;
- imported settings preserving the current device startup preference;
- explicit startup preference changes being applied once;
- world-clock additions being rejected before persistence when the dashboard is at capacity;
- world-clock removal and restoration at the original dashboard position;
- timezone-search empty and populated feedback states.

### Tray visibility safety

`TrayVisibilityPolicyTests` verifies that ChronoDesk hides its main window only when all required conditions are true.

Close-to-tray requires:

- the close was not an explicit application quit;
- minimize-to-tray is enabled;
- reliable tray menu restoration is available for the current desktop session.

Background startup hiding similarly requires the `--background` request, the preference, and reliable tray restoration. If tray integration is unavailable, the app remains visible/closeable rather than becoming an unreachable hidden process.

Actual tray availability is still a native-desktop release check because a headless test runner cannot prove the operating-system tray/menu implementation exists.

### External links

`ExternalUriLauncherTests` verifies that the application-level launcher policy accepts HTTPS and mail destinations used by ChronoDesk while rejecting insecure HTTP, local-file, script, relative, and credential-bearing HTTPS destinations.

The tests do not launch a real browser or mail client.

### Property-style robustness tests

`DomainPropertyTests` runs deterministic seeded randomized cases against reference invariants. It verifies thousands of quiet-hour combinations and checks that settings normalization is idempotent, bounded, and produces unique clock IDs.

This test style gives broad edge coverage while staying deterministic in CI. A failure can always be reproduced from the committed seed.

### Import fuzz tests

`SettingsImportFuzzTests` feeds a deterministic corpus of malformed binary/JSON inputs into the settings importer and verifies that the primary settings document is not changed. It also verifies rejection of files above the configured size limit.

Fuzz inputs are generated locally inside the test and never contain production/user data.

### Headless Avalonia UI smoke tests

`AvaloniaTestSetup` and `HeadlessUiSmokeTests` use `Avalonia.Headless.XUnit` with the same Avalonia 11.3 maintenance baseline as the application. Current smoke coverage verifies:

- main-window XAML/resource loading;
- key clock, timezone search, search-feedback, and undo controls exist;
- mini mode can enter and restore normal dimensions;
- leaving mini mode follows the current saved always-on-top preference rather than stale pre-mini state;
- focus mode hides/restores application chrome;
- Settings loads primary preference controls;
- onboarding and About windows load localized resources.

Headless UI tests strengthen cross-platform CI but do **not** replace real desktop testing for system tray, startup registration, sound playback, accessibility APIs, display scaling, or native file pickers.

## Manual UI checklist

Automated Core/headless tests are not a substitute for desktop validation. Before a tagged release, test each supported primary platform.

### Launch/onboarding

- [ ] Fresh data directory opens onboarding.
- [ ] Onboarding explains offline/private behavior accurately.
- [ ] Completing onboarding persists and it does not reappear next launch.

### Main clock

- [ ] Time updates smoothly without visible duplicate/reordered text.
- [ ] 12/24-hour toggle is immediate.
- [ ] Seconds toggle is immediate.
- [ ] Date/weekday/week/calendar settings match visible output.
- [ ] Large font sizes remain usable at the documented minimum window size.

### World clocks

- [ ] Search accepts city/region/timezone-ID fragments.
- [ ] Search result count changes with the query and an empty search state is visible when there are no matches.
- [ ] A selected timezone can be added.
- [ ] Duplicate timezone add is rejected with status text.
- [ ] The capacity limit rejects an additional clock with explicit feedback rather than reporting a false success.
- [ ] A card can be removed.
- [ ] Undo restores the most recently removed card at the expected position.
- [ ] The last remaining card cannot be removed.
- [ ] Restart preserves the list.

### Focus mode

- [ ] `F11` enters full screen.
- [ ] `F11` exits full screen.
- [ ] `Esc` exits focus mode.
- [ ] Header, world clocks, add section, and footer hide during focus mode.

### Mini mode

- [ ] `Ctrl+M` enters a compact window.
- [ ] Mini mode is always on top.
- [ ] `Esc` exits mini mode.
- [ ] Previous window dimensions/position are restored reasonably.
- [ ] Normal always-on-top preference remains correct after leaving mini mode, including if the preference changed while mini mode was active.

### Tray

- [ ] Tray icon/menu appears where the OS/desktop supports reliable tray restoration.
- [ ] Show restores and activates the window.
- [ ] Focus toggles focus mode.
- [ ] Mini toggles mini mode.
- [ ] Quit exits the process.
- [ ] Closing the main window hides it only when minimize-to-tray is enabled and reliable tray restoration is available.
- [ ] Closing exits normally when minimize-to-tray is disabled.
- [ ] On a desktop without reliable tray integration, closing never leaves an unreachable hidden ChronoDesk process.
- [ ] `--background` startup remains visible when tray restoration is unavailable.

### Settings

- [ ] Theme changes apply.
- [ ] High contrast applies without unreadable controls.
- [ ] Layout changes visibly affect the hero clock.
- [ ] Font/size/spacing values persist.
- [ ] Startup setting writes/removes the correct current-user integration.
- [ ] Import/export round-trips a settings file.
- [ ] Invalid import displays a safe error and does not replace good settings.
- [ ] Reset returns to defaults.

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

Coverage percentage is not a release target by itself. Prefer tests that protect invariants and failure paths. A high line percentage that does not exercise timezone boundaries, persistence corruption, schema migration, chime suppression, malformed import handling, startup document generation, tray visibility safety, URI policy, or window-mode transitions is less useful than focused behavioral coverage.

## Performance testing

Clock ticks should not perform settings I/O or network I/O. If a future change adds heavy work to the tick path, capture CPU/allocation measurements and update `docs/performance.md`.
