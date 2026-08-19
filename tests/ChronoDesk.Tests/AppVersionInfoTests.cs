using System.Reflection;
using ChronoDesk.App.Services;

namespace ChronoDesk.Tests;

public sealed class AppVersionInfoTests
{
    [Fact]
    public void GetDisplayVersion_UsesInformationalVersionWithoutBuildMetadata()
    {
        var assembly = typeof(AppVersionInfo).Assembly;
        var informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        var actual = AppVersionInfo.GetDisplayVersion(assembly);

        Assert.False(string.IsNullOrWhiteSpace(actual));
        Assert.DoesNotContain('+', actual);
        if (!string.IsNullOrWhiteSpace(informationalVersion))
        {
            Assert.Equal(informationalVersion.Split('+')[0], actual);
        }
    }
}
