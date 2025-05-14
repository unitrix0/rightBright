namespace rightBright.Services.Brightness
{
    public interface ISetBrightnessService
    {
        void SetBrightness(DisplayInfo monitor, int newValue);
    }
}
