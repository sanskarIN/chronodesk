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
        var clocks = Enumerable.Range(0, AppSettings.MaximumWorldClockCount + 16)
            .Select(index => new WorldClock(
                index.ToString(),
                $"Clock {index}",
                $"Test/Zone-{index}"))
            .ToList();
        var settings = new AppSettings { WorldClocks = clocks };

        var normalized = settings.Normalize();

        Assert.Equal(AppSettings.MaximumWorldClockCount, normalized.WorldClocks.Count);
    }

    [Fact]
    public void Normalize_RemovesDuplicateTimezoneIdsCaseInsensitively()
    {
        var settings = new AppSettings
        {
            WorldClocks =
            [
                new WorldClock("first", "UTC", "Etc/UTC"),
                new WorldClock("second", "Duplicate UTC", "etc/utc"),
                new WorldClock("third", "Another zone", "Test/Other"),
            ],
        };

        var normalized = settings.Normalize();

        Assert.Equal(2, normalized.WorldClocks.Count);
        Assert.Equal("first", normalized.WorldClocks[0].Id);
        Assert.Equal("third", normalized.WorldClocks[1].Id);
    }

    [Fact]
    public void Normalize_RepairsRuntimeNullNestedValuesAndInvalidEnums()
    {
        var settings = new AppSettings
        {
            ClockFormat = (ClockFormat)999,
            Theme = (ThemeMode)999,
            Layout = (ClockLayout)999,
            Chime = null!,
            WorldClocks = null!,
        };

        var normalized = settings.Normalize();

        Assert.Equal(ClockFormat.TwentyFourHour, normalized.ClockFormat);
        Assert.Equal(ThemeMode.System, normalized.Theme);
        Assert.Equal(ClockLayout.Centered, normalized.Layout);
        Assert.NotNull(normalized.Chime);
        Assert.NotNull(normalized.Chime.QuietHours);
        Assert.Single(normalized.WorldClocks);
    }

    [Fact]
    public void Normalize_BoundsAndFlattensImportedText()
    {
        var longLabel = new string('L', 220) + "\nInjected line";
        var longZone = new string('Z', 300);
        var longFont = new string('F', 160);
        var settings = new AppSettings
        {
            FontFamilyName = longFont,
            WorldClocks =
            [
                new WorldClock("clock\r\nid", longLabel, longZone),
            ],
        };

        var normalized = settings.Normalize();
        var clock = Assert.Single(normalized.WorldClocks);

        Assert.InRange(normalized.FontFamilyName.Length, 1, 120);
        Assert.InRange(clock.Id.Length, 1, 128);
        Assert.InRange(clock.DisplayName.Length, 1, 160);
        Assert.InRange(clock.TimeZoneId.Length, 1, 256);
        Assert.DoesNotContain('\r', clock.Id);
        Assert.DoesNotContain('\n', clock.Id);
        Assert.DoesNotContain('\r', clock.DisplayName);
        Assert.DoesNotContain('\n', clock.DisplayName);
    }
}
