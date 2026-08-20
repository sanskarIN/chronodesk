# ChronoDesk Testing Guide

## Quality gates

ChronoDesk contains workload-specific platform projects, so release validation is intentionally split by host rather than forcing every machine to restore/build `ChronoDesk.sln`.

### Shared and desktop gate

```bash
dotnet restore src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj
dotnet restore tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj
dotnet format src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj --verify-no-changes --no-restore
dotnet format tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj --verify-no-changes --no-restore
dotnet build src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj -c Release --no-restore
dotnet test tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj -c Release --no-restore --collect:"XPlat Code Coverage"
```

Repository checks:

```powershell
./scripts/check-version.ps1
./scripts/check-markdown-links.ps1
```

Tagged candidate:

```powershell
./scripts/check-version.ps1 -Tag "v2.6.0.2"
```

### Android gate

```bash
dotnet workload install android
dotnet restore src/ChronoDesk.Android/ChronoDesk.Android.csproj
dotnet build src/ChronoDesk.Android/ChronoDesk.Android.csproj -c Release --no-restore
```

### iOS/iPadOS gate

Run on macOS with compatible Xcode:

```bash
dotnet workload install ios
dotnet restore src/ChronoDesk.iOS/ChronoDesk.iOS.csproj
dotnet build src/ChronoDesk.iOS/ChronoDesk.iOS.csproj -c Release --no-restore
```

CI selects an iOS simulator runtime identifier based on the runner architecture.

### Browser gate

```bash
dotnet workload install wasm-tools
dotnet restore src/ChronoDesk.Browser/ChronoDesk.Browser.csproj
dotnet build src/ChronoDesk.Browser/ChronoDesk.Browser.csproj -c Release --no-restore
```

## CI matrix

`.github/workflows/ci.yml` validates:

| Job | Runner | Purpose |
|---|---|---|
| Desktop | Ubuntu, Windows, macOS | Version/docs/format/build/tests/vulnerability inspection. |
| Android | Ubuntu | Install Android workload and compile Android host. |
| iOS/iPadOS | macOS | Install iOS workload and compile simulator host. |
| Browser | Ubuntu | Install `wasm-tools` and compile WebAssembly host. |

CodeQL and Dependency Review remain separate pull-request security gates.

A successful desktop test run alone is not enough to claim a cross-platform release; all applicable host-build checks must pass for the exact release commit.

## Automated test areas

### Clock formatting

`ClockFormatterTests` verifies:

- 24-hour time with seconds;
- 12-hour time without seconds;
- ISO week number;
- UTC offset/calendar detail rendering.

Use explicit `DateTimeOffset`, timezone, and culture inputs so tests do not depend on the current wall clock.

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

Cadence behavior must remain independent of actual native sound playback so it is testable on every host.

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
- normal loading resumes after a transient lock is released.

Tests must never read or write the developer's real ChronoDesk data folder.

Browser persistence is a different runtime boundary: WebAssembly uses a browser sandbox/virtual filesystem. Native temporary-directory tests validate the serializer/store contract, while browser-host CI validates compilation. Browser persistence lifetime must also be manually verified on the intended deployment host.

### View-model reliability

`MainWindowViewModelTests` verifies:

- explicit startup changes are applied once;
- startup integration is rolled back when persistence fails;
- portable imports cannot silently change machine startup preference;
- unreadable settings fall back to defaults while clock/world clocks/timezone search still initialize.

Platform-specific startup is kept behind `IStartupManager`; mobile/browser hosts must not assume it is supported.

### Timezone catalog

`SystemTimeZoneCatalogTests` verifies:

- system timezone discovery;
- UTC availability;
- invalid-ID fallback;
- bounded case-insensitive search.

### Property-style robustness tests

`DomainPropertyTests` runs deterministic seeded randomized cases against reference invariants. It verifies thousands of quiet-hour combinations and checks settings normalization idempotence, bounds, and unique clock IDs.

### Import fuzz tests

`SettingsImportFuzzTests` feeds a deterministic corpus of malformed binary/JSON inputs into the settings importer and verifies that the primary settings document is not changed. It also verifies rejection of inputs above the configured size limit.

### Headless Avalonia UI smoke tests

`AvaloniaTestSetup` and `HeadlessUiSmokeTests` use `Avalonia.Headless.XUnit` with the application Avalonia baseline.

Coverage verifies:

- desktop `MainWindow` XAML/resource loading;
- key named controls;
- `MainView` single-view XAML/resource loading for mobile/browser reuse;
- `MainView` keeps the supplied shared view model as its data context;
- mini mode can enter and restore normal dimensions;
- focus mode hides/restores desktop application chrome;
- focus mode restores prior window state;
- Settings loads primary preference controls;
- onboarding and About load localized resources;
- About displays complete canonical version `2.6.0.2`.

Headless tests strengthen shared UI validation but do **not** replace native/emulator/device/browser testing for tray, startup, sound, accessibility APIs, display scaling, touch, orientations, store packaging, or browser hosting.

## Version verification

`scripts/check-version.ps1` enforces:

- four-component canonical `Version` (`MAJOR.MINOR.PATCH.REVISION`);
- matching shared `PackageVersion`, `AssemblyVersion`, and `FileVersion`;
- matching desktop package/assembly/file version fields;
- Android display version equality and a positive numeric version code;
- iOS three-component marketing-version mapping plus positive build number;
- no conflicting shared `VersionPrefix`/`VersionSuffix` values;
- assembly component bounds;
- exact `v<canonical-version>` tag equality when `-Tag` is supplied.

Current canonical version: `2.6.0.2`.

Apple package mapping: marketing version `2.6.0`, build `2602`.

## Documentation-link verification

`scripts/check-markdown-links.ps1` recursively validates repository-local Markdown file/directory destinations. It intentionally ignores external URLs and same-document anchors so transient network failures cannot make this offline repository check nondeterministic.

## Manual cross-platform checklist

### Shared clock/world-clock behavior

Test on representative desktop, Android, Apple, and browser hosts:

- [ ] Clock starts without an account/network dependency.
- [ ] 12/24-hour toggle works.
- [ ] Seconds toggle works.
- [ ] Date/timezone display is correct for the host.
- [ ] Timezone search returns usable results.
- [ ] Selected timezone can be added.
- [ ] Duplicate add is handled safely.
- [ ] World-clock card can be removed without violating the minimum-card invariant.
- [ ] Narrow/small displays remain usable.
- [ ] Landscape/portrait transitions remain usable where applicable.

### Desktop launch/onboarding

- [ ] Fresh data directory opens onboarding.
- [ ] Onboarding explains offline/private behavior accurately.
- [ ] Completing onboarding persists.
- [ ] Temporary settings-file failures do not destroy a valid file.

### Desktop focus mode

- [ ] `F11` enters/exits full screen.
- [ ] `Esc` exits focus mode.
- [ ] Header/world-clock/add/footer sections hide while focused.
- [ ] Previous maximized/normal state restores correctly.

### Desktop mini mode

- [ ] `Ctrl+M` enters/exits compact mode.
- [ ] Mini mode is always on top.
- [ ] Previous dimensions/position restore reasonably.
- [ ] Normal always-on-top preference remains correct afterward.

### Desktop tray/startup

- [ ] Tray icon appears where supported.
- [ ] Show/Focus/Mini/Quit actions work.
- [ ] Minimize-to-tray behavior follows settings.
- [ ] Startup preference creates/removes only the current-user integration.
- [ ] Unsupported/non-desktop platforms never attempt desktop startup registration.

### Desktop settings

- [ ] Theme/high-contrast/layout changes apply.
- [ ] Font/size/spacing persist.
- [ ] Import/export round-trips.
- [ ] Invalid import cannot replace good settings.
- [ ] Reset returns to defaults.

### Android

- [ ] Debug app installs/launches on at least one emulator/device matching supported API expectations.
- [ ] Single-view clock fills the activity correctly.
- [ ] Back/resume/reopen lifecycle does not create duplicate timers.
- [ ] Portrait/landscape remains usable.
- [ ] No desktop tray/startup/window API is invoked.
- [ ] Release package can be signed through the protected maintainer signing process.

### iOS/iPadOS

- [ ] Simulator launch succeeds.
- [ ] iPhone portrait/landscape layouts remain usable.
- [ ] iPad portrait/landscape layouts remain usable.
- [ ] App lifecycle resume does not duplicate timers.
- [ ] Package metadata uses marketing `2.6.0`, build `2602`.
- [ ] Device/App Store signing is performed only with protected credentials.

### Browser/WebAssembly

- [ ] Published `wwwroot` loads over HTTP(S).
- [ ] Browser console shows no startup/runtime errors.
- [ ] Narrow and wide responsive layouts remain usable.
- [ ] Reload behavior matches documented browser storage expectations.
- [ ] No `file://`, unrestricted filesystem, registry, or external-process assumption exists.
- [ ] Keyboard focus/automation semantics remain usable.

### About/version

- [ ] About displays `2.6.0.2` exactly on shared UI.
- [ ] Desktop package metadata reports `2.6.0.2` where exposed.
- [ ] Android version name reports `2.6.0.2`.
- [ ] Apple package mapping is `2.6.0` / build `2602`.

### Chime

Desktop:

- [ ] Disabled chime is silent.
- [ ] Enabled cadence fires once on a boundary.
- [ ] Quiet hours suppress playback.
- [ ] Clock continues when native sound helpers are missing.

Mobile/browser:

- [ ] Unsupported native chime path safely no-ops and never stops the clock.

### Accessibility

Use the detailed checklist in `docs/accessibility.md`, and add phone/tablet/browser touch and scaling checks for the single-view shell.

## Regression rule

When fixing a bug:

1. reproduce it;
2. add the smallest failing automated test when below the UI/platform boundary;
3. apply the fix;
4. run relevant shared tests;
5. run the affected platform-host build;
6. document native/manual reproduction for UI/device/browser-only defects.

## Coverage philosophy

Coverage percentage is not a release target by itself. Prefer tests that protect invariants and failure paths. Cross-platform host compilation plus focused behavior tests are more meaningful than a high line percentage that ignores timezone, persistence, lifecycle, and unsupported-capability boundaries.

## Performance testing

Clock ticks must not perform network I/O or routine settings writes. If a future change adds work to the tick path, capture CPU/allocation measurements and update `docs/performance.md`. On mobile/browser, also test lifecycle suspension/detachment so timers do not run unnecessarily after the view is detached.
