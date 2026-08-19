# ChronoDesk Development Guide

This guide describes the day-to-day engineering workflow. Use `README.md` for the product overview, `docs/README.md` for the complete documentation map, and `docs/repository-reference.md` to find the responsibility of every tracked file.

## Daily workflow

From the repository root:

```bash
python3 scripts/check_markdown_links.py
python3 scripts/check_documentation_inventory.py
python3 scripts/check_repository_secrets.py
python3 -m unittest discover -s scripts/tests -p 'test_*.py'
dotnet restore ChronoDesk.sln
dotnet build ChronoDesk.sln
dotnet test ChronoDesk.sln
```

Before committing production/release-impacting work:

```bash
python3 scripts/check_markdown_links.py
python3 scripts/check_documentation_inventory.py
python3 scripts/check_repository_secrets.py
python3 -m unittest discover -s scripts/tests -p 'test_*.py'
dotnet restore ChronoDesk.sln
dotnet format ChronoDesk.sln --verify-no-changes --no-restore
dotnet build ChronoDesk.sln -c Release --no-restore
dotnet test ChronoDesk.sln -c Release --no-build
```

Use `CHRONODESK_DATA_DIR` when you want a development run isolated from normal user data. See `configuration-reference.md` for exact configuration behavior.

## Project boundaries

### Core

`src/ChronoDesk.Core`

Put product rules here when they do not require UI, filesystem, registry, process, or other platform APIs. Examples:

- formatting policy;
- quiet-hour calculations;
- settings invariants;
- chime cadence rules;
- domain models and interfaces.

Core should remain deterministic enough that most behavior can be tested without Avalonia or a real desktop.

### Infrastructure

`src/ChronoDesk.Infrastructure`

Put implementations of local/platform boundaries here:

- JSON persistence;
- timezone discovery;
- startup adapters;
- chime playback;
- structured logging;
- local path resolution.

Infrastructure may reference Core. Core may not reference Infrastructure.

### App

`src/ChronoDesk.App`

Put Avalonia views, presentation state, resource/localization facades, theme composition, tray/window behavior, and user interaction here.

Avoid moving business rules into click handlers merely because the behavior is triggered by a button. Prefer the view model or Core policy for deterministic behavior.

### Tests

`tests/ChronoDesk.Tests`

Keep production-contract tests here, using the shared fakes when possible. The file-by-file coverage map is in `test-catalog.md`.

### Repository scripts

`scripts/`

Keep deterministic repository/release validators here. They should prefer the Python standard library so the Repository integrity job remains lightweight. Add unit tests under `scripts/tests/` when parser/policy logic is nontrivial.

## Explicit dependency wiring

`AppServices` is the composition root. It creates concrete infrastructure services and domain services once and passes them to the main view model.

ChronoDesk currently does not need a dependency-injection container. If composition becomes meaningfully complex, document the reason for introducing one in an ADR before adding a framework solely for convenience.

Tests can use the alternate `AppServices` constructor to supply deterministic logger/settings/timezone/startup/chime dependencies.

## Settings changes

Use a new `AppSettings` snapshot with a `with` expression, then call `MainWindowViewModel.UpdateSettingsAsync`.

That method:

1. normalizes values;
2. detects whether user startup integration changed;
3. applies the external startup state when supported;
4. persists settings;
5. attempts startup rollback if persistence fails after the external change;
6. replaces the live snapshot only after persistence succeeds;
7. rebuilds world-clock cards;
8. refreshes the clock;
9. notifies the application theme/window layer.

Do not write directly to `settings.json` from UI code.

See `settings-reference.md` and `runtime-behavior.md` for the full contract.

## Settings schema changes

When adding/changing a persistent field:

1. decide whether the default preserves old behavior;
2. define/maintain normalization bounds and null/invalid-value behavior;
3. increment `CurrentSchemaVersion` when compatibility requires a migration boundary;
4. add migration logic before relying on the new field if old serialized data needs transformation;
5. add/update persistence and model tests;
6. update Settings UI/headless tests when user configurable;
7. update `settings-reference.md`;
8. update `PRIVACY.md` if the stored data category changes;
9. update `SECURITY.md` if the trust boundary changes;
10. update `CHANGELOG.md` for user-visible changes.

Never repurpose an existing serialized field to mean something incompatible.

## Time and timezone changes

Prefer UTC instants as inputs to Core. Convert to the target zone at the formatting/policy boundary.

Use `TimeProvider` for new time-dependent application logic so tests can inject deterministic time.

Avoid manual UTC offset math for civil time. Use `TimeZoneInfo` so DST and platform timezone rules are applied by the runtime.

If timezone behavior or portability rules change, update ADR 0003 plus `platform-integration.md`/`runtime-behavior.md` as appropriate.

## UI changes

Keep reusable visual rules in `Styles/DesignSystem.axaml` instead of duplicating margins/corner radii/button styles across windows.

For a new user-facing control:

- put translatable text in the resource catalogs rather than hard-coding language where practical;
- make it keyboard reachable;
- provide a visible label or explicit automation name;
- keep target size usable;
- avoid using color as the only state signal;
- verify high-contrast behavior;
- check that text/clock scaling does not make the window unusable;
- add headless smoke/interaction assertions where the behavior is deterministic.

See `localization.md` and `accessibility.md`.

## Localization changes

The primary resource catalog is `Localization/Strings.resx` with accessors in `Strings.cs`. Settings Updates & About currently uses the companion `SettingsExtras` catalog.

When adding user-facing text:

- use semantic keys;
- keep storage identifiers/enums unlocalized;
- use format resources for dynamic word order rather than string fragments when grammar could vary;
- preserve privacy/security semantics exactly;
- include automation/accessibility strings;
- test long/transformed strings in affected UI surfaces.

Do not make Core depend directly on App resource classes just to localize a label. If Core currently emits human-language fragments that need localization, prefer returning structured values and formatting them in the App layer in a deliberate refactor.

## Focus and mini modes

Focus/mini state is intentionally ephemeral and lives in `MainWindow`.

When changing these modes, verify:

- entering/exiting repeatedly;
- `Esc` behavior;
- `F11` behavior;
- `Ctrl+M` behavior;
- restoration of window dimensions/position;
- restoration of always-on-top preference;
- tray Show/Focus/Mini actions;
- interaction with close/minimize-to-tray behavior;
- keyboard focus remains usable after transitions.

## Platform-specific code

Use runtime guards such as:

```csharp
if (OperatingSystem.IsWindows())
{
    // Windows-only operation
}
```

Use platform annotations where analyzers need them. Keep machine-wide privileges out of settings features unless a future feature has a compelling, reviewed reason.

Platform adapters must fail safely and should not stop the core clock when an optional OS facility is unavailable.

Where possible, isolate platform/filesystem/registry decisions behind narrow seams so generated behavior can be tested on any CI host. Real OS execution still needs native validation.

See `platform-integration.md`.

## External process policy

ChronoDesk currently uses external processes only for fixed local sound helpers on Unix-like systems and OS shell/default handlers for validated user-initiated external URIs.

Rules for local helpers:

- no shell command string built from user input;
- prefer a fixed executable path;
- pass arguments through `ProcessStartInfo.ArgumentList`;
- set `UseShellExecute=false`;
- document why the process is needed;
- preserve a no-op/graceful fallback when the feature is optional.

Rules for user-facing external navigation:

- validate the URI before handoff;
- currently allow only absolute HTTPS and mailto;
- route through the centralized `ExternalLinkLauncher`;
- do not introduce background fetching/polling as a side effect.

## Logging

Use short stable event names such as:

```text
settings.load_failed
chime.play_failed
tray.initialize_failed
```

Pass a user-safe message. Do not place imported JSON, arbitrary exception messages, credentials, email addresses, or sensitive filesystem content into the log message.

`SafeFileLogger` provides additional redaction/length/rotation defense, but callers still own safe event/message design.

## Testing style

Prefer deterministic tests with explicit dates/timezones/cultures. Avoid tests that rely on the developer's wall clock or network access.

For timezone tests, use UTC where the test does not specifically need DST behavior. When testing DST, construct/select the target timezone intentionally and skip only when a platform genuinely cannot provide the required fixture.

For randomized robustness tests:

- use a committed fixed seed;
- keep corpus sizes bounded for CI;
- assert invariants, not just “does not throw.”

For platform startup tests, never mutate the runner's real login/startup settings; use the test filesystem/registry seams.

## Documentation changes

Documentation is a maintained subsystem.

The complete map is `docs/README.md`. The canonical tracked-file coverage document is `docs/repository-reference.md`.

Whenever you add, move, rename, or delete any tracked file, update the repository reference in the same commit/PR. This includes:

- source code;
- tests and fakes;
- assets/icons;
- `.resx`/XAML/manifest files;
- scripts;
- GitHub workflows/templates;
- ADRs and other docs.

Validate:

```bash
python3 scripts/check_markdown_links.py
python3 scripts/check_documentation_inventory.py
```

When behavior changes, update the closest specialized guide. When an enduring architecture decision changes, add a new/superseding ADR rather than rewriting historical rationale without context.

## Performance

Do not optimize the 250 ms display timer by adding background complexity without measurement. Clock formatting is small; persistence and network access do not happen on every tick.

If a performance-sensitive feature is introduced, add a benchmark/measurement note to `docs/performance.md` before and after optimization.

Avoid turning Updates & About into a timer/startup network poll unless the product/privacy architecture is deliberately changed and documented.

## Commit discipline

Prefer one coherent change per commit. Good boundaries include one service, one test area, one view, one policy document, or one CI capability.

Requested project commit identity for local maintainer work:

```bash
git config user.name "Sanskar"
git config user.email "sanskarin@outlook.in"
```

Connected GitHub APIs may use the authenticated GitHub identity according to the integration's commit behavior.

## Updating the handoff

`what_changed.md` is the continuation source of truth. Update it after meaningful milestones with:

- completed work;
- files changed;
- verification commands/results;
- limitations;
- next exact tasks;
- recent meaningful commits.

Permanent project behavior belongs in the specialized documentation; the handoff should point at it rather than becoming the only place a design rule is recorded.
