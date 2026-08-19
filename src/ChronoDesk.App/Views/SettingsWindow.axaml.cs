using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using ChronoDesk.App.Localization;
using ChronoDesk.App.Services;
using ChronoDesk.App.ViewModels;
using ChronoDesk.Core.Models;

namespace ChronoDesk.App.Views;

public sealed partial class SettingsWindow : Window
{
    private readonly MainWindowViewModel viewModel;

    public SettingsWindow(MainWindowViewModel viewModel)
    {
        this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        InitializeComponent();
        LoadControls(viewModel.Settings);
        PopulateStaticInformation();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void PopulateStaticInformation()
    {
        var displayVersion = AppVersionInfo.GetDisplayVersion();
        Control<TextBlock>("UpdateVersionText").Text = displayVersion;
        Control<TextBlock>("SettingsAboutVersionText").Text =
            Strings.Format(nameof(Strings.VersionFormat), displayVersion);

        var diagnostics = AppDiagnosticsInfo.Capture();
        Control<TextBox>("DiagnosticsVersionText").Text = diagnostics.AppVersion;
        Control<TextBox>("DiagnosticsOsText").Text = diagnostics.OperatingSystem;
        Control<TextBox>("DiagnosticsFrameworkText").Text = diagnostics.Framework;
        Control<TextBox>("DiagnosticsArchitectureText").Text = diagnostics.ProcessArchitecture;
        Control<TextBox>("DiagnosticsDataDirectoryText").Text = diagnostics.DataDirectory;
        Control<TextBox>("DiagnosticsSettingsPathText").Text = diagnostics.SettingsPath;
        Control<TextBox>("DiagnosticsLogPathText").Text = diagnostics.LogPath;
    }

    private void LoadControls(AppSettings settings)
    {
        Control<ComboBox>("FormatCombo").SelectedIndex =
            settings.ClockFormat == ClockFormat.TwelveHour ? 0 : 1;
        Control<CheckBox>("ShowSecondsCheck").IsChecked = settings.ShowSeconds;
        Control<CheckBox>("ShowDateCheck").IsChecked = settings.ShowDate;
        Control<CheckBox>("ShowWeekdayCheck").IsChecked = settings.ShowWeekday;
        Control<CheckBox>("ShowWeekNumberCheck").IsChecked = settings.ShowWeekNumber;
        Control<CheckBox>("ShowCalendarDetailsCheck").IsChecked = settings.ShowCalendarDetails;

        Control<ComboBox>("ThemeCombo").SelectedIndex = settings.Theme switch
        {
            ThemeMode.Light => 1,
            ThemeMode.Dark => 2,
            ThemeMode.HighContrast => 3,
            _ => 0,
        };
        Control<ComboBox>("LayoutCombo").SelectedIndex = settings.Layout switch
        {
            ClockLayout.Compact => 1,
            ClockLayout.Dashboard => 2,
            _ => 0,
        };
        Control<TextBox>("FontFamilyText").Text = settings.FontFamilyName;
        Control<Slider>("FontSizeSlider").Value = settings.ClockFontSize;
        Control<Slider>("SpacingSlider").Value = settings.ContentSpacing;

        Control<CheckBox>("ReducedMotionCheck").IsChecked = settings.ReducedMotion;
        Control<CheckBox>("HighContrastCheck").IsChecked = settings.HighContrast;
        Control<CheckBox>("AlwaysOnTopCheck").IsChecked = settings.AlwaysOnTop;
        Control<CheckBox>("StartWithSystemCheck").IsChecked = settings.StartWithSystem;
        Control<CheckBox>("MinimizeToTrayCheck").IsChecked = settings.MinimizeToTray;

        Control<CheckBox>("ChimeEnabledCheck").IsChecked = settings.Chime.Enabled;
        Control<ComboBox>("ChimeIntervalCombo").SelectedIndex = settings.Chime.Interval switch
        {
            ChimeInterval.HalfHourly => 1,
            ChimeInterval.QuarterHourly => 2,
            _ => 0,
        };
        Control<CheckBox>("QuietHoursCheck").IsChecked = settings.Chime.QuietHours.Enabled;
        Control<TextBox>("QuietStartText").Text = settings.Chime.QuietHours.Start.ToString("HH:mm", CultureInfo.InvariantCulture);
        Control<TextBox>("QuietEndText").Text = settings.Chime.QuietHours.End.ToString("HH:mm", CultureInfo.InvariantCulture);
        SetStatus(string.Empty);
    }

    private async void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!TryReadQuietTime("QuietStartText", out var quietStart)
            || !TryReadQuietTime("QuietEndText", out var quietEnd))
        {
            SetStatus(Strings.InvalidQuietHours);
            return;
        }

        var current = viewModel.Settings;
        var theme = Control<ComboBox>("ThemeCombo").SelectedIndex switch
        {
            1 => ThemeMode.Light,
            2 => ThemeMode.Dark,
            3 => ThemeMode.HighContrast,
            _ => ThemeMode.System,
        };
        var layout = Control<ComboBox>("LayoutCombo").SelectedIndex switch
        {
            1 => ClockLayout.Compact,
            2 => ClockLayout.Dashboard,
            _ => ClockLayout.Centered,
        };
        var interval = Control<ComboBox>("ChimeIntervalCombo").SelectedIndex switch
        {
            1 => ChimeInterval.HalfHourly,
            2 => ChimeInterval.QuarterHourly,
            _ => ChimeInterval.Hourly,
        };

        var updated = current with
        {
            ClockFormat = Control<ComboBox>("FormatCombo").SelectedIndex == 0
                ? ClockFormat.TwelveHour
                : ClockFormat.TwentyFourHour,
            ShowSeconds = IsChecked("ShowSecondsCheck"),
            ShowDate = IsChecked("ShowDateCheck"),
            ShowWeekday = IsChecked("ShowWeekdayCheck"),
            ShowWeekNumber = IsChecked("ShowWeekNumberCheck"),
            ShowCalendarDetails = IsChecked("ShowCalendarDetailsCheck"),
            Theme = theme,
            Layout = layout,
            FontFamilyName = Control<TextBox>("FontFamilyText").Text ?? "Inter",
            ClockFontSize = Control<Slider>("FontSizeSlider").Value,
            ContentSpacing = Control<Slider>("SpacingSlider").Value,
            ReducedMotion = IsChecked("ReducedMotionCheck"),
            HighContrast = IsChecked("HighContrastCheck"),
            AlwaysOnTop = IsChecked("AlwaysOnTopCheck"),
            StartWithSystem = IsChecked("StartWithSystemCheck"),
            MinimizeToTray = IsChecked("MinimizeToTrayCheck"),
            Chime = current.Chime with
            {
                Enabled = IsChecked("ChimeEnabledCheck"),
                Interval = interval,
                QuietHours = new QuietHours
                {
                    Enabled = IsChecked("QuietHoursCheck"),
                    Start = quietStart,
                    End = quietEnd,
                },
            },
        };

        try
        {
            await viewModel.UpdateSettingsAsync(updated);
            Close();
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            SetStatus(Strings.SettingsSaveError);
        }
    }

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e) => Close();

    private void OpenReleasesButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!ExternalUriLauncher.TryOpen(AppLinks.Releases))
        {
            SetStatus(UpdateStrings.ReleaseOpenError);
        }
    }

    private void SettingsGitHubButton_OnClick(object? sender, RoutedEventArgs e) =>
        TryOpenApprovedLink(AppLinks.Repository);

    private void SettingsBmcButton_OnClick(object? sender, RoutedEventArgs e) =>
        TryOpenApprovedLink(AppLinks.Funding);

    private void SettingsBusinessPrimaryButton_OnClick(object? sender, RoutedEventArgs e) =>
        TryOpenApprovedLink(AppLinks.BusinessPrimary);

    private void SettingsBusinessSecondaryButton_OnClick(object? sender, RoutedEventArgs e) =>
        TryOpenApprovedLink(AppLinks.BusinessSecondary);

    private void SettingsSupportButton_OnClick(object? sender, RoutedEventArgs e) =>
        TryOpenApprovedLink(AppLinks.Support);

    private void TryOpenApprovedLink(string destination)
    {
        if (!ExternalUriLauncher.TryOpen(destination))
        {
            SetStatus(UpdateStrings.ExternalOpenError);
        }
    }

    private async void ExportButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var jsonType = new FilePickerFileType(Strings.SettingsFileType)
        {
            Patterns = ["*.json"],
            MimeTypes = ["application/json"],
        };
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = Strings.ExportDialogTitle,
            SuggestedFileName = "chronodesk-settings.json",
            DefaultExtension = "json",
            FileTypeChoices = [jsonType],
        });

        if (file is null)
        {
            return;
        }

        try
        {
            await viewModel.ExportSettingsAsync(file.Path.LocalPath);
            SetStatus(Strings.ExportSuccess);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            SetStatus(Strings.ExportError);
        }
    }

    private async void ImportButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var jsonType = new FilePickerFileType(Strings.SettingsFileType)
        {
            Patterns = ["*.json"],
            MimeTypes = ["application/json"],
        };
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = Strings.ImportDialogTitle,
            AllowMultiple = false,
            FileTypeFilter = [jsonType],
        });

        var file = files.FirstOrDefault();
        if (file is null)
        {
            return;
        }

        try
        {
            await viewModel.ImportSettingsAsync(file.Path.LocalPath);
            LoadControls(viewModel.Settings);
            SetStatus(Strings.ImportSuccess);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or InvalidDataException or System.Text.Json.JsonException)
        {
            SetStatus(Strings.ImportError);
        }
    }

    private async void ResetButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            await viewModel.ResetSettingsAsync();
            LoadControls(viewModel.Settings);
            SetStatus(Strings.DefaultsRestored);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            SetStatus(Strings.DefaultsSaveError);
        }
    }

    private T Control<T>(string name)
        where T : Control =>
        this.FindControl<T>(name)
        ?? throw new InvalidOperationException($"Required settings control '{name}' was not found.");

    private bool IsChecked(string name) => Control<CheckBox>(name).IsChecked == true;

    private bool TryReadQuietTime(string controlName, out TimeOnly value)
    {
        var input = Control<TextBox>(controlName).Text;
        return TimeOnly.TryParseExact(
                input,
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out value)
            || TimeOnly.TryParse(
                input,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out value);
    }

    private void SetStatus(string message) => Control<TextBlock>("SettingsStatusText").Text = message;
}
