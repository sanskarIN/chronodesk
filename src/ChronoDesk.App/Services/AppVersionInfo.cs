using System.Reflection;

namespace ChronoDesk.App.Services;

public static class AppVersionInfo
{
    public static string GetDisplayVersion(Assembly? assembly = null)
    {
        assembly ??= typeof(AppVersionInfo).Assembly;
        var informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;
        if (!string.IsNullOrWhiteSpace(informationalVersion))
        {
            var buildMetadataIndex = informationalVersion.IndexOf('+');
            return buildMetadataIndex >= 0
                ? informationalVersion[..buildMetadataIndex]
                : informationalVersion;
        }

        return assembly.GetName().Version?.ToString(3) ?? "development";
    }
}
