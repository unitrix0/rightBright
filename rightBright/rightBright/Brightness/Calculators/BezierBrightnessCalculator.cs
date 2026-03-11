using System;
using rightBright.Models.Monitors;

namespace rightBright.Brightness.Calculators
{
    public class BezierBrightnessCalculator : IBrightnessCalculator
    {
        public double Calculate(double lux, BrightnessCalculationParameters p)
        {
            double minBrightness = p.MinBrightness;
            double cx = p.ControlPointX;
            double cy = p.ControlPointY;
            double maxLux = p.MaxLux;

            if (lux <= 0) return minBrightness;
            if (lux >= maxLux) return 100;

            double t = SolveForT(lux, cx, maxLux);
            double oneMinusT = 1 - t;
            double brightness = oneMinusT * oneMinusT * minBrightness
                              + 2 * oneMinusT * t * cy
                              + t * t * 100;

            return Math.Round(Math.Clamp(brightness, 0, 100), 1);
        }

        /// <summary>
        /// Solves (maxLux - 2*cx)*t^2 + 2*cx*t - lux = 0 for t in [0,1].
        /// </summary>
        private static double SolveForT(double lux, double cx, double maxLux)
        {
            double a = maxLux - 2 * cx;
            double b = 2 * cx;

            if (Math.Abs(a) < 1e-9)
                return Math.Clamp(lux / b, 0, 1);

            double discriminant = b * b + 4 * a * lux;
            if (discriminant < 0) return 0;

            double sqrtD = Math.Sqrt(discriminant);
            double t1 = (-b + sqrtD) / (2 * a);
            double t2 = (-b - sqrtD) / (2 * a);

            if (t1 >= 0 && t1 <= 1) return t1;
            if (t2 >= 0 && t2 <= 1) return t2;

            return Math.Clamp(t1, 0, 1);
        }
    }
}
