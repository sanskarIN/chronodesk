using ChronoDesk.Core.Abstractions;
using ChronoDesk.Core.Models;

namespace ChronoDesk.Tests.Fakes;

internal sealed class MemorySettingsStore(AppSettings initial) : ISettingsStore
{
    private AppSettings current = initial.Normalize();

    public string SettingsPath => "memory://settings";

    public bool ThrowOnSave { get; set; }

    public AppSettings? LastSaved { get; private set; }

    public AppSettings ImportedSettings { get; set; } = new();

    public Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(current);
    }

    public Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (ThrowOnSave)
        {
            throw new IOException("Synthetic persistence failure.");
        }

        current = settings.Normalize();
        LastSaved = current;
        return Task.CompletedTask;
    }

    public Task ExportAsync(
        AppSettings settings,
        string destinationPath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }

    public Task<AppSettings> ImportAsync(
        string sourcePath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(ImportedSettings.Normalize());
    }
}
