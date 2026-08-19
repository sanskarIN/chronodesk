using System.Globalization;
using System.Resources;

namespace ChronoDesk.App.Localization;

public static class UpdateStrings
{
    private static readonly ResourceManager ResourceManager =
        new("ChronoDesk.App.Localization.UpdateStrings", typeof(UpdateStrings).Assembly);

    public static string Get(string name) =>
        ResourceManager.GetString(name, CultureInfo.CurrentUICulture) ?? name;

    public static string TabUpdates => Get(nameof(TabUpdates));
    public static string UpdatesTitle => Get(nameof(UpdatesTitle));
    public static string UpdatesDescription => Get(nameof(UpdatesDescription));
    public static string CurrentVersionLabel => Get(nameof(CurrentVersionLabel));
    public static string OpenReleases => Get(nameof(OpenReleases));
    public static string UpdatePrivacyNote => Get(nameof(UpdatePrivacyNote));
    public static string ReleaseOpenError => Get(nameof(ReleaseOpenError));
}
