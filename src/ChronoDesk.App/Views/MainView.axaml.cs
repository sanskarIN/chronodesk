using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ChronoDesk.App.ViewModels;
using ChronoDesk.Core.Models;

namespace ChronoDesk.App.Views;

public sealed partial class MainView : UserControl
{
    private readonly MainWindowViewModel viewModel;
    private readonly DispatcherTimer clockTimer;
    private bool tickInProgress;

    public MainView(MainWindowViewModel viewModel)
    {
        this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        InitializeComponent();
        DataContext = viewModel;

        clockTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
        clockTimer.Tick += ClockTimer_OnTick;
        AttachedToVisualTree += MainView_OnAttachedToVisualTree;
        DetachedFromVisualTree += MainView_OnDetachedFromVisualTree;
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private async void MainView_OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        try
        {
            if (!viewModel.IsInitialized)
            {
                await viewModel.InitializeAsync();
            }
        }
        catch (Exception exception)
        {
            LogFailure("single_view.initialize_failed", exception, "The single-view shell could not initialize.");
            return;
        }

        if (viewModel.Settings.IsFirstRun)
        {
            try
            {
                await viewModel.CompleteOnboardingAsync();
            }
            catch (Exception exception)
            {
                LogFailure(
                    "single_view.onboarding_persist_failed",
                    exception,
                    "The first-run state could not be persisted; the clock will continue running.");
            }
        }

        clockTimer.Start();
    }

    private void MainView_OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e) =>
        clockTimer.Stop();

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
            LogFailure("single_view.tick_failed", exception, "The clock could not refresh for one tick.");
        }
        finally
        {
            tickInProgress = false;
        }
    }

    private async void FormatButton_OnClick(object? sender, RoutedEventArgs e) =>
        await ExecuteUiActionAsync(
            () => viewModel.ToggleClockFormatAsync(),
            "single_view.format_failed",
            "The clock format could not be changed.");

    private async void SecondsButton_OnClick(object? sender, RoutedEventArgs e) =>
        await ExecuteUiActionAsync(
            () => viewModel.ToggleSecondsAsync(),
            "single_view.seconds_failed",
            "The seconds preference could not be changed.");

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
            await ExecuteUiActionAsync(
                () => viewModel.AddWorldClockAsync(descriptor),
                "single_view.world_clock_add_failed",
                "The selected world clock could not be added.");
        }
    }

    private async void RemoveWorldClock_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string id })
        {
            await ExecuteUiActionAsync(
                () => viewModel.RemoveWorldClockAsync(id),
                "single_view.world_clock_remove_failed",
                "The selected world clock could not be removed.");
        }
    }

    private static async Task ExecuteUiActionAsync(
        Func<Task> action,
        string eventName,
        string safeMessage)
    {
        try
        {
            await action();
        }
        catch (Exception exception)
        {
            LogFailure(eventName, exception, safeMessage);
        }
    }

    private static void LogFailure(string eventName, Exception exception, string safeMessage)
    {
        if (Application.Current is App app)
        {
            app.Services.Logger.LogError(eventName, exception, safeMessage);
        }
    }
}
