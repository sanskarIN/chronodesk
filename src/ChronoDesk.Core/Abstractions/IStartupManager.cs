namespace ChronoDesk.Core.Abstractions;

public interface IStartupManager
{
    bool IsSupported { get; }

    Task<bool> IsEnabledAsync(CancellationToken cancellationToken = default);

    Task SetEnabledAsync(bool enabled, CancellationToken cancellationToken = default);
}
