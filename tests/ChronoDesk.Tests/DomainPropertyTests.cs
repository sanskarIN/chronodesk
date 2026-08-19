using ChronoDesk.Core.Models;

namespace ChronoDesk.Tests;

public sealed class DomainPropertyTests
{
    [Fact]
    public void QuietHours_MatchesReferencePredicateAcrossDeterministicRandomCases()
    {
        var random = new Random(0xC10C);

        for (var sample = 0; sample < 5_000; sample++)
        {
            var startMinute = random.Next(24 * 60);
            var endMinute = random.Next(24 * 60);
            var probeMinute = random.Next(24 * 60);
            var enabled = random.Next(2) == 1;

            var start = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(startMinute));
            var end = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(endMinute));
            var probe = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(probeMinute));
            var quietHours = new QuietHours
            {
                Enabled = enabled,
                Start = start,
                End = end,
            };

            var expected = ReferenceContains(enabled, startMinute, endMinute, probeMinute);

            Assert.Equal(expected, quietHours.Contains(probe));
        }
    }

    [Fact]
    public void AppSettings_NormalizationIsIdempotentAcrossRandomVisualInputs()
    {
        var random = new Random(0x5E771A65);

        for (var sample = 0; sample < 2_000; sample++)
        {
            var fontSize = NextFloatingPointCandidate(random);
            var spacing = NextFloatingPointCandidate(random);
            var clockCount = random.Next(0, 40);
            var clocks = Enumerable.Range(0, clockCount)
                .Select(index => new WorldClock(
                    index % 7 == 0 ? "duplicate" : $"clock-{index}",
                    $"Clock {index}",
                    "UTC"))
                .ToList();

            var input = new AppSettings
            {
                ClockFontSize = fontSize,
                ContentSpacing = spacing,
                FontFamilyName = sample % 5 == 0 ? "  " : "Inter",
                WorldClocks = clocks,
            };

            var first = input.Normalize();
            var second = first.Normalize();

            Assert.Equal(first.ClockFontSize, second.ClockFontSize);
            Assert.Equal(first.ContentSpacing, second.ContentSpacing);
            Assert.Equal(first.FontFamilyName, second.FontFamilyName);
            Assert.Equal(first.WorldClocks, second.WorldClocks);
            Assert.InRange(first.ClockFontSize, 42, 240);
            Assert.InRange(first.ContentSpacing, 4, 48);
            Assert.InRange(first.WorldClocks.Count, 1, 24);
            Assert.Equal(
                first.WorldClocks.Count,
                first.WorldClocks.Select(clock => clock.Id).Distinct(StringComparer.Ordinal).Count());
        }
    }

    private static bool ReferenceContains(
        bool enabled,
        int startMinute,
        int endMinute,
        int probeMinute)
    {
        if (!enabled || startMinute == endMinute)
        {
            return false;
        }

        if (startMinute < endMinute)
        {
            return probeMinute >= startMinute && probeMinute < endMinute;
        }

        return probeMinute >= startMinute || probeMinute < endMinute;
    }

    private static double NextFloatingPointCandidate(Random random) => random.Next(8) switch
    {
        0 => double.NaN,
        1 => double.PositiveInfinity,
        2 => double.NegativeInfinity,
        3 => -random.NextDouble() * 10_000,
        4 => random.NextDouble() * 10_000,
        _ => random.NextDouble() * 300,
    };
}
