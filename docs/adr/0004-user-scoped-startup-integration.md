# ADR 0004: Keep Startup Integration User-Scoped and Opt-In

- Status: Accepted
- Date: 2026-08-19

## Context

A desktop clock can be useful immediately after sign-in, but automatically registering startup without consent is intrusive. Machine-wide startup installation would require elevated permissions and complicate security, uninstall behavior, enterprise policy, and cross-platform maintenance.

ChronoDesk must support configurable startup behavior without making startup an intrusive default.

## Decision

Startup remains disabled by default and is changed only after the user explicitly saves the preference.

Use current-user mechanisms:

- Windows: current-user `Run` registry value;
- macOS: a plist in `~/Library/LaunchAgents`;
- Linux: an XDG autostart desktop file under the current user's configuration directory.

The startup command passes `--background`. ChronoDesk loads settings first and hides the main window when that argument is present and minimize-to-tray is enabled.

Do not request administrator/root privileges for this feature.

## Consequences

### Positive

- Respects user choice.
- No elevation is needed.
- Changes are reversible from Settings.
- Integration remains isolated to the current user.

### Negative

- Enterprise policy or desktop-session behavior can override/block the setting.
- Moving the executable after registration can leave a stale path until startup is toggled again.
- Linux desktop environments differ in XDG autostart/tray behavior.

## Safety rules

- Escape/quote platform file contents correctly.
- Never construct startup commands from arbitrary user input.
- Do not silently re-enable startup after the user disables it.
- Failure to configure startup must not prevent the clock from running normally.
