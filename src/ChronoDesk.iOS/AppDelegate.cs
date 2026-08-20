using Avalonia;
using Avalonia.iOS;
using ChronoDesk.App;
using Foundation;

namespace ChronoDesk.iOS;

[Register("AppDelegate")]
#pragma warning disable CA1711
public sealed class AppDelegate : AvaloniaAppDelegate<App>
#pragma warning restore CA1711
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder) =>
        base.CustomizeAppBuilder(builder)
            .WithInterFont();
}
