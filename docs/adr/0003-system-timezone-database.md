# ADR 0003: Use the Operating System Timezone Database

- Status: Accepted
- Date: 2026-08-19

## Context

Correct timezone rules change over time because governments can alter UTC offsets and daylight-saving rules. ChronoDesk must work offline and across Windows, macOS, and Linux without embedding a private update service or silently fetching timezone data.

Bundling a second timezone database would create a separate update lifecycle and risk divergence from the host system. A remote timezone API would make a basic clock depend on networking, service availability, and additional privacy/security policy.

## Decision

Use .NET `TimeZoneInfo` as the timezone source of truth for the running host.

ChronoDesk will:

- enumerate `TimeZoneInfo.GetSystemTimeZones()` at application startup;
- store timezone IDs in world-clock settings;
- resolve an exact stored ID first;
- attempt .NET-supported IANA-to-Windows and Windows-to-IANA conversion when needed;
- fall back safely to UTC when the current platform cannot resolve an imported ID;
- rebuild the catalog when the user restarts the app after an OS/runtime timezone update.

ChronoDesk will not implement an automatic network timezone updater in the core application.

## Consequences

### Positive

- Core clock operation remains offline.
- Timezone maintenance follows the platform/runtime maintenance path.
- No ChronoDesk-hosted timezone service or update signing channel is required.
- Cross-platform imports have a best-effort Windows/IANA mapping path.

### Negative

- Available IDs/display names differ across platforms.
- A very recent timezone-rule change is only available after the OS/runtime provides it.
- An imported ID that cannot be mapped displays using UTC until the user replaces it.
- The in-memory catalog needs an application restart to observe a newly installed system database.

## Rejected alternatives

- Bundled tzdb package: duplicates system maintenance and adds a dependency/update policy.
- Remote time/timezone API: unnecessary network dependency and privacy surface.
- Manual UTC offsets: incorrect for DST and historical/future civil-time rules.
