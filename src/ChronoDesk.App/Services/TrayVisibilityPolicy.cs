namespace ChronoDesk.App.Services;

public static class TrayVisibilityPolicy
{
    public static bool ShouldHideOnClose(
        bool allowClose,
        bool minimizeToTray,
        bool trayIntegrationAvailable) =>
        !allowClose && minimizeToTray && trayIntegrationAvailable;

    public static bool ShouldStartHidden(
        bool backgroundStart,
        bool minimizeToTray,
        bool trayIntegrationAvailable) =>
        backgroundStart && minimizeToTray && trayIntegrationAvailable;
}
