namespace ChronoDesk.Core.Models;

public sealed record ClockDisplayLabels(
    string WeekPrefix,
    string DayPrefix,
    string IsoWeekPrefix,
    string UtcPrefix)
{
    public static ClockDisplayLabels English { get; } =
        new("Week", "Day", "ISO week", "UTC");
}
