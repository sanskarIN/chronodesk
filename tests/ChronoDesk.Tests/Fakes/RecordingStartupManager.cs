using ChronoDesk.Core.Abstractions;

namespace ChronoDesk.Tests.Fakes;

internal sealed class RecordingStartupManager : IStartupManager
{
    public bool IsSupported => true;

    public List<bool> SetCalls { get; } = [];

    public Task<bool> IsEnabledAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(SetCalls.LastOrDefault());
    }

    public Task SetEnabledAsync(bool enabled, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SetCalls.Add(enabled);
        return Task.CompletedTask;
    }
}
