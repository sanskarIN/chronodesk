using System.Xml.Linq;
using ChronoDesk.Infrastructure.Platform;

namespace ChronoDesk.Tests;

public sealed class MacLaunchAgentPlistTests
{
    [Fact]
    public void Create_PreservesExecutablePathAsProgramArgument()
    {
        const string executablePath = "/Applications/Chrono & Desk.app/Contents/MacOS/Chrono<Desk>";

        var document = XDocument.Parse(MacLaunchAgentPlist.Create(executablePath));
        var stringValues = document
            .Descendants("string")
            .Select(element => element.Value)
            .ToArray();

        Assert.Contains("com.sanskar.chronodesk", stringValues);
        Assert.Contains(executablePath, stringValues);
        Assert.Contains("--background", stringValues);
    }

    [Fact]
    public void Create_ProducesLaunchAgentWithRunAtLoad()
    {
        var document = XDocument.Parse(MacLaunchAgentPlist.Create("/Applications/ChronoDesk"));

        Assert.Contains(
            document.Descendants("key"),
            element => string.Equals(element.Value, "RunAtLoad", StringComparison.Ordinal));
        Assert.Single(document.Descendants("true"));
    }
}
