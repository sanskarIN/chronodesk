# ChronoDesk Support

## Start here

Before requesting help:

1. read `README.md`;
2. read `docs/setup.md` and `docs/troubleshooting.md`;
3. check existing GitHub issues;
4. identify the exact ChronoDesk host/platform you are using;
5. confirm the relevant OS/browser/runtime and, for development problems, .NET 10 SDK/workload versions;
6. retry with the latest `main` commit or newest published release, depending on how you installed ChronoDesk.

## Support channels

- Support email: **supportramsandesh@gmail.com**
- Business email: **sanskarin@outlook.in**
- Business email: **sanskarin.business@gmail.com**
- GitHub profile: **https://github.com/sanskarIN**
- Repository issues: **https://github.com/sanskarIN/chronodesk/issues**

Use GitHub issues for reproducible public bugs and focused feature requests. Use email when the subject is not appropriate for a public issue.

For security vulnerabilities, follow `SECURITY.md` and do not post exploit details, signing credentials, or unpatched vulnerability information publicly.

## Supported host families

ChronoDesk source includes:

- Windows desktop;
- macOS desktop;
- Linux desktop;
- Android;
- iOS / iPhone;
- iPadOS / iPad;
- Browser / WebAssembly.

Not every desktop capability has a mobile/browser equivalent. Tray, desktop focus/mini window modes, start-with-system registration, and the current process-based native chime playback path are desktop capabilities. Android/iOS/iPadOS/Browser use the shared single-view clock/world-clock shell.

## Information that helps

For a technical support request, include only the non-sensitive details relevant to the problem:

- ChronoDesk version/commit;
- host: Windows, macOS, Linux, Android, iOS, iPadOS, or Browser/WebAssembly;
- OS/browser version;
- architecture/ABI such as x64, arm64, or browser-wasm when relevant;
- desktop environment on Linux when relevant;
- Android API/device/emulator class when relevant;
- iPhone/iPad simulator/device class when relevant;
- browser name/version and hosting method for WebAssembly issues;
- exact steps that led to the problem;
- expected and actual behavior;
- minimal sanitized log/console lines if relevant.

Do **not** send passwords, access tokens, Android keystores, Apple private keys/certificates, provisioning secrets, private device identifiers, full private files, or unrelated personal information.

## Logs

On filesystem-backed hosts, ChronoDesk can write structured JSONL logs under its local application-data area. Logs are designed to redact common email/secret patterns, but users should still review excerpts before sharing them.

Browser/mobile sandbox behavior can affect where/if such logs are persisted. Do not assume a browser log maps to a normal desktop filesystem path.

## Platform-specific limitations

### Desktop

System tray, startup registration, and chime behavior can depend on desktop services supplied by the operating system/environment. Linux desktop environments vary most. Include the desktop environment and available session/sound details when those features differ from main clock behavior.

### Android / iOS / iPadOS

Report lifecycle/orientation/touch issues with the device/emulator/simulator class and OS version. Do not publish account identifiers or production signing/provisioning material.

### Browser / WebAssembly

The Browser host must be served over HTTP(S) and runs inside the browser sandbox. Include the browser version, hosting method, and first sanitized console/network error when reporting startup failures. Browser storage persistence can differ from native application-data semantics.

## Development support

The solution contains workload-specific projects. A developer who changes only shared/Desktop code should normally restore/build:

```bash
dotnet restore src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj
dotnet restore tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj
dotnet build src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj -c Release --no-restore
dotnet test tests/ChronoDesk.Tests/ChronoDesk.Tests.csproj -c Release --no-restore
```

Install/use the Android, iOS, or `wasm-tools` workload only when building the corresponding host. See `docs/setup.md`.

## Funding

ChronoDesk remains fully usable without donating. If you want to support continued development:

[![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-sanskarIN-FFDD00?logo=buy-me-a-coffee&logoColor=000000)](https://buymeacoffee.com/sanskarIN)

Funding does not purchase priority security treatment or guaranteed response times.

**Made by the Sanskar**
