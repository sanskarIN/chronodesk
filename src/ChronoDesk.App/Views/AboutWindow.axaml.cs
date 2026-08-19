using System.ComponentModel;
using System.Diagnostics;
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
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "development";
        var versionText = this.FindControl<TextBlock>("VersionText");
        if (versionText is not null)
        {
            versionText.Text = Strings.Format(nameof(Strings.VersionFormat), version);
        }
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void GitHubButton_OnClick(object? sender, RoutedEventArgs e) =>
        OpenUri("https://github.com/sanskarIN/chronodesk");

    private void BmcButton_OnClick(object? sender, RoutedEventArgs e) =>
        OpenUri("https://buymeacoffee.com/sanskarIN");

    private void BusinessPrimaryButton_OnClick(object? sender, RoutedEventArgs e) =>
        OpenUri("mailto:sanskarin@outlook.in");

    private void BusinessSecondaryButton_OnClick(object? sender, RoutedEventArgs e) =>
        OpenUri("mailto:sanskarin.business@gmail.com");

    private void SupportButton_OnClick(object? sender, RoutedEventArgs e) =>
        OpenUri("mailto:supportramsandesh@gmail.com");

    private void CloseButton_OnClick(object? sender, RoutedEventArgs e) => Close();

    private static void OpenUri(string value)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeMailto))
        {
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
        }
        catch (Exception exception) when (exception is Win32Exception or InvalidOperationException)
        {
            // A missing browser/mail handler is non-fatal; the visible address remains copyable.
        }
    }
}
