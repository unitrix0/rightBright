using System;
using rightBright.Models.Monitors;
using rightBright.Views.Controls;

namespace rightBright.Brightness.Calculators
{
    public class BezierBrightnessCalculator : IBrightnessCalculator
    {
        public double Calculate(double lux, BrightnessCalculationParameters p)
        {
            double p1x = p.ControlPointX;
            double p1y = p.ControlPointY;
            double minBrightness = p.MinBrightness;
            double maxLux = p.MaxLux;

            if (lux <= 0) return minBrightness;
            if (lux >= maxLux) return 100;

            var (c1, c2) = BezierCurveEditorControl.ComputeSegmentControlPoints(
                0, minBrightness, p1x, p1y, maxLux, 100);

            double brightness;
            if (lux <= p1x)
            {
                double t = SolveSegment(lux, 0, c1.x, p1x);
                double u = 1 - t;
                brightness = u * u * minBrightness + 2 * u * t * c1.y + t * t * p1y;
            }
            else
            {
                double t = SolveSegment(lux, p1x, c2.x, maxLux);
                double u = 1 - t;
                brightness = u * u * p1y + 2 * u * t * c2.y + t * t * 100;
            }

            return Math.Round(Math.Clamp(brightness, 0, 100), 1);
        }

        /// <summary>
        /// Solves the quadratic Bezier X equation for t in [0,1] within one segment.
        /// X(t) = (1-t)^2*startX + 2(1-t)t*controlX + t^2*endX
        /// Rearranged: (startX - 2*controlX + endX)*t^2 + 2*(controlX - startX)*t + (startX - lux) = 0
        /// </summary>
        private static double SolveSegment(double lux, double startX, double controlX, double endX)
        {
            double a = startX - 2 * controlX + endX;
            double b = 2 * (controlX - startX);
            double c = startX - lux;

            if (Math.Abs(a) < 1e-9)
            {
                if (Math.Abs(b) < 1e-9) return 0;
                return Math.Clamp(-c / b, 0, 1);
            }

            double discriminant = b * b - 4 * a * c;
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
