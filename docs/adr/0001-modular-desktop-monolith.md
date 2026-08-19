# ADR 0001: Use a Modular Desktop Monolith

- Status: Accepted
- Date: 2026-08-19

## Context

ChronoDesk is a cross-platform desktop clock. It needs testable time/chime rules, local persistence, platform-specific startup/chime behavior, and an Avalonia UI. It does not need independent deployable services, a network backend, or a database server.

Putting all behavior in Avalonia code-behind would make timezone, quiet-hour, persistence, and error-handling logic harder to test. Splitting the application into microservices would introduce networking, deployment, failure modes, and operational complexity that have no product benefit.

## Decision

Use one desktop process with three code projects:

```text
ChronoDesk.Core
ChronoDesk.Infrastructure -> ChronoDesk.Core
ChronoDesk.App -> ChronoDesk.Core + ChronoDesk.Infrastructure
```

`ChronoDesk.Core` owns domain models, rules, and boundary interfaces and remains independent from Avalonia and concrete operating-system/filesystem APIs.

`ChronoDesk.Infrastructure` implements persistence and platform boundaries.

`ChronoDesk.App` composes services and owns presentation/window behavior.

Use explicit service construction through `AppServices` instead of adding a dependency-injection container while composition remains small.

## Consequences

### Positive

- Core time/chime/settings behavior is directly testable.
- Platform code is isolated from domain rules.
- UI can evolve without turning persistence into code-behind logic.
- Build/deployment remains one desktop application.
- Dependency count stays small.

### Negative

- Some presentation orchestration remains in view models/code-behind.
- Platform adapters require explicit runtime guards.
- A future large feature set may make manual composition verbose.

## Revisit when

Reconsider the composition approach only if dependency wiring becomes difficult to reason about or independently test. Reconsider process boundaries only if ChronoDesk gains a requirement that genuinely needs an independent process/service, not merely to follow a distributed-systems pattern.
