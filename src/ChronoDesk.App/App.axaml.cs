using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using ChronoDesk.App.Localization;
using ChronoDesk.App.ViewModels;
using ChronoDesk.App.Views;
using ChronoDesk.Core.Models;

namespace ChronoDesk.App;

public sealed partial class App : Application
{
    private TrayIcon? trayIcon;

    public AppServices Services { get; } = new();

    public bool IsTrayIntegrationAvailable { get; private set; }

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var viewModel = new MainWindowViewModel(Services);
            var window = new MainWindow(viewModel);
            viewModel.SettingsChanged += (_, settings) => ApplyTheme(settings);
            desktop.MainWindow = window;
            TryCreateTrayIcon(window, desktop);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ApplyTheme(AppSettings settings)
    {
        var highContrast = settings.HighContrast || settings.Theme == ThemeMode.HighContrast;
        RequestedThemeVariant = highContrast
            ? ThemeVariant.Dark
            : settings.Theme switch
            {
                ThemeMode.Light => ThemeVariant.Light,
                ThemeMode.Dark => ThemeVariant.Dark,
                _ => ThemeVariant.Default,
            };

        if (highContrast)
        {
            SetPalette("#000000", "#101010", "#FFFFFF", "#FFD400", "#FFFFFF");
            return;
        }

        var dark = RequestedThemeVariant == ThemeVariant.Dark
            || (RequestedThemeVariant == ThemeVariant.Default
                && ActualThemeVariant == ThemeVariant.Dark);

        if (dark)
        {
            SetPalette("#10131A", "#1F2430", "#AEB7C7", "#6D5DFB", "#354052");
        }
        else
        {
            SetPalette("#F5F7FB", "#FFFFFF", "#5F6878", "#5B4AF0", "#DCE2EC");
        }
    }

    private void SetPalette(
        string surface,
        string card,
        string muted,
        string accent,
        string border)
    {
        Resources["ChronoSurfaceBrush"] = new SolidColorBrush(Color.Parse(surface));
        Resources["ChronoCardBrush"] = new SolidColorBrush(Color.Parse(card));
        Resources["ChronoMutedBrush"] = new SolidColorBrush(Color.Parse(muted));
        Resources["ChronoAccentBrush"] = new SolidColorBrush(Color.Parse(accent));
        Resources["ChronoBorderBrush"] = new SolidColorBrush(Color.Parse(border));
    }

    private void TryCreateTrayIcon(
        MainWindow window,
        IClassicDesktopStyleApplicationLifetime desktop)
    {
        IsTrayIntegrationAvailable = false;

        try
        {
            using var stream = AssetLoader.Open(new Uri("avares://ChronoDesk/Assets/chronodesk.ico"));
            var icon = new WindowIcon(stream);
            trayIcon = new TrayIcon
            {
                Icon = icon,
                ToolTipText = Strings.AppName,
                Menu = BuildTrayMenu(window, desktop),
                IsVisible = true,
            };
            TrayIcon.SetIcons(this, new TrayIcons { trayIcon });

            IsTrayIntegrationAvailable = trayIcon.NativeMenuExporter is not null;
            if (!IsTrayIntegrationAvailable)
            {
                Services.Logger.Warning(
                    "tray.restore_unavailable",
                    "Tray menu integration is unavailable; minimize-to-tray behavior is disabled for this session.");
            }

            desktop.Exit += (_, _) => trayIcon?.Dispose();
        }
        catch (Exception exception)
        {
            IsTrayIntegrationAvailable = false;
            Services.Logger.Error("tray.initialize_failed", exception, "System tray integration could not be initialized.");
        }
    }

    private static NativeMenu BuildTrayMenu(
        MainWindow window,
        IClassicDesktopStyleApplicationLifetime desktop)
    {
        var menu = new NativeMenu();

        var show = new NativeMenuItem(Strings.TrayShow);
        show.Click += (_, _) => window.ShowFromTray();
        menu.Add(show);

        var focus = new NativeMenuItem(Strings.TrayFocus);
        focus.Click += (_, _) => window.ToggleFocusMode();
        menu.Add(focus);

        var mini = new NativeMenuItem(Strings.TrayMini);
        mini.Click += (_, _) => window.ToggleMiniMode();
        menu.Add(mini);

        menu.Add(new NativeMenuItemSeparator());

        var quit = new NativeMenuItem(Strings.TrayQuit);
        quit.Click += (_, _) =>
        {
            window.AllowClose();
            desktop.Shutdown();
        };
        menu.Add(quit);

        return menu;
    }
}
