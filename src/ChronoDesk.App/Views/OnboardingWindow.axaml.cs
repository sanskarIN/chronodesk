using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ChronoDesk.App.ViewModels;

namespace ChronoDesk.App.Views;

public sealed partial class OnboardingWindow : Window
{
    private readonly MainWindowViewModel viewModel;

    public OnboardingWindow(MainWindowViewModel viewModel)
    {
        this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private async void ContinueButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await viewModel.CompleteOnboardingAsync();
        Close();
    }
}
