namespace ChronoDesk.Infrastructure;

public static class AppPaths
{
    private const string DataDirectoryEnvironmentVariable = "CHRONODESK_DATA_DIR";
    private const string SettingsFileName = "settings.json";
    private const string LogFileName = "chronodesk.log.jsonl";

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

    public static string GetSettingsPath() =>
        Path.Combine(GetDataDirectory(), SettingsFileName);

    public static string GetLogDirectory() =>
        Path.Combine(GetDataDirectory(), "logs");

    public static string GetLogPath() =>
        Path.Combine(GetLogDirectory(), LogFileName);
}
