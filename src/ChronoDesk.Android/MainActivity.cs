using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using ChronoDesk.App;

namespace ChronoDesk.Android;

[Activity(
    Label = "ChronoDesk",
    Theme = "@android:style/Theme.Material.Light.NoActionBar",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public sealed class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder) =>
        base.CustomizeAppBuilder(builder)
            .WithInterFont();
}
