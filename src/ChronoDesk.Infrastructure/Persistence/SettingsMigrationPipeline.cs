using ChronoDesk.Core.Models;

namespace ChronoDesk.Infrastructure.Persistence;

public sealed class SettingsMigrationPipeline
{
    public AppSettings Migrate(AppSettings settings, int sourceSchemaVersion)
    {
        ArgumentNullException.ThrowIfNull(settings);

        if (sourceSchemaVersion < 0)
        {
            throw new InvalidDataException("Settings schema version cannot be negative.");
        }

        if (sourceSchemaVersion > AppSettings.CurrentSchemaVersion)
        {
            throw new InvalidDataException(
                "Settings were created by a newer unsupported ChronoDesk version.");
        }

        var migrated = settings;
        var version = sourceSchemaVersion;
        while (version < AppSettings.CurrentSchemaVersion)
        {
            migrated = version switch
            {
                0 => MigrateVersionZeroToOne(migrated),
                _ => throw new InvalidDataException(
                    $"No settings migration is available from schema version {version}."),
            };
            version++;
        }

        return migrated.Normalize();
    }

    private static AppSettings MigrateVersionZeroToOne(AppSettings settings) =>
        settings with { SchemaVersion = 1 };
}
