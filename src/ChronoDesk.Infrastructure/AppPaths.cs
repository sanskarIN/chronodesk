namespace ChronoDesk.Infrastructure;

public static class AppPaths
{
    private const string DataDirectoryEnvironmentVariable = "CHRONODESK_DATA_DIR";

    public static string GetDataDirectory()
    {
        var configured = Environment.GetEnvironmentVariable(DataDirectoryEnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return Path.GetFullPath(configured.Trim());
        }

        var baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (string.IsNullOrWhiteSpace(baseDirectory))
        {
            baseDirectory = AppContext.BaseDirectory;
        }

        return Path.Combine(baseDirectory, "ChronoDesk");
    }

    public static string GetSettingsPath() => Path.Combine(GetDataDirectory(), "settings.json");

    public static string GetLogDirectory() => Path.Combine(GetDataDirectory(), "logs");
}
