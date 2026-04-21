using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using rightBright.Models.Sensors;

namespace rightBright.Views.Controls;

public class LuxHistoryChartControl : Control
{
    private const double PaddingLeft = 55;
    private const double PaddingBottom = 30;
    private const double PaddingTop = 20;
    private const double PaddingRight = 15;

    private static readonly Color LineColor = Color.Parse("#5A60B4");
    private static readonly Color FillColor = Color.Parse("#3FBFC9FF");
    private static readonly Color GridLineColor = Color.Parse("#20808080");
    private static readonly Color AxisColor = Color.Parse("#60808080");
    private static readonly Color LabelColor = Color.Parse("#A0A0A0");
    private static readonly Color CurrentLuxColor = Color.Parse("#FFA726");

    private static readonly TimeSpan DefaultChartSpan = TimeSpan.FromHours(8);

    public static readonly StyledProperty<IReadOnlyList<LuxReading>?> ReadingsProperty =
        AvaloniaProperty.Register<LuxHistoryChartControl, IReadOnlyList<LuxReading>?>(nameof(Readings));

    public static readonly StyledProperty<double> CurrentLuxProperty =
        AvaloniaProperty.Register<LuxHistoryChartControl, double>(nameof(CurrentLux), defaultValue: -1);

    public static readonly StyledProperty<DateTime> TimeRangeStartProperty =
        AvaloniaProperty.Register<LuxHistoryChartControl, DateTime>(nameof(TimeRangeStart));

    public static readonly StyledProperty<DateTime> TimeRangeEndProperty =
        AvaloniaProperty.Register<LuxHistoryChartControl, DateTime>(nameof(TimeRangeEnd));

    public IReadOnlyList<LuxReading>? Readings
    {
        get => GetValue(ReadingsProperty);
        set => SetValue(ReadingsProperty, value);
    }

    public double CurrentLux
    {
        get => GetValue(CurrentLuxProperty);
        set => SetValue(CurrentLuxProperty, value);
    }

    public DateTime TimeRangeStart
    {
        get => GetValue(TimeRangeStartProperty);
        set => SetValue(TimeRangeStartProperty, value);
    }

    public DateTime TimeRangeEnd
    {
        get => GetValue(TimeRangeEndProperty);
        set => SetValue(TimeRangeEndProperty, value);
    }

    static LuxHistoryChartControl()
    {
        AffectsRender<LuxHistoryChartControl>(
            ReadingsProperty,
            CurrentLuxProperty,
            TimeRangeStartProperty,
            TimeRangeEndProperty);
    }

    public LuxHistoryChartControl()
    {
        ClipToBounds = true;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = Bounds;
        var chart = new Rect(PaddingLeft, PaddingTop,
            bounds.Width - PaddingLeft - PaddingRight,
            bounds.Height - PaddingTop - PaddingBottom);

        if (chart.Width <= 0 || chart.Height <= 0) return;

        var timeStart = TimeRangeStart;
        var timeEnd = TimeRangeEnd;
        if (timeStart == default || timeEnd == default)
        {
            timeStart = DateTime.Now;
            timeEnd = timeStart + DefaultChartSpan;
        }

        var readings = Readings;
        double luxMax = ComputeLuxMax(readings);

        DrawGridAndAxes(context, chart, timeStart, timeEnd, luxMax);

        if (readings is { Count: > 0 })
            DrawLuxLine(context, chart, readings, timeStart, timeEnd, luxMax);

        DrawCurrentLuxBadge(context, chart);
    }

    private static double ComputeLuxMax(IReadOnlyList<LuxReading>? readings)
    {
        double max = 100;
        if (readings != null)
        {
            foreach (var r in readings)
            {
                if (r.Lux > max) max = r.Lux;
            }
        }

        max = NiceRoundMax(max);
        return max;
    }

    private static double NiceRoundMax(double value)
    {
        if (value <= 100) return 100;
        if (value <= 200) return 200;
        if (value <= 500) return 500;
        if (value <= 1000) return 1000;
        if (value <= 2000) return 2000;

        double magnitude = Math.Pow(10, Math.Floor(Math.Log10(value)));
        return Math.Ceiling(value / magnitude) * magnitude;
    }

    #region Drawing

    private void DrawGridAndAxes(DrawingContext context, Rect chart,
        DateTime timeStart, DateTime timeEnd, double luxMax)
    {
        var axisPen = new Pen(new SolidColorBrush(AxisColor), 1);
        var gridPen = new Pen(new SolidColorBrush(GridLineColor), 1);
        var labelBrush = new SolidColorBrush(LabelColor);
        var typeface = new Typeface("Inter", FontStyle.Normal, FontWeight.Normal);

        context.DrawLine(axisPen, new Point(chart.Left, chart.Top), new Point(chart.Left, chart.Bottom));
        context.DrawLine(axisPen, new Point(chart.Left, chart.Bottom), new Point(chart.Right, chart.Bottom));

        // Y-axis grid lines and labels
        int yStep = ChooseYStep(luxMax);
        for (int lux = 0; lux <= (int)luxMax; lux += yStep)
        {
            double py = LuxToPixelY(chart, lux, luxMax);
            if (lux > 0)
                context.DrawLine(gridPen, new Point(chart.Left, py), new Point(chart.Right, py));

            var text = new FormattedText(lux.ToString(), CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight, typeface, 11, labelBrush);
            context.DrawText(text, new Point(chart.Left - text.Width - 6, py - text.Height / 2));
        }

        // X-axis: time labels at whole-hour boundaries
        var firstHour = new DateTime(timeStart.Year, timeStart.Month, timeStart.Day,
            timeStart.Hour, 0, 0).AddHours(1);
        double totalSeconds = (timeEnd - timeStart).TotalSeconds;

        for (var t = firstHour; t < timeEnd; t = t.AddHours(1))
        {
            double frac = (t - timeStart).TotalSeconds / totalSeconds;
            double px = chart.Left + frac * chart.Width;

            context.DrawLine(gridPen, new Point(px, chart.Top), new Point(px, chart.Bottom));

            var label = new FormattedText(t.ToString("HH:mm"), CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight, typeface, 11, labelBrush);
            context.DrawText(label, new Point(px - label.Width / 2, chart.Bottom + 4));
        }

        var yTitle = new FormattedText("Lux", CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight, typeface, 10, labelBrush);
        context.DrawText(yTitle, new Point(chart.Left + 4, chart.Top - 16));
    }

    private void DrawLuxLine(DrawingContext context, Rect chart,
        IReadOnlyList<LuxReading> readings, DateTime timeStart, DateTime timeEnd, double luxMax)
    {
        double totalSeconds = (timeEnd - timeStart).TotalSeconds;
        if (totalSeconds <= 0) return;

        var points = new List<Point>(readings.Count);
        foreach (var r in readings)
        {
            if (r.Timestamp < timeStart || r.Timestamp > timeEnd) continue;
            double frac = (r.Timestamp - timeStart).TotalSeconds / totalSeconds;
            double px = chart.Left + frac * chart.Width;
            double py = LuxToPixelY(chart, r.Lux, luxMax);
            points.Add(new Point(px, py));
        }

        if (points.Count == 0) return;

        // Area fill
        var fillGeometry = new StreamGeometry();
        using (var ctx = fillGeometry.Open())
        {
            ctx.BeginFigure(new Point(points[0].X, chart.Bottom), true);
            foreach (var p in points) ctx.LineTo(p);
            ctx.LineTo(new Point(points[^1].X, chart.Bottom));
            ctx.EndFigure(true);
        }

        context.DrawGeometry(new SolidColorBrush(FillColor), null, fillGeometry);

        // Line stroke
        var lineGeometry = new StreamGeometry();
        using (var ctx = lineGeometry.Open())
        {
            ctx.BeginFigure(points[0], false);
            for (int i = 1; i < points.Count; i++) ctx.LineTo(points[i]);
            ctx.EndFigure(false);
        }

        context.DrawGeometry(null, new Pen(new SolidColorBrush(LineColor), 2), lineGeometry);
    }

    private void DrawCurrentLuxBadge(DrawingContext context, Rect chart)
    {
        double lux = CurrentLux;
        if (lux < 0) return;

        var typeface = new Typeface("Inter", FontStyle.Normal, FontWeight.SemiBold);
        var brush = new SolidColorBrush(CurrentLuxColor);
        var label = new FormattedText($"{lux:F0} lx", CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight, typeface, 14, brush);

        context.DrawText(label, new Point(chart.Right - label.Width, chart.Top - 18));
    }

    #endregion

    #region Coordinate Mapping

    private static double LuxToPixelY(Rect chart, double lux, double luxMax)
    {
        if (luxMax <= 0) return chart.Bottom;
        return chart.Bottom - (lux / luxMax) * chart.Height;
    }

    private static int ChooseYStep(double luxMax)
    {
        if (luxMax <= 100) return 20;
        if (luxMax <= 200) return 50;
        if (luxMax <= 500) return 100;
        if (luxMax <= 1000) return 200;
        if (luxMax <= 2000) return 500;
        return 1000;
    }

    #endregion
}
