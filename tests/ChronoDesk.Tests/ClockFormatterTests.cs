using System.Globalization;
using ChronoDesk.Core.Models;
using ChronoDesk.Core.Services;

namespace ChronoDesk.Tests;

public sealed class ClockFormatterTests
{
    private readonly ClockFormatter formatter = new();

    [Fact]
    public void CreateSnapshot_FormatsTwentyFourHourTimeWithSeconds()
    {
        var instant = new DateTimeOffset(2026, 8, 19, 13, 5, 9, TimeSpan.Zero);
        var settings = new AppSettings
        {
            ClockFormat = ClockFormat.TwentyFourHour,
            ShowSeconds = true,
            ShowDate = false,
            ShowWeekday = false,
            ShowWeekNumber = false,
        };

        var snapshot = formatter.CreateSnapshot(
            instant,
            TimeZoneInfo.Utc,
            settings,
            CultureInfo.InvariantCulture);

        Assert.Equal("13:05:09", snapshot.TimeText);
        Assert.Equal(string.Empty, snapshot.DateText);
        Assert.Equal(string.Empty, snapshot.WeekdayText);
    }

    [Fact]
    public void CreateSnapshot_FormatsTwelveHourTimeWithoutSeconds()
    {
        var instant = new DateTimeOffset(2026, 8, 19, 13, 5, 9, TimeSpan.Zero);
        var settings = new AppSettings
        {
            ClockFormat = ClockFormat.TwelveHour,
            ShowSeconds = false,
            ShowDate = false,
            ShowWeekday = false,
            ShowWeekNumber = false,
        };

        var snapshot = formatter.CreateSnapshot(
            instant,
            TimeZoneInfo.Utc,
            settings,
            CultureInfo.InvariantCulture);

        Assert.Equal("01:05 PM", snapshot.TimeText);
    }

    [Fact]
    public void CreateSnapshot_ProvidesIsoWeekAndCalendarDetails()
    {
        var instant = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var settings = new AppSettings
        {
            ShowWeekNumber = true,
            ShowCalendarDetails = true,
        };

        var snapshot = formatter.CreateSnapshot(
            instant,
            TimeZoneInfo.Utc,
            settings,
            CultureInfo.InvariantCulture);

        Assert.Equal("Week 01", snapshot.WeekNumberText);
        Assert.Contains("ISO week 01", snapshot.CalendarDetailsText, StringComparison.Ordinal);
        Assert.Contains("UTC+00:00", snapshot.CalendarDetailsText, StringComparison.Ordinal);
    }
}
