using ChronoDesk.Core.Models;
using ChronoDesk.Core.Services;

namespace ChronoDesk.App.ViewModels;

public sealed class WorldClockCardViewModel : ObservableObject
{
    private readonly ClockFormatter formatter;
    private readonly TimeZoneInfo timeZone;
    private string timeText = string.Empty;
    private string dateText = string.Empty;
    private string zoneText = string.Empty;

    public WorldClockCardViewModel(
        WorldClock model,
        TimeZoneInfo timeZone,
        ClockFormatter formatter)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
        this.timeZone = timeZone ?? throw new ArgumentNullException(nameof(timeZone));
        this.formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    public WorldClock Model { get; }

    public string Id => Model.Id;

    public string DisplayName => Model.DisplayName;

    public string TimeZoneId => Model.TimeZoneId;

    public string TimeText
    {
        get => timeText;
        private set => SetProperty(ref timeText, value);
    }

    public string DateText
    {
        get => dateText;
        private set => SetProperty(ref dateText, value);
    }

    public string ZoneText
    {
        get => zoneText;
        private set => SetProperty(ref zoneText, value);
    }

    public void Update(DateTimeOffset instant, AppSettings settings)
    {
        var snapshot = formatter.CreateSnapshot(instant, timeZone, settings);
        TimeText = snapshot.TimeText;
        DateText = snapshot.DateText;
        ZoneText = snapshot.TimeZoneDisplayName;
    }
}
