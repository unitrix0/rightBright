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
    private readonly DispatcherTimer _historyWindowTimer;

    [ObservableProperty] private IReadOnlyList<LuxReading> _readings = [];
    [ObservableProperty] private double _currentLux = -1;
    [ObservableProperty] private DateTime _historyRangeStart;
    [ObservableProperty] private DateTime _historyRangeEnd;

    public LuxHistoryViewModel(ISensorService sensorService)
    {
        _sensorService = sensorService;
        _historyWindowTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
        _historyWindowTimer.Tick += OnHistoryWindowTick;

        _sensorService.Update += OnSensorUpdate;
        RefreshReadings();
        _historyWindowTimer.Start();
    }

    private void OnHistoryWindowTick(object? sender, EventArgs e) => RefreshReadings();

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
        var now = DateTime.Now;
        var start = now - HistoryWindow;
        var snapshot = _sensorService.GetValueHistorySnapshot();
        HistoryRangeStart = start;
        HistoryRangeEnd = now;
        Readings = Array.FindAll(snapshot, r => r.Timestamp >= start && r.Timestamp <= now);
    }

    public void Detach()
    {
        _historyWindowTimer.Stop();
        _historyWindowTimer.Tick -= OnHistoryWindowTick;
        _sensorService.Update -= OnSensorUpdate;
    }
}
