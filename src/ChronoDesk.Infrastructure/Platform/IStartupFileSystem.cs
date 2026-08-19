namespace ChronoDesk.Infrastructure.Platform;

internal interface IStartupFileSystem
{
    bool FileExists(string path);

    void DeleteFile(string path);

    void CreateDirectory(string path);

    Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken);
}
