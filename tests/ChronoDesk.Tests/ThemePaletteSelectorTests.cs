using Avalonia.Styling;
using ChronoDesk.App.Services;
using ChronoDesk.Core.Models;

namespace ChronoDesk.Tests;

public sealed class ThemePaletteSelectorTests
{
    [Fact]
    public void Select_SystemThemeFollowsActualDarkVariant()
    {
        var palette = ThemePaletteSelector.Select(
            new AppSettings { Theme = ThemeMode.System },
            ThemeVariant.Dark);

        Assert.Equal("#10131A", palette.Surface);
        Assert.Equal("#1F2430", palette.Card);
    }

    [Fact]
    public void Select_SystemThemeFollowsActualLightVariant()
    {
        var palette = ThemePaletteSelector.Select(
            new AppSettings { Theme = ThemeMode.System },
            ThemeVariant.Light);

        Assert.Equal("#F5F7FB", palette.Surface);
        Assert.Equal("#FFFFFF", palette.Card);
    }

    [Fact]
    public void Select_ExplicitThemeOverridesActualVariant()
    {
        var palette = ThemePaletteSelector.Select(
            new AppSettings { Theme = ThemeMode.Light },
            ThemeVariant.Dark);

        Assert.Equal("#F5F7FB", palette.Surface);
    }

    [Fact]
    public void Select_HighContrastPreferenceOverridesThemeSelection()
    {
        var palette = ThemePaletteSelector.Select(
            new AppSettings
            {
                Theme = ThemeMode.Light,
                HighContrast = true,
            },
            ThemeVariant.Light);

        Assert.Equal("#000000", palette.Surface);
        Assert.Equal("#FFFFFF", palette.Muted);
        Assert.Equal("#FFD400", palette.Accent);
    }
}
