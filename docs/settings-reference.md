# ChronoDesk Settings Reference

This document is the canonical field-by-field reference for ChronoDesk persistent settings, normalization rules, import/export behavior, and the Settings UI mapping.

## Storage overview

ChronoDesk stores one normalized `AppSettings` JSON document at the path returned by `AppPaths.GetSettingsPath()`.

Default data directory:

- the operating system's application-data directory plus `ChronoDesk`;
- if the application-data directory cannot be resolved, the executable base directory is used as a fallback.

Development/test override:

```text
CHRONODESK_DATA_DIR
```

When that environment variable is nonblank, its trimmed full path becomes the application data directory.

The default settings filename is:

```text
settings.json
```

## Schema version

Current schema version: `1`.

`AppSettings.SchemaVersion` is normalized to the current schema version before persistence.

Load/import behavior:

- documents from the current or older supported schema are normalized;
- a document declaring a schema version newer than the running application is rejected;
- empty documents are rejected;
- files larger than 2 MiB are rejected;
- JSON trailing commas and comments are accepted;
- enum values are serialized as camel-case strings and integer enum values are not accepted.

When changing serialized meaning, do not repurpose a field incompatibly. Introduce migration logic and update the schema version when compatibility requires it.

## Top-level settings

### `schemaVersion`

Type: integer.

Default/current value: `1`.

Purpose: identifies the persistent settings schema understood by the application.

Normalization: always rewritten to the current schema version.

### `isFirstRun`

Type: boolean.

Default: `true`.

Purpose: controls whether first-run onboarding should be shown.

Behavior:

- completing onboarding sets it to `false`;
- reset creates defaults with `IsFirstRun = false`, so resetting preferences does not repeat onboarding;
- import forces it to `false` regardless of the imported file.

### `clockFormat`

Type: enum string.

Values:

- `twelveHour`;
- `twentyFourHour`.

Default: `twentyFourHour`.

Invalid enum values normalize to `twentyFourHour`.

UI: Clock format selector and main-window format toggle.

### `showSeconds`

Type: boolean.

Default: `true`.

Purpose: chooses whether the primary/world-clock formatted time includes seconds.

### `showDate`

Type: boolean.

Default: `true`.

Purpose: shows/hides the locale-aware short date line.

### `showWeekday`

Type: boolean.

Default: `true`.

Purpose: shows/hides the localized weekday name.

### `showWeekNumber`

Type: boolean.

Default: `true`.

Purpose: shows/hides the ISO week-number field.

### `showCalendarDetails`

Type: boolean.

Default: `false`.

Purpose: enables the detail line containing day-of-year, ISO week, and UTC offset.

### `theme`

Type: enum string.

Values:

- `system`;
- `light`;
- `dark`;
- `highContrast`.

Default: `system`.

Invalid enum values normalize to `system`.

The dedicated `highContrast` boolean can also force the high-contrast palette even when this enum is not `highContrast`.

### `layout`

Type: enum string.

Values:

- `centered`;
- `compact`;
- `dashboard`.

Default: `centered`.

Invalid enum values normalize to `centered`.

Layout affects primary clock alignment and hero-card sizing. Focus and mini mode remain separate ephemeral window states.

### `fontFamilyName`

Type: string.

Default: `Inter`.

Normalization:

- blank/whitespace becomes `Inter`;
- leading/trailing whitespace is removed;
- control characters are replaced with spaces;
- maximum length is 120 UTF-16 characters, with surrogate-pair-safe truncation.

ChronoDesk does not download fonts. The resulting family name is resolved by the local UI/font stack.

### `clockFontSize`

Type: number.

Default: `96`.

Allowed normalized range: `42` through `240`.

Non-finite values normalize to `96`.

### `contentSpacing`

Type: number.

Default: `16`.

Allowed normalized range: `4` through `48`.

Non-finite values normalize to `16`.

### `reducedMotion`

Type: boolean.

Default: `false`.

Purpose: user accessibility preference. ChronoDesk currently avoids decorative motion by design; the field is retained as an explicit accessibility contract for present/future UI behavior.

### `highContrast`

Type: boolean.

Default: `false`.

Purpose: independently forces the high-contrast application palette.

Effective high-contrast behavior is enabled when this field is true **or** `theme == highContrast`.

### `alwaysOnTop`

Type: boolean.

Default: `false`.

Purpose: controls normal main-window topmost behavior.

Mini mode temporarily forces topmost regardless of this value and restores the appropriate state on exit.

### `startWithSystem`

Type: boolean.

Default: `false`.

Purpose: opt-in user-scoped login startup.

Important semantics:

- a change is applied to the OS startup integration before JSON persistence;
- if JSON persistence then fails, ChronoDesk attempts to restore the previous OS startup state;
- imported settings are not permitted to change this value; import replaces the imported value with the current local value before applying the snapshot;
- unsupported operating systems do not expose a successful startup change.

See `platform-integration.md` for platform details.

### `minimizeToTray`

Type: boolean.

Default: `true`.

Purpose:

- normal close hides the main window instead of exiting unless explicit Quit is requested;
- a `--background` startup hides the initialized main window when this preference is enabled.

Tray availability is platform-dependent, so real-desktop validation remains a release gate.

## Chime settings

Persistent property: `chime`.

If a deserialized `chime` object is unexpectedly null, normalization replaces it with default `ChimeSettings`.

### `chime.enabled`

Type: boolean.

Default: `false`.

No sound is played unless explicitly enabled.

### `chime.interval`

Type: enum string.

Values:

- `hourly`;
- `halfHourly`;
- `quarterHourly`.

Default: `hourly`.

Invalid enum values normalize to `hourly`.

Playback boundaries require `Second == 0` and the matching minute boundary.

### `chime.quietHours`

Object containing `enabled`, `start`, and `end`.

If the object is unexpectedly null, normalization replaces it with default `QuietHours`.

#### `quietHours.enabled`

Type: boolean.

Default: `false`.

#### `quietHours.start`

Type: `TimeOnly` JSON representation produced by `System.Text.Json`.

Default: `22:00`.

Settings UI input accepts an invariant-culture exact `HH:mm` value and also falls back to invariant-culture `TimeOnly` parsing.

#### `quietHours.end`

Type: `TimeOnly` JSON representation.

Default: `07:00`.

#### Quiet-hour interval semantics

If quiet hours are disabled, no time is considered quiet.

If start equals end, the range is treated as disabled rather than as a 24-hour block.

When `start < end`, the interval is:

```text
[start, end)
```

When the range crosses midnight (`start > end`), a time is quiet when it is greater than/equal to start **or** less than end.

Example default enabled range:

```text
22:00 through 06:59:59...
```

## World clocks

Persistent property: `worldClocks`.

Default list:

1. a `Local` clock using `TimeZoneInfo.Local.Id`;
2. a `UTC` clock using `TimeZoneInfo.Utc.Id`.

Each entry contains:

### `worldClocks[].id`

Type: string.

New clocks use a lowercase 32-hex-character GUID representation (`Guid.NewGuid().ToString("N")`).

Normalization:

- required and nonblank;
- controls converted to spaces;
- trimmed;
- maximum length 128;
- entries are de-duplicated by exact ordinal ID.

### `worldClocks[].displayName`

Type: string.

Normalization:

- required and nonblank;
- controls converted to spaces;
- trimmed;
- maximum length 160.

### `worldClocks[].timeZoneId`

Type: string.

Normalization:

- required and nonblank;
- controls converted to spaces;
- trimmed;
- maximum length 256.

The system timezone catalog attempts direct lookup, IANA-to-Windows conversion, and Windows-to-IANA conversion. UTC is the final runtime fallback when the stored timezone cannot be resolved.

### Collection constraints

Normalization:

- removes null/malformed entries;
- de-duplicates IDs;
- keeps at most 24 entries;
- guarantees at least one clock; if no valid entries remain, a Local clock is added.

The main UI additionally prevents adding the same timezone ID twice and prevents removing the final remaining clock.

## JSON naming and formatting

The settings serializer uses camel-case property names, indented output, camel-case string enums, optional trailing commas on read, and comment skipping on read.

Representative structure:

```json
{
  "schemaVersion": 1,
  "isFirstRun": false,
  "clockFormat": "twentyFourHour",
  "showSeconds": true,
  "showDate": true,
  "showWeekday": true,
  "showWeekNumber": true,
  "showCalendarDetails": false,
  "theme": "system",
  "layout": "centered",
  "fontFamilyName": "Inter",
  "clockFontSize": 96,
  "contentSpacing": 16,
  "reducedMotion": false,
  "highContrast": false,
  "alwaysOnTop": false,
  "startWithSystem": false,
  "minimizeToTray": true,
  "chime": {
    "enabled": false,
    "interval": "hourly",
    "quietHours": {
      "enabled": false,
      "start": "22:00:00",
      "end": "07:00:00"
    }
  },
  "worldClocks": []
}
```

The exact `TimeOnly` serialization representation is controlled by the target .NET runtime. Do not hand-edit files based only on the illustrative example; prefer the in-app export flow for a canonical document.

## Atomic persistence

Save/export operations:

1. resolve the destination to a full path;
2. create the destination directory if necessary;
3. serialize to a unique sibling temporary file;
4. flush the stream;
5. replace/move the temporary file over the destination;
6. delete a leftover temporary file in `finally` if an operation fails.

This avoids directly truncating the previous settings file before a complete replacement is available.

## Corrupt settings handling

Normal application load catches JSON, I/O, and invalid-data failures.

When possible, the failing `settings.json` is moved to a timestamped sibling file:

```text
settings.json.corrupt-YYYYMMDD-HHMMSS.json
```

The application then uses defaults.

A failure to preserve the corrupt file is logged, but logging/preservation failures do not crash the application.

Import is stricter: invalid imported files are reported back to the Settings UI instead of being moved automatically.

## Import security boundary

Imported settings are treated as untrusted local input.

Controls include:

- 2 MiB maximum size;
- schema-version rejection for newer unsupported documents;
- string enum validation;
- settings normalization and length bounds;
- maximum 24 world clocks;
- startup preference preservation;
- no automatic command execution or external network access from imported values.

The import path accepts a user-selected filesystem path from the native file picker; it does not interpret imported JSON as code.

## Export behavior

Export writes the current normalized in-memory settings to a user-selected JSON path using the same atomic writer as normal persistence.

Exported data can include preferences, clock labels, timezone IDs, and accessibility/behavior selections. Users should review exported files before sharing because display labels may be personally chosen text.

## Settings UI mapping

### Clock area

- format;
- seconds;
- date;
- weekday;
- week number;
- calendar details.

### Appearance area

- theme;
- layout;
- font family;
- clock font size;
- content spacing.

### Accessibility/behavior area

- reduced motion;
- high contrast;
- always on top;
- start with system;
- minimize to tray.

### Chime area

- enabled;
- interval;
- quiet hours enabled;
- quiet start;
- quiet end.

### Privacy/data area

- export settings;
- import settings;
- reset defaults.

### Updates & About area

This area is not persistent configuration. It displays current semantic version information and provides user-initiated navigation to GitHub Releases and the About dialog.

## Adding a new setting

A persistent setting change should normally include:

1. the field/default in the relevant Core model;
2. normalization/invariant logic;
3. Settings UI mapping if user configurable;
4. persistence/migration tests;
5. headless UI coverage when applicable;
6. this reference update;
7. privacy/security documentation if the data/trust boundary changes;
8. changelog entry for user-visible behavior;
9. repository file reference update if new tracked files are introduced.
