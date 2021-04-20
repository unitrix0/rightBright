using Prism.Mvvm;

namespace unitrix0.rightbright.Monitors.Models
{
    public class BrightnessCalculationParameters :  BindableBase
    {
        private int _minBrightness;
        private int _curve = 80;
        private double _progression = 2.105;

        public BrightnessCalculationParameters()
        {
        }

        public BrightnessCalculationParameters(BrightnessCalculationParameters source)
        {
            _minBrightness = source.MinBrightness;
            _curve = source.Curve;
            _progression = source.Progression;
        }

        public int Curve
        {
            get => _curve;
            set => SetProperty(ref _curve, value);
        }

        public double Progression
        {
            get => _progression;
            set => SetProperty(ref _progression, value);
        }

        public int MinBrightness
        {
            get => _minBrightness;
            set => SetProperty(ref _minBrightness, value);
        }
    }
}