using ChronoDesk.Core.Models;
using ChronoDesk.Core.Services;

namespace ChronoDesk.Tests;

public sealed class ChimePolicyTests
{
    private readonly ChimePolicy policy = new();

    [Fact]
    public void ShouldChime_AllowsHourlyBoundaryOncePerAbsoluteMinute()
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

    [Fact]
    public void ShouldChime_AllowsSecondOccurrenceOfRepeatedFallBackHour()
    {
        var zone = CreateNorthAmericanStyleTestZone();
        var settings = new ChimeSettings
        {
            Enabled = true,
            Interval = ChimeInterval.Hourly,
        };
        var firstOneAm = new DateTimeOffset(2026, 11, 1, 5, 0, 0, TimeSpan.Zero);
        var repeatedOneAm = new DateTimeOffset(2026, 11, 1, 6, 0, 0, TimeSpan.Zero);

        Assert.Equal(1, TimeZoneInfo.ConvertTime(firstOneAm, zone).Hour);
        Assert.Equal(1, TimeZoneInfo.ConvertTime(repeatedOneAm, zone).Hour);
        Assert.True(policy.ShouldChime(firstOneAm, zone, settings, null));
        Assert.True(policy.ShouldChime(repeatedOneAm, zone, settings, firstOneAm));
    }

    private static TimeZoneInfo CreateNorthAmericanStyleTestZone()
    {
        var daylightStart = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(
            new DateTime(1, 1, 1, 2, 0, 0),
            3,
            2,
            DayOfWeek.Sunday);
        var daylightEnd = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(
            new DateTime(1, 1, 1, 2, 0, 0),
            11,
            1,
            DayOfWeek.Sunday);
        var rule = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(
            new DateTime(2026, 1, 1),
            new DateTime(2026, 12, 31),
            TimeSpan.FromHours(1),
            daylightStart,
            daylightEnd);

        return TimeZoneInfo.CreateCustomTimeZone(
            "ChronoDesk.Tests/FallBack",
            TimeSpan.FromHours(-5),
            "Test fallback zone",
            "Test standard time",
            "Test daylight time",
            [rule]);
    }
}
