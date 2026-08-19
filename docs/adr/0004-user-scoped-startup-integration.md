# ADR 0004: Keep Startup Integration User-Scoped and Opt-In

- Status: Accepted
- Date: 2026-08-19

## Context

A desktop clock can be useful immediately after sign-in, but automatically registering startup without consent is intrusive. Machine-wide startup installation would require elevated permissions and complicate security, uninstall behavior, enterprise policy, and cross-platform maintenance.

ChronoDesk must support configurable startup behavior without making startup an intrusive default. It must also avoid creating startup files outside the intended user scope when platform environment variables are missing/malformed.

## Decision

Startup remains disabled by default and is changed only after the user explicitly saves the preference.

Use current-user mechanisms:

- Windows: current-user `Run` registry value;
- macOS: a plist in `<validated-user-profile>/Library/LaunchAgents`;
- Linux: an XDG autostart desktop file under an absolute `XDG_CONFIG_HOME`, otherwise `<validated-user-profile>/.config/autostart`.

The startup command passes the fixed `--background` argument.

Registration generation is deterministic and isolated from OS writes:

- Windows command generation is quoted and rejects embedded quote characters;
- macOS plist generation XML-escapes the executable path;
- Linux desktop-entry generation follows desktop `Exec` quoting/field-code rules, doubles literal `%`, and rejects executable paths containing `=`;
- executable paths reject control characters;
- file-based registrations are written through a unique temporary file before replacement;
- expected registration files are size-bounded before comparison;
- enabled-state detection requires the exact registration ChronoDesk expects for the current executable.

ChronoDesk hides after a `--background` launch only when minimize-to-tray is enabled **and** the current Avalonia platform exposes a reliable tray-menu restoration route. Without that route, the main window remains accessible.

If the OS startup state changes successfully but persistence of the matching preference fails, ChronoDesk performs a best-effort rollback to the previous startup state. Rollback uses its own non-cancelled operation so cancellation of the failed save does not itself prevent recovery.

Do not request administrator/root privileges for this feature.

## Consequences

### Positive

- Respects user choice.
- No elevation is needed.
- Changes are reversible from Settings.
- Integration remains isolated to the current user.
- Platform registration strings/documents are unit-testable without modifying a developer's real startup state.
- Relative/missing configuration paths do not silently redirect Linux autostart output into an arbitrary working directory.
- Failed preference persistence does not intentionally leave OS startup registration inconsistent.
- Background startup cannot intentionally strand the app without a restoration route.

### Negative

- Enterprise policy or desktop-session behavior can override/block the setting.
- Moving the executable after registration can leave a stale path until startup is toggled again.
- Linux desktop environments differ in XDG autostart/tray behavior.
- Native registry/session-manager behavior still requires real-desktop release validation.

## Safety rules

- Escape/quote platform file contents according to the target format.
- Never construct startup commands from arbitrary user input.
- Never honor a relative `XDG_CONFIG_HOME` as a startup-file root.
- Reject missing/invalid user-profile resolution rather than writing relative startup files.
- Keep registration-file reads bounded.
- Do not silently re-enable startup after the user disables it.
- Do not let imported settings modify OS startup registration implicitly.
- Roll back an already-applied startup change when matching preference persistence fails, where possible.
- Do not hide the main window on background launch unless reliable tray restoration is available.
- Failure to configure startup must not prevent the clock from running normally.

## Verification

Automated tests cover registration generation/escaping, path resolution, expected-registration matching helpers, startup preference rollback/import consistency, rollback after cancellation, and tray-hide policy.

Real Windows registry behavior, macOS LaunchAgent/session behavior, Linux desktop-session behavior, and tray restoration remain manual release gates documented in `docs/testing.md` and `docs/release.md`.
