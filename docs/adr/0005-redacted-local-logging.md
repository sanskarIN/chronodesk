# ADR 0005: Use Redacted Local Structured Logging

- Status: Accepted
- Date: 2026-08-19

## Context

ChronoDesk needs enough diagnostics to explain settings, tray, startup, or chime failures without adding a remote telemetry service or recording arbitrary user content. Raw exception messages can contain local paths or input content, and unrestricted logs can grow continuously.

## Decision

Use a small local JSON Lines logger with a deliberately constrained event schema:

- UTC timestamp;
- severity;
- short event name;
- user-safe message authored by ChronoDesk;
- exception type only when relevant.

Apply common email and secret-assignment redaction, cap field lengths, avoid success logging on the clock tick path, and rotate the active log near 1 MiB.

Logging failures are non-fatal.

Do not serialize raw settings, imported JSON, authentication headers, tokens, arbitrary exception messages, or sensitive user content into diagnostic events.

## Consequences

### Positive

- Diagnostics remain useful without remote analytics.
- Logs are easy to inspect and machine-parse.
- Continuous clock operation does not create continuous disk writes.
- Common accidental secret/PII patterns receive another protection layer.

### Negative

- Redaction cannot mathematically recognize every sensitive string.
- Omitting arbitrary exception messages can reduce debugging detail.
- Users must still review log excerpts before sharing.

## Follow-up rule

If a future feature needs richer diagnostics, add narrow structured fields with a privacy review rather than falling back to dumping arbitrary objects or exception text.
