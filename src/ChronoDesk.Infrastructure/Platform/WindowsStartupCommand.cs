namespace ChronoDesk.Infrastructure.Platform;

internal static class WindowsStartupCommand
{
    internal static string Create(string executablePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executablePath);

        if (executablePath.Contains('"') || executablePath.Contains('\r') || executablePath.Contains('\n'))
        {
            throw new ArgumentException(
                "Windows startup executable paths cannot contain quotes, carriage returns, or newlines.",
                nameof(executablePath));
        }

        return $"\"{executablePath}\" --background";
    }

    internal static bool Matches(string? command, string executablePath)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            return false;
        }

        return string.Equals(
            command.Trim(),
            Create(executablePath),
            StringComparison.OrdinalIgnoreCase);
    }
}
