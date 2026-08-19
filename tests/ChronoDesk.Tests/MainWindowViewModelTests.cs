using ChronoDesk.App;
using ChronoDesk.App.ViewModels;
using ChronoDesk.Core.Models;
using ChronoDesk.Tests.Fakes;

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

    private static MainWindowViewModel CreateViewModel(
        MemorySettingsStore store,
        RecordingStartupManager startup)
    {
        var services = new AppServices(
            new NullAppLogger(),
            store,
            new UtcTimeZoneCatalog(),
            startup,
            new NullChimePlayer());
        return new MainWindowViewModel(services);
    }
}
