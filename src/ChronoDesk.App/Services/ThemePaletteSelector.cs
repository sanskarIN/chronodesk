using Avalonia.Styling;
using ChronoDesk.Core.Models;

namespace ChronoDesk.App.Services;

public sealed record ThemePalette(
    string Surface,
    string Card,
    string Muted,
    string Accent,
    string Border);

public static class ThemePaletteSelector
{
    private static readonly ThemePalette Light =
        new("#F5F7FB", "#FFFFFF", "#5F6878", "#5B4AF0", "#DCE2EC");

    private static readonly ThemePalette Dark =
        new("#10131A", "#1F2430", "#AEB7C7", "#6D5DFB", "#354052");

    private static readonly ThemePalette HighContrast =
        new("#000000", "#101010", "#FFFFFF", "#FFD400", "#FFFFFF");

    public static ThemePalette Select(AppSettings settings, ThemeVariant actualThemeVariant)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(actualThemeVariant);

        if (settings.HighContrast || settings.Theme == ThemeMode.HighContrast)
        {
            return HighContrast;
        }

        if (settings.Theme == ThemeMode.Light)
        {
            return Light;
        }

        if (settings.Theme == ThemeMode.Dark)
        {
            return Dark;
        }

        return actualThemeVariant == ThemeVariant.Dark ? Dark : Light;
    }
}
