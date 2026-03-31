using System.Threading.Tasks;

namespace rightBright.Services.Brightness
{
    public interface ISetBrightnessService
    {
        Task<bool> SetBrightness(DisplayInfo monitor, int newValue);
    }
}
