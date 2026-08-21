using System.Runtime.Versioning;
using ChronoDesk.Core.Abstractions;
using Microsoft.Win32;

namespace ChronoDesk.Infrastructure.Platform;

public sealed class PlatformStartupManager : IStartupManager
{
    private const string WindowsRunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "ChronoDesk";
    private const string MacLaunchAgentName = "com.sanskar.chronodesk.plist";
    private readonly string executablePath;

    public PlatformStartupManager(string? executablePath = null)
    {
        this.executablePath = executablePath
            ?? Environment.ProcessPath
            ?? throw new InvalidOperationException("ChronoDesk executable path could not be determined.");
    }

    public bool IsSupported =>
        OperatingSystem.IsWindows() || OperatingSystem.IsMacOS() || OperatingSystem.IsLinux();

    public Task<bool> IsEnabledAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (OperatingSystem.IsWindows())
        {
            return Task.FromResult(IsWindowsStartupEnabled());
        }

        if (OperatingSystem.IsMacOS())
        {
            return Task.FromResult(File.Exists(GetMacLaunchAgentPath()));
        }

        if (OperatingSystem.IsLinux())
        {
            return Task.FromResult(File.Exists(GetLinuxAutostartPath()));
        }

        return Task.FromResult(false);
    }

    public async Task SetEnabledAsync(bool enabled, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (OperatingSystem.IsWindows())
        {
            SetWindowsStartup(enabled);
            return;
        }

        if (OperatingSystem.IsMacOS())
        {
            await SetMacStartupAsync(enabled, cancellationToken);
            return;
        }

        if (OperatingSystem.IsLinux())
        {
            await SetLinuxStartupAsync(enabled, cancellationToken);
            return;
        }

        throw new PlatformNotSupportedException("Startup integration is not supported on this platform.");
    }

    [SupportedOSPlatform("windows")]
    private bool IsWindowsStartupEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(WindowsRunKey, writable: false);
        return key?.GetValue(AppName) is string value
            && value.Contains(executablePath, StringComparison.OrdinalIgnoreCase);
    }

    [SupportedOSPlatform("windows")]
    private void SetWindowsStartup(bool enabled)
    {
        using var key = Registry.CurrentUser.CreateSubKey(WindowsRunKey, writable: true)
            ?? throw new InvalidOperationException("Windows startup registry key could not be opened.");

        if (enabled)
        {
            key.SetValue(AppName, $"\"{executablePath}\" --background", RegistryValueKind.String);
        }
        else
        {
            key.DeleteValue(AppName, throwOnMissingValue: false);
        }
    }

    private async Task SetMacStartupAsync(bool enabled, CancellationToken cancellationToken)
    {
        var path = GetMacLaunchAgentPath();
        if (!enabled)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var content = MacLaunchAgentPlist.Create(executablePath);
        await File.WriteAllTextAsync(path, content, cancellationToken);
    }

    private async Task SetLinuxStartupAsync(bool enabled, CancellationToken cancellationToken)
    {
        var path = GetLinuxAutostartPath();
        if (!enabled)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var content = LinuxDesktopEntry.Create(executablePath);
        await File.WriteAllTextAsync(path, content, cancellationToken);
    }

    private static string GetMacLaunchAgentPath()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(home, "Library", "LaunchAgents", MacLaunchAgentName);
    }

    private static string GetLinuxAutostartPath()
    {
        var configHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        if (string.IsNullOrWhiteSpace(configHome))
        {
            configHome = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".config");
        }

        return Path.Combine(configHome, "autostart", "chronodesk.desktop");
    }
}
