using System.Threading.Tasks;
using rightBright.Models.Sensors;

namespace rightBright.Brightness
{
    public interface IBrightnessController
    {
        /// <summary>
        /// Runs the <see cref="BrightnessController"/> with settings loaded from the settings file
        /// </summary>
        Task Run();

        /// <summary>
        /// Trys to run with the passed sensor and starts polling. Does not load settings from the settings file.
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        bool Run(AmbientLightSensor sensor);

        bool PauseSettingBrightness { get; set; }
        AmbientLightSensor? ConnectedSensor { get; }
    }
}
