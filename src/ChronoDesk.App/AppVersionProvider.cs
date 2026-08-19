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
        return NormalizeDisplayVersion(informationalVersion, assembly.GetName().Version);
    }

    internal static string NormalizeDisplayVersion(
        string? informationalVersion,
        Version? assemblyVersion)
    {
        if (!string.IsNullOrWhiteSpace(informationalVersion))
        {
            var trimmed = informationalVersion.Trim();
            var metadataSeparator = trimmed.IndexOf('+', StringComparison.Ordinal);
            return metadataSeparator >= 0
                ? trimmed[..metadataSeparator]
                : trimmed;
        }

        return assemblyVersion?.ToString(3) ?? "development";
    }
}
