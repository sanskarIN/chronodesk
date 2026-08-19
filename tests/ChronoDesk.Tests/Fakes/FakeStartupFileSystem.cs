using ChronoDesk.Infrastructure.Platform;

namespace ChronoDesk.Tests.Fakes;

internal sealed class FakeStartupFileSystem : IStartupFileSystem
{
    private readonly Dictionary<string, string> files = new(StringComparer.Ordinal);

    public HashSet<string> Directories { get; } = new(StringComparer.Ordinal);

    public bool FileExists(string path) => files.ContainsKey(path);

    public void DeleteFile(string path) => files.Remove(path);

    public void CreateDirectory(string path) => Directories.Add(path);

    public Task WriteAllTextAsync(
        string path,
        string content,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        files[path] = content;
        return Task.CompletedTask;
    }

    public string ReadAllText(string path) => files[path];

    public void SeedFile(string path, string content = "") => files[path] = content;
}
