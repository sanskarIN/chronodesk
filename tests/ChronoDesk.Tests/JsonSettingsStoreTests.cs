using System.Text.Json;
using ChronoDesk.Core.Models;
using ChronoDesk.Infrastructure.Logging;
using ChronoDesk.Infrastructure.Persistence;

namespace ChronoDesk.Tests;

public sealed class JsonSettingsStoreTests
{
    [Fact]
    public async Task SaveAndLoad_RoundTripsNormalizedSettings()
    {
        var root = CreateTemporaryDirectory();
        try
        {
            var settingsPath = Path.Combine(root, "settings.json");
            var store = new JsonSettingsStore(
                new SafeFileLogger(Path.Combine(root, "logs")),
                settingsPath);
            var settings = new AppSettings
            {
                IsFirstRun = false,
                ClockFormat = ClockFormat.TwelveHour,
                ShowSeconds = false,
                ClockFontSize = 120,
                WorldClocks = [new WorldClock("utc", "UTC", "UTC")],
            };

            await store.SaveAsync(settings);
            var loaded = await store.LoadAsync();

            Assert.False(loaded.IsFirstRun);
            Assert.Equal(ClockFormat.TwelveHour, loaded.ClockFormat);
            Assert.False(loaded.ShowSeconds);
            Assert.Equal(120, loaded.ClockFontSize);
            Assert.Single(loaded.WorldClocks);
            Assert.Equal("UTC", loaded.WorldClocks[0].TimeZoneId);
        }
        finally
        {
            DeleteTemporaryDirectory(root);
        }
    }

    [Fact]
    public async Task ImportAndExport_RoundTripsPortableDocument()
    {
        var root = CreateTemporaryDirectory();
        try
        {
            var store = new JsonSettingsStore(
                new SafeFileLogger(Path.Combine(root, "logs")),
                Path.Combine(root, "settings.json"));
            var exportPath = Path.Combine(root, "backup", "chronodesk.json");
            var settings = new AppSettings { Theme = ThemeMode.Dark, ContentSpacing = 22 };

            await store.ExportAsync(settings, exportPath);
            var imported = await store.ImportAsync(exportPath);

            Assert.Equal(ThemeMode.Dark, imported.Theme);
            Assert.Equal(22, imported.ContentSpacing);
        }
        finally
        {
            DeleteTemporaryDirectory(root);
        }
    }

    [Fact]
    public async Task ImportAsync_MigratesMissingSchemaVersionFromLegacyDocument()
    {
        var root = CreateTemporaryDirectory();
        try
        {
            var sourcePath = Path.Combine(root, "legacy-no-schema.json");
            await File.WriteAllTextAsync(
                sourcePath,
                """
                {
                  "isFirstRun": false,
                  "showSeconds": false,
                  "theme": "dark"
                }
                """);
            var store = new JsonSettingsStore(
                new SafeFileLogger(Path.Combine(root, "logs")),
                Path.Combine(root, "settings.json"));

            var imported = await store.ImportAsync(sourcePath);

            Assert.Equal(AppSettings.CurrentSchemaVersion, imported.SchemaVersion);
            Assert.False(imported.IsFirstRun);
            Assert.False(imported.ShowSeconds);
            Assert.Equal(ThemeMode.Dark, imported.Theme);
        }
        finally
        {
            DeleteTemporaryDirectory(root);
        }
    }

    [Fact]
    public async Task ImportAsync_MigratesExplicitSchemaVersionZero()
    {
        var root = CreateTemporaryDirectory();
        try
        {
            var sourcePath = Path.Combine(root, "legacy-schema-zero.json");
            await File.WriteAllTextAsync(
                sourcePath,
                """
                {
                  "schemaVersion": 0,
                  "isFirstRun": false,
                  "contentSpacing": 21
                }
                """);
            var store = new JsonSettingsStore(
                new SafeFileLogger(Path.Combine(root, "logs")),
                Path.Combine(root, "settings.json"));

            var imported = await store.ImportAsync(sourcePath);

            Assert.Equal(AppSettings.CurrentSchemaVersion, imported.SchemaVersion);
            Assert.False(imported.IsFirstRun);
            Assert.Equal(21, imported.ContentSpacing);
        }
        finally
        {
            DeleteTemporaryDirectory(root);
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public async Task ImportAsync_RejectsUnsupportedSchemaVersions(int schemaVersion)
    {
        var root = CreateTemporaryDirectory();
        try
        {
            var sourcePath = Path.Combine(root, "unsupported-schema.json");
            await File.WriteAllTextAsync(
                sourcePath,
                $$"""
                {
                  "schemaVersion": {{schemaVersion}},
                  "isFirstRun": false
                }
                """);
            var store = new JsonSettingsStore(
                new SafeFileLogger(Path.Combine(root, "logs")),
                Path.Combine(root, "settings.json"));

            await Assert.ThrowsAsync<InvalidDataException>(() => store.ImportAsync(sourcePath));
        }
        finally
        {
            DeleteTemporaryDirectory(root);
        }
    }

    [Fact]
    public async Task ImportAsync_RejectsNumericEnumValues()
    {
        var root = CreateTemporaryDirectory();
        try
        {
            var sourcePath = Path.Combine(root, "numeric-enum.json");
            await File.WriteAllTextAsync(
                sourcePath,
                """
                {
                  "schemaVersion": 1,
                  "clockFormat": 24,
                  "theme": "system",
                  "layout": "centered"
                }
                """);
            var store = new JsonSettingsStore(
                new SafeFileLogger(Path.Combine(root, "logs")),
                Path.Combine(root, "settings.json"));

            await Assert.ThrowsAsync<JsonException>(() => store.ImportAsync(sourcePath));
        }
        finally
        {
            DeleteTemporaryDirectory(root);
        }
    }

    [Fact]
    public async Task Load_CorruptJsonFallsBackAndPreservesCorruptFile()
    {
        var root = CreateTemporaryDirectory();
        try
        {
            var settingsPath = Path.Combine(root, "settings.json");
            Directory.CreateDirectory(root);
            await File.WriteAllTextAsync(settingsPath, "{ not-json ");
            var store = new JsonSettingsStore(
                new SafeFileLogger(Path.Combine(root, "logs")),
                settingsPath);

            var loaded = await store.LoadAsync();

            Assert.True(loaded.IsFirstRun);
            Assert.False(File.Exists(settingsPath));
            Assert.Single(Directory.GetFiles(root, "settings.json.corrupt-*.json"));
        }
        finally
        {
            DeleteTemporaryDirectory(root);
        }
    }

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "ChronoDesk.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private static void DeleteTemporaryDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
