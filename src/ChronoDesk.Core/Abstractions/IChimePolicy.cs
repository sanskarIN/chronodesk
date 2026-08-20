using ChronoDesk.Core.Models;

namespace ChronoDesk.Core.Abstractions;

public interface IChimePolicy
{
    bool ShouldChime(
        DateTimeOffset instant,
        TimeZoneInfo timeZone,
        ChimeSettings settings,
        DateTimeOffset? lastChimeInstant);
}
