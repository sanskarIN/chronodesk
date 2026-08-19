using ChronoDesk.Infrastructure.Platform;

namespace ChronoDesk.Tests.Fakes;

internal sealed class FakeStartupRegistry : IStartupRegistry
{
    private readonly Dictionary<string, string> values = new(StringComparer.OrdinalIgnoreCase);

    public string? GetCurrentUserString(string subKeyPath, string valueName) =>
        values.GetValueOrDefault(GetKey(subKeyPath, valueName));

    public void SetCurrentUserString(string subKeyPath, string valueName, string value) =>
        values[GetKey(subKeyPath, valueName)] = value;

    public void DeleteCurrentUserValue(string subKeyPath, string valueName) =>
        values.Remove(GetKey(subKeyPath, valueName));

    public void SeedValue(string subKeyPath, string valueName, string value) =>
        values[GetKey(subKeyPath, valueName)] = value;

    private static string GetKey(string subKeyPath, string valueName) =>
        $"{subKeyPath}\0{valueName}";
}
