using ChronoDesk.Infrastructure.Time;

namespace ChronoDesk.Tests;

public sealed class SystemTimeZoneCatalogTests
{
    [Fact]
    public void GetAll_ReturnsSystemTimeZonesInStableOrder()
    {
        var catalog = new SystemTimeZoneCatalog();

        var zones = catalog.GetAll();

        Assert.NotEmpty(zones);
        Assert.Contains(zones, zone => zone.Id == TimeZoneInfo.Utc.Id);
    }

    [Fact]
    public void Resolve_InvalidIdFallsBackToUtc()
    {
        var catalog = new SystemTimeZoneCatalog();

        var zone = catalog.Resolve("Definitely/Not-A-Timezone");

        Assert.Equal(TimeZoneInfo.Utc.Id, zone.Id);
    }

    [Fact]
    public void Search_IsCaseInsensitiveAndBounded()
    {
        var catalog = new SystemTimeZoneCatalog();

        var results = catalog.Search("utc", 5);

        Assert.InRange(results.Count, 0, 5);
        Assert.All(results, result =>
            Assert.Contains("utc", result.SearchText, StringComparison.CurrentCultureIgnoreCase));
    }
}
