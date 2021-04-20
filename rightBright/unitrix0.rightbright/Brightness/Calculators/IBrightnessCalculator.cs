namespace unitrix0.rightbright.Brightness.Calculators
{
    public interface IBrightnessCalculator
    {
        double Calculate(double lux, double progression, int curve, int lowestBrightness);
    }
}