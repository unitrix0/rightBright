using System.Threading.Tasks;

namespace rightBright.Services.Brightness
{
    public interface ISetBrightnessService
    {
        Task SetBrightness(DisplayInfo monitor, int newValue);
    }
}
