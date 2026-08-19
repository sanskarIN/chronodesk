using System.Globalization;
using System.Resources;

namespace ChronoDesk.App.Localization;

public static class SettingsExtras
{
    private static readonly ResourceManager ResourceManager =
        new("ChronoDesk.App.Localization.SettingsExtras", typeof(SettingsExtras).Assembly);

    private static string Get(string name) =>
        ResourceManager.GetString(name, CultureInfo.CurrentUICulture) ?? name;

    public static string TabUpdatesAbout => Get(nameof(TabUpdatesAbout));

    public static string UpdatesTitle => Get(nameof(UpdatesTitle));

    public static string UpdatesBody => Get(nameof(UpdatesBody));

    public static string OpenReleases => Get(nameof(OpenReleases));

    public static string UpdatePrivacyNote => Get(nameof(UpdatePrivacyNote));

    public static string AboutSectionTitle => Get(nameof(AboutSectionTitle));

    public static string AboutSectionBody => Get(nameof(AboutSectionBody));

    public static string OpenAbout => Get(nameof(OpenAbout));
}
