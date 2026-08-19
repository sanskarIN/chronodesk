# ChronoDesk Support

## Start here

Before requesting help:

1. read `README.md`;
2. read `docs/setup.md` and `docs/troubleshooting.md`;
3. check existing GitHub issues;
4. open **Settings → Data & Privacy → Local diagnostics** and note the relevant version/platform fields;
5. retry with the latest `main` commit or newest published release, depending on how you installed ChronoDesk.

The Local diagnostics panel is generated entirely on the current device. ChronoDesk does not upload those values.

## Support channels

- Support email: **supportramsandesh@gmail.com**
- Business email: **sanskarin@outlook.in**
- Business email: **sanskarin.business@gmail.com**
- GitHub profile: **https://github.com/sanskarIN**
- Repository issues: **https://github.com/sanskarIN/chronodesk/issues**

Use GitHub issues for reproducible public bugs and focused feature requests. Use email when the subject is not appropriate for a public issue.

For security vulnerabilities, follow `SECURITY.md` and do not post exploit details publicly.

## Information that helps

For a technical support request, include only the relevant fields from Local diagnostics:

- ChronoDesk version;
- Windows/macOS/Linux description;
- process architecture;
- .NET runtime description;
- desktop environment on Linux when relevant;
- exact steps that led to the problem;
- expected and actual behavior;
- minimal sanitized log lines if relevant.

The diagnostics panel also shows the data/settings/log paths to help you locate local files. **Do not post those paths blindly**: they can contain a local account name or private folder structure.

Do **not** send passwords, access tokens, private keys, complete private settings exports, complete unreviewed logs, or unrelated personal information.

## Logs

ChronoDesk writes structured JSONL logs at the local path shown by **Settings → Data & Privacy → Local diagnostics**.

Logs are designed to:

- redact common email and secret-assignment patterns;
- bound logged event/message lengths;
- record exception type rather than arbitrary raw exception messages;
- rotate near 1 MiB using collision-resistant archive names.

Redaction is defense-in-depth, not a guarantee that every possible sensitive string format can be recognized. Review every excerpt before sharing it.

## Updates

ChronoDesk does not automatically contact GitHub to check for releases. Settings → Updates displays the version from local application metadata and opens the official Releases page only when you activate the button.

If that button or an About/support link does not open, include the OS and desktop environment in the report. The application should remain usable when no browser/mail handler is available.

## Platform-specific limitations

System tray and chime behavior can depend on desktop services installed by the operating system. Linux desktop environments vary most.

If tray behavior differs from the main clock behavior, report whether the tray menu was available. ChronoDesk is designed not to hide its only window when reliable tray restoration is unavailable.

If chime behavior differs, include the configured cadence/quiet hours and available local sound facilities. Do not attach unrelated system logs.

## Funding

ChronoDesk remains fully usable without donating. If you want to support continued development:

[![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-sanskarIN-FFDD00?logo=buy-me-a-coffee&logoColor=000000)](https://buymeacoffee.com/sanskarIN)

Funding does not purchase priority security treatment or guaranteed response times.

**Made by the Sanskar**
