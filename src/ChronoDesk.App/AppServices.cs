using ChronoDesk.Core.Abstractions;
using ChronoDesk.Core.Services;
using ChronoDesk.Infrastructure.Logging;
using ChronoDesk.Infrastructure.Persistence;
using ChronoDesk.Infrastructure.Platform;
using ChronoDesk.Infrastructure.Time;

namespace ChronoDesk.App;

public sealed class AppServices
{
    public AppServices()
    {
        Logger = new SafeFileLogger();
        SettingsStore = new JsonSettingsStore(Logger);
        TimeZones = new SystemTimeZoneCatalog();
        StartupManager = new PlatformStartupManager();
        ChimePlayer = new SystemChimePlayer();
        ClockFormatter = new ClockFormatter();
        ChimePolicy = new ChimePolicy();
    }

    public IAppLogger Logger { get; }

    public ISettingsStore SettingsStore { get; }

    public ITimeZoneCatalog TimeZones { get; }

    public IStartupManager StartupManager { get; }

    public IChimePlayer ChimePlayer { get; }

    public ClockFormatter ClockFormatter { get; }

    public ChimePolicy ChimePolicy { get; }
}
