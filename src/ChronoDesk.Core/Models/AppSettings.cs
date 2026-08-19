namespace ChronoDesk.Core.Models;

public sealed record AppSettings
{
    public const int CurrentSchemaVersion = 1;

    public int SchemaVersion { get; init; } = CurrentSchemaVersion;

    public bool IsFirstRun { get; init; } = true;

    public ClockFormat ClockFormat { get; init; } = ClockFormat.TwentyFourHour;

    public bool ShowSeconds { get; init; } = true;

    public bool ShowDate { get; init; } = true;

    public bool ShowWeekday { get; init; } = true;

    public bool ShowWeekNumber { get; init; } = true;

    public bool ShowCalendarDetails { get; init; }

    public ThemeMode Theme { get; init; } = ThemeMode.System;

    public ClockLayout Layout { get; init; } = ClockLayout.Centered;

    public string FontFamilyName { get; init; } = "Inter";

    public double ClockFontSize { get; init; } = 96;

    public double ContentSpacing { get; init; } = 16;

    public bool ReducedMotion { get; init; }

    public bool HighContrast { get; init; }

    public bool AlwaysOnTop { get; init; }

    public bool StartWithSystem { get; init; }

    public bool MinimizeToTray { get; init; } = true;

    public ChimeSettings Chime { get; init; } = new();

    public List<WorldClock> WorldClocks { get; init; } =
    [
        WorldClock.Create("Local", TimeZoneInfo.Local.Id),
        WorldClock.Create("UTC", TimeZoneInfo.Utc.Id),
    ];

    public AppSettings Normalize()
    {
        var fontSize = double.IsFinite(ClockFontSize) ? Math.Clamp(ClockFontSize, 42, 240) : 96;
        var spacing = double.IsFinite(ContentSpacing) ? Math.Clamp(ContentSpacing, 4, 48) : 16;
        var font = string.IsNullOrWhiteSpace(FontFamilyName) ? "Inter" : FontFamilyName.Trim();
        var clocks = WorldClocks
            .Where(clock => !string.IsNullOrWhiteSpace(clock.Id)
                && !string.IsNullOrWhiteSpace(clock.DisplayName)
                && !string.IsNullOrWhiteSpace(clock.TimeZoneId))
            .DistinctBy(clock => clock.Id, StringComparer.Ordinal)
            .Take(24)
            .ToList();

        if (clocks.Count == 0)
        {
            clocks.Add(WorldClock.Create("Local", TimeZoneInfo.Local.Id));
        }

        return this with
        {
            SchemaVersion = CurrentSchemaVersion,
            ClockFontSize = fontSize,
            ContentSpacing = spacing,
            FontFamilyName = font,
            WorldClocks = clocks,
        };
    }
}
