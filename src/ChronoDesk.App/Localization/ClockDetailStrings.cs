using System.Globalization;
using System.Resources;
using ChronoDesk.Core.Models;

namespace ChronoDesk.App.Localization;

public static class ClockDetailStrings
{
    private static readonly ResourceManager ResourceManager =
        new("ChronoDesk.App.Localization.ClockDetailStrings", typeof(ClockDetailStrings).Assembly);

    public static string Get(string name) =>
        ResourceManager.GetString(name, CultureInfo.CurrentUICulture) ?? name;

    public static string WeekPrefix => Get(nameof(WeekPrefix));
    public static string DayPrefix => Get(nameof(DayPrefix));
    public static string IsoWeekPrefix => Get(nameof(IsoWeekPrefix));
    public static string UtcPrefix => Get(nameof(UtcPrefix));

    public static ClockDisplayLabels CreateLabels() =>
        new(WeekPrefix, DayPrefix, IsoWeekPrefix, UtcPrefix);
}
