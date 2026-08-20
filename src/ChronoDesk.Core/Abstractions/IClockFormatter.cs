using System.Globalization;
using ChronoDesk.Core.Models;

namespace ChronoDesk.Core.Abstractions;

public interface IClockFormatter
{
    ClockSnapshot CreateSnapshot(
        DateTimeOffset instant,
        TimeZoneInfo timeZone,
        AppSettings settings,
        CultureInfo? culture = null);
}
