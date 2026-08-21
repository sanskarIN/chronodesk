using System.Diagnostics.CodeAnalysis;
using ChronoDesk.Core.Models;

namespace ChronoDesk.Core.Services;

public sealed class ChimePolicy
{
    [SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Justification = "The policy is intentionally an instance service so it can remain an injectable application dependency.")]
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

        var lastLocal = TimeZoneInfo.ConvertTime(lastChimeInstant.Value, timeZone);
        return lastLocal.Year != local.Year
            || lastLocal.DayOfYear != local.DayOfYear
            || lastLocal.Hour != local.Hour
            || lastLocal.Minute != local.Minute;
    }

    private static bool IsIntervalBoundary(DateTimeOffset local, ChimeInterval interval)
    {
        return local.Second == 0
            && (interval switch
            {
                ChimeInterval.Hourly => local.Minute == 0,
                ChimeInterval.HalfHourly => local.Minute is 0 or 30,
                ChimeInterval.QuarterHourly => local.Minute % 15 == 0,
                _ => false,
            });
    }
}
