using ChronoDesk.Infrastructure.Platform;
using ChronoDesk.Tests.Fakes;

namespace ChronoDesk.Tests;

public sealed class PlatformStartupManagerTests
{
    private const string WindowsRunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "ChronoDesk";

    [Fact]
    public async Task Windows_Enable_WritesQuotedBackgroundCommand()
    {
        var registry = new FakeStartupRegistry();
        var manager = CreateManager(StartupPlatform.Windows, registry: registry);

        await manager.SetEnabledAsync(true);

        Assert.Equal(
            "\"C:\\Program Files\\ChronoDesk\\ChronoDesk.exe\" --background",
            registry.GetCurrentUserString(WindowsRunKey, AppName));
        Assert.True(await manager.IsEnabledAsync());
    }

    [Fact]
    public async Task Windows_Disable_RemovesStartupValue()
    {
        var registry = new FakeStartupRegistry();
        registry.SeedValue(
            WindowsRunKey,
            AppName,
            "\"C:\\Program Files\\ChronoDesk\\ChronoDesk.exe\" --background");
        var manager = CreateManager(StartupPlatform.Windows, registry: registry);

        await manager.SetEnabledAsync(false);

        Assert.Null(registry.GetCurrentUserString(WindowsRunKey, AppName));
        Assert.False(await manager.IsEnabledAsync());
    }

    [Fact]
    public async Task MacOS_Enable_WritesEscapedLaunchAgent()
    {
        var fileSystem = new FakeStartupFileSystem();
        var manager = CreateManager(
            StartupPlatform.MacOS,
            fileSystem,
            executablePath: "/Applications/Chrono&Desk<Preview>/ChronoDesk");
        var expectedPath = Path.Combine(
            "/Users/tester",
            "Library",
            "LaunchAgents",
            "com.sanskar.chronodesk.plist");

        await manager.SetEnabledAsync(true);

        Assert.Contains(Path.GetDirectoryName(expectedPath)!, fileSystem.Directories);
        var content = fileSystem.ReadAllText(expectedPath);
        Assert.Contains("/Applications/Chrono&amp;Desk&lt;Preview&gt;/ChronoDesk", content, StringComparison.Ordinal);
        Assert.Contains("<string>--background</string>", content, StringComparison.Ordinal);
        Assert.True(await manager.IsEnabledAsync());
    }

    [Fact]
    public async Task Linux_Enable_UsesXdgConfigHomeAndQuotedExec()
    {
        var fileSystem = new FakeStartupFileSystem();
        var manager = CreateManager(
            StartupPlatform.Linux,
            fileSystem,
            executablePath: "/opt/Chrono Desk/chronodesk",
            xdgConfigHome: "/var/tmp/chronodesk-config");
        var expectedPath = Path.Combine(
            "/var/tmp/chronodesk-config",
            "autostart",
            "chronodesk.desktop");

        await manager.SetEnabledAsync(true);

        Assert.Contains(Path.GetDirectoryName(expectedPath)!, fileSystem.Directories);
        var content = fileSystem.ReadAllText(expectedPath);
        Assert.Contains("Exec=\"/opt/Chrono Desk/chronodesk\" --background", content, StringComparison.Ordinal);
        Assert.Contains("X-GNOME-Autostart-enabled=true", content, StringComparison.Ordinal);
        Assert.True(await manager.IsEnabledAsync());
    }

    [Fact]
    public async Task UnsupportedPlatform_IsNotSupportedAndRejectsWrites()
    {
        var manager = CreateManager(StartupPlatform.Unsupported);

        Assert.False(manager.IsSupported);
        Assert.False(await manager.IsEnabledAsync());
        await Assert.ThrowsAsync<PlatformNotSupportedException>(
            () => manager.SetEnabledAsync(true));
    }

    private static PlatformStartupManager CreateManager(
        StartupPlatform platform,
        FakeStartupFileSystem? fileSystem = null,
        FakeStartupRegistry? registry = null,
        string executablePath = "C:\\Program Files\\ChronoDesk\\ChronoDesk.exe",
        string userProfilePath = "/Users/tester",
        string? xdgConfigHome = null) =>
        new(
            executablePath,
            platform,
            fileSystem ?? new FakeStartupFileSystem(),
            registry ?? new FakeStartupRegistry(),
            userProfilePath,
            xdgConfigHome);
}
