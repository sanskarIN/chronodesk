namespace ChronoDesk.Infrastructure.Platform;

internal sealed class SystemStartupFileSystem : IStartupFileSystem
{
    public bool FileExists(string path) => File.Exists(path);

    public void DeleteFile(string path) => File.Delete(path);

    public void CreateDirectory(string path) => Directory.CreateDirectory(path);

    public Task WriteAllTextAsync(
        string path,
        string content,
        CancellationToken cancellationToken) =>
        File.WriteAllTextAsync(path, content, cancellationToken);
}
