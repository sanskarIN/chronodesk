using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using ChronoDesk.App;
using ChronoDesk.App.Localization;
using ChronoDesk.App.ViewModels;
using ChronoDesk.App.Views;

namespace ChronoDesk.Tests;

public sealed class HeadlessUiSmokeTests
{
    [AvaloniaFact]
    public void MainWindow_LoadsLocalizedClockShell()
    {
        var viewModel = new MainWindowViewModel(new AppServices());
        var window = new MainWindow(viewModel);

        Assert.Equal(Strings.AppName, window.Title);
        Assert.NotNull(window.FindControl<TextBlock>("ClockText"));
        Assert.NotNull(window.FindControl<TextBox>("TimeZoneSearchBox"));
        Assert.NotNull(window.FindControl<ListBox>("TimeZoneResults"));
    }

    [AvaloniaFact]
    public void MainWindow_MiniModeCanRoundTripWithoutLosingNormalSize()
    {
        var viewModel = new MainWindowViewModel(new AppServices());
        var window = new MainWindow(viewModel)
        {
            Width = 1000,
            Height = 700,
        };

        window.ToggleMiniMode();

        Assert.Equal(430, window.Width);
        Assert.Equal(240, window.Height);
        Assert.True(window.Topmost);

        window.ToggleMiniMode();

        Assert.Equal(1000, window.Width);
        Assert.Equal(700, window.Height);
    }

    [AvaloniaFact]
    public void MainWindow_FocusModeHidesAndRestoresApplicationChrome()
    {
        var viewModel = new MainWindowViewModel(new AppServices());
        var window = new MainWindow(viewModel);
        var header = window.FindControl<Border>("HeaderPanel");

        Assert.NotNull(header);
        Assert.True(header.IsVisible);

        window.ToggleFocusMode();
        Assert.False(header.IsVisible);

        window.ToggleFocusMode();
        Assert.True(header.IsVisible);
    }

    [AvaloniaFact]
    public void SettingsWindow_LoadsAllPrimaryPreferenceControls()
    {
        var viewModel = new MainWindowViewModel(new AppServices());
        var window = new SettingsWindow(viewModel);

        Assert.Equal(Strings.SettingsTitle, window.Title);
        Assert.NotNull(window.FindControl<ComboBox>("FormatCombo"));
        Assert.NotNull(window.FindControl<ComboBox>("ThemeCombo"));
        Assert.NotNull(window.FindControl<CheckBox>("ReducedMotionCheck"));
        Assert.NotNull(window.FindControl<CheckBox>("StartWithSystemCheck"));
        Assert.NotNull(window.FindControl<CheckBox>("ChimeEnabledCheck"));
    }

    [AvaloniaFact]
    public void OnboardingAndAboutWindowsLoadLocalizedResources()
    {
        var viewModel = new MainWindowViewModel(new AppServices());
        var onboarding = new OnboardingWindow(viewModel);
        var about = new AboutWindow();
        var versionText = about.FindControl<TextBlock>("VersionText");

        Assert.Equal(Strings.OnboardingTitle, onboarding.Title);
        Assert.Equal(Strings.AboutTitle, about.Title);
        Assert.NotNull(versionText);
        Assert.Contains("0.1.0-preview", versionText.Text, StringComparison.Ordinal);
    }
}
