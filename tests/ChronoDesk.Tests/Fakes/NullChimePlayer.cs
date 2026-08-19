using ChronoDesk.Core.Abstractions;

namespace ChronoDesk.Tests.Fakes;

internal sealed class NullChimePlayer : IChimePlayer
{
    public Task PlayAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}
