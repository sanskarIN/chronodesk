# ADR 0007: Migrate Settings Schemas Stepwise Before Normalization

- Status: Accepted
- Date: 2026-08-19

## Context

ChronoDesk stores a bounded local JSON settings document. The current release line uses schema version `1`, but a long-lived desktop application must be able to evolve persisted settings without silently reinterpreting older files or forcing users to reset preferences unnecessarily.

Before the first tagged release, some development builds may also have produced JSON without an explicit `schemaVersion` field. Treating those files as current merely because the C# model has a property initializer would hide the distinction between legacy and versioned data.

## Decision

Read the JSON document first, determine the declared source schema version, then deserialize and pass the resulting model through a dedicated `SettingsMigrationPipeline` before normal settings normalization.

Migration rules:

- a document without `schemaVersion` is treated as legacy schema `0`;
- explicit schema `0` is migrated to schema `1`;
- negative schema versions are rejected;
- schema versions newer than `AppSettings.CurrentSchemaVersion` are rejected;
- migrations advance exactly one version at a time through an explicit switch;
- an unknown intermediate migration step fails instead of guessing;
- normal saves/exports always emit the current normalized schema;
- migration does not directly enable startup integration or execute platform side effects.

The schema `0 -> 1` migration is intentionally data-preserving because pre-versioned development documents already used the same preference field meanings. Its purpose is to make the transition explicit and testable rather than to invent a semantic change.

## Consequences

### Positive

- Missing/legacy schema handling is explicit.
- Future migrations have one predictable extension point.
- Unsupported future documents fail safely.
- Migration behavior can be regression-tested through the same import path users exercise.
- Normalization remains responsible for bounded/default-safe values after version conversion.

### Negative

- Settings reads parse a small JSON document before model deserialization rather than using a single streaming deserialize call.
- Every future schema increment requires a deliberately implemented migration step and tests.

The existing 2 MiB input cap keeps the additional in-memory JSON representation bounded and appropriate for preference data.

## Testing policy

For every new schema version:

1. add a fixture or inline document for the immediately previous version;
2. verify migration preserves intended preferences;
3. verify the result reports the current schema version;
4. verify future/invalid versions remain rejected;
5. verify import-side-effect restrictions still hold;
6. retain the older migration tests so multi-step upgrades remain covered.

## Rejected alternatives

- Treat missing schema as current automatically: hides legacy compatibility assumptions.
- Reset all older files to defaults: unnecessary data loss.
- Accept future schema versions and normalize them: risks silently changing unknown semantics.
- Add a database solely to obtain a migration framework: disproportionate to ChronoDesk's bounded local preference document.
