# ADR 0006: Externalize User-Facing Strings with .resx Resources

- Status: Accepted
- Date: 2026-08-19

## Context

ChronoDesk ships English first but must remain internationalization-ready. Hard-coded UI strings spread across Avalonia XAML, view models, tray menus, status messages, dialogs, About/Updates/diagnostics content, and Core formatting would make later translation expensive and inconsistent.

The application does not currently require runtime language switching, and adding a localization framework solely for that possibility would increase dependency and binding complexity.

Core must also remain independent of the Avalonia/App resource layer. That means domain formatting cannot directly reference `ChronoDesk.App.Localization` even when formatted output contains user-visible labels such as Week/Day/ISO week.

## Decision

Use .NET `.resx` resources as the localization source of truth for user-facing application text.

Resource catalogs are grouped by concern under `src/ChronoDesk.App/Localization/`, including:

- `Strings.resx` — general UI/settings/About/tray copy;
- `StateStrings.resx` — loading/count/status sentence formats;
- `UpdateStrings.resx` — Updates/external-handler copy;
- `DiagnosticsStrings.resx` — local diagnostics labels/privacy note;
- `ClockDetailStrings.resx` — Week/Day/ISO-week/UTC labels used by clock formatting.

Each catalog has a small C# accessor surface. Avalonia XAML reads static UI resources through `x:Static`, while view models/code-behind use the same resource accessors for formatted status/dialog text.

For Core formatting labels, `ChronoDesk.Core` defines the presentation-neutral `ClockDisplayLabels` value model. `ClockFormatter` accepts an optional label-provider function and otherwise falls back to English defaults. `ChronoDesk.App.AppServices` composes the production formatter with `ClockDetailStrings.CreateLabels`.

This keeps dependencies in the intended direction:

```text
App resources -> Core label model
Core -X-> App/Avalonia/resources
```

Internal structured-log event names and deliberately developer-safe diagnostics are not treated as localizable UI.

Future translations should add culture-specific resource files for every affected catalog, for example `Strings.hi.resx`, `StateStrings.hi.resx`, and `ClockDetailStrings.hi.resx`, without changing Core business logic.

The app initially follows `CultureInfo.CurrentUICulture`. Runtime culture switching is not required for the English-first baseline; if added later, it must include a reliable refresh strategy for already-created windows and resource-backed formatter labels.

## Consequences

### Positive

- User-facing copy has explicit resource sources rather than scattered literals.
- Concern-specific catalogs prevent one large resource file from becoming unreviewable.
- New cultures can use normal .NET satellite-resource behavior.
- Core and Infrastructure remain free of App/Avalonia localization dependencies.
- Clock formatting labels can be localized through composition without polluting domain dependencies.
- Formatted status messages can preserve grammar through whole-sentence resource formats.
- XAML remains readable without a third-party localization package.

### Negative

- Static resource access means changing culture while windows are already open would not automatically refresh every text node.
- Multiple resource accessor classes must remain synchronized with their `.resx` catalogs.
- Pluralization currently uses simple singular/plural English resource forms rather than a full CLDR message-format engine.
- A future runtime-language switch needs a broader window/view-model refresh design.

## Testing

Current tests protect localization boundaries by verifying:

- main/settings/onboarding/About resources load in headless Avalonia;
- loading/world-clock count text comes from dedicated resources;
- Updates/About/diagnostics fields use the expected resource/version values;
- `ClockFormatter` accepts injected non-English detail labels without changing clock logic.

Runtime translation completeness is not claimed because only the English resource set currently ships.

## Rules for future UI work

- Add new user-visible text to the appropriate resource catalog in the same commit as the feature.
- Do not construct translatable sentences by concatenating fragments when a formatted resource can represent the whole phrase.
- Keep shortcut tokens and technical identifiers unchanged only when platform meaning requires it.
- When adding a new culture, create matching resource files for every catalog used by visible UI/domain formatting.
- Test at least one non-English resource set before declaring runtime language switching complete.
- Do not introduce an App-resource dependency into Core or Infrastructure; use an interface/value/provider composed by App instead.
