namespace ChronoDesk.Core.Models;

public sealed record AppSettings
{
    public const int CurrentSchemaVersion = 1;
    private const int MaximumFontFamilyLength = 120;
    private const int MaximumWorldClockIdLength = 128;
    private const int MaximumWorldClockLabelLength = 160;
    private const int MaximumTimeZoneIdLength = 256;

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
        var font = string.IsNullOrWhiteSpace(FontFamilyName)
            ? "Inter"
            : NormalizeSingleLine(FontFamilyName, MaximumFontFamilyLength);
        var clockFormat = Enum.IsDefined(ClockFormat)
            ? ClockFormat
            : ClockFormat.TwentyFourHour;
        var theme = Enum.IsDefined(Theme)
            ? Theme
            : ThemeMode.System;
        var layout = Enum.IsDefined(Layout)
            ? Layout
            : ClockLayout.Centered;
        var sourceChime = Chime is null ? new ChimeSettings() : Chime;
        var quietHours = sourceChime.QuietHours is null ? new QuietHours() : sourceChime.QuietHours;
        var chime = sourceChime with
        {
            Interval = Enum.IsDefined(sourceChime.Interval)
                ? sourceChime.Interval
                : ChimeInterval.Hourly,
            QuietHours = quietHours,
        };
        var sourceClocks = WorldClocks ?? [];
        var clocks = sourceClocks
            .Where(clock => clock is not null
                && !string.IsNullOrWhiteSpace(clock.Id)
                && !string.IsNullOrWhiteSpace(clock.DisplayName)
                && !string.IsNullOrWhiteSpace(clock.TimeZoneId))
            .Select(clock => new WorldClock(
                NormalizeSingleLine(clock.Id, MaximumWorldClockIdLength),
                NormalizeSingleLine(clock.DisplayName, MaximumWorldClockLabelLength),
                NormalizeSingleLine(clock.TimeZoneId, MaximumTimeZoneIdLength)))
            .Where(clock => clock.Id.Length > 0
                && clock.DisplayName.Length > 0
                && clock.TimeZoneId.Length > 0)
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
            ClockFormat = clockFormat,
            Theme = theme,
            Layout = layout,
            ClockFontSize = fontSize,
            ContentSpacing = spacing,
            FontFamilyName = font,
            Chime = chime,
            WorldClocks = clocks,
        };
    }

    private static string NormalizeSingleLine(string value, int maximumLength)
    {
        var normalized = new string(
            value.Trim()
                .Select(character => char.IsControl(character) ? ' ' : character)
                .ToArray());

        if (normalized.Length <= maximumLength)
        {
            return normalized;
        }

        var length = maximumLength;
        if (length > 0
            && char.IsHighSurrogate(normalized[length - 1])
            && length < normalized.Length
            && char.IsLowSurrogate(normalized[length]))
        {
            length--;
        }

        return normalized[..length].TrimEnd();
    }
}
