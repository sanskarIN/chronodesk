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

`ClockFormatterTests` verifies 24-hour/12-hour output, seconds behavior, ISO week number, and UTC offset/calendar detail rendering using explicit deterministic time/culture inputs.

### Quiet hours and chime policy

`QuietHoursTests` and `ChimePolicyTests` verify overnight intervals, inclusive start/exclusive end behavior, disabled/equal ranges, hourly/quarter-hour cadence, duplicate suppression, and quiet-hour suppression independently of actual sound playback.

### Settings model

`AppSettingsTests` verifies:

- visual range normalization;
- default font fallback;
- invalid clock removal;
- duplicate clock-ID removal;
- duplicate timezone-ID removal case-insensitively;
- at least one clock invariant;
- the world-clock list is bounded by `AppSettings.MaximumWorldClockCount`;
- runtime-null/invalid enum repair;
- bounded single-line imported text.

### Persistence and schema migration

`JsonSettingsStoreTests` uses isolated temporary directories and verifies:

- settings save/load round-trip;
- portable export/import;
- malformed JSON fallback;
- corrupt-file preservation;
- repeated corruption creates distinct recovery files;
- JSON root must be an object;
- legacy documents without `schemaVersion` are treated as schema `0` and migrated;
- explicit schema `0` migration;
- negative/future schema rejection;
- numeric enum rejection.

The store checks the 2 MiB maximum using the opened stream before parsing, avoiding dependence on an earlier metadata-only size observation. Corrupt recovery names include timestamp precision plus a random suffix so rapid repeated failures do not collide.

`SettingsMigrationPipeline` advances supported documents one schema version at a time before normal settings normalization. The current `0 -> 1` migration is data-preserving because pre-versioned development files used the same field semantics.

### Timezone catalog

`SystemTimeZoneCatalogTests` verifies system timezone discovery, UTC availability, invalid-ID fallback, and bounded case-insensitive search.

### Startup registration documents

`StartupRegistrationDocumentsTests` validates pure platform-registration builders without modifying real user startup state. It covers Windows Run-command quoting/background arguments, Windows embedded-quote rejection, macOS LaunchAgent XML and escaping, Linux desktop-entry escaping, control-character rejection, and harmless outer-whitespace normalization.

### Main-window orchestration and startup consistency

`MainWindowViewModelTests` verifies:

- startup rollback when persistence fails;
- imported settings preserve the current device startup preference;
- explicit startup preference changes apply once;
- world-clock additions are rejected before persistence at capacity;
- world-clock remove/undo restores original order;
- timezone-search empty/populated feedback.

`StartupRollbackCancellationTests` verifies a more specific failure path: when a settings save cancels its caller token after startup integration was already changed, rollback still restores the previous startup state using a separate non-cancelled operation.

### Loading and localized count state

`MainWindowStateTests` verifies:

- the view model begins in an explicit localized local-loading state;
- `IsLoading`/`IsInitialized` transition correctly;
- successful initialization changes status to ready;
- singular/plural world-clock count text comes from dedicated localization resources rather than modifying an English heading.

### Tray visibility safety

`TrayVisibilityPolicyTests` verifies close/background hiding occurs only when the relevant preference/request is active **and** reliable tray restoration exists. If tray integration is unavailable, the app remains visible/closeable rather than becoming an unreachable hidden process.

Actual tray availability remains a native-desktop release check.

### External links

`ExternalUriLauncherTests` verifies the application-level launcher accepts the approved HTTPS/mail destinations while rejecting insecure HTTP, local-file, script, relative, empty, and credential-bearing HTTPS destinations. Tests do not launch a real browser/mail client.

### Version and update metadata

`AppVersionInfoTests` verifies application display-version generation uses informational version metadata when available, preserves prerelease labels, and strips build metadata after `+`.

`HeadlessUiSmokeTests` verifies Settings exposes the offline-safe Updates controls, displays the same application version, and loads the Settings About version field.

The update UI itself does not perform background network requests; opening the official Releases page requires explicit user activation and native browser handling, which is a manual desktop check.

### Theme palette selection

`ThemePaletteSelectorTests` verifies:

- System mode follows Avalonia's actual dark variant;
- System mode follows the actual light variant;
- explicit Light overrides a dark actual system variant;
- high-contrast preference overrides normal theme selection.

The `App` subscribes to `ActualThemeVariantChanged`, so real desktop release validation must also toggle the OS theme while ChronoDesk is running and confirm the custom palette follows it.

### Property-style robustness tests

`DomainPropertyTests` runs deterministic seeded randomized cases against reference invariants for quiet hours and settings normalization. Failures remain reproducible from the committed seed.

### Import fuzz tests

`SettingsImportFuzzTests` feeds a deterministic malformed binary/JSON corpus into the importer, verifies the primary settings document is not changed, and checks oversized input rejection.

### Headless Avalonia UI smoke tests

`AvaloniaTestSetup` and `HeadlessUiSmokeTests` use `Avalonia.Headless.XUnit` with the same Avalonia maintenance baseline as the app. Current smoke coverage verifies:

- main-window XAML/resource loading;
- clock/search/search-feedback/undo controls;
- mini-mode dimension round trip;
- current always-on-top preference after mini exit;
- focus mode hides/restores application chrome;
- primary Settings controls;
- Settings Updates controls/version;
- Settings About version;
- onboarding and standalone About windows load localized resources.

Headless UI tests do **not** replace real desktop testing for tray, startup registration, sound playback, external URI handlers, system-theme switching, accessibility APIs, display scaling, or native file pickers.

## Manual UI checklist

Before a tagged release, test each supported primary platform.

### Launch/onboarding

- [ ] Fresh data directory opens onboarding.
- [ ] A visible local-loading state appears naturally without fake delay when initialization is observable.
- [ ] Onboarding explains offline/private behavior accurately.
- [ ] Completing onboarding persists and it does not reappear next launch.

### Main clock

- [ ] Time updates smoothly without duplicate/reordered text.
- [ ] 12/24-hour and seconds toggles are immediate.
- [ ] Date/weekday/week/calendar output matches settings.
- [ ] Large clock sizes remain usable at documented minimum window size.

### World clocks

- [ ] Search accepts city/region/timezone-ID fragments.
- [ ] Result count changes and no-results state is visible.
- [ ] A selected timezone can be added.
- [ ] Duplicate timezone add is rejected.
- [ ] Capacity rejects another clock with explicit feedback.
- [ ] Remove + Undo restores the expected card position.
- [ ] The last remaining card cannot be removed.
- [ ] Restart preserves the normalized list.

### Themes and appearance

- [ ] Light, Dark, System, and High Contrast are readable.
- [ ] In System mode, switch the OS between light/dark while ChronoDesk is running and verify the custom palette updates.
- [ ] Explicit Light/Dark does not unexpectedly follow later OS theme changes.
- [ ] Layout, font, size, spacing persist.

### Focus and mini

- [ ] `F11` enters/exits focus and `Esc` exits focus.
- [ ] `Ctrl+M` enters/exits mini mode.
- [ ] Mini mode is always topmost.
- [ ] Normal dimensions/position restore reasonably.
- [ ] Current normal always-on-top preference is correct after mini mode.

### Tray

- [ ] Tray icon/menu appears where reliable tray restoration is supported.
- [ ] Show/Focus/Mini/Quit work.
- [ ] Close hides only when minimize-to-tray and reliable tray restoration are both active.
- [ ] Tray-unavailable desktop does not leave an unreachable hidden process.
- [ ] `--background` remains visible when reliable tray restoration is unavailable.

### Settings / data

- [ ] Startup setting writes/removes the correct current-user integration.
- [ ] Import/export round-trips a settings file.
- [ ] Invalid import displays a safe error and does not replace good settings.
- [ ] Reset returns to defaults.
- [ ] Updates tab displays the same prerelease version as About when applicable.
- [ ] Opening official releases happens only after clicking the button.
- [ ] Updates tab does not trigger observable background network activity while idle.
- [ ] Settings About shows project/license/version/credit/support/funding content.
- [ ] External-link failure leaves Settings usable and shows safe status text.

### Chime

- [ ] Disabled chime is silent.
- [ ] Enabled cadence fires once on a boundary.
- [ ] Quiet hours suppress playback.
- [ ] Clock continues normally when optional sound helpers are absent/fail.

### Accessibility

Use `docs/accessibility.md`.

## Regression test rule

When fixing a bug:

1. reproduce it;
2. add the smallest failing automated test when the defect is below the UI/platform boundary;
3. apply the fix;
4. confirm the new test passes with the complete suite;
5. document manual reproduction/verification for UI-only defects.

## Coverage philosophy

Coverage percentage is not a release target by itself. Prefer tests that protect invariants and failure paths. Focused coverage of timezone boundaries, persistence corruption, migration, cancellation rollback, chime suppression, malformed imports, startup generation, tray safety, update/privacy behavior, theme selection, URI policy, and window modes matters more than a cosmetic line percentage.

## Performance testing

Clock ticks must not perform settings I/O or network I/O. The Updates section must not add background polling. If a future change adds heavy tick/startup work, capture measurements and update `docs/performance.md`.
