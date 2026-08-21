using System.Text;

namespace ChronoDesk.Infrastructure.Platform;

internal static class LinuxDesktopEntry
{
    internal static string Create(string executablePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executablePath);

        var quotedExecutable = QuoteExecToken(executablePath);
        return $"""
            [Desktop Entry]
            Type=Application
            Version=1.0
            Name=ChronoDesk
            Comment=Start ChronoDesk with the desktop session
            Exec={quotedExecutable} --background
            Terminal=false
            X-GNOME-Autostart-enabled=true
            """;
    }

    internal static string QuoteExecToken(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (value.Contains('=') || value.Contains('\r') || value.Contains('\n'))
        {
            throw new ArgumentException(
                "Linux desktop Exec executable paths cannot contain '=', carriage returns, or newlines.",
                nameof(value));
        }

        var builder = new StringBuilder(value.Length + 8);
        builder.Append('"');

        foreach (var character in value)
        {
            switch (character)
            {
                case '\\':
                    builder.Append('\\', 4);
                    break;
                case '"':
                case '$':
                case '`':
                    builder.Append('\\', 2);
                    builder.Append(character);
                    break;
                case '%':
                    builder.Append("%%");
                    break;
                default:
                    builder.Append(character);
                    break;
            }
        }

        builder.Append('"');
        return builder.ToString();
    }
}
