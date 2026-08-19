using ChronoDesk.Core.Models;

namespace ChronoDesk.Core.Abstractions;

public interface ITimeZoneCatalog
{
    IReadOnlyList<TimeZoneDescriptor> GetAll();

    TimeZoneInfo Resolve(string timeZoneId);

    IReadOnlyList<TimeZoneDescriptor> Search(string query, int limit = 50);
}
