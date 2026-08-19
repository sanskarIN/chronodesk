namespace ChronoDesk.Core.Models;

public enum ChimeInterval
{
    Hourly,
    HalfHourly,
    QuarterHourly,
}

public sealed record ChimeSettings
{
    public bool Enabled { get; init; }

    public ChimeInterval Interval { get; init; } = ChimeInterval.Hourly;

    public QuietHours QuietHours { get; init; } = new();
}
