using System.Text.Json;
using ChronoDesk.Infrastructure.Logging;
using ChronoDesk.Infrastructure.Persistence;

namespace ChronoDesk.Tests;

public sealed class SettingsStrictnessTests
{
    [Fact]
    public async Task ImportAsync_RejectsUnknownMembers()
    {
        var root = CreateTemporaryDirectory();
        try
        {
            var sourcePath = Path.Combine(root, "unknown-member.json");
            await File.WriteAllTextAsync(
                sourcePath,
                """
                {
                  "schemaVersion": 1,
                  "showSeconds": true,
                  "unexpectedSetting": "must-not-be-silently-ignored"
                }
                """);
            var store = CreateStore(root);

            await Assert.ThrowsAsync<JsonException>(() => store.ImportAsync(sourcePath));
        }
        finally
        {
            DeleteTemporaryDirectory(root);
        }
    }

    [Fact]
    public async Task ImportAsync_RejectsDuplicateRootMembers()
    {
        var root = CreateTemporaryDirectory();
        try
        {
            var sourcePath = Path.Combine(root, "duplicate-root.json");
            await File.WriteAllTextAsync(
                sourcePath,
                """
                {
                  "schemaVersion": 1,
                  "showSeconds": true,
                  "showSeconds": false
                }
                """);
            var store = CreateStore(root);

            await Assert.ThrowsAsync<InvalidDataException>(() => store.ImportAsync(sourcePath));
        }
        finally
        {
            DeleteTemporaryDirectory(root);
        }
    }

    [Fact]
    public async Task ImportAsync_RejectsDuplicateNestedMembers()
    {
        var root = CreateTemporaryDirectory();
        try
        {
            var sourcePath = Path.Combine(root, "duplicate-nested.json");
            await File.WriteAllTextAsync(
                sourcePath,
                """
                {
                  "schemaVersion": 1,
                  "chime": {
                    "enabled": true,
                    "enabled": false,
                    "interval": "hourly"
                  }
                }
                """);
            var store = CreateStore(root);

            await Assert.ThrowsAsync<InvalidDataException>(() => store.ImportAsync(sourcePath));
        }
        finally
        {
            DeleteTemporaryDirectory(root);
        }
    }

    private static JsonSettingsStore CreateStore(string root) =>
        new(
            new SafeFileLogger(Path.Combine(root, "logs")),
            Path.Combine(root, "settings.json"));

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(
            Path.GetTempPath(),
            "ChronoDesk.Tests",
            Guid.NewGuid().ToString("N"));
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
