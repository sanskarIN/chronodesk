using System.Text.Json;
using System.Text.Json.Serialization;
using ChronoDesk.Core.Abstractions;
using ChronoDesk.Core.Models;

namespace ChronoDesk.Infrastructure.Persistence;

public sealed class JsonSettingsStore : ISettingsStore
{
    private const long MaximumSettingsBytes = 2 * 1024 * 1024;
    private readonly IAppLogger logger;
    private readonly JsonSerializerOptions serializerOptions;

    public JsonSettingsStore(IAppLogger logger, string? settingsPath = null)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        SettingsPath = settingsPath ?? AppPaths.GetSettingsPath();
        serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };
        serializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
    }

    public string SettingsPath { get; }

    public async Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(SettingsPath))
        {
            return new AppSettings();
        }

        try
        {
            return await ReadAndValidateAsync(SettingsPath, cancellationToken);
        }
        catch (Exception exception) when (exception is JsonException or IOException or InvalidDataException)
        {
            logger.Error("settings.load_failed", exception, "Settings could not be loaded; defaults were used.");
            PreserveCorruptSettings();
            return new AppSettings();
        }
    }

    public Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        return WriteAtomicallyAsync(settings.Normalize(), SettingsPath, cancellationToken);
    }

    public Task ExportAsync(
        AppSettings settings,
        string destinationPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationPath);
        return WriteAtomicallyAsync(settings.Normalize(), destinationPath, cancellationToken);
    }

    public Task<AppSettings> ImportAsync(
        string sourcePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourcePath);
        return ReadAndValidateAsync(sourcePath, cancellationToken);
    }

    private async Task<AppSettings> ReadAndValidateAsync(
        string path,
        CancellationToken cancellationToken)
    {
        var file = new FileInfo(path);
        if (!file.Exists)
        {
            throw new FileNotFoundException("Settings file was not found.", path);
        }

        if (file.Length > MaximumSettingsBytes)
        {
            throw new InvalidDataException("Settings file exceeds the allowed size.");
        }

        await using var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 16 * 1024,
            useAsync: true);
        var settings = await JsonSerializer.DeserializeAsync<AppSettings>(
            stream,
            serializerOptions,
            cancellationToken);

        if (settings is null)
        {
            throw new InvalidDataException("Settings document is empty.");
        }

        if (settings.SchemaVersion > AppSettings.CurrentSchemaVersion)
        {
            throw new InvalidDataException("Settings were created by a newer unsupported ChronoDesk version.");
        }

        return settings.Normalize();
    }

    private async Task WriteAtomicallyAsync(
        AppSettings settings,
        string destinationPath,
        CancellationToken cancellationToken)
    {
        var fullPath = Path.GetFullPath(destinationPath);
        var directory = Path.GetDirectoryName(fullPath)
            ?? throw new InvalidOperationException("A destination directory could not be resolved.");
        Directory.CreateDirectory(directory);

        var temporaryPath = fullPath + $".{Guid.NewGuid():N}.tmp";
        try
        {
            await using (var stream = new FileStream(
                temporaryPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 16 * 1024,
                useAsync: true))
            {
                await JsonSerializer.SerializeAsync(
                    stream,
                    settings,
                    serializerOptions,
                    cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }

            File.Move(temporaryPath, fullPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    private void PreserveCorruptSettings()
    {
        try
        {
            if (!File.Exists(SettingsPath))
            {
                return;
            }

            var backup = SettingsPath + $".corrupt-{DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}.json";
            File.Move(SettingsPath, backup, overwrite: false);
            logger.Warning("settings.corrupt_preserved", "A corrupt settings file was preserved for manual recovery.");
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            logger.Error("settings.corrupt_preserve_failed", exception, "Corrupt settings could not be preserved.");
        }
    }
}
