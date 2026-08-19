using ChronoDesk.App;
using ChronoDesk.App.Localization;
using ChronoDesk.App.ViewModels;
using ChronoDesk.Core.Abstractions;
using ChronoDesk.Core.Models;

namespace ChronoDesk.Tests;

public sealed class MainWindowStateTests
{
    [Fact]
    public async Task InitializeAsync_TransitionsFromLoadingToReady()
    {
        var viewModel = CreateViewModel(
            new AppSettings
            {
                IsFirstRun = false,
                WorldClocks = [new WorldClock("one", "One", "Test/One")],
            });

        Assert.True(viewModel.IsLoading);
        Assert.False(viewModel.IsInitialized);
        Assert.Equal(StateStrings.LoadingLocalData, viewModel.StatusMessage);
        Assert.Equal(
            StateStrings.Format(nameof(StateStrings.WorldClockCountManyFormat), 0),
            viewModel.WorldClockCountText);

        await viewModel.InitializeAsync();

        Assert.False(viewModel.IsLoading);
        Assert.True(viewModel.IsInitialized);
        Assert.Equal(Strings.AppReady, viewModel.StatusMessage);
        Assert.Equal(StateStrings.WorldClockCountOne, viewModel.WorldClockCountText);
    }

    [Fact]
    public async Task WorldClockCountText_UsesLocalizedPluralFormat()
    {
        var viewModel = CreateViewModel(
            new AppSettings
            {
                IsFirstRun = false,
                WorldClocks =
                [
                    new WorldClock("one", "One", "Test/One"),
                    new WorldClock("two", "Two", "Test/Two"),
                ],
            });

        await viewModel.InitializeAsync();

        Assert.Equal(
            StateStrings.Format(nameof(StateStrings.WorldClockCountManyFormat), 2),
            viewModel.WorldClockCountText);
    }

    private static MainWindowViewModel CreateViewModel(AppSettings settings)
    {
        var services = new AppServices(
            new NullLogger(),
            new MemorySettingsStore(settings),
            new UtcTimeZoneCatalog(),
            new NullStartupManager(),
            new NullChimePlayer());
        return new MainWindowViewModel(services);
    }

    private sealed class MemorySettingsStore(AppSettings initial) : ISettingsStore
    {
        private AppSettings current = initial.Normalize();

        public string SettingsPath => "memory://settings";

        public Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(current);

        public Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
        {
            current = settings.Normalize();
            return Task.CompletedTask;
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

    private sealed class UtcTimeZoneCatalog : ITimeZoneCatalog
    {
        public IReadOnlyList<TimeZoneDescriptor> GetAll() =>
            [new("UTC", "UTC", TimeSpan.Zero)];

        public TimeZoneInfo Resolve(string timeZoneId) => TimeZoneInfo.Utc;

        public IReadOnlyList<TimeZoneDescriptor> Search(string query, int limit = 50) =>
            [new("UTC", "UTC", TimeSpan.Zero)];
    }

    private sealed class NullStartupManager : IStartupManager
    {
        public bool IsSupported => false;

        public Task<bool> IsEnabledAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task SetEnabledAsync(bool enabled, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
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
