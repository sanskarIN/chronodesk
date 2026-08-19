using ChronoDesk.Core.Models;

namespace ChronoDesk.Core.Services;

public sealed class ChimePolicy
{
    public bool ShouldChime(
        DateTimeOffset instant,
        TimeZoneInfo timeZone,
        ChimeSettings settings,
        DateTimeOffset? lastChimeInstant)
    {
        ArgumentNullException.ThrowIfNull(timeZone);
        ArgumentNullException.ThrowIfNull(settings);

        if (!settings.Enabled)
        {
            return false;
        }

        var local = TimeZoneInfo.ConvertTime(instant, timeZone);
        if (settings.QuietHours.Contains(TimeOnly.FromDateTime(local.DateTime)))
        {
            return false;
        }

        if (!IsIntervalBoundary(local, settings.Interval))
        {
            return false;
        }

        if (lastChimeInstant is null)
        {
            return true;
        }

        return !IsSameUtcMinute(lastChimeInstant.Value, instant);
    }

    private static bool IsIntervalBoundary(DateTimeOffset local, ChimeInterval interval)
    {
        if (local.Second != 0)
        {
            return false;
        }

        return interval switch
        {
            ChimeInterval.Hourly => local.Minute == 0,
            ChimeInterval.HalfHourly => local.Minute is 0 or 30,
            ChimeInterval.QuarterHourly => local.Minute % 15 == 0,
            _ => false,
        };
    }

    private static bool IsSameUtcMinute(DateTimeOffset first, DateTimeOffset second)
    {
        var firstUtc = first.ToUniversalTime();
        var secondUtc = second.ToUniversalTime();
        return firstUtc.Year == secondUtc.Year
            && firstUtc.DayOfYear == secondUtc.DayOfYear
            && firstUtc.Hour == secondUtc.Hour
            && firstUtc.Minute == secondUtc.Minute;
    }
}
