using System;
using System.Collections.Generic;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using rightBright.Models.Sensors;
using rightBright.Services.Sensors;

namespace rightBright.ViewModels;

public partial class LuxHistoryViewModel : MainWindowContentViewModel
{
    private static readonly TimeSpan HistoryWindow = TimeSpan.FromHours(8);

    private readonly ISensorService _sensorService;
    private readonly DateTime _appStart;

    [ObservableProperty] private IReadOnlyList<LuxReading> _readings = [];
    [ObservableProperty] private double _currentLux = -1;

    public DateTime HistoryRangeStart => _appStart;

    public DateTime HistoryRangeEnd => _appStart + HistoryWindow;

    public LuxHistoryViewModel(ISensorService sensorService)
    {
        _sensorService = sensorService;
        _appStart = DateTime.Now;
        _sensorService.Update += OnSensorUpdate;
        RefreshReadings();
    }

    private void OnSensorUpdate(object? sender, double lux)
    {
        Dispatcher.UIThread.Post(() =>
        {
            CurrentLux = lux;
            RefreshReadings();
        });
    }

    private void RefreshReadings()
    {
        var windowEnd = _appStart + HistoryWindow;
        var snapshot = _sensorService.GetValueHistorySnapshot();
        Readings = Array.FindAll(snapshot, r => r.Timestamp >= _appStart && r.Timestamp <= windowEnd);
    }

    public void Detach()
    {
        _sensorService.Update -= OnSensorUpdate;
    }
}
