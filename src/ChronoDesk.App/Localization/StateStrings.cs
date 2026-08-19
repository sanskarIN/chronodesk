using System.Globalization;
using System.Resources;

namespace ChronoDesk.App.Localization;

public static class StateStrings
{
    private static readonly ResourceManager ResourceManager =
        new("ChronoDesk.App.Localization.StateStrings", typeof(StateStrings).Assembly);

    public static string Get(string name) =>
        ResourceManager.GetString(name, CultureInfo.CurrentUICulture) ?? name;

    public static string Format(string name, params object?[] args) =>
        string.Format(CultureInfo.CurrentCulture, Get(name), args);

    public static string LoadingLocalData => Get(nameof(LoadingLocalData));
    public static string WorldClockCountOne => Get(nameof(WorldClockCountOne));
    public static string WorldClockCountManyFormat => Get(nameof(WorldClockCountManyFormat));
}
