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

            if (viewModel.Settings.IsFirstRun)
            {
                await viewModel.CompleteOnboardingAsync();
            }

            clockTimer.Start();
        }
        catch (Exception exception)
        {
            LogFailure("single_view.initialize_failed", exception, "The single-view shell could not finish initialization.");
        }
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

    private static void LogFailure(string eventName, Exception exception, string safeMessage)
    {
        if (Application.Current is App app)
        {
            app.Services.Logger.Error(eventName, exception, safeMessage);
        }
    }
}
