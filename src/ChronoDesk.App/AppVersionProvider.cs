using System.Reflection;

namespace ChronoDesk.App;

internal static class AppVersionProvider
{
    public static string GetDisplayVersion(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;
        if (!string.IsNullOrWhiteSpace(informationalVersion))
        {
            var metadataSeparator = informationalVersion.IndexOf('+', StringComparison.Ordinal);
            return metadataSeparator >= 0
                ? informationalVersion[..metadataSeparator]
                : informationalVersion;
        }

        return assembly.GetName().Version?.ToString(3) ?? "development";
    }
}
