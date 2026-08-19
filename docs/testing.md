# ChronoDesk Testing Guide

## Quality gates

The intended release gate is:

```bash
dotnet restore ChronoDesk.sln
dotnet format ChronoDesk.sln --verify-no-changes --no-restore
dotnet build ChronoDesk.sln -c Release --no-restore
dotnet test ChronoDesk.sln -c Release --no-build --collect:"XPlat Code Coverage"
```

CI executes equivalent formatting/build/test work on Ubuntu, Windows, and macOS and also inspects NuGet dependencies for reported vulnerabilities.

## Automated test areas

### Clock formatting

`ClockFormatterTests` verifies:

- 24-hour time with seconds;
- 12-hour time without seconds;
- ISO week number;
- UTC offset/calendar detail rendering.

Use explicit `DateTimeOffset`, timezone, and culture inputs so the tests do not depend on the machine's current date/time.

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
- 24-card limit.

### Persistence

`JsonSettingsStoreTests` uses isolated temporary directories and verifies:

- settings save/load round-trip;
- portable export/import;
- malformed JSON fallback;
- corrupt-file preservation.

Tests must not read or write the developer's real ChronoDesk data folder.

### Timezone catalog

`SystemTimeZoneCatalogTests` verifies:

- system timezone discovery;
- UTC availability;
- invalid-ID fallback;
- bounded case-insensitive search.

## Manual UI checklist

Automated Core tests are not a substitute for desktop validation. Before a release, test each supported primary platform.

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

Coverage percentage is not a release target by itself. Prefer tests that protect invariants and failure paths. A high line percentage that does not exercise timezone boundaries, persistence corruption, or chime suppression is less useful than focused behavioral coverage.

## Future UI automation

Headless Avalonia smoke tests are a roadmap item. They should be introduced only with a package/tool version that is confirmed compatible with the repository's pinned Avalonia baseline and must not replace real platform checks for tray/startup/chime behavior.

## Performance testing

Clock ticks should not perform settings I/O or network I/O. If a future change adds heavy work to the tick path, capture CPU/allocation measurements and update `docs/performance.md`.
