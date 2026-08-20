# ChronoDesk Accessibility

Accessibility is a release requirement for ChronoDesk across Desktop, Android, iOS/iPadOS, and Browser/WebAssembly. The project follows platform-appropriate, WCAG-oriented interaction principles while recognizing that final behavior also depends on Avalonia and each host operating system/browser accessibility stack.

## Current design commitments

ChronoDesk currently includes or intentionally supports:

- keyboard navigation through native Avalonia controls where a keyboard is present;
- visible native focus indicators;
- desktop keyboard shortcuts for common clock/window actions;
- semantic automation names on the primary clock and timezone search;
- text labels rather than icon-only critical actions;
- high-contrast application palette;
- reduced-motion preference;
- no decorative animation in the baseline;
- scalable main-clock typography;
- touch-friendly interactive targets in the shared single-view shell;
- vertically scrollable responsive content for narrow phone/browser widths;
- status messages expressed as text rather than color alone;
- orientation-tolerant mobile/tablet layout;
- desktop focus/mini modes that can be exited from the keyboard.

## Platform interaction model

### Desktop

Windows/macOS/Linux use `MainWindow` and expose desktop-specific keyboard/window/tray/settings workflows.

### Android / iOS / iPadOS / Browser

These hosts use shared `MainView`. The shell must remain usable with touch, pointer, keyboard where available, platform text scaling/zoom, orientation changes, and narrow/wide viewports.

Desktop-only focus/mini/tray/startup concepts are not accessibility requirements for single-view hosts because those platform concepts are intentionally absent there.

## Desktop keyboard shortcuts

| Shortcut | Behavior |
|---|---|
| `F11` | Toggle focus clock |
| `Ctrl+M` | Toggle mini mode |
| `Ctrl+K` | Focus timezone search |
| `Ctrl+,` | Open Settings |
| `Ctrl+Shift+T` | Toggle normal always-on-top preference |
| `Esc` | Exit focus or mini mode |

Shortcuts supplement normal focus navigation; they are not the only way to reach core functionality.

## Desktop keyboard-only release checklist

Using no pointing device:

- [ ] Launch and complete desktop onboarding.
- [ ] Reach all main-window header actions.
- [ ] Toggle clock format and seconds.
- [ ] Focus timezone search with normal tab navigation and `Ctrl+K`.
- [ ] Enter search text, navigate results, and activate Add.
- [ ] Reach world-clock remove actions.
- [ ] Open Settings.
- [ ] Navigate every Settings section/tab.
- [ ] Change checkboxes, combos, sliders, and text fields.
- [ ] Reach import/export/reset actions.
- [ ] Close Settings without trapping focus.
- [ ] Enter/exit focus mode.
- [ ] Enter/exit mini mode.
- [ ] Exit the application through an accessible path.

Any keyboard trap is a release-blocking accessibility defect.

## Single-view keyboard/pointer checklist

For Browser and any mobile/tablet setup with a hardware keyboard/pointer:

- [ ] Tab/shift-tab moves through interactive controls in a logical order.
- [ ] Clock format and seconds controls can be activated without touch.
- [ ] Timezone search receives keyboard input and exposes its purpose.
- [ ] Search result selection and Add are reachable.
- [ ] World-clock remove controls are reachable.
- [ ] Focus does not disappear into clipped/off-screen content.
- [ ] Scrolling does not trap keyboard focus.

## Touch checklist

On Android, iPhone, and iPad:

- [ ] Primary buttons have practical touch targets and spacing.
- [ ] Timezone search/result/add flow is usable with touch.
- [ ] World-clock remove actions are not too close to unrelated common actions.
- [ ] Vertical scrolling works in portrait and landscape.
- [ ] Content near notches/home indicators/safe areas remains reachable.
- [ ] Large text does not force critical actions permanently off-screen.
- [ ] Rotation does not leave an unreachable focused control or duplicate UI.

## Screen-reader / assistive-technology review

Before a tagged release, manually exercise representative hosts with platform assistive technology where practical.

Suggested coverage:

- Windows: Narrator or another available screen reader.
- macOS: VoiceOver.
- Android: TalkBack.
- iOS/iPadOS: VoiceOver.
- Browser: browser + OS screen-reader combination appropriate to the tested platform.

Check that:

- [ ] application/window/view context is announced meaningfully;
- [ ] current local time has meaningful accessible context;
- [ ] timezone search identifies itself as a search field;
- [ ] buttons expose visible names;
- [ ] checkboxes expose state/label where present;
- [ ] combo boxes expose selected value where present;
- [ ] desktop Settings navigation exposes labels/selected state;
- [ ] world-clock cards are understandable in reading order;
- [ ] status/error text is discoverable and not conveyed only visually;
- [ ] desktop focus/mini transitions do not leave assistive focus on hidden content;
- [ ] mobile orientation/lifecycle transitions restore usable focus/navigation;
- [ ] browser reload/navigation does not create duplicate or unlabeled app roots.

Where Avalonia defaults are insufficient, add an appropriate `AutomationProperties` name/help text or structural fix rather than relying only on tooltip text.

## Contrast

ChronoDesk provides light, dark, and high-contrast palettes. Release review must inspect:

- clock text against hero/card background;
- body text;
- muted text;
- control labels;
- borders/focus indicators;
- disabled states;
- selected list/state presentation;
- error/status text.

Do not communicate success/error/selection only by hue.

High contrast must be reviewed together with host-level contrast/theme overrides where applicable.

## Text scaling, zoom, and typography

Manual review should include:

### Desktop

- minimum/default/maximum clock size;
- increased OS text scaling;
- narrowest supported desktop window;
- mini mode;
- Settings at minimum supported dimensions.

### Android / iOS / iPadOS

- default text size;
- increased accessibility text size where Avalonia/host scaling applies;
- portrait and landscape;
- phone and tablet-sized layouts.

### Browser

- browser zoom below/above 100% within practical supported ranges;
- OS/browser text scaling;
- narrow mobile-style viewport;
- wide desktop-style viewport.

Text should wrap or scroll rather than become clipped so badly that actions are impossible to understand/reach.

## Reduced motion

The current baseline does not use decorative animation or fake loading delays. The `ReducedMotion` setting is persisted so future transitions/animations have one product preference to honor.

Any future animation must:

- have functional reason or clear UX value;
- avoid blocking input;
- avoid flashing effects;
- be disabled/reduced when requested;
- preserve state clarity without motion;
- respect mobile/browser lifecycle so hidden/inactive views do not animate unnecessarily.

## Desktop focus and mini behavior

Focus mode intentionally hides non-clock chrome. Verify:

- clock remains understandable;
- `Esc`/`F11` exit works;
- no hidden focused interactive control traps input;
- screen-reader focus can return to meaningful content after exiting.

Mini mode must remain escapable with `Esc`/`Ctrl+M` and must not permanently change normal always-on-top preference.

These checks apply only to the Desktop host.

## Single-view lifecycle behavior

`MainView` starts its display timer when attached to the visual tree and stops it when detached. Accessibility/lifecycle review should verify:

- returning to the app does not create duplicate updating surfaces;
- rotation/resizing preserves usable content;
- current clock text remains available after resume;
- no desktop modal onboarding window is required on mobile/browser;
- world-clock controls remain in a logical visual/assistive reading order.

## Chimes

Audio is optional and disabled by default. Important information must never be available only through a chime. Quiet hours/chime failure do not alter displayed time.

The current native playback path is desktop-specific; mobile/browser users must still receive the full visual clock experience without audio.

Future audio/notification features must not require sound for core operation and must not autoplay unexpectedly at first run.

## Error messages

Errors should:

- use plain text;
- avoid raw stack traces in UI;
- explain what the user can do next when practical;
- remain available long enough to read;
- not rely on red color alone;
- avoid exposing private filesystem/imported content;
- remain usable at large text/zoom and narrow widths.

## Touch and pointer targets

Touch is a primary input method on Android/iOS/iPadOS, not an incidental desktop feature. Reusable button/control styles must maintain practical size/spacing.

Avoid densely packing critical/destructive actions. A destructive action should not be immediately adjacent to a high-frequency primary action without adequate visual and pointer/focus separation.

## Accessibility regression process

For an accessibility defect:

1. document host OS/browser, architecture/device class, assistive technology, input method, scaling/zoom/orientation, and exact flow;
2. reproduce at default and high-contrast/system themes when relevant;
3. fix at reusable component/style/semantic layer where possible;
4. add an automated regression check when Avalonia exposes a reliable test surface;
5. build/test the affected host;
6. add the scenario to this checklist if it represents a reusable failure mode.

## Release evidence

`what_changed.md` or the release evidence must record which accessibility checks were actually run. Do not write `PASS` for TalkBack, VoiceOver, screen-reader, device, browser, or platform checks that were not performed.

## Known limits before native validation

Source-level semantics, headless view loading, and keyboard design can be reviewed without real device/UI sessions, but actual screen-reader announcements, touch behavior, focus visuals, OS theme interactions, mobile scaling/orientation, browser zoom, and desktop tray accessibility require real target environments. Those checks remain explicit release gates rather than being inferred from source code.
