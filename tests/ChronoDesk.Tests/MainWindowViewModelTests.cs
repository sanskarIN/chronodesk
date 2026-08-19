using ChronoDesk.App;
using ChronoDesk.App.Localization;
using ChronoDesk.App.ViewModels;
using ChronoDesk.Core.Abstractions;
using ChronoDesk.Core.Models;

namespace ChronoDesk.Tests;

public sealed class MainWindowViewModelTests
{
    [Fact]
    public async Task UpdateSettingsAsync_RollsBackStartupWhenPersistenceFails()
    {
        var store = new MemorySettingsStore(new AppSettings
        {
            IsFirstRun = false,
            StartWithSystem = false,
        });
        var startup = new RecordingStartupManager();
        var viewModel = CreateViewModel(store, startup);
        await viewModel.InitializeAsync();
        store.ThrowOnSave = true;

        await Assert.ThrowsAsync<IOException>(() =>
            viewModel.UpdateSettingsAsync(viewModel.Settings with { StartWithSystem = true }));

        Assert.Equal(new[] { true, false }, startup.SetCalls);
        Assert.False(viewModel.Settings.StartWithSystem);
    }

    [Fact]
    public async Task ImportSettingsAsync_DoesNotEnableStartupFromImportedFile()
    {
        var store = new MemorySettingsStore(new AppSettings
        {
            IsFirstRun = false,
            StartWithSystem = false,
        })
        {
            ImportedSettings = new AppSettings
            {
                IsFirstRun = false,
                StartWithSystem = true,
                ShowSeconds = false,
            },
        };
        var startup = new RecordingStartupManager();
        var viewModel = CreateViewModel(store, startup);
        await viewModel.InitializeAsync();

        await viewModel.ImportSettingsAsync("portable-settings.json");

        Assert.Empty(startup.SetCalls);
        Assert.False(viewModel.Settings.StartWithSystem);
        Assert.False(viewModel.Settings.ShowSeconds);
        Assert.NotNull(store.LastSaved);
        Assert.False(store.LastSaved!.StartWithSystem);
    }

    [Fact]
    public async Task UpdateSettingsAsync_AppliesStartupOnceAfterExplicitPreferenceChange()
    {
        var store = new MemorySettingsStore(new AppSettings
        {
            IsFirstRun = false,
            StartWithSystem = false,
        });
        var startup = new RecordingStartupManager();
        var viewModel = CreateViewModel(store, startup);
        await viewModel.InitializeAsync();

        await viewModel.UpdateSettingsAsync(viewModel.Settings with { StartWithSystem = true });

        Assert.Equal(new[] { true }, startup.SetCalls);
        Assert.True(viewModel.Settings.StartWithSystem);
        Assert.NotNull(store.LastSaved);
        Assert.True(store.LastSaved!.StartWithSystem);
    }

    [Fact]
    public async Task AddWorldClockAsync_RejectsAdditionAtMaximumCapacity()
    {
        var clocks = Enumerable.Range(0, AppSettings.MaximumWorldClockCount)
            .Select(index => new WorldClock(
                $"clock-{index}",
                $"Clock {index}",
                $"Test/Zone-{index}"))
            .ToList();
        var store = new MemorySettingsStore(new AppSettings
        {
            IsFirstRun = false,
            WorldClocks = clocks,
        });
        var viewModel = CreateViewModel(store, new RecordingStartupManager());
        await viewModel.InitializeAsync();

        await viewModel.AddWorldClockAsync(
            new TimeZoneDescriptor("Test/New", "New timezone", TimeSpan.Zero));

        Assert.Equal(AppSettings.MaximumWorldClockCount, viewModel.Settings.WorldClocks.Count);
        Assert.Equal(
            Strings.Format(
                nameof(Strings.WorldClockLimitReachedFormat),
                AppSettings.MaximumWorldClockCount),
            viewModel.StatusMessage);
        Assert.Null(store.LastSaved);
    }

    [Fact]
    public async Task RemoveAndUndoWorldClock_RestoresOriginalPosition()
    {
        var store = new MemorySettingsStore(new AppSettings
        {
            IsFirstRun = false,
            WorldClocks =
            [
                new WorldClock("first", "First", "Test/First"),
                new WorldClock("second", "Second", "Test/Second"),
            ],
        });
        var viewModel = CreateViewModel(store, new RecordingStartupManager());
        await viewModel.InitializeAsync();

        await viewModel.RemoveWorldClockAsync("first");

        Assert.True(viewModel.CanUndoWorldClockRemoval);
        Assert.Single(viewModel.Settings.WorldClocks);
        Assert.Equal("second", viewModel.Settings.WorldClocks[0].Id);
        Assert.Equal(Strings.WorldClockRemoved, viewModel.StatusMessage);

        await viewModel.UndoWorldClockRemovalAsync();

        Assert.False(viewModel.CanUndoWorldClockRemoval);
        Assert.Equal(2, viewModel.Settings.WorldClocks.Count);
        Assert.Equal("first", viewModel.Settings.WorldClocks[0].Id);
        Assert.Equal("second", viewModel.Settings.WorldClocks[1].Id);
        Assert.Equal(Strings.WorldClockRestored, viewModel.StatusMessage);
    }

    [Fact]
    public async Task SearchTimeZones_ReportsEmptyAndPopulatedStates()
    {
        var store = new MemorySettingsStore(new AppSettings { IsFirstRun = false });
        var viewModel = CreateViewModel(store, new RecordingStartupManager());
        await viewModel.InitializeAsync();

        viewModel.SearchTimeZones("missing");

        Assert.Empty(viewModel.SearchResults);
        Assert.Equal(Strings.TimezoneSearchEmpty, viewModel.TimeZoneSearchStatus);

        viewModel.SearchTimeZones("UTC");

        Assert.Single(viewModel.SearchResults);
        Assert.Equal(
            Strings.Format(nameof(Strings.TimezoneSearchCountFormat), 1),
            viewModel.TimeZoneSearchStatus);
    }

    private static MainWindowViewModel CreateViewModel(
        MemorySettingsStore store,
        RecordingStartupManager startup)
    {
        var services = new AppServices(
            new NullLogger(),
            store,
            new UtcTimeZoneCatalog(),
            startup,
            new NullChimePlayer());
        return new MainWindowViewModel(services);
    }

    private sealed class MemorySettingsStore(AppSettings initial) : ISettingsStore
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

    private sealed class RecordingStartupManager : IStartupManager
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

    private sealed class UtcTimeZoneCatalog : ITimeZoneCatalog
    {
        private static readonly TimeZoneDescriptor Utc = new(
            TimeZoneInfo.Utc.Id,
            TimeZoneInfo.Utc.DisplayName,
            TimeSpan.Zero);

        public IReadOnlyList<TimeZoneDescriptor> GetAll() => [Utc];

        public TimeZoneInfo Resolve(string timeZoneId) => TimeZoneInfo.Utc;

        public IReadOnlyList<TimeZoneDescriptor> Search(string query, int limit = 50) =>
            string.Equals(query, "missing", StringComparison.OrdinalIgnoreCase) ? [] : [Utc];
    }

    private sealed class NullChimePlayer : IChimePlayer
    {
        public Task PlayAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }
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
