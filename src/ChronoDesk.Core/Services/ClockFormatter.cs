using System.Globalization;
using ChronoDesk.Core.Models;

namespace ChronoDesk.Core.Services;

public sealed class ClockFormatter
{
    private readonly Func<ClockDisplayLabels>? labelsProvider;

    public ClockFormatter(Func<ClockDisplayLabels>? labelsProvider = null)
    {
        this.labelsProvider = labelsProvider;
    }

    public ClockSnapshot CreateSnapshot(
        DateTimeOffset instant,
        TimeZoneInfo timeZone,
        AppSettings settings,
        CultureInfo? culture = null,
        ClockDisplayLabels? labels = null)
    {
        ArgumentNullException.ThrowIfNull(timeZone);
        ArgumentNullException.ThrowIfNull(settings);

        culture ??= CultureInfo.CurrentCulture;
        labels ??= labelsProvider?.Invoke() ?? ClockDisplayLabels.English;
        var normalized = settings.Normalize();
        var local = TimeZoneInfo.ConvertTime(instant, timeZone);
        var timeFormat = BuildTimeFormat(normalized.ClockFormat, normalized.ShowSeconds);
        var week = ISOWeek.GetWeekOfYear(local.DateTime);
        var dateText = normalized.ShowDate
            ? local.ToString(culture.DateTimeFormat.ShortDatePattern, culture)
            : string.Empty;
        var weekdayText = normalized.ShowWeekday
            ? local.ToString("dddd", culture)
            : string.Empty;
        var weekText = normalized.ShowWeekNumber
            ? string.Create(culture, $"{labels.WeekPrefix} {week:00}")
            : string.Empty;
        var calendarDetails = normalized.ShowCalendarDetails
            ? BuildCalendarDetails(local, week, culture, labels)
            : string.Empty;
        var zoneName = timeZone.IsDaylightSavingTime(local)
            ? timeZone.DaylightName
            : timeZone.StandardName;

        return new ClockSnapshot(
            local.ToString(timeFormat, culture),
            dateText,
            weekdayText,
            weekText,
            calendarDetails,
            local,
            zoneName);
    }

    private static string BuildTimeFormat(ClockFormat format, bool showSeconds) =>
        (format, showSeconds) switch
        {
            (ClockFormat.TwelveHour, true) => "hh:mm:ss tt",
            (ClockFormat.TwelveHour, false) => "hh:mm tt",
            (ClockFormat.TwentyFourHour, true) => "HH:mm:ss",
            _ => "HH:mm",
        };

    private static string BuildCalendarDetails(
        DateTimeOffset local,
        int week,
        CultureInfo culture,
        ClockDisplayLabels labels)
    {
        var sign = local.Offset < TimeSpan.Zero ? '-' : '+';
        var absolute = local.Offset.Duration();
        var offset = string.Create(
            culture,
            $"{labels.UtcPrefix}{sign}{absolute.Hours:00}:{absolute.Minutes:00}");
        var dayOfYear = local.DayOfYear.ToString(culture);

        return string.Create(
            culture,
            $"{labels.DayPrefix} {dayOfYear} · {labels.IsoWeekPrefix} {week:00} · {offset}");
    }
}
