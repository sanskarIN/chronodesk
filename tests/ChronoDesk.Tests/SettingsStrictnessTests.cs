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
            var store = new JsonSettingsStore(
                new SafeFileLogger(Path.Combine(root, "logs")),
                Path.Combine(root, "settings.json"));

            await Assert.ThrowsAsync<JsonException>(() => store.ImportAsync(sourcePath));
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(
            Path.GetTempPath(),
            "ChronoDesk.Tests",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
