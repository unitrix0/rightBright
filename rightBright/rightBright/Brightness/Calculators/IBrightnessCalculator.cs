using rightBright.Models.Monitors;

namespace rightBright.Brightness.Calculators
{
    public interface IBrightnessCalculator
    {
        double Calculate(double lux, BrightnessCalculationParameters parameters);
    }
}
