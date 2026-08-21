using ChronoDesk.Infrastructure.Platform;

namespace ChronoDesk.Tests;

public sealed class LinuxDesktopEntryTests
{
    [Fact]
    public void Create_QuotesAndEscapesExecutablePath()
    {
        var content = LinuxDesktopEntry.Create("/opt/Chrono Desk/$clock`100%/chrono\\desk");
        var execLine = GetExecLine(content);
        var expected =
            "Exec=\"/opt/Chrono Desk/"
            + new string('\\', 2)
            + "$clock"
            + new string('\\', 2)
            + "`100%%/chrono"
            + new string('\\', 4)
            + "desk\" --background";

        Assert.Equal(expected, execLine);
    }

    [Fact]
    public void Create_EscapesQuotesInExecutablePath()
    {
        var content = LinuxDesktopEntry.Create("/opt/Chrono \"Desk\"/chronodesk");
        var execLine = GetExecLine(content);
        var expected =
            "Exec=\"/opt/Chrono "
            + new string('\\', 2)
            + "\"Desk"
            + new string('\\', 2)
            + "\"/chronodesk\" --background";

        Assert.Equal(expected, execLine);
    }

    [Theory]
    [InlineData("/opt/chrono=desk")]
    [InlineData("/opt/chrono\ndesk")]
    [InlineData("/opt/chrono\rdesk")]
    public void Create_RejectsUnrepresentableExecutablePaths(string executablePath)
    {
        Assert.Throws<ArgumentException>(() => LinuxDesktopEntry.Create(executablePath));
    }

    private static string GetExecLine(string content) =>
        content
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Single(line => line.StartsWith("Exec=", StringComparison.Ordinal));
}
