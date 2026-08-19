using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ChronoDesk.App.ViewModels;
using ChronoDesk.Core.Models;

namespace ChronoDesk.App.Views;

public sealed partial class MainWindow : Window
{
    private readonly MainWindowViewModel viewModel;
    private readonly DispatcherTimer clockTimer;
    private bool allowClose;
    private bool isFocusMode;
    private bool isMiniMode;
    private bool tickInProgress;
    private double restoredWidth = 1180;
    private double restoredHeight = 760;
    private PixelPoint restoredPosition;
    private bool hasRestoredPosition;
    private bool restoredTopmost;

    public MainWindow(MainWindowViewModel viewModel)
    {
        this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        InitializeComponent();
        DataContext = viewModel;

        clockTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
        clockTimer.Tick += ClockTimer_OnTick;

        Opened += MainWindow_OnOpened;
        Closing += MainWindow_OnClosing;
        viewModel.SettingsChanged += (_, settings) => ApplySettings(settings);
    }

    public void ShowFromTray()
    {
        Show();
        ShowInTaskbar = true;
        if (WindowState == WindowState.Minimized)
        {
            WindowState = WindowState.Normal;
        }

        Activate();
    }

    public void ToggleFocusMode()
    {
        if (isMiniMode)
        {
            ExitMiniMode();
        }

        isFocusMode = !isFocusMode;
        SetChromeVisibility(!isFocusMode);
        WindowState = isFocusMode ? WindowState.FullScreen : WindowState.Normal;
        var heroCard = this.FindControl<Border>("HeroCard");
        if (heroCard is not null)
        {
            heroCard.MinHeight = isFocusMode ? 520 : 300;
        }
    }

    public void ToggleMiniMode()
    {
        if (isMiniMode)
        {
            ExitMiniMode();
            return;
        }

        if (isFocusMode)
        {
            ToggleFocusMode();
        }

        isMiniMode = true;
        restoredWidth = Width;
        restoredHeight = Height;
        restoredPosition = Position;
        hasRestoredPosition = true;
        restoredTopmost = Topmost;

        MinWidth = 360;
        MinHeight = 180;
        Width = 430;
        Height = 240;
        Topmost = true;
        SystemDecorations = SystemDecorations.BorderOnly;
        SetChromeVisibility(false);

        var heroCard = this.FindControl<Border>("HeroCard");
        if (heroCard is not null)
        {
            heroCard.MinHeight = 190;
        }
    }

    public void AllowClose()
    {
        allowClose = true;
        clockTimer.Stop();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private async void MainWindow_OnOpened(object? sender, EventArgs e)
    {
        await viewModel.InitializeAsync();
        ApplySettings(viewModel.Settings);
        clockTimer.Start();

        if (viewModel.Settings.IsFirstRun)
        {
            var onboarding = new OnboardingWindow(viewModel);
            await onboarding.ShowDialog(this);
        }

        var backgroundStart = Environment.GetCommandLineArgs()
            .Any(argument => string.Equals(argument, "--background", StringComparison.OrdinalIgnoreCase));
        if (backgroundStart && viewModel.Settings.MinimizeToTray)
        {
            Hide();
        }
    }

    private void MainWindow_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (!allowClose && viewModel.Settings.MinimizeToTray)
        {
            e.Cancel = true;
            Hide();
            return;
        }

        clockTimer.Stop();
    }

    private async void ClockTimer_OnTick(object? sender, EventArgs e)
    {
        if (tickInProgress)
        {
            return;
        }

        tickInProgress = true;
        try
        {
            await viewModel.TickAsync();
        }
        catch (Exception exception)
        {
            if (Application.Current is App app)
            {
                app.Services.Logger.Error("clock.tick_failed", exception, "The clock could not refresh for one tick.");
            }
        }
        finally
        {
            tickInProgress = false;
        }
    }

    private void ApplySettings(AppSettings settings)
    {
        Topmost = isMiniMode || settings.AlwaysOnTop;
        ApplyLayout(settings.Layout);
    }

    private void ApplyLayout(ClockLayout layout)
    {
        var clockText = this.FindControl<TextBlock>("ClockText");
        var heroCard = this.FindControl<Border>("HeroCard");
        if (clockText is null || heroCard is null)
        {
            return;
        }

        switch (layout)
        {
            case ClockLayout.Compact:
                clockText.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                clockText.TextAlignment = Avalonia.Media.TextAlignment.Left;
                heroCard.MinHeight = isMiniMode ? 190 : 240;
                break;
            case ClockLayout.Dashboard:
                clockText.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
                clockText.TextAlignment = Avalonia.Media.TextAlignment.Center;
                heroCard.MinHeight = isMiniMode ? 190 : 340;
                break;
            default:
                clockText.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
                clockText.TextAlignment = Avalonia.Media.TextAlignment.Center;
                heroCard.MinHeight = isMiniMode ? 190 : 300;
                break;
        }
    }

    private void ExitMiniMode()
    {
        isMiniMode = false;
        MinWidth = 780;
        MinHeight = 560;
        Width = restoredWidth;
        Height = restoredHeight;
        if (hasRestoredPosition)
        {
            Position = restoredPosition;
        }

        Topmost = viewModel.Settings.AlwaysOnTop || restoredTopmost;
        SystemDecorations = SystemDecorations.Full;
        SetChromeVisibility(true);
        ApplyLayout(viewModel.Settings.Layout);
    }

    private void SetChromeVisibility(bool visible)
    {
        SetVisible("HeaderPanel", visible);
        SetVisible("WorldClockSection", visible);
        SetVisible("QuickAddSection", visible);
        SetVisible("FooterPanel", visible);

        var content = this.FindControl<StackPanel>("ContentStack");
        if (content is not null)
        {
            content.Margin = visible ? new Thickness(24) : new Thickness(8);
        }
    }

    private void SetVisible(string controlName, bool visible)
    {
        var control = this.FindControl<Control>(controlName);
        if (control is not null)
        {
            control.IsVisible = visible;
        }
    }

    private async Task OpenSettingsAsync()
    {
        var settingsWindow = new SettingsWindow(viewModel);
        await settingsWindow.ShowDialog(this);
    }

    private async Task OpenAboutAsync()
    {
        var aboutWindow = new AboutWindow();
        await aboutWindow.ShowDialog(this);
    }

    private void FocusButton_OnClick(object? sender, RoutedEventArgs e) => ToggleFocusMode();

    private void MiniButton_OnClick(object? sender, RoutedEventArgs e) => ToggleMiniMode();

    private async void SettingsButton_OnClick(object? sender, RoutedEventArgs e) =>
        await OpenSettingsAsync();

    private async void AboutButton_OnClick(object? sender, RoutedEventArgs e) =>
        await OpenAboutAsync();

    private async void FormatButton_OnClick(object? sender, RoutedEventArgs e) =>
        await viewModel.ToggleClockFormatAsync();

    private async void SecondsButton_OnClick(object? sender, RoutedEventArgs e) =>
        await viewModel.ToggleSecondsAsync();

    private void TimeZoneSearchBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            viewModel.SearchTimeZones(textBox.Text);
        }
    }

    private async void AddTimeZoneButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var list = this.FindControl<ListBox>("TimeZoneResults");
        if (list?.SelectedItem is TimeZoneDescriptor descriptor)
        {
            await viewModel.AddWorldClockAsync(descriptor);
        }
    }

    private async void RemoveWorldClock_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string id })
        {
            await viewModel.RemoveWorldClockAsync(id);
        }
    }

    private async void MainWindow_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.F11)
        {
            ToggleFocusMode();
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape && (isFocusMode || isMiniMode))
        {
            if (isFocusMode)
            {
                ToggleFocusMode();
            }
            else
            {
                ExitMiniMode();
            }

            e.Handled = true;
            return;
        }

        var control = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        var shift = e.KeyModifiers.HasFlag(KeyModifiers.Shift);

        if (control && e.Key == Key.M)
        {
            ToggleMiniMode();
            e.Handled = true;
        }
        else if (control && e.Key == Key.K)
        {
            this.FindControl<TextBox>("TimeZoneSearchBox")?.Focus();
            e.Handled = true;
        }
        else if (control && e.Key == Key.OemComma)
        {
            await OpenSettingsAsync();
            e.Handled = true;
        }
        else if (control && shift && e.Key == Key.T)
        {
            await viewModel.UpdateSettingsAsync(
                viewModel.Settings with { AlwaysOnTop = !viewModel.Settings.AlwaysOnTop });
            e.Handled = true;
        }
    }
}
