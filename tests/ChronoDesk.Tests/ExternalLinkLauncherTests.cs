using ChronoDesk.App;

namespace ChronoDesk.Tests;

public sealed class ExternalLinkLauncherTests
{
    [Theory]
    [InlineData("https://github.com/sanskarIN/chronodesk", "https")]
    [InlineData("mailto:sanskarin@outlook.in", "mailto")]
    public void TryGetAllowedUri_AllowsOnlyExpectedExternalSchemes(
        string value,
        string expectedScheme)
    {
        var allowed = ExternalLinkLauncher.TryGetAllowedUri(value, out var uri);

        Assert.True(allowed);
        Assert.Equal(expectedScheme, uri.Scheme);
    }

    [Theory]
    [InlineData("http://example.com")]
    [InlineData("file:///tmp/settings.json")]
    [InlineData("javascript:alert(1)")]
    [InlineData("relative/path")]
    [InlineData("")]
    public void TryGetAllowedUri_RejectsNonAllowlistedOrRelativeTargets(string value)
    {
        var allowed = ExternalLinkLauncher.TryGetAllowedUri(value, out _);

        Assert.False(allowed);
    }
}
