using ChronoDesk.App;
using ChronoDesk.App.ViewModels;
using ChronoDesk.Core.Abstractions;
using ChronoDesk.Core.Models;

namespace ChronoDesk.Tests;

public sealed class OnboardingPersistenceTests
{
    [Fact]
    public async Task CompleteOnboardingAsync_DoesNotChangeInMemoryStateWhenSaveFails()
    {
        var store = new FailingSettingsStore(new AppSettings { IsFirstRun = true });
        var services = new AppServices(
            new NullLogger(),
            store,
            new UtcTimeZoneCatalog(),
            new NullStartupManager(),
            new NullChimePlayer());
        var viewModel = new MainWindowViewModel(services);
        await viewModel.InitializeAsync();

        await Assert.ThrowsAsync<IOException>(() => viewModel.CompleteOnboardingAsync());

        Assert.True(viewModel.Settings.IsFirstRun);
    }

    private sealed class FailingSettingsStore(AppSettings initial) : ISettingsStore
    {
        private readonly AppSettings current = initial.Normalize();

        public string SettingsPath => "memory://settings";

        public Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(current);

        public Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default) =>
            throw new IOException("Synthetic onboarding persistence failure.");

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
