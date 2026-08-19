using System.Runtime.Versioning;
using ChronoDesk.Core.Abstractions;
using Microsoft.Win32;

namespace ChronoDesk.Infrastructure.Platform;

public sealed class PlatformStartupManager : IStartupManager
{
    private const string WindowsRunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "ChronoDesk";
    private const int MaximumRegistrationBytes = 16 * 1024;
    private readonly string executablePath;

    public PlatformStartupManager(string? executablePath = null)
    {
        this.executablePath = StartupRegistrationDocuments.NormalizeExecutablePath(
            executablePath
                ?? Environment.ProcessPath
                ?? throw new InvalidOperationException("ChronoDesk executable path could not be determined."));
    }

    public bool IsSupported =>
        OperatingSystem.IsWindows() || OperatingSystem.IsMacOS() || OperatingSystem.IsLinux();

    public async Task<bool> IsEnabledAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (OperatingSystem.IsWindows())
        {
            return IsWindowsStartupEnabled();
        }

        if (OperatingSystem.IsMacOS())
        {
            return await HasExpectedFileContentAsync(
                StartupPathResolver.GetMacLaunchAgentPath(GetUserProfile()),
                StartupRegistrationDocuments.BuildMacLaunchAgent(executablePath),
                cancellationToken);
        }

        if (OperatingSystem.IsLinux())
        {
            return await HasExpectedFileContentAsync(
                StartupPathResolver.GetLinuxAutostartPath(
                    GetUserProfile(),
                    Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")),
                StartupRegistrationDocuments.BuildLinuxDesktopEntry(executablePath),
                cancellationToken);
        }

        return false;
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
            && string.Equals(
                value,
                StartupRegistrationDocuments.BuildWindowsCommand(executablePath),
                StringComparison.OrdinalIgnoreCase);
    }

    [SupportedOSPlatform("windows")]
    private void SetWindowsStartup(bool enabled)
    {
        using var key = Registry.CurrentUser.CreateSubKey(WindowsRunKey, writable: true)
            ?? throw new InvalidOperationException("Windows startup registry key could not be opened.");

        if (enabled)
        {
            key.SetValue(
                AppName,
                StartupRegistrationDocuments.BuildWindowsCommand(executablePath),
                RegistryValueKind.String);
        }
        else
        {
            key.DeleteValue(AppName, throwOnMissingValue: false);
        }
    }

    private async Task SetMacStartupAsync(bool enabled, CancellationToken cancellationToken)
    {
        var path = StartupPathResolver.GetMacLaunchAgentPath(GetUserProfile());
        if (!enabled)
        {
            DeleteIfExists(path);
            return;
        }

        await WriteTextAtomicallyAsync(
            path,
            StartupRegistrationDocuments.BuildMacLaunchAgent(executablePath),
            cancellationToken);
    }

    private async Task SetLinuxStartupAsync(bool enabled, CancellationToken cancellationToken)
    {
        var path = StartupPathResolver.GetLinuxAutostartPath(
            GetUserProfile(),
            Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"));
        if (!enabled)
        {
            DeleteIfExists(path);
            return;
        }

        await WriteTextAtomicallyAsync(
            path,
            StartupRegistrationDocuments.BuildLinuxDesktopEntry(executablePath),
            cancellationToken);
    }

    private static string GetUserProfile()
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (string.IsNullOrWhiteSpace(userProfile))
        {
            throw new InvalidOperationException("The current user profile directory could not be resolved.");
        }

        return userProfile;
    }

    private static async Task<bool> HasExpectedFileContentAsync(
        string path,
        string expectedContent,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(path))
        {
            return false;
        }

        await using var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4 * 1024,
            options: FileOptions.Asynchronous | FileOptions.SequentialScan);
        if (stream.Length > MaximumRegistrationBytes)
        {
            return false;
        }

        using var reader = new StreamReader(stream);
        var actualContent = await reader.ReadToEndAsync(cancellationToken);
        return string.Equals(
            NormalizeDocument(actualContent),
            NormalizeDocument(expectedContent),
            StringComparison.Ordinal);
    }

    private static string NormalizeDocument(string value) =>
        value.Replace("\r\n", "\n", StringComparison.Ordinal).Trim();

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private static async Task WriteTextAtomicallyAsync(
        string path,
        string content,
        CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(path)
            ?? throw new InvalidOperationException("Startup registration directory could not be resolved.");
        Directory.CreateDirectory(directory);

        var temporaryPath = path + $".{Guid.NewGuid():N}.tmp";
        try
        {
            await File.WriteAllTextAsync(temporaryPath, content, cancellationToken);
            File.Move(temporaryPath, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }
}
