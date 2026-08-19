using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ChronoDesk.App.Localization;
using ChronoDesk.App.Services;

namespace ChronoDesk.App.Views;

public sealed partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        var versionText = this.FindControl<TextBlock>("VersionText");
        if (versionText is not null)
        {
            versionText.Text = Strings.Format(
                nameof(Strings.VersionFormat),
                AppVersionInfo.GetDisplayVersion());
        }
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void GitHubButton_OnClick(object? sender, RoutedEventArgs e) =>
        ExternalUriLauncher.TryOpen(AppLinks.Repository);

    private void BmcButton_OnClick(object? sender, RoutedEventArgs e) =>
        ExternalUriLauncher.TryOpen(AppLinks.Funding);

    private void BusinessPrimaryButton_OnClick(object? sender, RoutedEventArgs e) =>
        ExternalUriLauncher.TryOpen(AppLinks.BusinessPrimary);

    private void BusinessSecondaryButton_OnClick(object? sender, RoutedEventArgs e) =>
        ExternalUriLauncher.TryOpen(AppLinks.BusinessSecondary);

    private void SupportButton_OnClick(object? sender, RoutedEventArgs e) =>
        ExternalUriLauncher.TryOpen(AppLinks.Support);

    private void CloseButton_OnClick(object? sender, RoutedEventArgs e) => Close();
}
