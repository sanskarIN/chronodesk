using ChronoDesk.Infrastructure.Logging;

namespace ChronoDesk.Tests;

public sealed class SafeFileLoggerTests
{
    [Fact]
    public void Info_RedactsEmailAndSecretAssignments()
    {
        var root = CreateTemporaryDirectory();
        try
        {
            var logger = new SafeFileLogger(root);

            logger.Info(
                "support.contact",
                "Email person@example.com token=super-secret-value api_key=another-secret");

            var content = File.ReadAllText(Path.Combine(root, "chronodesk.log.jsonl"));
            Assert.DoesNotContain("person@example.com", content, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("super-secret-value", content, StringComparison.Ordinal);
            Assert.DoesNotContain("another-secret", content, StringComparison.Ordinal);
            Assert.Contains("[redacted-email]", content, StringComparison.Ordinal);
            Assert.Contains("[redacted]", content, StringComparison.Ordinal);
        }
        finally
        {
            DeleteTemporaryDirectory(root);
        }
    }

    [Fact]
    public void Rotation_CreatesUniqueArchivesAcrossRapidRepeatedRotations()
    {
        var root = CreateTemporaryDirectory();
        try
        {
            Directory.CreateDirectory(root);
            var activePath = Path.Combine(root, "chronodesk.log.jsonl");
            var logger = new SafeFileLogger(root);

            File.WriteAllBytes(activePath, new byte[1_048_576]);
            logger.Info("rotation.first", "first rotation");

            File.WriteAllBytes(activePath, new byte[1_048_576]);
            logger.Info("rotation.second", "second rotation");

            var archives = Directory.GetFiles(root, "chronodesk-*.log.jsonl");
            Assert.Equal(2, archives.Length);
            Assert.Equal(2, archives.Distinct(StringComparer.Ordinal).Count());
            Assert.True(File.Exists(activePath));
        }
        finally
        {
            DeleteTemporaryDirectory(root);
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

    private static void DeleteTemporaryDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
