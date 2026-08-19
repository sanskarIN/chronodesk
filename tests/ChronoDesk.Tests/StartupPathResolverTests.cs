using ChronoDesk.Infrastructure.Platform;

namespace ChronoDesk.Tests;

public sealed class StartupPathResolverTests
{
    [Fact]
    public void GetMacLaunchAgentPath_UsesUserProfile()
    {
        var home = CreateAbsoluteTestDirectory("mac-home");

        var path = StartupPathResolver.GetMacLaunchAgentPath(home);

        Assert.Equal(
            Path.Combine(home, "Library", "LaunchAgents", "com.sanskar.chronodesk.plist"),
            path);
    }

    [Fact]
    public void GetLinuxAutostartPath_UsesAbsoluteXdgConfigHome()
    {
        var home = CreateAbsoluteTestDirectory("linux-home");
        var config = CreateAbsoluteTestDirectory("xdg-config");

        var path = StartupPathResolver.GetLinuxAutostartPath(home, config);

        Assert.Equal(Path.Combine(config, "autostart", "chronodesk.desktop"), path);
    }

    [Fact]
    public void GetLinuxAutostartPath_IgnoresRelativeXdgConfigHome()
    {
        var home = CreateAbsoluteTestDirectory("linux-home-relative");

        var path = StartupPathResolver.GetLinuxAutostartPath(home, "relative-config");

        Assert.Equal(
            Path.Combine(home, ".config", "autostart", "chronodesk.desktop"),
            path);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void StartupPaths_RejectMissingUserProfile(string home)
    {
        Assert.Throws<ArgumentException>(() =>
            StartupPathResolver.GetMacLaunchAgentPath(home));
        Assert.Throws<ArgumentException>(() =>
            StartupPathResolver.GetLinuxAutostartPath(home, null));
    }

    private static string CreateAbsoluteTestDirectory(string name) =>
        Path.GetFullPath(Path.Combine(Path.GetTempPath(), "ChronoDesk.Tests", name));
}
