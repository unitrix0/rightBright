using System;
using System.Collections.Generic;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using rightBright.Models.Sensors;
using rightBright.Services.Sensors;

namespace rightBright.ViewModels;

public partial class LuxHistoryViewModel : MainWindowContentViewModel
{
    private static readonly TimeSpan HistoryWindow = TimeSpan.FromHours(12);

    private readonly ISensorService _sensorService;

    [ObservableProperty] private IReadOnlyList<LuxReading> _readings = [];
    [ObservableProperty] private double _currentLux = -1;

    public LuxHistoryViewModel(ISensorService sensorService)
    {
        _sensorService = sensorService;
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
        var cutoff = DateTime.Now - HistoryWindow;
        var snapshot = _sensorService.GetValueHistorySnapshot();
        Readings = Array.FindAll(snapshot, r => r.Timestamp >= cutoff);
    }

    public void Detach()
    {
        _sensorService.Update -= OnSensorUpdate;
    }
}
