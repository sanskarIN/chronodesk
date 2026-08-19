using System.Text.Json;
using ChronoDesk.Infrastructure.Persistence;

namespace ChronoDesk.Tests;

public sealed class SettingsMigrationPipelineTests
{
    private readonly SettingsMigrationPipeline pipeline = new();

    [Fact]
    public void Migrate_LegacyDocumentAddsCurrentSchemaBeforeDeserialization()
    {
        using var document = JsonDocument.Parse(
            """
            {
              "showSeconds": false,
              "legacyPlaceholder": "still-present-for-a-future-step"
            }
            """);

        var migrated = pipeline.Migrate(document.RootElement, 0);

        Assert.Equal(1, migrated["schemaVersion"]?.GetValue<int>());
        Assert.Equal(false, migrated["showSeconds"]?.GetValue<bool>());
        Assert.Equal(
            "still-present-for-a-future-step",
            migrated["legacyPlaceholder"]?.GetValue<string>());
    }

    [Fact]
    public void Migrate_CurrentSchemaReturnsIndependentJsonObject()
    {
        using var document = JsonDocument.Parse(
            """
            {
              "schemaVersion": 1,
              "showSeconds": true
            }
            """);

        var migrated = pipeline.Migrate(document.RootElement, 1);

        Assert.Equal(1, migrated["schemaVersion"]?.GetValue<int>());
        Assert.Equal(true, migrated["showSeconds"]?.GetValue<bool>());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public void Migrate_RejectsInvalidOrFutureSchema(int schemaVersion)
    {
        using var document = JsonDocument.Parse("{ \"schemaVersion\": 1 }");

        Assert.Throws<InvalidDataException>(() =>
            pipeline.Migrate(document.RootElement, schemaVersion));
    }
}
