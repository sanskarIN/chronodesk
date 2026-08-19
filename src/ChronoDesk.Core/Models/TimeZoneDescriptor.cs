namespace ChronoDesk.Core.Models;

public sealed record TimeZoneDescriptor(
    string Id,
    string DisplayName,
    TimeSpan BaseUtcOffset)
{
    public string SearchText => $"{Id} {DisplayName}";
}
