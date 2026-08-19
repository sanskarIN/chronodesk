# ChronoDesk Localization Guide

ChronoDesk uses .NET resource files for user-facing application text. This document explains the current resource layout, lookup behavior, formatting culture, XAML usage, accessibility strings, and the process for adding future translations safely.

## Current localization architecture

Application resources live under:

```text
src/ChronoDesk.App/Localization/
```

Current catalogs:

- `Strings.resx` — primary application English resource catalog;
- `Strings.cs` — typed-style static accessors and formatting helper for the primary catalog;
- `SettingsExtras.resx` — companion catalog introduced for the Settings Updates & About surface;
- `SettingsExtras.cs` — accessors for the companion catalog.

The project currently ships the neutral/default resources. Future culture-specific files can follow standard .NET resource naming, for example:

```text
Strings.hi.resx
Strings.fr.resx
SettingsExtras.hi.resx
```

Do not add a translation file that only partially changes safety/privacy wording without reviewing semantic equivalence.

## Resource lookup

`Strings` owns a `ResourceManager` for:

```text
ChronoDesk.App.Localization.Strings
```

`Strings.Get(name)` resolves against `CultureInfo.CurrentUICulture` and falls back to the resource key name when no value is found.

That fallback makes a missing key visible instead of throwing, but a visible key name is still a localization defect and should be caught by review/tests.

`SettingsExtras` follows the same ResourceManager pattern for its own catalog.

## Formatting culture vs UI culture

Two culture concepts are deliberately distinct:

- `CurrentUICulture` selects localized resource text;
- `CurrentCulture` controls culture-sensitive formatting such as formatted resource placeholders and normal clock/date display.

`Strings.Format` uses `CurrentCulture` with the localized format string returned by `Get`.

Clock formatting also accepts an explicit `CultureInfo` in Core tests, which allows deterministic formatting verification without changing global process state.

## Primary resource categories

`Strings.resx`/`Strings.cs` currently cover these groups.

### Product identity and status

Examples:

- application name;
- tagline;
- creator credit;
- ready/warning status text;
- chime status/unavailable messages;
- add/remove world-clock status.

### Main-window actions

Examples:

- Focus;
- Mini;
- Settings;
- About;
- Remove;
- Add selected timezone;
- toggle clock format;
- toggle seconds.

### Tooltips and automation semantics

Examples:

- focus/mini/settings tooltips;
- main-window automation name;
- current-time automation name;
- timezone-search automation name.

Accessibility/automation text is user-facing assistive-technology content and must be localized with the same care as visible labels.

### World-clock/search content

Examples:

- world clocks section title/description;
- timezone-add title/description;
- search watermark;
- offline timezone note.

### Onboarding

Includes onboarding title, introduction, time/world-clock/accessibility explanatory cards, privacy statement, and continue action.

### Settings: clock and chime

Includes:

- clock format labels;
- 12/24-hour options;
- seconds/date/weekday/week/calendar toggles;
- chime enable/interval options;
- quiet-hours labels and note.

### Settings: appearance/accessibility/behavior

Includes:

- theme and layout names;
- font/size/spacing labels;
- reduced-motion/high-contrast text;
- keyboard shortcut descriptions;
- always-on-top/start-with-system/minimize-to-tray text.

### Settings: privacy/data

Includes:

- private-by-default wording;
- logging privacy wording;
- backup/restore titles and notes;
- import/export/reset actions;
- validation and persistence status/error messages;
- native file-picker titles and JSON file type.

### About/support

Includes:

- About title/description;
- project/funding/support labels;
- license/privacy text;
- business/support contact labels;
- version format;
- tray menu labels.

## Settings companion catalog

`SettingsExtras.resx` is intentionally small and contains the new Updates & About section strings rather than forcing a risky bulk edit of the already-large primary resource catalog during Phase 7.

It contains text for:

- Updates & About tab name;
- update/release explanation;
- open Releases action;
- open About action;
- creator/version/update privacy messaging;
- external-link failure status.

Future maintainers may merge the companion catalog into the primary catalog in a deliberate localization refactor, but there is no runtime need to do so merely for aesthetics.

## XAML usage

Avalonia XAML imports the localization namespace:

```xml
xmlns:loc="using:ChronoDesk.App.Localization"
```

Static resource-backed properties can then be used through `x:Static`, for example conceptually:

```xml
Text="{x:Static loc:Strings.AppName}"
```

This keeps user-facing text out of view markup where practical and makes localization gaps easier to identify.

Some dynamic text is composed by view models/code-behind because it includes runtime values or status context. Those paths should still use resource strings for the human-language portions.

## Dynamic strings and formatted values

### Version text

`VersionFormat` is a resource format string. `SettingsWindow` and About pass the semantic display version through resource-based formatting.

Do not concatenate a translated label and version manually when a formatted resource can preserve grammar/order across languages.

### Added timezone status

The current status composes `AddedPrefix` plus a runtime label. If future languages require a different word order/case, migrate this to one format resource with a placeholder rather than building more fragments.

### World-clock count

`WorldClockCountText` currently composes a numeric count with lower-cased `WorldClocksTitle`. This is acceptable for the current neutral resource but is not ideal pluralization architecture for many languages. If full translations are added, introduce dedicated plural/count resources instead of assuming English grammar.

## Dates and times

Primary clock/date display uses `CultureInfo.CurrentCulture` by default:

- short date uses the culture's `DateTimeFormat.ShortDatePattern`;
- weekday uses `dddd`;
- 12-hour/24-hour selection is an explicit product preference rather than inferred from culture;
- calendar details use culture formatting for day-of-year text but invariant construction for UTC offset and ISO week structure.

Before a localization release, review whether fixed English words such as `Week`, `Day`, and `ISO week` in Core-generated formatting should become resource-backed. Core currently does not reference App resources by design, so a future change should preserve the dependency boundary—e.g., return structured values from Core and localize labels in the App layer.

## Enum values vs localized labels

Persistent enum values are serialized as stable camel-case strings, for example:

```text
twentyFourHour
highContrast
quarterHourly
```

These are storage identifiers and must **not** be translated.

The Settings UI displays localized resource labels that map to enum values by selected index. Translation changes should never change the persisted enum spelling.

## Quiet-hour input

Quiet-hour text is parsed using invariant culture and supports exact `HH:mm` plus invariant `TimeOnly` parsing.

This is an intentional data-entry format contract rather than a translated free-form time expression. If locale-native time input is added later, document and test ambiguity carefully before changing persisted/runtime semantics.

## Accessibility localization

When translating:

- translate automation names, not only visible labels;
- keep controls understandable without relying on adjacent visual text;
- avoid abbreviations that become ambiguous to a screen reader;
- ensure button action text describes the action, not only the destination;
- preserve explicit privacy/network semantics in the update section;
- validate keyboard navigation after translated strings expand controls.

Large translated strings can change layout dramatically, especially Settings tabs, About cards, onboarding, and small/mini window content.

## Security/privacy localization

Text describing any of these must preserve meaning exactly:

- local-only settings/timezone behavior;
- absence of telemetry/background update polling;
- user-initiated external navigation;
- imported-settings safety behavior;
- logging/redaction limitations;
- release checksum limitations;
- security reporting instructions.

A translation should not overstate guarantees beyond the source behavior.

## Adding a new resource key

For the primary catalog:

1. add the `<data>` entry to `Strings.resx`;
2. add a corresponding accessor in `Strings.cs`;
3. use that accessor from XAML/code;
4. add/update headless tests if the string is required for a critical UI surface;
5. update translation catalogs when they exist.

For Settings extras, use the equivalent `SettingsExtras` files.

Prefer meaningful semantic keys such as:

```text
ImportError
AutomationSearchTimezones
OpenReleasesButton
```

Avoid keys based on screen coordinates or temporary implementation names.

## Adding a translation

Recommended workflow:

1. keep neutral `.resx` as the fallback/source-of-truth meaning;
2. create culture-specific `.resx` files with matching keys;
3. do not translate technical identifiers, product name, file paths, command flags, or enum storage values unless explicitly intended;
4. run the application with the target UI culture;
5. test every window at normal and increased text scaling;
6. test keyboard-only navigation;
7. test at least one screen reader where available;
8. test long strings in Settings, onboarding, About, tray menus, and main-window buttons;
9. update documentation and changelog for newly supported UI languages.

## Resource review checklist

Before merging localization changes:

- no user-facing string was unnecessarily hard-coded in new XAML/code;
- format placeholders are preserved and ordered correctly;
- apostrophes/special characters remain valid XML;
- accelerator/keyboard behavior was not unintentionally altered;
- automation names remain descriptive;
- privacy/security wording remains semantically equivalent;
- date/time examples do not imply a locale-independent format unless the parser really requires one;
- all resource files are included in `repository-reference.md`;
- headless tests still load all relevant views/resources.
