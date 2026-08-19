# ADR 0002: Persist Settings as Versioned Local JSON

- Status: Accepted
- Date: 2026-08-19

## Context

ChronoDesk persists a small amount of local preference data: clock display options, appearance/accessibility settings, chime policy, startup/minimize behavior, and a bounded list of world-clock descriptors.

A relational database would add migrations, native/database dependencies, backup complexity, and failure modes disproportionate to this data. An opaque platform-specific preference API would make cross-platform import/export and schema review harder.

## Decision

Persist one versioned JSON settings document under the current user's application-data directory.

The persistence implementation must:

- cap input size;
- parse with `System.Text.Json`;
- include a schema version on current saves/exports;
- determine the source schema before using the document as current settings;
- migrate supported older schemas stepwise before normalization;
- normalize values after migration/deserialization;
- reject a document declaring a schema newer than supported;
- reject invalid/negative schema versions;
- write through a temporary file before replacement;
- preserve malformed normal settings with a timestamped corrupt suffix when possible;
- support explicit user-controlled export/import.

Use `CHRONODESK_DATA_DIR` only as a local development/portable-data-root override.

The concrete migration pipeline and compatibility rules are specified in [ADR 0007](0007-stepwise-settings-schema-migrations.md).

## Consequences

### Positive

- Human-inspectable and portable.
- No database runtime.
- Easy backup/export.
- Small persistence attack surface.
- Settings compatibility can be tested explicitly.
- Missing/pre-versioned development schema handling is explicit rather than inferred accidentally from model defaults.

### Negative

- Whole-document writes rather than field-level updates.
- Concurrent multi-process writers are not a supported scenario.
- Every incompatible future schema change requires an explicit migration step and retained regression tests.

## Migration policy

When persisted semantics become incompatible, increment the schema version and migrate stepwise before using the current model. Never silently reinterpret old serialized data.

A document without `schemaVersion` is treated as legacy schema `0`; the current `0 -> 1` migration is data-preserving because those pre-versioned development documents used the same field meanings. Future migrations must be implemented one version at a time and covered by tests.

## Rejected alternatives

- SQLite: unnecessary for bounded preference data.
- OS-specific registry/preferences only: harms portability and import/export consistency.
- Cloud database: violates offline-first requirements and adds account/privacy complexity.
