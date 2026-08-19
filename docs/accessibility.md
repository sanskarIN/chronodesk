# ChronoDesk Accessibility

Accessibility is a release requirement for ChronoDesk. The project aims for WCAG-oriented desktop practices while recognizing that final behavior also depends on Avalonia and the host operating system's accessibility stack.

## Current design commitments

ChronoDesk currently includes or intentionally supports:

- keyboard navigation through native Avalonia controls;
- visible native focus indicators;
- dedicated keyboard shortcuts for common clock/window actions;
- semantic automation names on the primary clock and timezone search;
- text labels alongside controls rather than icon-only critical actions;
- high-contrast application palette;
- reduced-motion preference;
- no decorative animation in the current baseline;
- scalable main-clock typography;
- reasonably large interactive targets;
- status messages expressed as text rather than color alone;
- simple, predictable first-run and settings windows;
- full-screen and compact modes that can be exited from the keyboard.

## Keyboard shortcuts

| Shortcut | Behavior |
|---|---|
| `F11` | Toggle focus clock |
| `Ctrl+M` | Toggle mini mode |
| `Ctrl+K` | Focus timezone search |
| `Ctrl+,` | Open Settings |
| `Ctrl+Shift+T` | Toggle normal always-on-top preference |
| `Esc` | Exit focus or mini mode |

Shortcuts supplement normal focus navigation; they are not intended to be the only way to reach functionality.

## Keyboard-only release checklist

Using no pointing device:

- [ ] Launch and complete onboarding.
- [ ] Reach all main-window header actions.
- [ ] Toggle clock format and seconds.
- [ ] Focus timezone search with normal tab navigation and `Ctrl+K`.
- [ ] Enter search text, move through results, and activate the add action.
- [ ] Reach world-clock remove actions.
- [ ] Open Settings.
- [ ] Navigate every settings tab.
- [ ] Change checkboxes, combos, sliders, and text fields.
- [ ] Reach import/export/reset actions.
- [ ] Close Settings without trapping focus.
- [ ] Enter/exit focus mode.
- [ ] Enter/exit mini mode.
- [ ] Exit the application through an available accessible path.

Any keyboard trap is a release-blocking accessibility defect.

## Screen-reader review

Before a tagged release, manually exercise ChronoDesk with the primary screen reader available on the target OS where practical.

Check that:

- [ ] window titles are announced meaningfully;
- [ ] current local time has a meaningful accessible name/context;
- [ ] timezone search identifies itself as a search field;
- [ ] buttons expose their visible names;
- [ ] checkboxes expose state and label;
- [ ] combo boxes expose selected value;
- [ ] settings tabs expose their labels and selected state;
- [ ] world-clock cards are understandable in reading order;
- [ ] status/error text is discoverable and not conveyed only visually;
- [ ] focus/mini transitions do not leave assistive focus on an inaccessible hidden control.

Where an Avalonia control does not expose sufficient semantics by default, add an `AutomationProperties.Name`, help text, or an appropriate structural change instead of relying on tooltip text alone.

## Contrast

ChronoDesk provides explicit light, dark, and high-contrast palettes. Release review must inspect:

- clock text against hero/card background;
- normal body text;
- muted text;
- control labels;
- borders/focus indicators;
- disabled states;
- selected list/tab states;
- error/status text.

Do not communicate success/error/selection only by a hue difference.

High contrast should remain readable even when the OS also applies contrast/theme overrides.

## Text scaling and typography

The clock size control supports a broad range. Manual review should include:

- minimum clock size;
- default clock size;
- maximum clock size;
- increased OS text scaling where available;
- narrowest supported main window;
- mini mode;
- Settings at minimum supported dimensions.

Text should wrap or scroll rather than become clipped in a way that makes an action impossible to understand.

Do not use the main clock's user-selected font as a reason to reduce settings/body legibility; general UI typography stays platform-appropriate.

## Reduced motion

The current ChronoDesk baseline does not use decorative animation or fake loading delays. The `ReducedMotion` setting is persisted so future transitions/animations have a single product preference to honor.

Any future animation must:

- have a functional reason or clear UX value;
- avoid blocking input;
- avoid flashing effects;
- be disabled/reduced when the preference is enabled;
- preserve state clarity without motion.

## Focus and full-screen behavior

Focus mode intentionally hides non-clock chrome. Verify:

- the clock remains understandable;
- `Esc`/`F11` exit works;
- there is no hidden focused interactive control that traps keyboard input;
- screen-reader focus can return to meaningful content after exiting.

Mini mode similarly hides most controls, but must remain escapable with `Esc`/`Ctrl+M` and must not permanently change the user's normal always-on-top preference.

## Chimes

Audio is optional and disabled by default. Important information must never be available only through a chime. Quiet hours and chime failure status do not alter the displayed time.

Future audio features should not require sound for core operation and should not autoplay unexpectedly at first run.

## Error messages

Errors should:

- use plain text;
- avoid raw stack traces in the user interface;
- explain what the user can do next when practical;
- remain visible long enough to read;
- not rely on red color alone;
- avoid exposing private filesystem or imported content unnecessarily.

## Touch and pointer targets

Although ChronoDesk targets desktop operating systems, controls may be used on touch-capable hardware. Reusable button styles maintain a practical minimum height and spacing.

Avoid densely packing critical actions. Destructive actions should not be placed immediately adjacent to commonly used primary actions without visual/focus separation.

## Accessibility regression process

For an accessibility defect:

1. document the OS, assistive technology, input method, and exact control/flow;
2. reproduce at default and high-contrast/system themes when relevant;
3. fix at the reusable component/style/semantic layer where possible;
4. add an automated regression check when the framework exposes a reliable test surface;
5. add the scenario to this checklist if it represents a reusable failure mode.

## Release evidence

`what_changed.md` should record which accessibility checks were actually run for a release candidate. Do not write `PASS` for screen-reader/platform checks that were not performed.

## Known limits before GUI validation

Source-level semantics and keyboard design can be reviewed without a GUI session, but actual screen-reader announcements, focus visuals, OS theme interactions, and tray accessibility require a real supported desktop environment. Those checks remain explicit release gates rather than being assumed from source code.
