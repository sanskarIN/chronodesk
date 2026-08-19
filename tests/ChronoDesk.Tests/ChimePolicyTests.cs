using ChronoDesk.Core.Models;
using ChronoDesk.Core.Services;

namespace ChronoDesk.Tests;

public sealed class ChimePolicyTests
{
    private readonly ChimePolicy policy = new();

    [Fact]
    public void ShouldChime_AllowsHourlyBoundaryOnce()
    {
        var settings = new ChimeSettings { Enabled = true, Interval = ChimeInterval.Hourly };
        var instant = new DateTimeOffset(2026, 8, 19, 10, 0, 0, TimeSpan.Zero);

        Assert.True(policy.ShouldChime(instant, TimeZoneInfo.Utc, settings, null));
        Assert.False(policy.ShouldChime(instant, TimeZoneInfo.Utc, settings, instant));
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(15, true)]
    [InlineData(30, true)]
    [InlineData(45, true)]
    [InlineData(10, false)]
    public void ShouldChime_RespectsQuarterHourBoundaries(int minute, bool expected)
    {
        var settings = new ChimeSettings
        {
            Enabled = true,
            Interval = ChimeInterval.QuarterHourly,
        };
        var instant = new DateTimeOffset(2026, 8, 19, 10, minute, 0, TimeSpan.Zero);

        Assert.Equal(expected, policy.ShouldChime(instant, TimeZoneInfo.Utc, settings, null));
    }

    [Fact]
    public void ShouldChime_SuppressesQuietHours()
    {
        var settings = new ChimeSettings
        {
            Enabled = true,
            Interval = ChimeInterval.Hourly,
            QuietHours = new QuietHours
            {
                Enabled = true,
                Start = new TimeOnly(22, 0),
                End = new TimeOnly(7, 0),
            },
        };
        var instant = new DateTimeOffset(2026, 8, 19, 23, 0, 0, TimeSpan.Zero);

        Assert.False(policy.ShouldChime(instant, TimeZoneInfo.Utc, settings, null));
    }
}
