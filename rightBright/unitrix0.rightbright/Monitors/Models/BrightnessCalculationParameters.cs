using Prism.Mvvm;

namespace unitrix0.rightbright.Monitors.Models
{
    public class BrightnessCalculationParameters : BindableBase
    {
        private int _minBrightness;
        private int _curve = 80;
        private double _progression = 2.105;
        private bool _active;

        public bool Active
        {
            get => _active;
            set => SetProperty(ref _active, value);
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

        public BrightnessCalculationParameters()
        {
        }

        public BrightnessCalculationParameters(BrightnessCalculationParameters source)
        {
            MapFrom(source);
        }

        public void MapFrom(BrightnessCalculationParameters source)
        {
            MinBrightness = source.MinBrightness;
            Curve = source.Curve;
            Progression = source.Progression;
            Active = source.Active;
        }


    }
}