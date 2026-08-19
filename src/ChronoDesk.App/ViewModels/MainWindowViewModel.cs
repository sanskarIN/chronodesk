using System.Collections.ObjectModel;
using ChronoDesk.App.Localization;
using ChronoDesk.Core.Models;

namespace ChronoDesk.App.ViewModels;

public sealed class MainWindowViewModel : ObservableObject
{
    private readonly AppServices services;
    private readonly TimeProvider timeProvider;
    private AppSettings settings = new();
    private DateTimeOffset? lastChimeInstant;
    private string currentTime = string.Empty;
    private string currentDate = string.Empty;
    private string currentWeekday = string.Empty;
    private string currentWeekNumber = string.Empty;
    private string calendarDetails = string.Empty;
    private string zoneName = string.Empty;
    private string statusMessage = Strings.Ready;
    private bool isInitialized;

    public MainWindowViewModel(AppServices services, TimeProvider? timeProvider = null)
    {
        this.services = services ?? throw new ArgumentNullException(nameof(services));
        this.timeProvider = timeProvider ?? TimeProvider.System;
        WorldClocks.CollectionChanged += (_, _) => OnPropertyChanged(nameof(WorldClockCountText));
    }

    public event EventHandler<AppSettings>? SettingsChanged;

    public AppSettings Settings
    {
        get => settings;
        private set
        {
            if (SetProperty(ref settings, value))
            {
                OnPropertyChanged(nameof(IsTwelveHour));
                OnPropertyChanged(nameof(IsTwentyFourHour));
            }
        }
    }

    public ObservableCollection<WorldClockCardViewModel> WorldClocks { get; } = [];

    public ObservableCollection<TimeZoneDescriptor> SearchResults { get; } = [];

    public string WorldClockCountText => $"{WorldClocks.Count} {Strings.WorldClocksTitle.ToLowerInvariant()}";

    public string CurrentTime
    {
        get => currentTime;
        private set => SetProperty(ref currentTime, value);
    }

    public string CurrentDate
    {
        get => currentDate;
        private set => SetProperty(ref currentDate, value);
    }

    public string CurrentWeekday
    {
        get => currentWeekday;
        private set => SetProperty(ref currentWeekday, value);
    }

    public string CurrentWeekNumber
    {
        get => currentWeekNumber;
        private set => SetProperty(ref currentWeekNumber, value);
    }

    public string CalendarDetails
    {
        get => calendarDetails;
        private set => SetProperty(ref calendarDetails, value);
    }

    public string ZoneName
    {
        get => zoneName;
        private set => SetProperty(ref zoneName, value);
    }

    public string StatusMessage
    {
        get => statusMessage;
        private set => SetProperty(ref statusMessage, value);
    }

    public bool IsInitialized
    {
        get => isInitialized;
        private set => SetProperty(ref isInitialized, value);
    }

    public bool IsTwelveHour => Settings.ClockFormat == ClockFormat.TwelveHour;

    public bool IsTwentyFourHour => Settings.ClockFormat == ClockFormat.TwentyFourHour;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Settings = await services.SettingsStore.LoadAsync(cancellationToken);
            RebuildWorldClocks();
            SearchTimeZones(string.Empty);
            await TickAsync(cancellationToken);
            StatusMessage = Strings.AppReady;
            IsInitialized = true;
            SettingsChanged?.Invoke(this, Settings);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            services.Logger.Error("app.initialize_failed", exception, "ChronoDesk could not initialize all local data.");
            StatusMessage = Strings.LocalDataLoadWarning;
            IsInitialized = true;
        }
    }

    public async Task TickAsync(CancellationToken cancellationToken = default)
    {
        var instant = timeProvider.GetUtcNow();
        var localTimeZone = TimeZoneInfo.Local;
        var snapshot = services.ClockFormatter.CreateSnapshot(instant, localTimeZone, Settings);

        CurrentTime = snapshot.TimeText;
        CurrentDate = snapshot.DateText;
        CurrentWeekday = snapshot.WeekdayText;
        CurrentWeekNumber = snapshot.WeekNumberText;
        CalendarDetails = snapshot.CalendarDetailsText;
        ZoneName = snapshot.TimeZoneDisplayName;

        foreach (var card in WorldClocks)
        {
            card.Update(instant, Settings);
        }

        if (services.ChimePolicy.ShouldChime(instant, localTimeZone, Settings.Chime, lastChimeInstant))
        {
            lastChimeInstant = instant;
            try
            {
                await services.ChimePlayer.PlayAsync(cancellationToken);
                StatusMessage = Strings.ChimeStatus;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                services.Logger.Error("chime.play_failed", exception, "The configured chime could not be played.");
                StatusMessage = Strings.ChimeUnavailable;
            }
        }
    }

    public void SearchTimeZones(string? query)
    {
        var results = services.TimeZones.Search(query ?? string.Empty, 60);
        SearchResults.Clear();
        foreach (var result in results)
        {
            SearchResults.Add(result);
        }
    }

    public async Task AddWorldClockAsync(
        TimeZoneDescriptor descriptor,
        string? displayName = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        if (Settings.WorldClocks.Any(clock =>
            string.Equals(clock.TimeZoneId, descriptor.Id, StringComparison.OrdinalIgnoreCase)))
        {
            StatusMessage = Strings.TimezoneAlreadyAdded;
            return;
        }

        var label = string.IsNullOrWhiteSpace(displayName)
            ? descriptor.DisplayName
            : displayName.Trim();
        var clocks = Settings.WorldClocks.ToList();
        clocks.Add(WorldClock.Create(label, descriptor.Id));

        await UpdateSettingsAsync(Settings with { WorldClocks = clocks }, cancellationToken);
        StatusMessage = $"{Strings.AddedPrefix} {label}";
    }

    public async Task RemoveWorldClockAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        if (Settings.WorldClocks.Count <= 1)
        {
            StatusMessage = Strings.KeepOneWorldClock;
            return;
        }

        var clocks = Settings.WorldClocks.Where(clock => clock.Id != id).ToList();
        if (clocks.Count == Settings.WorldClocks.Count)
        {
            return;
        }

        await UpdateSettingsAsync(Settings with { WorldClocks = clocks }, cancellationToken);
        StatusMessage = Strings.WorldClockRemoved;
    }

    public Task ToggleClockFormatAsync(CancellationToken cancellationToken = default)
    {
        var format = Settings.ClockFormat == ClockFormat.TwentyFourHour
            ? ClockFormat.TwelveHour
            : ClockFormat.TwentyFourHour;
        return UpdateSettingsAsync(Settings with { ClockFormat = format }, cancellationToken);
    }

    public Task ToggleSecondsAsync(CancellationToken cancellationToken = default) =>
        UpdateSettingsAsync(Settings with { ShowSeconds = !Settings.ShowSeconds }, cancellationToken);

    public async Task CompleteOnboardingAsync(CancellationToken cancellationToken = default)
    {
        if (!Settings.IsFirstRun)
        {
            return;
        }

        await UpdateSettingsAsync(Settings with { IsFirstRun = false }, cancellationToken);
    }

    public async Task UpdateSettingsAsync(
        AppSettings newSettings,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(newSettings);
        var normalized = newSettings.Normalize();
        var startupChanged = normalized.StartWithSystem != Settings.StartWithSystem;

        if (startupChanged && services.StartupManager.IsSupported)
        {
            await services.StartupManager.SetEnabledAsync(normalized.StartWithSystem, cancellationToken);
        }

        await services.SettingsStore.SaveAsync(normalized, cancellationToken);
        Settings = normalized;
        RebuildWorldClocks();
        await TickAsync(cancellationToken);
        SettingsChanged?.Invoke(this, Settings);
    }

    public async Task ExportSettingsAsync(
        string destinationPath,
        CancellationToken cancellationToken = default)
    {
        await services.SettingsStore.ExportAsync(Settings, destinationPath, cancellationToken);
        StatusMessage = Strings.SettingsExported;
    }

    public async Task ImportSettingsAsync(
        string sourcePath,
        CancellationToken cancellationToken = default)
    {
        var imported = await services.SettingsStore.ImportAsync(sourcePath, cancellationToken);
        await UpdateSettingsAsync(imported with { IsFirstRun = false }, cancellationToken);
        StatusMessage = Strings.SettingsImported;
    }

    public async Task ResetSettingsAsync(CancellationToken cancellationToken = default)
    {
        var defaults = new AppSettings { IsFirstRun = false };
        await UpdateSettingsAsync(defaults, cancellationToken);
        StatusMessage = Strings.SettingsReset;
    }

    private void RebuildWorldClocks()
    {
        WorldClocks.Clear();
        foreach (var worldClock in Settings.WorldClocks)
        {
            var zone = services.TimeZones.Resolve(worldClock.TimeZoneId);
            WorldClocks.Add(new WorldClockCardViewModel(worldClock, zone, services.ClockFormatter));
        }
    }
}
