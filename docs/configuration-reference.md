# ChronoDesk Configuration Reference

This document explains the repository's SDK, MSBuild, package, project, environment, formatting, Git, and runtime-data configuration. It is intended to make local builds and CI behavior reproducible and to prevent configuration drift between projects.

## Configuration hierarchy

ChronoDesk uses repository-level configuration wherever possible:

1. `global.json` selects the .NET SDK family.
2. `Directory.Build.props` applies shared MSBuild/compiler/analyzer properties to all projects.
3. `Directory.Packages.props` centrally owns NuGet package versions.
4. Individual `.csproj` files define project-specific references/output behavior.
5. `.editorconfig` defines repository formatting and C# code-style expectations.
6. `.gitattributes` normalizes text/binary handling and line endings.
7. `.gitignore` prevents generated output, local secrets, runtime data, and signing material from becoming tracked accidentally.
8. `.env.example` documents the only application-specific environment override.

## .NET SDK selection — `global.json`

The repository requests:

```json
{
  "sdk": {
    "version": "9.0.100",
    "rollForward": "latestFeature",
    "allowPrerelease": false
  }
}
```

Meaning:

- the baseline SDK is .NET 9.0.100;
- newer compatible .NET 9 feature bands may be selected through `latestFeature` roll-forward;
- preview SDKs are not selected automatically.

CI explicitly installs `9.0.x`, so local and CI builds remain within the .NET 9 product family.

## Shared MSBuild rules — `Directory.Build.props`

All projects inherit:

- `TargetFramework`: `net9.0`;
- `LangVersion`: `latest`;
- nullable reference types enabled;
- implicit usings enabled;
- warnings treated as errors;
- analyzer level `latest-recommended`;
- code-style enforcement during builds;
- deterministic compilation;
- `ContinuousIntegrationBuild=true` when the `CI` environment property is true;
- repository metadata and author/company/copyright fields.

### Why warnings-as-errors matters

A warning that appears only on one platform can fail that platform's CI job. New platform-specific code should therefore use runtime guards and platform annotations correctly rather than suppressing warnings broadly.

### Determinism

`Deterministic=true` reduces build variation for the same inputs. Release artifacts can still vary because native runtime packs, toolchain versions, timestamps in archives, and platform packaging are external inputs; deterministic compilation is not equivalent to byte-for-byte reproducible release archives.

## Central package management — `Directory.Packages.props`

`ManagePackageVersionsCentrally` and `CentralPackageTransitivePinningEnabled` are enabled.

Current direct version catalog:

| Package | Version | Purpose |
|---|---:|---|
| `Avalonia` | 11.3.18 | UI framework core |
| `Avalonia.Desktop` | 11.3.18 | Desktop lifetime/platform integration |
| `Avalonia.Fonts.Inter` | 11.3.18 | Bundled Inter font integration |
| `Avalonia.Themes.Fluent` | 11.3.18 | Fluent theme |
| `Avalonia.Diagnostics` | 11.3.18 | Debug-only Avalonia diagnostics |
| `Avalonia.Headless.XUnit` | 11.3.18 | Headless Avalonia test host |
| `Microsoft.NET.Test.Sdk` | 17.14.1 | .NET test runner integration |
| `xunit` | 2.9.3 | Test framework |
| `xunit.runner.visualstudio` | 3.1.4 | Test discovery/runner adapter |
| `coverlet.collector` | 6.0.4 | Coverage collector |

Individual project files reference packages without versions. Update package versions centrally unless a strong documented reason requires a project-specific override.

CI and release preflight run a transitive vulnerability check. Dependency Review and Dependabot provide additional GitHub-side dependency controls.

## Solution — `ChronoDesk.sln`

The solution contains four projects:

```text
ChronoDesk.Core
ChronoDesk.Infrastructure
ChronoDesk.App
ChronoDesk.Tests
```

Debug and Release configurations build all four projects for Any CPU at the solution layer.

Dependency direction:

```text
Core
  ↑
Infrastructure
  ↑
App

Tests → Core + Infrastructure + App
```

More precisely:

- Core references no ChronoDesk project.
- Infrastructure references Core.
- App references Core and Infrastructure.
- Tests reference all three production projects.

Do not introduce a Core → Infrastructure/App dependency.

## Core project — `src/ChronoDesk.Core/ChronoDesk.Core.csproj`

Purpose: domain models, contracts, formatting policy, and chime policy.

Project-specific configuration:

- root namespace `ChronoDesk.Core`;
- assembly name `ChronoDesk.Core`;
- no external NuGet dependencies declared by the project;
- no project references.

Keeping Core free from Avalonia/filesystem/registry/process dependencies preserves deterministic testing and portability.

## Infrastructure project — `src/ChronoDesk.Infrastructure/ChronoDesk.Infrastructure.csproj`

Purpose: persistence, local paths, logging, timezone discovery, startup integration, and sound playback.

Project-specific configuration:

- root namespace `ChronoDesk.Infrastructure`;
- assembly name `ChronoDesk.Infrastructure`;
- references Core.

Infrastructure may call local OS/runtime APIs but should implement Core abstractions instead of exposing platform details into Core.

## Application project — `src/ChronoDesk.App/ChronoDesk.App.csproj`

Purpose: Avalonia desktop executable and presentation layer.

Important properties:

- output type `WinExe`;
- root namespace `ChronoDesk.App`;
- assembly name `ChronoDesk`;
- development version prefix `0.1.0`;
- development version suffix `preview`;
- assembly/file versions `0.1.0.0`;
- Windows manifest `app.manifest`;
- Windows application icon `Assets/chronodesk.ico`;
- default project settings are framework-dependent and not single-file;
- release workflow overrides publish properties to produce self-contained single-file platform builds.

References:

- Core;
- Infrastructure;
- Avalonia;
- Avalonia Desktop;
- Inter fonts;
- Fluent theme;
- Avalonia Diagnostics in Debug configuration only.

`Assets/**` is registered as Avalonia resources.

### Development version vs tagged release version

The project file intentionally retains `0.1.0-preview` as the ordinary development identity.

During tagged release publishing, `.github/workflows/release.yml` overrides:

- `Version`;
- `AssemblyVersion`;
- `FileVersion`;
- `InformationalVersion`.

The semantic Git tag is therefore the release identity source for packaged binaries.

## Test project — `tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj`

Properties:

- non-packable;
- marked as a test project;
- root namespace/assembly `ChronoDesk.Tests`.

References all production projects so the suite can test:

- pure domain behavior;
- infrastructure adapters;
- view-model orchestration;
- Avalonia headless views.

Test packages are centrally versioned. Runner/coverage packages are private assets so they do not become production dependencies.

## Environment variables

### `CHRONODESK_DATA_DIR`

The only ChronoDesk-specific environment variable.

Purpose: override the local application data directory for development, test isolation, or troubleshooting.

It is not a credential and should not contain a secret.

Examples:

PowerShell:

```powershell
$env:CHRONODESK_DATA_DIR = "$PWD/.local-data"
```

bash/zsh:

```bash
export CHRONODESK_DATA_DIR="$PWD/.local-data"
```

The value is trimmed and converted to a full path.

### CI-related `CI`

The standard CI environment property controls `ContinuousIntegrationBuild` in `Directory.Build.props`. GitHub-hosted runners provide the expected CI environment automatically.

### Release/GitHub variables

Release workflow steps use GitHub-provided values such as `github.ref_name`, `github.repository`, `github.token`, and runner metadata. These are workflow execution inputs, not application configuration.

## `.env.example`

This file exists to document that ChronoDesk requires no API credentials or remote-service secrets. It contains only the optional empty `CHRONODESK_DATA_DIR` setting.

A real `.env` file is intentionally ignored.

## `.editorconfig`

Repository-wide text rules:

- UTF-8;
- LF line endings;
- final newline required;
- trailing whitespace trimmed except Markdown.

C# rules include:

- four-space indentation;
- system usings sorted first;
- braces preferred;
- file-scoped namespaces preferred;
- accessibility modifiers required for non-interface members;
- readonly fields preferred;
- formatting diagnostic IDE0055 at warning severity.

XML/XAML/MSBuild files use two spaces. YAML/JSON use two spaces.

Because `EnforceCodeStyleInBuild=true` and warnings are errors, style rules can affect CI build success.

## `.gitattributes`

Repository text defaults to LF.

Explicit text patterns include C#, AXAML, project/MSBuild files, Markdown, YAML, JSON, shell scripts, and PowerShell. PowerShell is the intentional exception and uses CRLF.

Binary patterns include PNG, ICO, ICNS, PDF, and ZIP.

When adding a new binary artifact type, consider updating `.gitattributes` so Git never applies text normalization accidentally.

## `.gitignore`

Ignored categories include:

### Build/test output

- `bin/`;
- `obj/`;
- `artifacts/`;
- `TestResults/`;
- coverage output;
- `publish/`.

### IDE/OS state

- Visual Studio, JetBrains, selected VS Code logs;
- macOS `.DS_Store`;
- Windows thumbnail/desktop metadata.

### Local configuration/secrets

- `.env` and `.env.*`, except `.env.example`;
- `*.secrets.json`;
- `secrets.json`;
- `appsettings.Local.json`.

### Package output

- app/dmg/msix/AppImage/deb/rpm/NuGet artifacts.

### Runtime data

- logs;
- temporary/backup files;
- `ChronoDeskData/`.

### Signing/private material

- PFX/P12/private-key/certificate files.

Ignoring a file pattern does **not** make already-committed secrets safe. `scripts/check_repository_secrets.py` and manual security review remain necessary.

## `app.manifest`

The application manifest is referenced only by the App project and supplies Windows executable metadata/compatibility declarations understood by the Windows host/toolchain.

Changes to requested execution level, DPI behavior, compatibility declarations, or other manifest-level platform semantics should be treated as release-impacting and tested on Windows.

## Assembly test visibility

`src/ChronoDesk.App/Properties/AssemblyInfo.cs` and `src/ChronoDesk.Infrastructure/Properties/AssemblyInfo.cs` grant the test assembly access to selected internal implementation seams.

This is intentionally narrow: production types remain internal where appropriate while deterministic tests can reach platform/testability boundaries and awaitable UI operations.

Do not make implementation types public solely to test them.

## Debug vs Release

### Debug

- includes Avalonia diagnostics;
- uses ordinary development project publish defaults unless explicitly overridden.

### Release

- is the configuration used by CI build/test quality gates and release preflight;
- tagged package jobs additionally publish self-contained single-file binaries and suppress debug symbols through release workflow properties.

## Recommended local verification

```bash
dotnet --info
python3 scripts/check_markdown_links.py
python3 scripts/check_documentation_inventory.py
python3 scripts/check_repository_secrets.py
python3 -m unittest discover -s scripts/tests -p 'test_*.py'
dotnet restore ChronoDesk.sln
dotnet format ChronoDesk.sln --verify-no-changes --no-restore
dotnet build ChronoDesk.sln -c Release --no-restore
dotnet test ChronoDesk.sln -c Release --no-build
```

For a release candidate, follow `release.md` rather than relying only on the commands above.

## Configuration change checklist

When changing configuration:

- update this document;
- update setup/development instructions when prerequisites change;
- update CI/release workflows when SDK/package/build behavior changes;
- update `README.md` when public prerequisites or platform support changes;
- update `SECURITY.md` for new credential/tooling boundaries;
- update `repository-reference.md` for new or removed tracked files;
- run formatting/build/test/integrity gates on all supported CI operating systems.
