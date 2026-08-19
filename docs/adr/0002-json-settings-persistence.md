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
- include a schema version;
- normalize values after deserialization;
- reject a document declaring a schema newer than supported;
- write through a temporary file before replacement;
- preserve malformed normal settings with a timestamped corrupt suffix when possible;
- support explicit user-controlled export/import.

Use `CHRONODESK_DATA_DIR` only as a local development/portable-data-root override.

## Consequences

### Positive

- Human-inspectable and portable.
- No database runtime.
- Easy backup/export.
- Small persistence attack surface.
- Settings compatibility can be tested explicitly.

### Negative

- Whole-document writes rather than field-level updates.
- Concurrent multi-process writers are not a supported scenario.
- Future incompatible schema changes require explicit migration logic.

## Migration policy

When persisted semantics become incompatible, increment the schema version and migrate stepwise before using the current model. Never silently reinterpret old serialized data.

## Rejected alternatives

- SQLite: unnecessary for bounded preference data.
- OS-specific registry/preferences only: harms portability and import/export consistency.
- Cloud database: violates offline-first requirements and adds account/privacy complexity.
