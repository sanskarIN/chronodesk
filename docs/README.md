# ChronoDesk Documentation Hub

This directory is the canonical technical documentation set for ChronoDesk. The root `README.md` is the product-facing overview; the documents here explain how to use the application, how the repository is built, how the application behaves, how it stores data, how platform integrations work, how releases are produced, and how every tracked file fits into the project.

## Start here

| Goal | Document |
|---|---|
| Learn how to use ChronoDesk | [User guide](user-guide.md) |
| Install prerequisites and run ChronoDesk | [Setup](setup.md) |
| Understand the codebase and dependency rules | [Architecture](architecture.md) |
| Understand production classes and key method contracts | [Source-code reference](source-code-reference.md) |
| Understand runtime behavior and application lifecycle | [Runtime behavior](runtime-behavior.md) |
| Understand every setting and persisted field | [Settings reference](settings-reference.md) |
| Understand local files, environment variables, build configuration, and packages | [Configuration reference](configuration-reference.md) |
| Understand Windows/macOS/Linux integration | [Platform integration](platform-integration.md) |
| Understand resources and localization rules | [Localization](localization.md) |
| Develop or extend ChronoDesk | [Development guide](development.md) |
| Understand automated tests and what remains manual | [Testing guide](testing.md) and [Test catalog](test-catalog.md) |
| Understand GitHub Actions and release automation | [CI/CD reference](ci-cd.md) |
| Prepare a release | [Release guide](release.md) |
| Diagnose a problem | [Troubleshooting](troubleshooting.md) |
| Validate accessibility | [Accessibility](accessibility.md) |
| Review performance assumptions | [Performance](performance.md) |
| Maintain GitHub settings and repository operations | [GitHub maintenance](github-maintenance.md) |
| Find the purpose of any tracked file | [Repository file reference](repository-reference.md) |
| Review architectural decisions and trade-offs | [Architecture decision records](adr/) |

## Repository-level documents

These files intentionally live at the repository root because GitHub and open-source conventions surface them directly:

- [`../README.md`](../README.md) — product overview, features, quick start, platform support, and public-facing project entry point.
- [`../CHANGELOG.md`](../CHANGELOG.md) — user-visible and maintainer-visible changes by release/unreleased state.
- [`../ROADMAP.md`](../ROADMAP.md) — completed and remaining product/release work.
- [`../CONTRIBUTING.md`](../CONTRIBUTING.md) — contributor workflow and expectations.
- [`../CODE_OF_CONDUCT.md`](../CODE_OF_CONDUCT.md) — community behavior standards.
- [`../SECURITY.md`](../SECURITY.md) — vulnerability reporting and secure-development boundaries.
- [`../PRIVACY.md`](../PRIVACY.md) — data collection/storage behavior and privacy guarantees.
- [`../SUPPORT.md`](../SUPPORT.md) — support routes and issue triage guidance.
- [`../LICENSE`](../LICENSE) — MIT license terms.
- [`../what_changed.md`](../what_changed.md) — cross-session implementation handoff and current continuation state.

## Documentation model

ChronoDesk documentation is maintained in four layers:

1. **Product documentation** — the user guide plus public README describe what users can do and what the application promises.
2. **Technical documentation** — architecture, production source reference, runtime flows, persistence, platform integration, configuration, localization, testing, and CI/CD.
3. **Operational documentation** — release, troubleshooting, accessibility validation, performance validation, and GitHub maintenance.
4. **Repository inventory** — a file-by-file reference covering every tracked file so new contributors can map code and configuration to responsibility quickly.

The file inventory is machine-checked by `scripts/check_documentation_inventory.py`. If a tracked file is added or removed, `docs/repository-reference.md` must be updated in the same change.

## Source-of-truth precedence

When documents appear to overlap, use this order:

1. Current source code and workflow files define executable behavior.
2. `SECURITY.md` and `PRIVACY.md` define security/privacy commitments.
3. `docs/release.md` defines release gates.
4. `docs/architecture.md` and ADRs define architectural intent and constraints.
5. Specialized references in this directory explain current implementation details.
6. Root `README.md` is a concise public overview and should not override deeper technical documentation.
7. `what_changed.md` records continuation state; it is not a replacement for permanent documentation.

If code and documentation disagree, treat that as a defect. Update the incorrect side and add a regression check where practical.

## Documentation maintenance rules

When changing behavior:

- update the closest specialized document;
- update `README.md` when the change affects public features, setup, supported platforms, or release artifacts;
- update `CHANGELOG.md` for user-visible changes;
- update `PRIVACY.md` if data categories, network behavior, external navigation, or logging behavior changes;
- update `SECURITY.md` when a trust boundary or security control changes;
- update `docs/user-guide.md` when normal user operation changes;
- update `docs/source-code-reference.md` when a production type's responsibility or important contract changes;
- update `docs/testing.md` and `docs/test-catalog.md` when coverage changes;
- update `docs/repository-reference.md` whenever tracked files are added, renamed, moved, or deleted;
- add or amend an ADR when a durable architectural decision changes;
- update `what_changed.md` after a meaningful implementation milestone.

## Documentation quality gates

Repository CI validates documentation in three ways:

```bash
python3 scripts/check_markdown_links.py
python3 scripts/check_documentation_inventory.py
python3 scripts/check_repository_secrets.py
```

The first validator checks repository-local Markdown targets. The second checks that every tracked file appears in the canonical file reference. The third prevents common high-confidence credential patterns from being committed in text files.

Documentation is also reviewed manually for accuracy, especially where behavior depends on real Windows, macOS, Linux, file-picker, tray, sound, accessibility, or default-handler behavior.
