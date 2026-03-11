namespace rightBright.Brightness.Calculators
{
    public interface IBrightnessCalculator
    {
        double Calculate(double lux, double progression, int curve, int lowestBrightness);
    }
}
