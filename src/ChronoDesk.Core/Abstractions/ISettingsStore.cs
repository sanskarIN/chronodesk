using ChronoDesk.Core.Models;

namespace ChronoDesk.Core.Abstractions;

public interface ISettingsStore
{
    string SettingsPath { get; }

    Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default);

    Task ExportAsync(
        AppSettings settings,
        string destinationPath,
        CancellationToken cancellationToken = default);

    Task<AppSettings> ImportAsync(
        string sourcePath,
        CancellationToken cancellationToken = default);
}
