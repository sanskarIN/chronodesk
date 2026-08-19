namespace ChronoDesk.Core.Abstractions;

public interface IChimePlayer
{
    Task PlayAsync(CancellationToken cancellationToken = default);
}
