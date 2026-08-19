using System.Runtime.InteropServices;
using ChronoDesk.Core.Abstractions;
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
    public static AppDiagnosticsInfo Capture(ISettingsStore settingsStore)
    {
        ArgumentNullException.ThrowIfNull(settingsStore);

        return new AppDiagnosticsInfo(
            AppVersionInfo.GetDisplayVersion(),
            RuntimeInformation.OSDescription.Trim(),
            RuntimeInformation.FrameworkDescription.Trim(),
            RuntimeInformation.ProcessArchitecture.ToString(),
            AppPaths.GetDataDirectory(),
            settingsStore.SettingsPath,
            AppPaths.GetLogPath());
    }
}
