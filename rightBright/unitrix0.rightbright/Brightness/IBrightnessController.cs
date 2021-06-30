using unitrix0.rightbright.Sensors.Model;

namespace unitrix0.rightbright.Brightness
{
    public interface IBrightnessController
    {
        void Run();
        bool PauseSettingBrightness { get; set; }
        AmbientLightSensor? ConnectedSensor { get; }
        bool ConnectSensor(AmbientLightSensor sensor);
    }
}