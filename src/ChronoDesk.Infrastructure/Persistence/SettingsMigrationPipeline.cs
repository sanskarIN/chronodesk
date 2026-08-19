using System.Text.Json;
using System.Text.Json.Nodes;
using ChronoDesk.Core.Models;

namespace ChronoDesk.Infrastructure.Persistence;

public sealed class SettingsMigrationPipeline
{
    public JsonObject Migrate(JsonElement document, int sourceSchemaVersion)
    {
        if (document.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException("Settings document root must be a JSON object.");
        }

        if (sourceSchemaVersion < 0)
        {
            throw new InvalidDataException("Settings schema version cannot be negative.");
        }

        if (sourceSchemaVersion > AppSettings.CurrentSchemaVersion)
        {
            throw new InvalidDataException(
                "Settings were created by a newer unsupported ChronoDesk version.");
        }

        var migrated = JsonNode.Parse(document.GetRawText()) as JsonObject
            ?? throw new InvalidDataException("Settings document root must be a JSON object.");
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

        return migrated;
    }

    private static JsonObject MigrateVersionZeroToOne(JsonObject document)
    {
        document["schemaVersion"] = 1;
        return document;
    }
}
