using System.Globalization;
using System.Resources;

namespace ChronoDesk.App.Localization;

public static class DiagnosticsStrings
{
    private static readonly ResourceManager ResourceManager =
        new("ChronoDesk.App.Localization.DiagnosticsStrings", typeof(DiagnosticsStrings).Assembly);

    public static string Get(string name) =>
        ResourceManager.GetString(name, CultureInfo.CurrentUICulture) ?? name;

    public static string DiagnosticsTitle => Get(nameof(DiagnosticsTitle));
    public static string DiagnosticsDescription => Get(nameof(DiagnosticsDescription));
    public static string AppVersionLabel => Get(nameof(AppVersionLabel));
    public static string OperatingSystemLabel => Get(nameof(OperatingSystemLabel));
    public static string FrameworkLabel => Get(nameof(FrameworkLabel));
    public static string ArchitectureLabel => Get(nameof(ArchitectureLabel));
    public static string DataDirectoryLabel => Get(nameof(DataDirectoryLabel));
    public static string SettingsPathLabel => Get(nameof(SettingsPathLabel));
    public static string LogPathLabel => Get(nameof(LogPathLabel));
    public static string DiagnosticsSharingNote => Get(nameof(DiagnosticsSharingNote));
}
