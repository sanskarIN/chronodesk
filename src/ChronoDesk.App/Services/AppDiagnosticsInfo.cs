using System.Runtime.InteropServices;
using ChronoDesk.Infrastructure;

namespace ChronoDesk.App.Services;

public sealed record AppDiagnosticsInfo(
    string AppVersion,
    string OperatingSystem,
    string Framework,
    string ProcessArchitecture,
    string DataDirectory,
    string SettingsPath,
    string LogPath)
{
    public static AppDiagnosticsInfo Capture() =>
        new(
            AppVersionInfo.GetDisplayVersion(),
            RuntimeInformation.OSDescription.Trim(),
            RuntimeInformation.FrameworkDescription.Trim(),
            RuntimeInformation.ProcessArchitecture.ToString(),
            AppPaths.GetDataDirectory(),
            AppPaths.GetSettingsPath(),
            AppPaths.GetLogPath());
}
