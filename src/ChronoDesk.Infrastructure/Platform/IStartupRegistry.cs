namespace ChronoDesk.Infrastructure.Platform;

internal interface IStartupRegistry
{
    string? GetCurrentUserString(string subKeyPath, string valueName);

    void SetCurrentUserString(string subKeyPath, string valueName, string value);

    void DeleteCurrentUserValue(string subKeyPath, string valueName);
}
