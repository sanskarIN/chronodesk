using ChronoDesk.App.Services;

namespace ChronoDesk.Tests;

public sealed class AppDiagnosticsInfoTests
{
    [Fact]
    public void Capture_ReportsCanonicalLocalPathsAndRuntimeMetadata()
    {
        var diagnostics = AppDiagnosticsInfo.Capture();

        Assert.False(string.IsNullOrWhiteSpace(diagnostics.AppVersion));
        Assert.False(string.IsNullOrWhiteSpace(diagnostics.OperatingSystem));
        Assert.False(string.IsNullOrWhiteSpace(diagnostics.Framework));
        Assert.False(string.IsNullOrWhiteSpace(diagnostics.ProcessArchitecture));
        Assert.True(Path.IsPathFullyQualified(diagnostics.DataDirectory));
        Assert.Equal(
            Path.Combine(diagnostics.DataDirectory, "settings.json"),
            diagnostics.SettingsPath);
        Assert.Equal(
            Path.Combine(diagnostics.DataDirectory, "logs", "chronodesk.log.jsonl"),
            diagnostics.LogPath);
    }
}
