using CommunityToolkit.Mvvm.ComponentModel;
using rightBright.Models.Monitors;
using rightBright.WindowsApi.Monitor.Structs;

public class DisplayInfo : ObservableObject
{
    private int _lastBrightnessSet;
    public bool IsPrimaryMonitor { get; set; }
    public int ScreenHeight { get; set; }
    public int ScreenWidth { get; set; }

    public string Resolution => $"{ScreenWidth}x{ScreenHeight}";
    public RectStruct MonitorArea { get; set; }
    public RectStruct WorkArea { get; set; }
    public string DeviceName { get; set; }
    public string ModelName { get; set; }

    public BrightnessCalculationParameters CalculationParameters { get; set; }

    public int LastBrightnessSet
    {
        get => _lastBrightnessSet;
        set => SetProperty(ref _lastBrightnessSet, value);
    }

    public DisplayInfo()
    {
        CalculationParameters = new BrightnessCalculationParameters();
        _lastBrightnessSet = -1;
    }

    public override string ToString()
    {
        return $"{DeviceName} {Resolution}";
    }
}
