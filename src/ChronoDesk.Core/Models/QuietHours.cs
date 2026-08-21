namespace ChronoDesk.Core.Models;

public sealed record QuietHours
{
    public bool Enabled { get; init; }

    public TimeOnly Start { get; init; } = new(22, 0);

    public TimeOnly End { get; init; } = new(7, 0);

    public bool Contains(TimeOnly time)
    {
        return Enabled
            && Start != End
            && (Start < End
                ? time >= Start && time < End
                : time >= Start || time < End);
    }
}
