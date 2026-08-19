namespace ChronoDesk.Core.Models;

public sealed record ClockSnapshot(
    string TimeText,
    string DateText,
    string WeekdayText,
    string WeekNumberText,
    string CalendarDetailsText,
    DateTimeOffset LocalInstant,
    string TimeZoneDisplayName);
