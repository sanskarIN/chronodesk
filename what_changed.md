# ChronoDesk — Work Handoff

## Current milestone

Phase 7 — automated platform/release hardening, 2026-08-19.

The product implementation through the current preview scope is present in source. This phase closes the remaining testable automation gaps from the roadmap and moves the unresolved release work toward genuine native-desktop validation rather than source-code placeholders.

## Source of truth

Repository: `sanskarIN/chronodesk`

Default branch: `main`

Active phase branch: `phase-7-automation-hardening`

Active pull request: `#16` — `Phase 7: automate startup adapter and release hardening`

The repository is being implemented from the ChronoDesk master prompt supplied for this project. This handoff file records implementation state, verification boundaries, and the next exact work so a later continuation can resume without relying on chat history.

## Phase 7 completed work

### Startup integration testability

- Added an internal startup-platform model/detector for deterministic platform selection.
- Added a narrow startup filesystem abstraction and the production system filesystem adapter.
- Added a narrow current-user startup registry abstraction and the production Windows registry adapter.
- Refactored `PlatformStartupManager` so production still uses the real environment while tests can inject platform/filesystem/registry/user-profile/XDG inputs.
- Preserved the public `IStartupManager` contract used by the application.
- Added test-assembly visibility only for the internal seams needed by deterministic tests.

### Startup adapter automated coverage

Added deterministic coverage for:

- Windows startup enable writes the quoted ChronoDesk executable plus `--background`.
- Windows startup disable removes the current-user value.
- Windows enabled-state detection requires the configured ChronoDesk executable path.
- macOS LaunchAgent generation uses the expected user path.
- macOS XML-sensitive executable characters are escaped.
- macOS disable removes an existing LaunchAgent.
- Linux uses `XDG_CONFIG_HOME` when supplied.
- Linux falls back to `~/.config/autostart` when XDG config home is absent.
- Linux desktop entries quote executable paths containing spaces.
- Linux disable removes an existing desktop entry.
- Unsupported platforms report unsupported and reject writes.
- Startup operations honor pre-cancelled cancellation tokens.

The tests use fake registry/filesystem implementations and do not modify the CI runner's real login/startup configuration.

### Settings-window headless interaction coverage

- Added test visibility for deterministic application UI internals.
- Refactored Settings save handling into an awaitable internal `SaveChangesAsync` operation while preserving the existing click handler.
- Refactored reset handling into an awaitable internal `ResetDefaultsAsync` operation while preserving the existing click handler.
- Added reusable in-memory settings store, recording startup manager, UTC timezone catalog, no-op chime player, and no-op logger test doubles.
- Consolidated `MainWindowViewModelTests` onto the shared test doubles.
- Added headless Avalonia tests verifying settings control-to-model mapping and persistence.
- Added headless validation coverage showing invalid quiet-hour input does not persist settings or alter startup integration.
- Added headless reset coverage showing defaults are persisted, startup is disabled when needed, and visible controls reload to defaults.

Native file-picker-backed import/export dialogs intentionally remain a real-desktop validation boundary. Import/export persistence and safety logic beneath the picker is already covered independently.

### Repository integrity automation

Added `scripts/check_markdown_links.py`:

- scans repository Markdown files without network access;
- validates repository-local inline/reference link and image targets;
- rejects links that escape the repository;
- reports missing local targets;
- deliberately excludes external URL availability from deterministic CI.

Added `scripts/check_repository_secrets.py`:

- scans committed text files for high-confidence credential patterns;
- detects common private-key, AWS, GitHub, Google, Slack, OpenAI-style, and Stripe secret formats;
- reports only file/line and finding type, not the matched secret value;
- skips build/test output directories and oversized/binary files;
- does not claim to replace manual privacy/credential review.

Updated CI with a dedicated `Repository integrity` job that runs both scripts before release work is considered complete.

### Documentation and release hardening

Updated:

- `CHANGELOG.md` with Phase 7 startup tests, headless interaction tests, repository integrity automation, security changes, and architecture refactors.
- `ROADMAP.md` to mark startup-adapter tests and deeper file-picker-independent Settings interaction tests complete.
- `ROADMAP.md` to record repository-local Markdown link validation as implemented release infrastructure.
- `docs/testing.md` with startup-adapter coverage, settings headless interaction coverage, documentation integrity behavior, and updated quality-gate commands.
- `docs/architecture.md` with the startup platform/filesystem/registry testability boundaries and awaitable Settings UI operation pattern.
- `docs/release.md` with local link validation, credential scanning, exact CI check-name guidance, and the distinction between automated and human release gates.
- `SECURITY.md` with the committed-credential scanner boundary, its limitations, and rotation/history-remediation guidance for any real leaked secret.

## Current automated verification state

GitHub pull request validation is enabled for:

- `CI / Repository integrity`;
- Ubuntu .NET 9 build/test/format/vulnerability inspection;
- Windows .NET 9 build/test/format/vulnerability inspection;
- macOS .NET 9 build/test/format/vulnerability inspection;
- CodeQL;
- Dependency Review.

Because Phase 7 intentionally uses many small commits, CI's `cancel-in-progress` behavior cancels older CI runs whenever a newer commit reaches the pull request. The phase branch must not be treated as fully verified until the final head commit receives a completed green validation run.

The connected execution environment used for repository edits does not provide a local .NET SDK checkout/build path, so GitHub Actions remains the authoritative compiler/analyzer/test runner for these connected writes.

## Remaining release gates

These are intentionally not marked complete without evidence from the required environment:

### Native desktop behavior

- Validate tray behavior on Windows 11.
- Validate tray/menu-bar behavior on current macOS Intel and Apple Silicon environments with a GUI session.
- Validate tray/status-notifier behavior on representative GNOME and KDE Linux sessions.
- Validate real startup enable/disable against the Windows current-user Run key.
- Validate real LaunchAgent startup behavior on macOS.
- Validate real XDG autostart behavior on Linux.
- Validate chime behavior on each platform, including Linux with and without optional sound helpers.

### Accessibility

- Run the full `docs/accessibility.md` checklist on each primary platform.
- Verify keyboard-only navigation, focus visibility, screen-reader behavior where applicable, high contrast, scaling, and large text on real desktops.

### Release candidate

- Replace the README screenshot placeholder only with screenshots captured from a verified release build.
- Complete a clean-checkout release verification using `docs/release.md`.
- Confirm the final release commit's CI, CodeQL, dependency/security checks, and repository-integrity job are green.
- Confirm branch-protection required checks use the actual check names shown by GitHub.
- Review the final tree/artifacts manually for private data in addition to the automated credential scan.
- Create the first release candidate tag only after those gates pass.
- Validate the settings migration path after a real prior tagged preview fixture exists.
- Publish stable `v1.0.0` only after all stable-release gates pass.

## Git commit identity note

The connected GitHub write API does not expose commit author/committer email fields. Commits made through this integration therefore use the authenticated GitHub identity rather than allowing an explicit `sanskarin@outlook.in` commit-email override.

For local Git work, use:

```bash
git config user.email "sanskarin@outlook.in"
```

Do not rewrite connected commits solely to alter attribution unless there is a separate repository-history reason to do so.

## Next exact tasks

1. Wait for the newest Phase 7 pull-request head to receive CI/CodeQL/Dependency Review results.
2. If any check fails, inspect the exact failed job/log, add a focused regression/fix commit, and re-run validation through the new pull-request head.
3. Keep PR #16 draft until the branch passes available automated checks.
4. Once automated checks are green, mark PR #16 ready and merge with a normal merge commit so the granular phase history is preserved.
5. Re-check `main` after merge and update this file only if the merge/release state materially changes.
6. Continue with native release validation only when actual GUI sessions/build artifacts are available; do not mark those roadmap items complete from source inspection alone.

## Phase 7 commit strategy

This phase deliberately uses small, reviewable commits instead of one large change. Separate commits were used for abstractions, production adapters, test fakes, platform tests, Settings testability, individual shared test doubles, headless tests, CI integrity scripts, workflow integration, and each major documentation surface.

This preserves a useful history for regression isolation and future maintenance while still grouping all Phase 7 work under one dedicated pull request.
