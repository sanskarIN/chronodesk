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

    public AppServices(
        IAppLogger logger,
        ISettingsStore settingsStore,
        ITimeZoneCatalog timeZones,
        IStartupManager startupManager,
        IChimePlayer chimePlayer,
        ClockFormatter? clockFormatter = null,
        ChimePolicy? chimePolicy = null)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        SettingsStore = settingsStore ?? throw new ArgumentNullException(nameof(settingsStore));
        TimeZones = timeZones ?? throw new ArgumentNullException(nameof(timeZones));
        StartupManager = startupManager ?? throw new ArgumentNullException(nameof(startupManager));
        ChimePlayer = chimePlayer ?? throw new ArgumentNullException(nameof(chimePlayer));
        ClockFormatter = clockFormatter ?? new ClockFormatter();
        ChimePolicy = chimePolicy ?? new ChimePolicy();
    }

    public IAppLogger Logger { get; }

    public ISettingsStore SettingsStore { get; }

    public ITimeZoneCatalog TimeZones { get; }

    public IStartupManager StartupManager { get; }

    public IChimePlayer ChimePlayer { get; }

    public ClockFormatter ClockFormatter { get; }

    public ChimePolicy ChimePolicy { get; }
}
