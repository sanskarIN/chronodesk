using ChronoDesk.Infrastructure.Platform;

namespace ChronoDesk.Tests;

public sealed class WindowsStartupCommandTests
{
    [Fact]
    public void Create_QuotesExecutableAndAddsBackgroundArgument()
    {
        var command = WindowsStartupCommand.Create(@"C:\Program Files\ChronoDesk\ChronoDesk.exe");

        Assert.Equal(
            "\"C:\\Program Files\\ChronoDesk\\ChronoDesk.exe\" --background",
            command);
    }

    [Fact]
    public void Matches_RequiresTheCanonicalCommand()
    {
        const string executablePath = @"C:\Apps\ChronoDesk\ChronoDesk.exe";
        var command = WindowsStartupCommand.Create(executablePath);

        Assert.True(WindowsStartupCommand.Matches(command, executablePath));
        Assert.True(WindowsStartupCommand.Matches($"  {command}  ", executablePath));
        Assert.False(
            WindowsStartupCommand.Matches(
                $"\"{executablePath}.old\" --background",
                executablePath));
        Assert.False(
            WindowsStartupCommand.Matches(
                $"\"{executablePath}\" --background --unexpected",
                executablePath));
    }

    [Theory]
    [InlineData("C:\\Apps\\Chrono\"Desk.exe")]
    [InlineData("C:\\Apps\\Chrono\nDesk.exe")]
    [InlineData("C:\\Apps\\Chrono\rDesk.exe")]
    public void Create_RejectsInvalidExecutablePaths(string executablePath)
    {
        Assert.Throws<ArgumentException>(() => WindowsStartupCommand.Create(executablePath));
    }
}
