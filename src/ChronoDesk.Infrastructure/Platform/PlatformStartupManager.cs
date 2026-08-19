using System.Security;
using ChronoDesk.Core.Abstractions;

namespace ChronoDesk.Infrastructure.Platform;

public sealed class PlatformStartupManager : IStartupManager
{
    private const string WindowsRunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "ChronoDesk";
    private const string MacLaunchAgentName = "com.sanskar.chronodesk.plist";
    private readonly string executablePath;
    private readonly StartupPlatform platform;
    private readonly IStartupFileSystem fileSystem;
    private readonly IStartupRegistry registry;
    private readonly string userProfilePath;
    private readonly string? xdgConfigHome;

    public PlatformStartupManager(string? executablePath = null)
        : this(
            executablePath
                ?? Environment.ProcessPath
                ?? throw new InvalidOperationException("ChronoDesk executable path could not be determined."),
            StartupPlatformDetector.Detect(),
            new SystemStartupFileSystem(),
            new SystemStartupRegistry(),
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"))
    {
    }

    internal PlatformStartupManager(
        string executablePath,
        StartupPlatform platform,
        IStartupFileSystem fileSystem,
        IStartupRegistry registry,
        string userProfilePath,
        string? xdgConfigHome)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executablePath);
        ArgumentNullException.ThrowIfNull(fileSystem);
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentException.ThrowIfNullOrWhiteSpace(userProfilePath);

        this.executablePath = executablePath;
        this.platform = platform;
        this.fileSystem = fileSystem;
        this.registry = registry;
        this.userProfilePath = userProfilePath;
        this.xdgConfigHome = xdgConfigHome;
    }

    public bool IsSupported => platform is not StartupPlatform.Unsupported;

    public Task<bool> IsEnabledAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return platform switch
        {
            StartupPlatform.Windows => Task.FromResult(IsWindowsStartupEnabled()),
            StartupPlatform.MacOS => Task.FromResult(fileSystem.FileExists(GetMacLaunchAgentPath())),
            StartupPlatform.Linux => Task.FromResult(fileSystem.FileExists(GetLinuxAutostartPath())),
            _ => Task.FromResult(false),
        };
    }

    public async Task SetEnabledAsync(bool enabled, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        switch (platform)
        {
            case StartupPlatform.Windows:
                SetWindowsStartup(enabled);
                return;
            case StartupPlatform.MacOS:
                await SetMacStartupAsync(enabled, cancellationToken);
                return;
            case StartupPlatform.Linux:
                await SetLinuxStartupAsync(enabled, cancellationToken);
                return;
            default:
                throw new PlatformNotSupportedException("Startup integration is not supported on this platform.");
        }
    }

    private bool IsWindowsStartupEnabled() =>
        registry.GetCurrentUserString(WindowsRunKey, AppName) is string value
        && value.Contains(executablePath, StringComparison.OrdinalIgnoreCase);

    private void SetWindowsStartup(bool enabled)
    {
        if (enabled)
        {
            registry.SetCurrentUserString(
                WindowsRunKey,
                AppName,
                $"\"{executablePath}\" --background");
        }
        else
        {
            registry.DeleteCurrentUserValue(WindowsRunKey, AppName);
        }
    }

    private async Task SetMacStartupAsync(bool enabled, CancellationToken cancellationToken)
    {
        var path = GetMacLaunchAgentPath();
        if (!enabled)
        {
            if (fileSystem.FileExists(path))
            {
                fileSystem.DeleteFile(path);
            }

            return;
        }

        fileSystem.CreateDirectory(Path.GetDirectoryName(path)!);
        var escapedExecutable = SecurityElement.Escape(executablePath) ?? executablePath;
        var content = $"""
            <?xml version="1.0" encoding="UTF-8"?>
            <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
            <plist version="1.0">
              <dict>
                <key>Label</key>
                <string>com.sanskar.chronodesk</string>
                <key>ProgramArguments</key>
                <array>
                  <string>{escapedExecutable}</string>
                  <string>--background</string>
                </array>
                <key>RunAtLoad</key>
                <true/>
              </dict>
            </plist>
            """;
        await fileSystem.WriteAllTextAsync(path, content, cancellationToken);
    }

    private async Task SetLinuxStartupAsync(bool enabled, CancellationToken cancellationToken)
    {
        var path = GetLinuxAutostartPath();
        if (!enabled)
        {
            if (fileSystem.FileExists(path))
            {
                fileSystem.DeleteFile(path);
            }

            return;
        }

        fileSystem.CreateDirectory(Path.GetDirectoryName(path)!);
        var quotedExecutable = QuoteDesktopExec(executablePath);
        var content = $"""
            [Desktop Entry]
            Type=Application
            Version=1.0
            Name=ChronoDesk
            Comment=Start ChronoDesk with the desktop session
            Exec={quotedExecutable} --background
            Terminal=false
            X-GNOME-Autostart-enabled=true
            """;
        await fileSystem.WriteAllTextAsync(path, content, cancellationToken);
    }

    private string GetMacLaunchAgentPath() =>
        Path.Combine(userProfilePath, "Library", "LaunchAgents", MacLaunchAgentName);

    private string GetLinuxAutostartPath()
    {
        var configHome = string.IsNullOrWhiteSpace(xdgConfigHome)
            ? Path.Combine(userProfilePath, ".config")
            : xdgConfigHome;
        return Path.Combine(configHome, "autostart", "chronodesk.desktop");
    }

    private static string QuoteDesktopExec(string value) =>
        $"\"{value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal)}\"";
}
