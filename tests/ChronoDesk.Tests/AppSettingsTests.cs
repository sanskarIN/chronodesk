using ChronoDesk.Core.Models;

namespace ChronoDesk.Tests;

public sealed class AppSettingsTests
{
    [Fact]
    public void Normalize_ClampsVisualValuesAndRemovesInvalidClocks()
    {
        var settings = new AppSettings
        {
            ClockFontSize = double.PositiveInfinity,
            ContentSpacing = -100,
            FontFamilyName = "  ",
            WorldClocks =
            [
                new WorldClock("", "Bad", "UTC"),
                new WorldClock("valid", "UTC", "UTC"),
                new WorldClock("valid", "Duplicate id", "UTC"),
            ],
        };

        var normalized = settings.Normalize();

        Assert.Equal(96, normalized.ClockFontSize);
        Assert.Equal(4, normalized.ContentSpacing);
        Assert.Equal("Inter", normalized.FontFamilyName);
        Assert.Single(normalized.WorldClocks);
        Assert.Equal("valid", normalized.WorldClocks[0].Id);
    }

    [Fact]
    public void Normalize_AlwaysKeepsAtLeastOneClock()
    {
        var settings = new AppSettings { WorldClocks = [] };

        var normalized = settings.Normalize();

        Assert.Single(normalized.WorldClocks);
        Assert.Equal("Local", normalized.WorldClocks[0].DisplayName);
    }

    [Fact]
    public void Normalize_LimitsWorldClockCount()
    {
        var clocks = Enumerable.Range(0, 40)
            .Select(index => new WorldClock(index.ToString(), $"Clock {index}", "UTC"))
            .ToList();
        var settings = new AppSettings { WorldClocks = clocks };

        var normalized = settings.Normalize();

        Assert.Equal(24, normalized.WorldClocks.Count);
    }
}
