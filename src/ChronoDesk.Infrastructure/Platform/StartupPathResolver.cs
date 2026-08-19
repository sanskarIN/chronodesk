namespace ChronoDesk.Infrastructure.Platform;

public static class StartupPathResolver
{
    private const string MacLaunchAgentName = "com.sanskar.chronodesk.plist";

    public static string GetMacLaunchAgentPath(string userProfile)
    {
        var home = NormalizeUserProfile(userProfile);
        return Path.Combine(home, "Library", "LaunchAgents", MacLaunchAgentName);
    }

    public static string GetLinuxAutostartPath(
        string userProfile,
        string? xdgConfigHome)
    {
        var home = NormalizeUserProfile(userProfile);
        var configHome = string.IsNullOrWhiteSpace(xdgConfigHome)
            || !Path.IsPathFullyQualified(xdgConfigHome.Trim())
            ? Path.Combine(home, ".config")
            : Path.GetFullPath(xdgConfigHome.Trim());

        return Path.Combine(configHome, "autostart", "chronodesk.desktop");
    }

    private static string NormalizeUserProfile(string userProfile)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userProfile);
        return Path.GetFullPath(userProfile.Trim());
    }
}
