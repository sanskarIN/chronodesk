using ChronoDesk.Core.Abstractions;
using ChronoDesk.Core.Models;

namespace ChronoDesk.Infrastructure.Time;

public sealed class SystemTimeZoneCatalog : ITimeZoneCatalog
{
    private readonly IReadOnlyList<TimeZoneDescriptor> zones;

    public SystemTimeZoneCatalog()
    {
        zones = TimeZoneInfo.GetSystemTimeZones()
            .Select(zone => new TimeZoneDescriptor(zone.Id, zone.DisplayName, zone.BaseUtcOffset))
            .OrderBy(zone => zone.BaseUtcOffset)
            .ThenBy(zone => zone.DisplayName, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();
    }

    public IReadOnlyList<TimeZoneDescriptor> GetAll() => zones;

    public TimeZoneInfo Resolve(string timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId))
        {
            return TimeZoneInfo.Utc;
        }

        var id = timeZoneId.Trim();
        if (TryResolve(id, out var result))
        {
            return result;
        }

        if (TimeZoneInfo.TryConvertIanaIdToWindowsId(id, out var windowsId)
            && TryResolve(windowsId, out result))
        {
            return result;
        }

        if (TimeZoneInfo.TryConvertWindowsIdToIanaId(id, out var ianaId)
            && TryResolve(ianaId, out result))
        {
            return result;
        }

        return TimeZoneInfo.Utc;
    }

    public IReadOnlyList<TimeZoneDescriptor> Search(string query, int limit = 50)
    {
        var boundedLimit = Math.Clamp(limit, 1, 200);
        if (string.IsNullOrWhiteSpace(query))
        {
            return zones.Take(boundedLimit).ToArray();
        }

        var terms = query.Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return zones
            .Where(zone => terms.All(term =>
                zone.SearchText.Contains(term, StringComparison.CurrentCultureIgnoreCase)))
            .Take(boundedLimit)
            .ToArray();
    }

    private static bool TryResolve(string id, out TimeZoneInfo result)
    {
        try
        {
            result = TimeZoneInfo.FindSystemTimeZoneById(id);
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            result = TimeZoneInfo.Utc;
            return false;
        }
        catch (InvalidTimeZoneException)
        {
            result = TimeZoneInfo.Utc;
            return false;
        }
    }
}
