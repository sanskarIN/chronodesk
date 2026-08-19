using System.Security;

namespace ChronoDesk.Infrastructure.Platform;

public static class StartupRegistrationDocuments
{
    private const string BackgroundArgument = "--background";
    private const string MacLabel = "com.sanskar.chronodesk";

    public static string BuildWindowsCommand(string executablePath)
    {
        var path = NormalizeExecutablePath(executablePath);
        if (path.Contains('"'))
        {
            throw new ArgumentException(
                "Windows startup executable paths cannot contain a double-quote character.",
                nameof(executablePath));
        }

        return $"\"{path}\" {BackgroundArgument}";
    }

    public static string BuildMacLaunchAgent(string executablePath)
    {
        var path = NormalizeExecutablePath(executablePath);
        var escapedExecutable = SecurityElement.Escape(path)
            ?? throw new ArgumentException("Executable path could not be XML-escaped.", nameof(executablePath));

        return $"""
            <?xml version="1.0" encoding="UTF-8"?>
            <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
            <plist version="1.0">
              <dict>
                <key>Label</key>
                <string>{MacLabel}</string>
                <key>ProgramArguments</key>
                <array>
                  <string>{escapedExecutable}</string>
                  <string>{BackgroundArgument}</string>
                </array>
                <key>RunAtLoad</key>
                <true/>
              </dict>
            </plist>
            """;
    }

    public static string BuildLinuxDesktopEntry(string executablePath)
    {
        var path = NormalizeExecutablePath(executablePath);
        var quotedExecutable = QuoteDesktopExec(path);

        return $"""
            [Desktop Entry]
            Type=Application
            Version=1.0
            Name=ChronoDesk
            Comment=Start ChronoDesk with the desktop session
            Exec={quotedExecutable} {BackgroundArgument}
            Terminal=false
            X-GNOME-Autostart-enabled=true
            """;
    }

    public static string NormalizeExecutablePath(string executablePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executablePath);

        var path = executablePath.Trim();
        if (path.Any(char.IsControl))
        {
            throw new ArgumentException(
                "Startup executable paths cannot contain control characters.",
                nameof(executablePath));
        }

        return path;
    }

    private static string QuoteDesktopExec(string value)
    {
        var escaped = value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal)
            .Replace("`", "\\`", StringComparison.Ordinal)
            .Replace("$", "\\$", StringComparison.Ordinal);
        return $"\"{escaped}\"";
    }
}
