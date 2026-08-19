using ChronoDesk.App;

namespace ChronoDesk.Tests;

public sealed class AppVersionProviderTests
{
    [Theory]
    [InlineData("0.1.0-preview", "0.1.0-preview")]
    [InlineData("1.0.0", "1.0.0")]
    [InlineData("1.2.3+abcdef123", "1.2.3")]
    [InlineData(" 2.0.0-rc.1+build.42 ", "2.0.0-rc.1")]
    public void NormalizeDisplayVersion_UsesSemVerWithoutBuildMetadata(
        string informationalVersion,
        string expected)
    {
        var actual = AppVersionProvider.NormalizeDisplayVersion(
            informationalVersion,
            new Version(9, 9, 9, 9));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NormalizeDisplayVersion_FallsBackToThreePartAssemblyVersion()
    {
        var actual = AppVersionProvider.NormalizeDisplayVersion(
            null,
            new Version(3, 4, 5, 6));

        Assert.Equal("3.4.5", actual);
    }

    [Fact]
    public void NormalizeDisplayVersion_UsesDevelopmentWhenMetadataUnavailable()
    {
        var actual = AppVersionProvider.NormalizeDisplayVersion("   ", null);

        Assert.Equal("development", actual);
    }
}
