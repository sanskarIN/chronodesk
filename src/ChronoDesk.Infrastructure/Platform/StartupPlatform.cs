namespace ChronoDesk.Infrastructure.Platform;

internal enum StartupPlatform
{
    Unsupported,
    Windows,
    MacOS,
    Linux,
}

internal static class StartupPlatformDetector
{
    public static StartupPlatform Detect()
    {
        if (OperatingSystem.IsWindows())
        {
            return StartupPlatform.Windows;
        }

        if (OperatingSystem.IsMacOS())
        {
            return StartupPlatform.MacOS;
        }

        if (OperatingSystem.IsLinux())
        {
            return StartupPlatform.Linux;
        }

        return StartupPlatform.Unsupported;
    }
}
