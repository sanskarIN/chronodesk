using ChronoDesk.Core.Abstractions;
using ChronoDesk.Core.Models;

namespace ChronoDesk.Tests.Fakes;

internal sealed class UtcTimeZoneCatalog : ITimeZoneCatalog
{
    private static readonly TimeZoneDescriptor Utc = new(
        TimeZoneInfo.Utc.Id,
        TimeZoneInfo.Utc.DisplayName,
        TimeSpan.Zero);

    public IReadOnlyList<TimeZoneDescriptor> GetAll() => [Utc];

    public TimeZoneInfo Resolve(string timeZoneId) => TimeZoneInfo.Utc;

    public IReadOnlyList<TimeZoneDescriptor> Search(string query, int limit = 50) => [Utc];
}
