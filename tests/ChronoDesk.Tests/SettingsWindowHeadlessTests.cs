using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using ChronoDesk.App;
using ChronoDesk.App.Localization;
using ChronoDesk.App.ViewModels;
using ChronoDesk.App.Views;
using ChronoDesk.Core.Models;
using ChronoDesk.Tests.Fakes;

namespace ChronoDesk.Tests;

public sealed class SettingsWindowHeadlessTests
{
    [AvaloniaFact]
    public async Task SaveChangesAsync_PersistsEditedControlsAndStartupPreference()
    {
        var store = new MemorySettingsStore(new AppSettings
        {
            IsFirstRun = false,
            StartWithSystem = false,
        });
        var startup = new RecordingStartupManager();
        var viewModel = CreateViewModel(store, startup);
        await viewModel.InitializeAsync();
        var window = new SettingsWindow(viewModel);

        window.FindControl<ComboBox>("FormatCombo")!.SelectedIndex = 0;
        window.FindControl<ComboBox>("ThemeCombo")!.SelectedIndex = 2;
        window.FindControl<CheckBox>("ShowSecondsCheck")!.IsChecked = false;
        window.FindControl<CheckBox>("StartWithSystemCheck")!.IsChecked = true;
        window.FindControl<TextBox>("QuietStartText")!.Text = "21:30";
        window.FindControl<TextBox>("QuietEndText")!.Text = "06:15";

        var saved = await window.SaveChangesAsync();

        Assert.True(saved);
        Assert.Equal(ClockFormat.TwelveHour, viewModel.Settings.ClockFormat);
        Assert.Equal(ThemeMode.Dark, viewModel.Settings.Theme);
        Assert.False(viewModel.Settings.ShowSeconds);
        Assert.True(viewModel.Settings.StartWithSystem);
        Assert.Equal(new[] { true }, startup.SetCalls);
        Assert.NotNull(store.LastSaved);
        Assert.Equal(new TimeOnly(21, 30), store.LastSaved!.Chime.QuietHours.Start);
        Assert.Equal(new TimeOnly(6, 15), store.LastSaved.Chime.QuietHours.End);
    }

    [AvaloniaFact]
    public async Task SaveChangesAsync_InvalidQuietHoursShowsValidationWithoutPersistence()
    {
        var store = new MemorySettingsStore(new AppSettings { IsFirstRun = false });
        var startup = new RecordingStartupManager();
        var viewModel = CreateViewModel(store, startup);
        await viewModel.InitializeAsync();
        var window = new SettingsWindow(viewModel);
        window.FindControl<TextBox>("QuietStartText")!.Text = "not-time";

        var saved = await window.SaveChangesAsync();

        Assert.False(saved);
        Assert.Null(store.LastSaved);
        Assert.Empty(startup.SetCalls);
        Assert.Equal(
            Strings.InvalidQuietHours,
            window.FindControl<TextBlock>("SettingsStatusText")!.Text);
    }

    [AvaloniaFact]
    public async Task ResetDefaultsAsync_PersistsDefaultsAndReloadsControls()
    {
        var store = new MemorySettingsStore(new AppSettings
        {
            IsFirstRun = false,
            StartWithSystem = true,
            Theme = ThemeMode.Dark,
            ShowSeconds = false,
            ClockFormat = ClockFormat.TwelveHour,
        });
        var startup = new RecordingStartupManager();
        var viewModel = CreateViewModel(store, startup);
        await viewModel.InitializeAsync();
        var window = new SettingsWindow(viewModel);

        var reset = await window.ResetDefaultsAsync();

        Assert.True(reset);
        Assert.False(viewModel.Settings.StartWithSystem);
        Assert.Equal(ThemeMode.System, viewModel.Settings.Theme);
        Assert.True(viewModel.Settings.ShowSeconds);
        Assert.Equal(ClockFormat.TwentyFourHour, viewModel.Settings.ClockFormat);
        Assert.Equal(new[] { false }, startup.SetCalls);
        Assert.Equal(0, window.FindControl<ComboBox>("ThemeCombo")!.SelectedIndex);
        Assert.Equal(1, window.FindControl<ComboBox>("FormatCombo")!.SelectedIndex);
        Assert.Equal(
            Strings.DefaultsRestored,
            window.FindControl<TextBlock>("SettingsStatusText")!.Text);
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
