using System.Runtime.Versioning;
using Microsoft.Win32;

namespace ChronoDesk.Infrastructure.Platform;

internal sealed class SystemStartupRegistry : IStartupRegistry
{
    [SupportedOSPlatform("windows")]
    public string? GetCurrentUserString(string subKeyPath, string valueName)
    {
        using var key = Registry.CurrentUser.OpenSubKey(subKeyPath, writable: false);
        return key?.GetValue(valueName) as string;
    }

    [SupportedOSPlatform("windows")]
    public void SetCurrentUserString(string subKeyPath, string valueName, string value)
    {
        using var key = Registry.CurrentUser.CreateSubKey(subKeyPath, writable: true)
            ?? throw new InvalidOperationException("Windows startup registry key could not be opened.");
        key.SetValue(valueName, value, RegistryValueKind.String);
    }

    [SupportedOSPlatform("windows")]
    public void DeleteCurrentUserValue(string subKeyPath, string valueName)
    {
        using var key = Registry.CurrentUser.CreateSubKey(subKeyPath, writable: true)
            ?? throw new InvalidOperationException("Windows startup registry key could not be opened.");
        key.DeleteValue(valueName, throwOnMissingValue: false);
    }
}
