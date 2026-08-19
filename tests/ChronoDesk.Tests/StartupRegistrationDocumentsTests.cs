using System.Xml.Linq;
using ChronoDesk.Infrastructure.Platform;

namespace ChronoDesk.Tests;

public sealed class StartupRegistrationDocumentsTests
{
    [Fact]
    public void BuildWindowsCommand_QuotesExecutableAndAddsBackgroundArgument()
    {
        var command = StartupRegistrationDocuments.BuildWindowsCommand(
            @"C:\Program Files\ChronoDesk\ChronoDesk.exe");

        Assert.Equal(
            "\"C:\\Program Files\\ChronoDesk\\ChronoDesk.exe\" --background",
            command);
    }

    [Fact]
    public void BuildWindowsCommand_RejectsEmbeddedDoubleQuote()
    {
        Assert.Throws<ArgumentException>(() =>
            StartupRegistrationDocuments.BuildWindowsCommand(
                "C:\\Apps\\Chrono\"Desk\\ChronoDesk.exe"));
    }

    [Fact]
    public void BuildMacLaunchAgent_ProducesValidXmlAndEscapesPath()
    {
        var document = StartupRegistrationDocuments.BuildMacLaunchAgent(
            "/Applications/ChronoDesk & Tools/ChronoDesk");
        var xml = XDocument.Parse(document);
        var strings = xml.Descendants("string").Select(element => element.Value).ToArray();

        Assert.Contains("com.sanskar.chronodesk", strings);
        Assert.Contains("/Applications/ChronoDesk & Tools/ChronoDesk", strings);
        Assert.Contains("--background", strings);
        Assert.Contains("&amp;", document, StringComparison.Ordinal);
    }

    [Fact]
    public void BuildLinuxDesktopEntry_QuotesAndEscapesReservedPathCharacters()
    {
        var document = StartupRegistrationDocuments.BuildLinuxDesktopEntry(
            "/home/user/Chrono Desk/$clock`test\\app");

        Assert.Contains("[Desktop Entry]", document, StringComparison.Ordinal);
        Assert.Contains("Exec=\"/home/user/Chrono Desk/\\$clock\\`test\\\\app\" --background", document, StringComparison.Ordinal);
        Assert.Contains("Terminal=false", document, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("/tmp/ChronoDesk\nInjected")]
    [InlineData("/tmp/ChronoDesk\rInjected")]
    [InlineData("/tmp/ChronoDesk\tInjected")]
    public void NormalizeExecutablePath_RejectsControlCharacters(string path)
    {
        Assert.Throws<ArgumentException>(() =>
            StartupRegistrationDocuments.NormalizeExecutablePath(path));
    }

    [Fact]
    public void NormalizeExecutablePath_TrimsOuterWhitespace()
    {
        Assert.Equal(
            "/opt/chronodesk/ChronoDesk",
            StartupRegistrationDocuments.NormalizeExecutablePath(
                "  /opt/chronodesk/ChronoDesk  "));
    }
}
