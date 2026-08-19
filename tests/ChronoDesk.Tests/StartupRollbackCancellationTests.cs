using ChronoDesk.App;
using ChronoDesk.App.ViewModels;
using ChronoDesk.Core.Abstractions;
using ChronoDesk.Core.Models;

namespace ChronoDesk.Tests;

public sealed class StartupRollbackCancellationTests
{
    [Fact]
    public async Task UpdateSettingsAsync_RollsBackStartupAfterSaveCancelsCallerToken()
    {
        using var cancellation = new CancellationTokenSource();
        var store = new CancellingSettingsStore(
            new AppSettings { IsFirstRun = false, StartWithSystem = false },
            cancellation);
        var startup = new RecordingStartupManager();
        var services = new AppServices(
            new NullLogger(),
            store,
            new UtcTimeZoneCatalog(),
            startup,
            new NullChimePlayer());
        var viewModel = new MainWindowViewModel(services);
        await viewModel.InitializeAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            viewModel.UpdateSettingsAsync(
                viewModel.Settings with { StartWithSystem = true },
                cancellation.Token));

        Assert.Equal(new[] { true, false }, startup.SetCalls);
        Assert.Equal(new[] { false, false }, startup.CancellationStates);
        Assert.False(viewModel.Settings.StartWithSystem);
    }

    private sealed class CancellingSettingsStore(
        AppSettings initial,
        CancellationTokenSource cancellation) : ISettingsStore
    {
        private readonly AppSettings current = initial.Normalize();

        public string SettingsPath => "memory://settings";

        public Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(current);
        }

        public Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
        {
            cancellation.Cancel();
            throw new OperationCanceledException(cancellationToken);
        }

        public Task ExportAsync(
            AppSettings settings,
            string destinationPath,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<AppSettings> ImportAsync(
            string sourcePath,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(current);
    }

    private sealed class RecordingStartupManager : IStartupManager
    {
        public bool IsSupported => true;

        public List<bool> SetCalls { get; } = [];

        public List<bool> CancellationStates { get; } = [];

        public Task<bool> IsEnabledAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(SetCalls.LastOrDefault());

        public Task SetEnabledAsync(bool enabled, CancellationToken cancellationToken = default)
        {
            CancellationStates.Add(cancellationToken.IsCancellationRequested);
            cancellationToken.ThrowIfCancellationRequested();
            SetCalls.Add(enabled);
            return Task.CompletedTask;
        }
    }

    private sealed class UtcTimeZoneCatalog : ITimeZoneCatalog
    {
        private static readonly TimeZoneDescriptor Utc = new(
            TimeZoneInfo.Utc.Id,
            TimeZoneInfo.Utc.DisplayName,
            TimeSpan.Zero);

        public IReadOnlyList<TimeZoneDescriptor> GetAll() => [Utc];

        public TimeZoneInfo Resolve(string timeZoneId) => TimeZoneInfo.Utc;

        public IReadOnlyList<TimeZoneDescriptor> Search(string query, int limit = 50) => [Utc];
    }

    private sealed class NullChimePlayer : IChimePlayer
    {
        public Task PlayAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class NullLogger : IAppLogger
    {
        public void Info(string eventName, string message)
        {
        }

        public void Warning(string eventName, string message)
        {
        }

        public void Error(string eventName, Exception exception, string safeMessage)
        {
        }
    }
}
