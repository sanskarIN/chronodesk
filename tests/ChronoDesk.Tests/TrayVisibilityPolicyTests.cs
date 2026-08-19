using ChronoDesk.App.Services;

namespace ChronoDesk.Tests;

public sealed class TrayVisibilityPolicyTests
{
    [Theory]
    [InlineData(false, true, true, true)]
    [InlineData(false, true, false, false)]
    [InlineData(false, false, true, false)]
    [InlineData(true, true, true, false)]
    public void ShouldHideOnClose_RequiresPreferenceAndReliableTray(
        bool allowClose,
        bool minimizeToTray,
        bool trayIntegrationAvailable,
        bool expected)
    {
        Assert.Equal(
            expected,
            TrayVisibilityPolicy.ShouldHideOnClose(
                allowClose,
                minimizeToTray,
                trayIntegrationAvailable));
    }

    [Theory]
    [InlineData(true, true, true, true)]
    [InlineData(true, true, false, false)]
    [InlineData(true, false, true, false)]
    [InlineData(false, true, true, false)]
    public void ShouldStartHidden_RequiresBackgroundRequestAndReliableTray(
        bool backgroundStart,
        bool minimizeToTray,
        bool trayIntegrationAvailable,
        bool expected)
    {
        Assert.Equal(
            expected,
            TrayVisibilityPolicy.ShouldStartHidden(
                backgroundStart,
                minimizeToTray,
                trayIntegrationAvailable));
    }
}
