using ChronoDesk.Core.Models;

namespace ChronoDesk.Tests;

public sealed class QuietHoursTests
{
    [Theory]
    [InlineData(22, 0, true)]
    [InlineData(23, 59, true)]
    [InlineData(0, 0, true)]
    [InlineData(6, 59, true)]
    [InlineData(7, 0, false)]
    [InlineData(12, 0, false)]
    public void Contains_HandlesOvernightRanges(int hour, int minute, bool expected)
    {
        var quietHours = new QuietHours
        {
            Enabled = true,
            Start = new TimeOnly(22, 0),
            End = new TimeOnly(7, 0),
        };

        Assert.Equal(expected, quietHours.Contains(new TimeOnly(hour, minute)));
    }

    [Fact]
    public void Contains_ReturnsFalseWhenDisabled()
    {
        var quietHours = new QuietHours
        {
            Enabled = false,
            Start = new TimeOnly(9, 0),
            End = new TimeOnly(17, 0),
        };

        Assert.False(quietHours.Contains(new TimeOnly(12, 0)));
    }

    [Fact]
    public void Contains_TreatsEqualBoundsAsNoQuietPeriod()
    {
        var quietHours = new QuietHours
        {
            Enabled = true,
            Start = new TimeOnly(8, 0),
            End = new TimeOnly(8, 0),
        };

        Assert.False(quietHours.Contains(new TimeOnly(8, 0)));
    }
}
