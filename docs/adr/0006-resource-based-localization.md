# ADR 0006: Externalize User-Facing Strings with .resx Resources

- Status: Accepted
- Date: 2026-08-19

## Context

ChronoDesk ships English first but must remain internationalization-ready. Hard-coded UI strings spread across Avalonia XAML, view models, tray menus, status messages, dialogs, and About content would make later translation expensive and inconsistent.

The application does not currently require runtime language switching, and adding a localization framework solely for that possibility would increase dependency and binding complexity.

## Decision

Use .NET `.resx` resources as the localization source of truth for user-facing application text.

- English resources live in `src/ChronoDesk.App/Localization/Strings.resx`.
- `Strings.cs` exposes a stable strongly named application-facing accessor surface.
- Avalonia XAML reads values through `x:Static` from `Strings`.
- View models/code-behind use the same accessor for user-visible status, tray, dialog, and About text.
- Internal structured-log event names and deliberately developer-safe diagnostic messages are not treated as localizable UI.
- Future translations should be added as standard culture-specific resource files such as `Strings.fr.resx` or `Strings.hi.resx` without changing domain logic.

The app initially follows `CultureInfo.CurrentUICulture`. Runtime culture switching is not required for the English-first baseline; if it is added later, the design must include a reliable resource refresh strategy for already-created windows.

## Consequences

### Positive

- User-facing copy has a single source of truth.
- New cultures can use normal .NET satellite-resource behavior.
- Core and Infrastructure remain free of presentation/localization dependencies.
- XAML remains readable without a third-party localization package.

### Negative

- Static resource access means changing culture while windows are already open would not automatically refresh every text node.
- Resource accessor maintenance is required when adding new UI strings.
- Pluralization and advanced message formatting will need a deliberate approach if the product grows beyond simple utility copy.

## Rules for future UI work

- Add new user-visible text to resources in the same commit as the feature.
- Do not construct translatable sentences by concatenating fragments when a formatted resource can represent the whole phrase.
- Keep shortcut tokens and technical identifiers unchanged only when the platform meaning requires it.
- Test at least one non-English resource set before declaring runtime language switching complete.
