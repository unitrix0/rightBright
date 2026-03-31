using CommunityToolkit.Mvvm.ComponentModel;

namespace rightBright.Models.Monitors
{
    public class BrightnessCalculationParameters : ObservableObject
    {
        private int _minBrightness;
        private double _controlPointX = 400;
        private double _controlPointY = 50;
        private int _maxLux = 800;
        private bool _active;

        public bool Active
        {
            get => _active;
            set => SetProperty(ref _active, value);
        }

        public double ControlPointX
        {
            get => _controlPointX;
            set => SetProperty(ref _controlPointX, value);
        }

        public double ControlPointY
        {
            get => _controlPointY;
            set => SetProperty(ref _controlPointY, value);
        }

        public int MaxLux
        {
            get => _maxLux;
            set => SetProperty(ref _maxLux, value);
        }

        public int MinBrightness
        {
            get => _minBrightness;
            set => SetProperty(ref _minBrightness, value);
        }

        public BrightnessCalculationParameters()
        {
        }

        public BrightnessCalculationParameters(BrightnessCalculationParameters source)
        {
            MapFrom(source);
        }

        private void MapFrom(BrightnessCalculationParameters source)
        {
            MinBrightness = source.MinBrightness;
            ControlPointX = source.ControlPointX;
            ControlPointY = source.ControlPointY;
            MaxLux = source.MaxLux;
            Active = source.Active;
        }
    }
}
