using ChronoDesk.App.Services;

namespace ChronoDesk.Tests;

public sealed class ExternalUriLauncherTests
{
    [Theory]
    [InlineData("https://github.com/sanskarIN/chronodesk")]
    [InlineData("https://buymeacoffee.com/sanskarIN")]
    [InlineData("mailto:sanskarin@outlook.in")]
    public void IsAllowed_AcceptsSupportedDestinations(string value)
    {
        Assert.True(ExternalUriLauncher.IsAllowed(value));
    }

    [Theory]
    [InlineData("http://github.com/sanskarIN/chronodesk")]
    [InlineData("file:///tmp/chronodesk")]
    [InlineData("javascript:alert(1)")]
    [InlineData("https://user:password@example.com/path")]
    [InlineData("relative/path")]
    [InlineData("")]
    [InlineData("   ")]
    public void IsAllowed_RejectsUnsupportedOrUnsafeDestinations(string value)
    {
        Assert.False(ExternalUriLauncher.IsAllowed(value));
    }

    [Fact]
    public void ApprovedProductLinks_RemainAllowed()
    {
        var links = new[]
        {
            AppLinks.Repository,
            AppLinks.Releases,
            AppLinks.Funding,
            AppLinks.BusinessPrimary,
            AppLinks.BusinessSecondary,
            AppLinks.Support,
        };

        Assert.All(links, link => Assert.True(ExternalUriLauncher.IsAllowed(link)));
    }
}
