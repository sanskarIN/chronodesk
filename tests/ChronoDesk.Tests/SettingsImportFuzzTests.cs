using System.Text;
using System.Text.Json;
using ChronoDesk.Infrastructure.Logging;
using ChronoDesk.Infrastructure.Persistence;

namespace ChronoDesk.Tests;

public sealed class SettingsImportFuzzTests
{
    [Fact]
    public async Task ImportAsync_RejectsDeterministicMalformedCorpusWithoutChangingPrimarySettings()
    {
        var root = CreateTemporaryDirectory();
        try
        {
            var settingsPath = Path.Combine(root, "settings.json");
            var logger = new SafeFileLogger(Path.Combine(root, "logs"));
            var store = new JsonSettingsStore(logger, settingsPath);
            await store.SaveAsync(new() { IsFirstRun = false, ShowSeconds = false });
            var original = await File.ReadAllTextAsync(settingsPath);

            var random = new Random(0xF022);
            for (var sample = 0; sample < 100; sample++)
            {
                var length = random.Next(1, 2_048);
                var bytes = new byte[length];
                random.NextBytes(bytes);
                var fuzzPath = Path.Combine(root, $"fuzz-{sample:000}.json");
                await File.WriteAllBytesAsync(fuzzPath, bytes);

                try
                {
                    _ = await store.ImportAsync(fuzzPath);
                }
                catch (Exception exception) when (
                    exception is JsonException or InvalidDataException or DecoderFallbackException)
                {
                    // Expected for malformed corpus members.
                }

                Assert.Equal(original, await File.ReadAllTextAsync(settingsPath));
            }
        }
        finally
        {
            DeleteTemporaryDirectory(root);
        }
    }

    [Fact]
    public async Task ImportAsync_RejectsFilesAboveMaximumSize()
    {
        var root = CreateTemporaryDirectory();
        try
        {
            var sourcePath = Path.Combine(root, "oversized.json");
            await using (var stream = new FileStream(sourcePath, FileMode.CreateNew, FileAccess.Write))
            {
                stream.SetLength((2 * 1024 * 1024) + 1);
            }

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
