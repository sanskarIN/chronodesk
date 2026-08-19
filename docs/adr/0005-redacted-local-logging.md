# ADR 0005: Use Redacted Local Structured Logging

- Status: Accepted
- Date: 2026-08-19

## Context

ChronoDesk needs enough diagnostics to explain settings, tray, startup, theme, external-handler, or chime failures without adding a remote telemetry service or recording arbitrary user content. Raw exception messages can contain local paths or input content, and unrestricted logs can grow continuously.

Users also need a reliable way to locate the local diagnostic file when asking for support without ChronoDesk uploading it automatically.

## Decision

Use a small local JSON Lines logger with a deliberately constrained event schema:

- UTC timestamp;
- severity;
- short event name;
- user-safe message authored by ChronoDesk;
- exception type only when relevant.

Use the canonical path exposed by `AppPaths.GetLogPath()`. Settings → Data & Privacy → Local diagnostics shows that local path read-only together with version/runtime/platform information.

Apply common email and secret-assignment redaction, cap field lengths, avoid success logging on the clock tick path, and rotate the active log near 1 MiB.

Rotation archive names include millisecond timestamp precision plus a random GUID so rapid repeated rotations do not collide.

Logging failures are non-fatal.

Do not serialize raw settings, imported JSON, authentication headers, tokens, arbitrary exception messages, or sensitive user content into diagnostic events. Do not upload logs or diagnostics automatically.

## Consequences

### Positive

- Diagnostics remain useful without remote analytics.
- Logs are easy to inspect and machine-parse.
- Users can find the canonical local log from Settings without guessing platform paths.
- Continuous clock operation does not create continuous disk writes.
- Common accidental secret/PII patterns receive another protection layer.
- Collision-resistant rotation preserves multiple rapid archives safely.
- Logger behavior is directly regression-tested.

### Negative

- Redaction cannot mathematically recognize every sensitive string.
- Omitting arbitrary exception messages can reduce debugging detail.
- Displayed filesystem paths can contain a local account/folder name, so users must still review diagnostic information before sharing.
- Users must explicitly share sanitized excerpts when seeking support; ChronoDesk does not transmit them.

## Verification

`SafeFileLoggerTests` verifies common email/secret redaction and repeated collision-resistant rotation. Diagnostics/headless tests verify Settings receives canonical local metadata without performing an upload.

`PRIVACY.md`, `SUPPORT.md`, and `docs/troubleshooting.md` instruct users to review paths/log excerpts before sharing.

## Follow-up rule

If a future feature needs richer diagnostics, add narrow structured fields with a privacy review rather than falling back to dumping arbitrary objects or exception text. Any future diagnostic export/upload feature must be explicit opt-in and requires a new privacy/security review before implementation.
