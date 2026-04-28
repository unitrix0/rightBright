using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using rightBright.Localization;
using rightBright.Services.Autostart;
using rightBright.Services.Sensors;
using rightBright.Settings;
using rightBright.Updates;
using Serilog;

namespace rightBright.ViewModels;

public partial class SettingsViewModel : MainWindowContentViewModel
{
    private readonly ISettings _settings;
    private readonly IAutostartService _autostartService;
    private readonly ISensorService _sensorService;
    private readonly IUpdateService _updateService;
    private readonly ApplicationViewModel? _applicationViewModel;
    private readonly ILogger _logger;

    public Action? CloseView;

    public ObservableCollection<LanguageOption> Languages { get; } =
    [
        new("English", "en"),
        new("Deutsch", "de")
    ];

    public ObservableCollection<YapiIntervalOption> YapiIntervalOptions { get; } =
    [
        new("3 s", 3000),
        new("5 s", 5000)
    ];

    [ObservableProperty] private LanguageOption _selectedLanguage = null!;

    [ObservableProperty] private YapiIntervalOption _selectedYapiInterval = null!;

    [ObservableProperty] private int _updateCheckIntervalHours;

    [ObservableProperty] private bool _autostartEnabled;

    public bool IsAutostartSupported => _autostartService.IsSupported;

    public bool IsUpdateCheckVisible => OperatingSystem.IsWindows();

    public SettingsViewModel()
    {
        _settings = null!;
        _autostartService = null!;
        _sensorService = null!;
        _updateService = null!;
        _logger = Log.Logger;
        SeedDesignTimeData();
    }

    public SettingsViewModel(ISettings settings,
        IAutostartService autostartService,
        ISensorService sensorService,
        IUpdateService updateService,
        ApplicationViewModel applicationViewModel,
        ILogger logger)
    {
        _settings = settings;
        _autostartService = autostartService;
        _sensorService = sensorService;
        _updateService = updateService;
        _applicationViewModel = applicationViewModel;
        _logger = logger;

        var currentLang = LocalizationService.ResolveLanguageCode(settings.UiLanguage);
        _selectedLanguage = FindLanguage(currentLang);

        _selectedYapiInterval = FindYapiInterval(settings.YapiEventsTimerInterval);

        _updateCheckIntervalHours = settings.UpdateCheckIntervalHours > 0
            ? settings.UpdateCheckIntervalHours
            : 6;

        _autostartEnabled = settings.AutostartEnabled;
    }

    partial void OnSelectedLanguageChanged(LanguageOption value)
    {
        if (_settings is null || value is null) return;

        var applied = LocalizationService.ApplyLanguage(value.Code);
        _settings.UiLanguage = applied;
        _settings.Save();
        _logger.Information("[Settings] Language changed to {Lang}", applied);
    }

    partial void OnSelectedYapiIntervalChanged(YapiIntervalOption value)
    {
        if (_settings is null || value is null) return;

        _settings.YapiEventsTimerInterval = value.Milliseconds;
        _settings.Save();
        _sensorService.SetPollInterval(value.Milliseconds);
        _logger.Information("[Settings] YapiEventsTimerInterval changed to {Ms}ms", value.Milliseconds);
    }

    partial void OnUpdateCheckIntervalHoursChanged(int value)
    {
        if (_settings is null) return;
        if (value <= 0) return;

        _settings.UpdateCheckIntervalHours = value;
        _settings.Save();
        _updateService.RestartPeriodicChecks();
        _logger.Information("[Settings] UpdateCheckIntervalHours changed to {Hours}h", value);
    }

    private bool _suppressAutostartSave;

    async partial void OnAutostartEnabledChanged(bool value)
    {
        if (_settings is null || _suppressAutostartSave) return;

        var granted = await _autostartService.SetAutostartAsync(value);
        var effective = granted && value;
        if (effective != value)
        {
            _suppressAutostartSave = true;
            AutostartEnabled = effective;
            _suppressAutostartSave = false;
        }

        _settings.AutostartEnabled = AutostartEnabled;
        _settings.Save();
        if (_applicationViewModel is not null)
            _applicationViewModel.AutostartEnabled = AutostartEnabled;
        _logger.Information("[Settings] AutostartEnabled set to {Enabled}", AutostartEnabled);
    }

    [RelayCommand]
    private void RequestClose()
    {
        CloseView?.Invoke();
    }

    private LanguageOption FindLanguage(string code)
    {
        foreach (var lang in Languages)
            if (lang.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
                return lang;

        return Languages[0];
    }

    private YapiIntervalOption FindYapiInterval(int ms)
    {
        foreach (var opt in YapiIntervalOptions)
            if (opt.Milliseconds == ms)
                return opt;

        return YapiIntervalOptions[1]; // default 5000
    }

    private void SeedDesignTimeData()
    {
        SelectedLanguage = Languages[0];
        SelectedYapiInterval = YapiIntervalOptions[1];
        UpdateCheckIntervalHours = 6;
        AutostartEnabled = false;
    }
}

public record LanguageOption(string DisplayName, string Code)
{
    public override string ToString() => DisplayName;
}

public record YapiIntervalOption(string DisplayName, int Milliseconds)
{
    public override string ToString() => DisplayName;
}
