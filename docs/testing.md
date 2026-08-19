# ChronoDesk Testing Guide

This guide explains the testing strategy, commands, and manual release boundary. For a file-by-file mapping of every xUnit/headless test, fake, and Python validator test, see `test-catalog.md`.

## Quality gates

The intended release-quality local gate is:

```bash
python3 scripts/check_markdown_links.py
python3 scripts/check_documentation_inventory.py
python3 scripts/check_repository_secrets.py
python3 -m unittest discover -s scripts/tests -p 'test_*.py'
dotnet restore ChronoDesk.sln
dotnet format ChronoDesk.sln --verify-no-changes --no-restore
dotnet build ChronoDesk.sln -c Release --no-restore
dotnet test ChronoDesk.sln -c Release --no-build --collect:"XPlat Code Coverage"
```

CI validates repository-local Markdown links, complete tracked-file documentation, committed text for high-confidence credential patterns, and the repository validator unit tests. It then executes equivalent formatting/build/test work on Ubuntu, Windows, and macOS. The .NET matrix also inspects NuGet dependencies for reported vulnerabilities.

A green automated suite is necessary but not sufficient for a release because tray, native startup execution, file pickers, real audio, window-manager behavior, external default handlers, and platform accessibility APIs require real desktop environments.

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
- invalid/null nested value repair;
- invalid/duplicate clock removal;
- at least one clock invariant;
- 24-card limit;
- bounded and flattened imported text.

### Persistence

`JsonSettingsStoreTests` uses isolated temporary directories and verifies:

- settings save/load round-trip;
- portable export/import;
- numeric enum rejection;
- malformed JSON fallback;
- corrupt-file preservation.

Tests must not read or write the developer's real ChronoDesk data folder.

### Timezone catalog

`SystemTimeZoneCatalogTests` verifies:

- system timezone discovery;
- UTC availability;
- invalid-ID fallback;
- bounded case-insensitive search.

The three-OS CI matrix helps catch differences in runtime-provided timezone databases without hard-coding a platform-specific catalog.

### Startup adapters

`PlatformStartupManagerTests` drives the production startup manager through fake filesystem and registry boundaries. The tests verify without changing the CI runner's real startup configuration:

- the Windows current-user Run entry uses a quoted executable plus `--background`;
- disabling Windows startup removes the application value;
- startup detection only accepts entries containing the ChronoDesk executable path;
- macOS LaunchAgent paths are derived from the supplied user profile and XML-sensitive executable characters are escaped;
- disabling macOS startup removes an existing LaunchAgent;
- Linux honors `XDG_CONFIG_HOME` and otherwise falls back to `~/.config/autostart`;
- Linux desktop entries quote executable paths containing spaces;
- disabling Linux startup removes an existing desktop entry;
- unsupported platforms reject startup writes;
- pre-cancelled operations honor cancellation.

These tests validate generated user-level startup artifacts, but real registry/LaunchAgent/XDG session behavior still requires native desktop verification before release.

### External-link policy

`ExternalLinkLauncherTests` verifies the application-wide external-navigation allowlist independently of an installed browser/mail client:

- absolute HTTPS project links are accepted;
- mailto support destinations are accepted;
- plain HTTP is rejected;
- local `file:` destinations are rejected;
- script-style destinations are rejected;
- relative and empty destinations are rejected.

The tests exercise URI policy only and intentionally do not launch an external process.

### Version display

`AppVersionProviderTests` verifies:

- normal preview semantic versions;
- stable semantic versions;
- prerelease versions;
- user-facing removal of `+build` metadata;
- three-part assembly-version fallback;
- the development fallback when no version metadata exists.

`HeadlessUiSmokeTests` additionally verifies the configured preview semantic version is rendered in About and Settings for an ordinary development build.

### Property-style robustness tests

`DomainPropertyTests` runs deterministic seeded randomized cases against reference invariants. It verifies thousands of quiet-hour combinations and checks that settings normalization is idempotent, bounded, and produces unique clock IDs.

This test style gives broad edge coverage while staying deterministic in CI. A failure can always be reproduced from the committed seed.

### Import fuzz tests

`SettingsImportFuzzTests` feeds a deterministic corpus of malformed binary/JSON inputs into the settings importer and verifies that the primary settings document is not changed. It also verifies rejection of files above the configured size limit.

Fuzz inputs are generated locally inside the test and never contain production/user data.

### View-model transaction tests

`MainWindowViewModelTests` verifies the critical settings/startup consistency boundary:

- startup is rolled back when settings persistence fails after an external startup change;
- the live settings snapshot does not claim a failed persistence update;
- imported settings cannot silently enable startup;
- an explicit user startup change is applied once and persisted.

### Headless Avalonia UI tests

`AvaloniaTestSetup`, `HeadlessUiSmokeTests`, and `SettingsWindowHeadlessTests` use `Avalonia.Headless.XUnit` with the same Avalonia maintenance baseline as the application. Current coverage verifies:

- main-window XAML/resource loading;
- key named controls exist;
- mini mode can enter and restore normal dimensions;
- focus mode hides/restores application chrome;
- Settings loads primary preference controls;
- Settings exposes the Updates & About surface, Releases action, About action, and current preview version;
- onboarding and About windows load localized resources;
- Settings save maps edited controls into normalized persisted preferences;
- explicit startup preference changes flow through the startup service;
- invalid quiet-hour text displays validation without persistence;
- reset-to-defaults persists defaults and reloads the visible controls.

File-picker-backed import/export remains outside the headless interaction suite because native picker behavior belongs to platform validation. Browser/mail-client launching and modal user interaction with the About dialog also remain native/UI boundaries; their underlying URI policy and resource/window loading are automated independently.

Headless UI tests strengthen cross-platform CI but do **not** replace real desktop testing for system tray, native startup registration, sound playback, accessibility APIs, display scaling, external OS handlers, or native file pickers.

## Documentation and repository integrity

### Local Markdown links

`scripts/check_markdown_links.py` scans repository Markdown without network access and validates repository-local link/image targets. Links that escape the repository or point at missing files fail the dedicated CI Repository integrity job.

External URLs are deliberately excluded from deterministic availability checking; release review should still verify important project/support destinations when preparing a tag.

### Tracked-file documentation coverage

`scripts/check_documentation_inventory.py` obtains the authoritative tracked paths from `git ls-files` and compares them with the canonical inventory entries in `docs/repository-reference.md`.

It fails when:

- a tracked file has no reference entry;
- an inventory entry points at a file that is no longer tracked.

Fenced syntax examples inside the reference are ignored so documentation can explain the inventory format without creating a fake path.

This gate means source, tests, assets, XAML, resources, workflows, templates, scripts, and documentation files cannot be added silently without at least an explicit responsibility entry.

### Credential-pattern scan

`scripts/check_repository_secrets.py` scans committed text for high-confidence private-key and common credential/token patterns without printing matched secret values. It is an automated tripwire rather than proof that every possible private datum is absent.

### Repository validator tests

Run all standard-library validator tests with:

```bash
python3 -m unittest discover -s scripts/tests -p 'test_*.py'
```

Current tests protect release metadata rules and documentation-inventory parser/comparison behavior. Add validator tests when a repository script gains nontrivial parsing or policy logic.

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
- [ ] A selected timezone can be added.
- [ ] Duplicate timezone add is rejected with status text.
- [ ] A card can be removed.
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
- [ ] Updates & About shows the same release version as the About dialog.
- [ ] Open GitHub Releases launches the expected public HTTPS page only after the button is activated.
- [ ] A missing/default browser handler fails safely and leaves the app usable.
- [ ] Open About displays the full About dialog from Settings.

### Chime

- [ ] Disabled chime is silent.
- [ ] Enabled cadence fires once on a boundary.
- [ ] Quiet hours suppress playback.
- [ ] Clock continues normally when OS sound helpers are missing.

### Accessibility

Use the detailed checklist in `docs/accessibility.md`. Pay particular attention to the Settings clock/appearance controls whose visual labels are represented explicitly through automation names for screen-reader users.

## Regression test rule

When fixing a bug:

1. reproduce it;
2. add the smallest failing automated test when the defect is below the UI/platform boundary;
3. apply the fix;
4. confirm the new test passes with the complete suite;
5. document manual reproduction/verification for UI-only defects.

## Coverage philosophy

Coverage percentage is not a release target by itself. Prefer tests that protect invariants and failure paths. A high line percentage that does not exercise timezone boundaries, persistence corruption, chime suppression, malformed import handling, startup artifact generation, external-link policy, release version identity, documentation completeness, or window-mode transitions is less useful than focused behavioral coverage.

## Performance testing

Clock ticks should not perform settings I/O or network I/O. The Updates & About feature must remain user-initiated; it must not turn the clock tick or application startup into a release/network polling path. If a future change adds heavy work to the tick path, capture CPU/allocation measurements and update `docs/performance.md`.
