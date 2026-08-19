using System.Reflection;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ChronoDesk.App.Localization;

namespace ChronoDesk.App.Views;

public sealed partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        var version = AppVersionProvider.GetDisplayVersion(Assembly.GetExecutingAssembly());
        var versionText = this.FindControl<TextBlock>("VersionText");
        if (versionText is not null)
        {
            versionText.Text = Strings.Format(nameof(Strings.VersionFormat), version);
        }
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void GitHubButton_OnClick(object? sender, RoutedEventArgs e) =>
        ExternalLinkLauncher.TryOpen("https://github.com/sanskarIN/chronodesk");

    private void BmcButton_OnClick(object? sender, RoutedEventArgs e) =>
        ExternalLinkLauncher.TryOpen("https://buymeacoffee.com/sanskarIN");

    private void BusinessPrimaryButton_OnClick(object? sender, RoutedEventArgs e) =>
        ExternalLinkLauncher.TryOpen("mailto:sanskarin@outlook.in");

    private void BusinessSecondaryButton_OnClick(object? sender, RoutedEventArgs e) =>
        ExternalLinkLauncher.TryOpen("mailto:sanskarin.business@gmail.com");

    private void SupportButton_OnClick(object? sender, RoutedEventArgs e) =>
        ExternalLinkLauncher.TryOpen("mailto:supportramsandesh@gmail.com");

    private void CloseButton_OnClick(object? sender, RoutedEventArgs e) => Close();
}
