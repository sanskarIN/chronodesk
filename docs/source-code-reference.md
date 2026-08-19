# ChronoDesk Production Source-Code Reference

This document is the maintainer-facing reference for the production code under `src/`. It explains namespaces, types, major methods/properties, dependency direction, invariants, and extension points. It complements `repository-reference.md` (every tracked file), `architecture.md` (system design), and `runtime-behavior.md` (execution flow).

## Dependency map

```text
ChronoDesk.Core
  ├─ models
  ├─ service contracts
  └─ pure policies/formatting

ChronoDesk.Infrastructure ──> ChronoDesk.Core
  ├─ settings/filesystem
  ├─ timezone database
  ├─ startup integration
  ├─ chime playback
  └─ local logging

ChronoDesk.App ──> ChronoDesk.Core + ChronoDesk.Infrastructure
  ├─ composition/lifecycle
  ├─ resources/themes
  ├─ view models
  └─ Avalonia views
```

Tests reference all three production projects. Production projects never reference the test project.

## ChronoDesk.Core

Namespace root: `ChronoDesk.Core`.

Core contains product contracts and deterministic behavior. It must not depend on Avalonia, Registry APIs, process launching, file I/O implementations, or other platform implementation details.

### Abstractions

#### `IAppLogger`

Namespace: `ChronoDesk.Core.Abstractions`.

Contract:

```csharp
void Info(string eventName, string message);
void Warning(string eventName, string message);
void Error(string eventName, Exception exception, string safeMessage);
```

Intent:

- keep callers independent of the JSONL file logger;
- encourage stable event names plus caller-selected safe messages;
- distinguish normal information, recoverable warnings, and exceptions.

Security rule: passing an exception does not give callers permission to log arbitrary exception text as the human message. Production `SafeFileLogger` intentionally stores safe metadata rather than blindly serializing exception content.

Extension point: alternate log sinks can implement this contract without changing Core/App orchestration.

#### `IChimePlayer`

Contract:

```csharp
Task PlayAsync(CancellationToken cancellationToken = default);
```

Intent: isolate **how** a sound is produced from `ChimePolicy`, which decides **whether** it should occur.

The interface deliberately does not accept an arbitrary path, command, URI, or user string. Current production playback is fixed/best-effort.

#### `ISettingsStore`

Key members:

```csharp
string SettingsPath { get; }
Task<AppSettings> LoadAsync(...);
Task SaveAsync(AppSettings settings, ...);
Task ExportAsync(AppSettings settings, string destinationPath, ...);
Task<AppSettings> ImportAsync(string sourcePath, ...);
```

Intent:

- give the view model one persistent-settings boundary;
- separate ordinary app settings path from user-selected portable import/export paths;
- keep validation/atomic persistence outside UI code.

The production implementation is `JsonSettingsStore`.

#### `IStartupManager`

Key members:

```csharp
bool IsSupported { get; }
Task<bool> IsEnabledAsync(...);
Task SetEnabledAsync(bool enabled, ...);
```

Intent: present one user-scoped startup preference to App code while hiding Windows Registry/macOS LaunchAgent/Linux XDG details.

`IsSupported` lets UI/orchestration distinguish a platform with no supported startup implementation from an ordinary disabled state.

#### `ITimeZoneCatalog`

Key members:

```csharp
IReadOnlyList<TimeZoneDescriptor> GetAll();
TimeZoneInfo Resolve(string timeZoneId);
IReadOnlyList<TimeZoneDescriptor> Search(string query, int limit = 50);
```

Intent:

- isolate operating-system timezone enumeration/search;
- keep UI independent of platform-specific timezone-ID conventions;
- provide a safe `Resolve` contract with production UTC fallback.

### Models

#### `AppSettings`

Central immutable settings record.

Responsibilities:

- own current schema version/default values;
- represent clock, appearance, accessibility, behavior, chime, and world-clock persistence;
- normalize untrusted/deserialized/in-memory values before use;
- repair invalid enums/null nested objects;
- bound user-controlled strings and numeric values;
- enforce at least one and at most 24 valid world clocks;
- de-duplicate clock IDs.

Important design rule: `Normalize()` is expected to be idempotent. Property-style tests protect this.

When extending `AppSettings`, update:

- normalization/defaults;
- settings UI mapping where applicable;
- persistence/import tests;
- `settings-reference.md`;
- schema migration/version logic when required.

#### `ChimeSettings`

Contains:

- `Enabled`;
- `Interval`;
- `QuietHours`.

Default behavior is non-intrusive: chimes disabled, hourly cadence selected as a dormant default, quiet hours present but disabled.

#### `ClockFormat`

Stable storage/application enum for:

- `TwelveHour`;
- `TwentyFourHour`.

Do not rename serialized enum values casually; settings use camel-case string enum serialization.

#### `ClockLayout`

Stable presentation preference enum for:

- centered;
- compact;
- dashboard.

The enum is persisted in Core, while actual visual interpretation belongs to App.

#### `ClockSnapshot`

Immutable output object produced by `ClockFormatter`.

It carries already-computed clock display values plus converted time/zone context so presentation code does not need to duplicate timezone/culture formatting rules.

Typical fields include time/date/weekday/week/calendar/zone output and the converted local instant used for policy decisions.

#### `QuietHours`

Value object containing:

- enabled flag;
- start `TimeOnly`;
- end `TimeOnly`.

Its predicate defines the quiet interval semantics:

- disabled → never quiet;
- start == end → no quiet interval;
- start < end → same-day `[start, end)`;
- start > end → overnight interval crossing midnight.

Keep boundary semantics centralized here rather than reimplementing them in UI or `ChimePolicy`.

#### `ThemeMode`

Stable persistent enum:

- System;
- Light;
- Dark;
- HighContrast.

App owns actual Avalonia palette/resource selection.

#### `TimeZoneDescriptor`

Search/display projection of one `TimeZoneInfo` entry.

Carries the information needed by search/UI without requiring views to understand how the system catalog was built.

Typical data:

- timezone ID;
- display name;
- base UTC offset;
- searchable representation.

#### `WorldClock`

Persistent world-clock model containing:

- stable per-card ID;
- display name;
- timezone ID.

Factory behavior generates a GUID-style ID for new clocks and trims supplied display/timezone text before the full `AppSettings.Normalize` safety pass.

### Services

#### `ClockFormatter`

Pure formatting service.

Primary responsibility: convert a UTC/offset-aware instant into a target `TimeZoneInfo`, then build one `ClockSnapshot` according to `AppSettings` and culture.

Behavior includes:

- `TimeZoneInfo.ConvertTime` rather than manual offset arithmetic;
- 12/24-hour patterns;
- optional seconds;
- current-culture date and weekday;
- ISO week number;
- optional calendar detail text;
- UTC offset formatting;
- daylight/standard timezone display name selection.

Keep it deterministic: all time, timezone, settings, and culture inputs should be explicit/testable.

#### `ChimePolicy`

Pure decision service.

Inputs conceptually include:

- current converted local time;
- chime settings;
- previous chime local time.

It returns whether playback is allowed based on:

- enabled state;
- quiet hours;
- exact minute/second cadence boundary;
- duplicate suppression within the same local calendar minute.

It does **not** play sound; that remains `IChimePlayer`'s responsibility.

## ChronoDesk.Infrastructure

Namespace root: `ChronoDesk.Infrastructure`.

Infrastructure implements Core contracts with local operating-system/runtime facilities.

### `AppPaths`

Static local-path resolver.

Responsibilities:

- resolve application data directory;
- honor optional `CHRONODESK_DATA_DIR` override;
- otherwise use `Environment.SpecialFolder.ApplicationData`;
- fall back to `AppContext.BaseDirectory` if the special folder cannot be resolved;
- construct settings/log paths under the ChronoDesk data directory.

Do not scatter path-building rules across UI/services; add new durable app-owned paths here when appropriate.

### `SafeFileLogger`

Implements `IAppLogger` as local JSONL.

Responsibilities:

- create/use the logs directory;
- serialize structured events;
- sanitize common email/secret-like text;
- bound stored string length;
- store safe exception metadata rather than arbitrary exception messages;
- synchronize concurrent writes;
- rotate the active log around 1 MiB;
- make logging failures nonfatal.

Threat boundary: redaction is defense in depth, not permission to pass secrets into the logger intentionally.

### `JsonSettingsStore`

Implements `ISettingsStore`.

Key policies:

- JSON camel-case property names;
- camel-case string enums;
- numeric enum values rejected;
- comments/trailing commas tolerated on read;
- read/import maximum size 2 MiB;
- schema newer than supported rejected;
- loaded/imported values normalized before use;
- atomic sibling temporary-file write then move/replace;
- corrupt primary settings preserved with timestamp suffix when possible;
- invalid portable imports reported rather than silently replacing current settings;
- cancellation supported through async file operations.

Public behaviors:

- `LoadAsync` — resilient application startup load;
- `SaveAsync` — normalized persistent replacement;
- `ExportAsync` — normalized user-selected backup write;
- `ImportAsync` — strict user-selected input validation/return.

UI must not bypass this store with direct JSON writes.

### Startup testability boundaries

#### `StartupPlatform`

Internal enum representing Windows/macOS/Linux/Unsupported.

`StartupPlatformDetector` converts the real runtime OS into that enum. Tests inject a value rather than trying to emulate another OS.

#### `IStartupFileSystem`

Internal narrow abstraction for startup artifact operations such as:

- directory creation;
- file existence;
- text read/write;
- deletion.

It exists for deterministic startup testing, not as a general filesystem abstraction for the entire application.

#### `SystemStartupFileSystem`

Production `IStartupFileSystem` implementation backed by `System.IO`.

Keep this adapter intentionally thin so startup policy remains in `PlatformStartupManager` and unit tests can cover that policy with a fake.

#### `IStartupRegistry`

Internal narrow abstraction for current-user string Registry values.

Used only by the Windows startup path to isolate Registry I/O from command/value decision logic.

#### `SystemStartupRegistry`

Production Windows Registry adapter.

Responsibilities:

- read a current-user string value;
- write it;
- delete it.

Platform annotations/guards should remain scoped to the Registry operations so warnings-as-errors builds stay correct on non-Windows targets.

#### `PlatformStartupManager`

Production `IStartupManager`.

Construction supports production defaults plus injected platform/filesystem/registry/user-profile/XDG values for tests.

Windows behavior:

- current-user `Software\Microsoft\Windows\CurrentVersion\Run`;
- value name `ChronoDesk`;
- quoted current executable plus `--background`;
- enabled detection verifies configured command references the current executable;
- disable deletes the value.

macOS behavior:

- `~/Library/LaunchAgents/com.sanskar.chronodesk.plist`;
- XML-escaped executable;
- `--background` program argument;
- `RunAtLoad=true`;
- disable deletes the plist.

Linux behavior:

- `$XDG_CONFIG_HOME/autostart/chronodesk.desktop` or `~/.config/autostart/chronodesk.desktop`;
- desktop entry with quoted/escaped executable + `--background`;
- disable deletes the file.

Unsupported platform:

- `IsSupported=false`;
- mutation request throws `PlatformNotSupportedException`.

All operations honor cancellation at the policy boundary.

### `SystemChimePlayer`

Production `IChimePlayer`.

Windows:

- local `Console.Beep` sequence executed through cancellable task work.

macOS:

- fixed `/usr/bin/afplay` executable;
- fixed Glass system-sound file.

Linux fallback order:

1. `/usr/bin/canberra-gtk-play`;
2. `/usr/bin/paplay`;
3. `/usr/bin/aplay`.

Security characteristics:

- no user-provided executable path;
- no user-built shell command;
- arguments supplied individually;
- helper absence/nonzero failure treated as optional failure.

Do not broaden this class into a generic command execution facility.

### `SystemTimeZoneCatalog`

Production `ITimeZoneCatalog`.

Construction caches `TimeZoneInfo.GetSystemTimeZones()` ordered for user display/search.

`GetAll()` returns the cached descriptors.

`Search(query, limit)`:

- trims/splits whitespace terms;
- uses local/current-culture case-insensitive matching;
- searches bounded results;
- protects the requested limit with a supported range.

`Resolve(timeZoneId)`:

1. direct runtime lookup;
2. IANA → Windows mapping attempt;
3. Windows → IANA mapping attempt;
4. UTC fallback.

No network request occurs.

## ChronoDesk.App

Namespace root: `ChronoDesk.App`.

App owns Avalonia composition, user interaction, presentation state, resources, and OS-default-handler handoff.

### `Program`

Process entry point.

Responsibilities:

- construct the Avalonia `AppBuilder`;
- configure platform detection/font integration as defined by the app bootstrap;
- start the classic desktop lifetime with command-line arguments.

Command-line behavior currently matters for `--background`, consumed later by `MainWindow` after initialization.

### `AppServices`

Explicit composition root/container-by-convention.

Production construction wires:

- `SafeFileLogger`;
- `JsonSettingsStore`;
- `SystemTimeZoneCatalog`;
- `PlatformStartupManager`;
- `SystemChimePlayer`;
- `ClockFormatter`;
- `ChimePolicy`;
- `TimeProvider`/other runtime dependencies used by `MainWindowViewModel`.

An alternate constructor accepts test dependencies.

Rule: add a dependency here when it is truly application-wide composition. Do not use hidden service locators/global mutable state to avoid explicit wiring.

### `AppVersionProvider`

Internal semantic display-version helper.

`GetDisplayVersion(Assembly)`:

- reads `AssemblyInformationalVersionAttribute` first;
- passes it to normalization with assembly-version fallback.

`NormalizeDisplayVersion(...)`:

- trims informational version;
- removes `+build.metadata` from the user-visible string;
- preserves prerelease suffix such as `-rc.1`;
- otherwise uses three-part assembly version;
- final fallback is `development`.

Release workflow overrides informational/assembly/file version from the semantic tag, while ordinary development builds retain preview metadata.

### `ExternalLinkLauncher`

Internal centralized external-navigation boundary.

`TryGetAllowedUri` accepts only:

- absolute HTTPS;
- absolute mailto.

It rejects relative/malformed/HTTP/file/script/custom values outside the allowlist.

`TryOpen`:

- validates first;
- passes `uri.AbsoluteUri` to `Process.Start` with `UseShellExecute=true` for OS default-handler behavior;
- catches expected platform/handler launch exceptions and returns false.

Current About/Settings destinations are fixed application constants, but the allowlist remains important defense in depth.

### `App.axaml`

Application XAML root.

Owns global Avalonia theme/resource inclusion, including the shared design system.

Global resources should remain centralized here rather than duplicated per-window when they represent application-wide behavior.

### `App` (`App.axaml.cs`)

Avalonia `Application` subclass/lifecycle coordinator.

Framework initialization responsibilities:

- create `AppServices`/`MainWindowViewModel`/`MainWindow`;
- subscribe to settings changes;
- assign main window to classic desktop lifetime;
- create tray icon/menu best-effort;
- dispose tray icon on desktop exit.

Theme responsibilities:

- interpret System/Light/Dark/HighContrast settings;
- update requested theme variant and ChronoDesk brush resources;
- honor dedicated HighContrast bool as an override.

Tray responsibilities:

- Show → restore/activate main window;
- Focus → toggle Focus mode;
- Mini → toggle Mini mode;
- Quit → authorize close then shut down lifetime.

Tray initialization failure is logged and nonfatal.

### Localization facades

#### `Strings`

Static facade around `ResourceManager("ChronoDesk.App.Localization.Strings")`.

Responsibilities:

- expose strongly named-ish static properties for primary user text;
- resolve using `CurrentUICulture`;
- fall back visibly to the key when a resource is absent;
- format localized strings using `CurrentCulture`.

#### `SettingsExtras`

Equivalent facade for the companion Updates & About Settings catalog.

It exists to isolate the Phase 7 resource additions without destabilizing the much larger primary catalog. It can be merged/refactored later only as an intentional localization change.

### `ObservableObject`

Minimal `INotifyPropertyChanged` base.

`SetProperty<T>`:

- compares with `EqualityComparer<T>.Default`;
- returns false without notification for equal values;
- writes and raises PropertyChanged when changed.

`OnPropertyChanged` raises the event using caller member name by default.

Keep this small; do not evolve it into an unrelated framework abstraction.

### `WorldClockCardViewModel`

Presentation wrapper for one `WorldClock` plus its resolved `TimeZoneInfo` and shared `ClockFormatter`.

Read-only identity properties delegate to the model:

- `Id`;
- `DisplayName`;
- `TimeZoneId`.

Mutable display properties:

- `TimeText`;
- `DateText`;
- `ZoneText`.

`Update(instant, settings)` creates one snapshot through `ClockFormatter` and updates presentation fields.

The card does not own persistence or timezone search.

### `MainWindowViewModel`

Primary application/presentation orchestrator.

Core responsibilities:

- own current normalized `Settings` snapshot;
- own world-clock card collection;
- own timezone search results;
- expose current local clock/date/week/calendar/zone/status strings;
- initialize settings and first clock/search state;
- perform every clock tick from injected `TimeProvider`;
- update local/world clocks from one coherent instant;
- evaluate and invoke optional chime playback;
- add/remove world clocks;
- search system timezones;
- perform settings transactions;
- coordinate startup external state and rollback;
- import/export/reset/onboarding settings operations;
- raise `SettingsChanged` for App/window behavior.

#### Initialization contract

Initialization loads settings, rebuilds cards/search state, refreshes the clock, raises settings notification, and contains expected load failures so the application can remain usable with defaults.

#### Tick contract

A tick:

- reads one current UTC instant;
- formats local clock;
- updates all world clocks from that instant;
- applies chime policy;
- records duplicate-suppression time before/around playback according to current implementation;
- contains/logs optional chime failure.

#### Settings transaction contract

`UpdateSettingsAsync` conceptually:

1. normalize candidate;
2. detect startup preference change;
3. apply OS startup state;
4. save JSON;
5. roll startup back best-effort if save fails;
6. replace live settings only after save;
7. rebuild clocks/refresh;
8. notify settings changed.

Do not duplicate this transaction in Settings code-behind.

#### Import contract

Imported settings are modified before application so:

- onboarding remains completed;
- current device `StartWithSystem` is preserved.

This prevents a portable JSON file from creating an OS startup side effect silently.

### `MainWindow.axaml`

Main presentation tree.

Contains:

- header/title/actions;
- primary hero clock;
- date/weekday/week/calendar/zone output;
- quick format/seconds actions;
- world-clock ItemsControl/card template;
- timezone search/results/add section;
- status/footer credit;
- automation names for key clock/search semantics.

Named controls used by code/tests are part of the internal UI contract; rename them with corresponding code/headless test updates.

### `MainWindow` (`MainWindow.axaml.cs`)

Window behavior coordinator.

State includes:

- 250 ms `DispatcherTimer`;
- non-overlap tick guard;
- focus-mode state;
- mini-mode state;
- stored pre-mini dimensions/position/topmost;
- explicit close authorization.

Major responsibilities:

- async initialization on Opened;
- start/stop tick timer;
- first-run onboarding;
- `--background` hide behavior;
- main-window action handlers;
- timezone search/add/remove handlers;
- Settings/About modal opening;
- Focus mode enter/exit;
- Mini mode enter/exit;
- keyboard shortcuts;
- close-to-tray vs real close;
- external tray Show/Focus/Mini/Quit helpers used by `App`.

The UI timer never performs settings file I/O merely to refresh time.

### `SettingsWindow.axaml`

Tabbed preferences presentation.

Contains controls for:

- clock fields;
- chime/quiet hours;
- theme/layout/font/size/spacing;
- accessibility/behavior/startup/tray;
- privacy/data import/export/reset;
- Updates & About actions/version;
- save/cancel/status.

Interactive controls with separate visual labels use explicit automation names where needed for screen-reader semantics.

### `SettingsWindow` (`SettingsWindow.axaml.cs`)

Code-behind translates UI controls into/from immutable `AppSettings` snapshots and delegates actual state changes to `MainWindowViewModel`.

Key behavior:

- populate controls from current settings;
- parse quiet times with invariant `HH:mm`/`TimeOnly` rules;
- internal awaitable `SaveChangesAsync` for deterministic headless tests;
- existing click handler delegates to that operation;
- internal awaitable `ResetDefaultsAsync` with control reload;
- import/export via Avalonia native storage provider;
- localized validation/error/status handling;
- semantic version display;
- user-initiated GitHub Releases navigation through `ExternalLinkLauncher`;
- About dialog opening.

Native picker behavior stays outside deterministic headless tests; persistence below it is separately tested.

### `OnboardingWindow.axaml`

Localized first-run modal view introducing product, world clocks, accessibility, and privacy/offline behavior.

### `OnboardingWindow` (`OnboardingWindow.axaml.cs`)

Owns onboarding completion interaction.

On completion it asks the main view model to persist onboarding completion (`IsFirstRun=false`) before closing. Failure should remain user-safe rather than pretending persistence succeeded.

### `AboutWindow.axaml`

Localized About UI containing:

- product identity/logo;
- version;
- project/funding/support destinations;
- MIT/privacy information;
- creator credit.

### `AboutWindow` (`AboutWindow.axaml.cs`)

Responsibilities:

- populate current display version through `AppVersionProvider`;
- route fixed project/funding/mailto actions through `ExternalLinkLauncher`;
- avoid duplicating external-link validation/process-launch code.

### `DesignSystem.axaml`

Shared application visual system.

Responsibilities include centralized styles/resources for reusable:

- typography;
- buttons;
- cards/soft cards;
- search/input controls;
- muted/section/hero clock presentation;
- spacing/corner/border behavior.

Prefer adding a reusable style here over repeating visually identical property lists across views.

### Assets

#### `chronodesk-logo.svg`

Editable/scalable source logo for repository/app presentation surfaces that can render vector content appropriately.

Do not assume every Avalonia image API can decode arbitrary SVG directly; the About implementation was previously hardened around this boundary.

#### `chronodesk.ico`

Windows/application/tray icon asset referenced by App project/window/tray configuration.

When replacing it, preserve appropriate icon sizes/format and test both window and tray rendering on Windows plus any cross-platform Avalonia use.

### `app.manifest`

Windows executable manifest referenced by the App project.

Changes can affect Windows host compatibility, requested privilege, DPI/runtime behavior, and therefore require real Windows validation.

### App/Infrastructure `AssemblyInfo.cs`

These files use `InternalsVisibleTo` for `ChronoDesk.Tests` so deterministic tests can reach selected internal seams.

Rule: keep production types internal when they are not intended public API; do not make classes public only for tests.

## Typical change paths

### Add a persistent preference

```text
Core AppSettings/model
  ↓
normalization/schema rules
  ↓
Settings XAML + code mapping
  ↓
MainWindowViewModel behavior
  ↓
JsonSettingsStore tests/headless tests
  ↓
settings-reference.md / changelog / privacy if needed
```

### Add a platform integration

```text
Core contract (only if generic capability needed)
  ↓
Infrastructure narrow boundary + implementation
  ↓
App composition/orchestration
  ↓
deterministic fake/adapter tests
  ↓
real native validation
  ↓
platform-integration.md / release docs
```

### Add user-facing text

```text
.resx resource
  ↓
Strings/SettingsExtras accessor
  ↓
XAML/code usage
  ↓
headless/resource test where critical
  ↓
localization.md / accessibility review
```

### Add a new tracked source file

The implementation is incomplete until:

- tests/docs are updated as needed;
- `docs/repository-reference.md` contains its exact path;
- `scripts/check_documentation_inventory.py` passes;
- local Markdown validation passes;
- CI validates the new final head.

## Public API philosophy

ChronoDesk is an application repository, not a published SDK. Public C# visibility currently means “needed across project/assembly boundaries” rather than a promise of long-term third-party binary API compatibility.

Internal helpers should stay internal when practical. Stable **persistent data** and **release/user behavior** compatibility is more important than exposing large public implementation surfaces.

## Concurrency/cancellation principles

- UI clock ticks are serialized with a non-overlap guard.
- async persistence/startup/chime operations accept cancellation tokens at service boundaries.
- settings persistence uses asynchronous file operations.
- shared file logging synchronizes writes.
- do not introduce fire-and-forget mutation paths that can silently outlive window/application shutdown without a deliberate lifecycle design.

## Failure-handling principles

Optional facility failure should degrade the optional feature rather than the core clock:

- tray unavailable → main clock still works;
- chime unavailable → clock continues;
- unknown timezone → UTC fallback;
- log write failure → application continues;
- external handler unavailable → link action fails safely;
- corrupt settings → preserve when possible and use defaults.

Explicit user mutation failures should be surfaced rather than reported as success:

- Settings save;
- import/export;
- startup preference change/persistence consistency.

## Security review hotspots

When reviewing source changes, pay special attention to:

- `JsonSettingsStore` — untrusted local import/parser/filesystem boundary;
- `PlatformStartupManager` — persistence into OS startup mechanisms;
- `SystemChimePlayer` — external local process execution;
- `ExternalLinkLauncher` — OS shell/default-handler handoff;
- `SafeFileLogger` — privacy/redaction/disk retention;
- release workflows — artifact integrity and permissions;
- settings models — length/count/value bounds.

## Testability review hotspots

Prefer adding narrow injectable seams only around behavior that otherwise depends on mutable external state. Existing examples:

- startup filesystem;
- startup Registry;
- `TimeProvider`;
- settings store;
- timezone catalog;
- chime player;
- app logger.

Avoid abstracting every framework method automatically; abstraction should buy determinism, safety, or architectural clarity.

## Source documentation maintenance

When production files change responsibility materially:

- update this reference;
- update the corresponding specialized document;
- update `repository-reference.md` if paths change;
- update tests/test catalog when contracts change;
- update an ADR if a durable architectural decision changes.
