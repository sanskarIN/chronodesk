using Avalonia;
using Avalonia.Browser;
using ChronoDesk.App;

namespace ChronoDesk.Browser;

internal static class Program
{
    private static Task Main(string[] args) =>
        AppBuilder.Configure<App>()
            .WithInterFont()
            .StartBrowserAppAsync("out");
}
