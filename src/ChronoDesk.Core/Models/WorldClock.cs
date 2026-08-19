namespace ChronoDesk.Core.Models;

public sealed record WorldClock(
    string Id,
    string DisplayName,
    string TimeZoneId)
{
    public static WorldClock Create(string displayName, string timeZoneId) =>
        new(Guid.NewGuid().ToString("N"), displayName.Trim(), timeZoneId.Trim());
}
