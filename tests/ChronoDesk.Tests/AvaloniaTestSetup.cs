using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;

[assembly: AvaloniaTestApplication(typeof(ChronoDesk.Tests.AvaloniaTestSetup))]

namespace ChronoDesk.Tests;

public static class AvaloniaTestSetup
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<ChronoDesk.App.App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
