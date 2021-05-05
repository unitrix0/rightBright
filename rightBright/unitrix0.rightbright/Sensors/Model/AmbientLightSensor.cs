using System;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace unitrix0.rightbright.Sensors.Model
{
    public class AmbientLightSensor : BindableBase
    {
        private int _currentValue;
        private int _maxValue;
        private int _minValue;
        public string FriendlyName { get; set; }
        public string SerialNumber { get; set; }
        [JsonIgnore]
        public bool IsOnline { get; set; }
        [JsonIgnore]
        public bool IsReady { get; set; }
        [JsonIgnore]
        public int CurrentValue
        {
            get => _currentValue;
            set
            {
                SetProperty(ref _currentValue, value);
                if (value > MaxValue) SetProperty(ref _maxValue, value, nameof(MaxValue));
                if (value < MinValue) SetProperty(ref _minValue, value, nameof(MinValue));
            }
        }

        public int MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        public int MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }
    }
}