using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace rightBright.Views.Controls;

public class BezierCurveEditorControl : Control
{
    private const double Padding_Left = 50;
    private const double Padding_Bottom = 30;
    private const double Padding_Top = 15;
    private const double Padding_Right = 15;
    private const double ControlPointRadius = 7;
    private const double HitTestRadius = 14;

    private static readonly Color CurveColor = Color.Parse("#5A60B4");
    private static readonly Color CurveFill = Color.Parse("#3FBFC9FF");
    private static readonly Color SavedCurveColor = Color.Parse("#BDBDBD");
    private static readonly Color SavedCurveFill = Color.Parse("#3FBDBDBD");
    private static readonly Color GridLineColor = Color.Parse("#20808080");
    private static readonly Color AxisColor = Color.Parse("#60808080");
    private static readonly Color LabelColor = Color.Parse("#A0A0A0");
    private static readonly Color PointP0Color = Color.Parse("#4CAF50");
    private static readonly Color PointP1Color = Color.Parse("#5A60B4");
    private static readonly Color PointP2Color = Color.Parse("#E57373");

    private DragTarget _dragTarget = DragTarget.None;
    private double _maxLuxScaleDuringDrag = 0;
    private const int MinLuxScale = 800;

    private enum DragTarget { None, P0, P1, P2 }

    #region Styled Properties

    public static readonly StyledProperty<int> MinBrightnessProperty =
        AvaloniaProperty.Register<BezierCurveEditorControl, int>(nameof(MinBrightness), defaultValue: 0,
            defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<double> ControlPointXProperty =
        AvaloniaProperty.Register<BezierCurveEditorControl, double>(nameof(ControlPointX), defaultValue: 400,
            defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<double> ControlPointYProperty =
        AvaloniaProperty.Register<BezierCurveEditorControl, double>(nameof(ControlPointY), defaultValue: 50,
            defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<int> MaxLuxProperty =
        AvaloniaProperty.Register<BezierCurveEditorControl, int>(nameof(MaxLux), defaultValue: 800,
            defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<int> SavedMinBrightnessProperty =
        AvaloniaProperty.Register<BezierCurveEditorControl, int>(nameof(SavedMinBrightness));

    public static readonly StyledProperty<double> SavedControlPointXProperty =
        AvaloniaProperty.Register<BezierCurveEditorControl, double>(nameof(SavedControlPointX), defaultValue: 400);

    public static readonly StyledProperty<double> SavedControlPointYProperty =
        AvaloniaProperty.Register<BezierCurveEditorControl, double>(nameof(SavedControlPointY), defaultValue: 50);

    public static readonly StyledProperty<int> SavedMaxLuxProperty =
        AvaloniaProperty.Register<BezierCurveEditorControl, int>(nameof(SavedMaxLux), defaultValue: 800);

    public static readonly StyledProperty<bool> ShowSavedCurveProperty =
        AvaloniaProperty.Register<BezierCurveEditorControl, bool>(nameof(ShowSavedCurve));

    public int MinBrightness
    {
        get => GetValue(MinBrightnessProperty);
        set => SetValue(MinBrightnessProperty, value);
    }

    public double ControlPointX
    {
        get => GetValue(ControlPointXProperty);
        set => SetValue(ControlPointXProperty, value);
    }

    public double ControlPointY
    {
        get => GetValue(ControlPointYProperty);
        set => SetValue(ControlPointYProperty, value);
    }

    public int MaxLux
    {
        get => GetValue(MaxLuxProperty);
        set => SetValue(MaxLuxProperty, value);
    }

    public int SavedMinBrightness
    {
        get => GetValue(SavedMinBrightnessProperty);
        set => SetValue(SavedMinBrightnessProperty, value);
    }

    public double SavedControlPointX
    {
        get => GetValue(SavedControlPointXProperty);
        set => SetValue(SavedControlPointXProperty, value);
    }

    public double SavedControlPointY
    {
        get => GetValue(SavedControlPointYProperty);
        set => SetValue(SavedControlPointYProperty, value);
    }

    public int SavedMaxLux
    {
        get => GetValue(SavedMaxLuxProperty);
        set => SetValue(SavedMaxLuxProperty, value);
    }

    public bool ShowSavedCurve
    {
        get => GetValue(ShowSavedCurveProperty);
        set => SetValue(ShowSavedCurveProperty, value);
    }

    #endregion

    static BezierCurveEditorControl()
    {
        AffectsRender<BezierCurveEditorControl>(
            MinBrightnessProperty, ControlPointXProperty, ControlPointYProperty, MaxLuxProperty,
            SavedMinBrightnessProperty, SavedControlPointXProperty, SavedControlPointYProperty,
            SavedMaxLuxProperty, ShowSavedCurveProperty);
    }

    public BezierCurveEditorControl()
    {
        ClipToBounds = true;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = Bounds;
        var chartRect = new Rect(Padding_Left, Padding_Top,
            bounds.Width - Padding_Left - Padding_Right,
            bounds.Height - Padding_Top - Padding_Bottom);

        if (chartRect.Width <= 0 || chartRect.Height <= 0) return;

        double xMax = EffectiveMaxLux();

        DrawGridAndAxes(context, chartRect, xMax);

        if (ShowSavedCurve)
        {
            DrawBezierCurve(context, chartRect, xMax,
                0, SavedMinBrightness, SavedControlPointX, SavedControlPointY, SavedMaxLux, 100,
                SavedCurveColor, SavedCurveFill);
        }

        DrawBezierCurve(context, chartRect, xMax,
            0, MinBrightness, ControlPointX, ControlPointY, MaxLux, 100,
            CurveColor, CurveFill);

        DrawControlPoints(context, chartRect, xMax);
    }

    private double EffectiveMaxLux()
    {
        double max = Math.Max(MaxLux, 100);
        if (ShowSavedCurve) max = Math.Max(max, SavedMaxLux);

        // During P2 drag, keep the scale at the maximum reached during this drag
        if (_dragTarget == DragTarget.P2 && _maxLuxScaleDuringDrag > 0)
        {
            max = Math.Max(max, _maxLuxScaleDuringDrag);
        }

        max = Math.Max(max, MinLuxScale);
        return max * 1.05;
    }

    #region Drawing

    private void DrawGridAndAxes(DrawingContext context, Rect chart, double xMax)
    {
        var axisPen = new Pen(new SolidColorBrush(AxisColor), 1);
        var gridPen = new Pen(new SolidColorBrush(GridLineColor), 1);
        var labelBrush = new SolidColorBrush(LabelColor);
        var typeface = new Typeface("Inter", FontStyle.Normal, FontWeight.Normal);

        // Y-axis
        context.DrawLine(axisPen,
            new Point(chart.Left, chart.Top),
            new Point(chart.Left, chart.Bottom));

        // X-axis
        context.DrawLine(axisPen,
            new Point(chart.Left, chart.Bottom),
            new Point(chart.Right, chart.Bottom));

        // Y grid lines and labels (0%, 20%, 40%, 60%, 80%, 100%)
        for (int pct = 0; pct <= 100; pct += 20)
        {
            double py = ChartToPixelY(chart, pct);
            if (pct > 0 && pct < 100)
                context.DrawLine(gridPen, new Point(chart.Left, py), new Point(chart.Right, py));

            var text = new FormattedText($"{pct}%", CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight, typeface, 11, labelBrush);
            context.DrawText(text, new Point(chart.Left - text.Width - 6, py - text.Height / 2));
        }

        // X grid lines and labels
        int xStep = ChooseXStep(xMax);
        for (int lux = 0; lux <= (int)xMax; lux += xStep)
        {
            double px = ChartToPixelX(chart, lux, xMax);
            if (lux > 0)
                context.DrawLine(gridPen, new Point(px, chart.Top), new Point(px, chart.Bottom));

            var text = new FormattedText(lux.ToString(), CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight, typeface, 11, labelBrush);
            context.DrawText(text, new Point(px - text.Width / 2, chart.Bottom + 4));
        }

        // Axis titles
        var yTitle = new FormattedText("Brightness", CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight, typeface, 10, labelBrush);
        context.DrawText(yTitle, new Point(chart.Left + 4, chart.Top - 14));

        var xTitle = new FormattedText("Lux", CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight, typeface, 10, labelBrush);
        context.DrawText(xTitle, new Point(chart.Right - xTitle.Width, chart.Bottom + 16));
    }

    private void DrawBezierCurve(DrawingContext context, Rect chart, double xMax,
        double p0x, double p0y, double p1x, double p1y, double p2x, double p2y,
        Color strokeColor, Color fillColor)
    {
        var (c1, c2) = ComputeSegmentControlPoints(p0x, p0y, p1x, p1y, p2x, p2y);

        var pxP0 = ChartToPixel(chart, p0x, p0y, xMax);
        var pxC1 = ChartToPixel(chart, c1.x, c1.y, xMax);
        var pxP1 = ChartToPixel(chart, p1x, p1y, xMax);
        var pxC2 = ChartToPixel(chart, c2.x, c2.y, xMax);
        var pxP2 = ChartToPixel(chart, p2x, p2y, xMax);

        var fillGeometry = new StreamGeometry();
        using (var ctx = fillGeometry.Open())
        {
            ctx.BeginFigure(pxP0, true);
            ctx.QuadraticBezierTo(pxC1, pxP1);
            ctx.QuadraticBezierTo(pxC2, pxP2);
            ctx.LineTo(new Point(pxP2.X, chart.Bottom));
            ctx.LineTo(new Point(pxP0.X, chart.Bottom));
            ctx.EndFigure(true);
        }

        context.DrawGeometry(new SolidColorBrush(fillColor), null, fillGeometry);

        var strokeGeometry = new StreamGeometry();
        using (var ctx = strokeGeometry.Open())
        {
            ctx.BeginFigure(pxP0, false);
            ctx.QuadraticBezierTo(pxC1, pxP1);
            ctx.QuadraticBezierTo(pxC2, pxP2);
            ctx.EndFigure(false);
        }

        context.DrawGeometry(null, new Pen(new SolidColorBrush(strokeColor), 2), strokeGeometry);
    }

    private void DrawControlPoints(DrawingContext context, Rect chart, double xMax)
    {
        DrawPoint(context, chart, 0, MinBrightness, xMax, PointP0Color, "P0", false);

        // Draw crosshairs for P1 (only towards the axes)
        var p1Pixel = ChartToPixel(chart, ControlPointX, ControlPointY, xMax);
        var crosshairPen = new Pen(new SolidColorBrush(Color.FromArgb(80, 90, 96, 180)), 1,
            new DashStyle(new[] { 3.0, 3.0 }, 0));
        // Horizontal line (from P1 to Y-axis)
        context.DrawLine(crosshairPen, new Point(chart.Left, p1Pixel.Y), new Point(p1Pixel.X, p1Pixel.Y));
        // Vertical line (from P1 to X-axis)
        context.DrawLine(crosshairPen, new Point(p1Pixel.X, p1Pixel.Y), new Point(p1Pixel.X, chart.Bottom));

        DrawPoint(context, chart, ControlPointX, ControlPointY, xMax, PointP1Color, "P1", false);
        DrawPoint(context, chart, MaxLux, 100, xMax, PointP2Color, "P2", true);
    }

    private void DrawPoint(DrawingContext context, Rect chart,
        double cx, double cy, double xMax, Color color, string label, bool labelBelow)
    {
        var px = ChartToPixel(chart, cx, cy, xMax);
        var brush = new SolidColorBrush(color);
        var bgBrush = new SolidColorBrush(Color.FromArgb(180, 255, 255, 255));

        context.DrawEllipse(bgBrush, null, px, ControlPointRadius + 1, ControlPointRadius + 1);
        context.DrawEllipse(brush, new Pen(Brushes.White, 1.5), px, ControlPointRadius, ControlPointRadius);

        var typeface = new Typeface("Inter", FontStyle.Normal, FontWeight.SemiBold);
        var text = new FormattedText(label, CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight, typeface, 10, brush);

        double labelY = labelBelow
            ? px.Y + ControlPointRadius + 2
            : px.Y - ControlPointRadius - text.Height - 2;

        context.DrawText(text, new Point(px.X - text.Width / 2, labelY));
    }

    #endregion

    #region Coordinate Mapping

    private static double ChartToPixelX(Rect chart, double chartX, double xMax)
    {
        return chart.Left + chartX / xMax * chart.Width;
    }

    private static double ChartToPixelY(Rect chart, double chartY)
    {
        return chart.Bottom - chartY / 100.0 * chart.Height;
    }

    private static Point ChartToPixel(Rect chart, double chartX, double chartY, double xMax)
    {
        return new Point(ChartToPixelX(chart, chartX, xMax), ChartToPixelY(chart, chartY));
    }

    private static (double chartX, double chartY) PixelToChart(Rect chart, Point pixel, double xMax)
    {
        double cx = (pixel.X - chart.Left) / chart.Width * xMax;
        double cy = (chart.Bottom - pixel.Y) / chart.Height * 100.0;
        return (cx, cy);
    }

    /// <summary>
    /// Computes Catmull-Rom-style control points for two quadratic Bezier segments
    /// joined at P1: segment 1 (P0->P1 via C1) and segment 2 (P1->P2 via C2).
    /// The tangent is uniformly scaled down to keep both control points inside their
    /// segment bounding boxes, preserving colinearity at P1 (smooth tangent continuity).
    /// </summary>
    internal static ((double x, double y) c1, (double x, double y) c2) ComputeSegmentControlPoints(
        double p0x, double p0y, double p1x, double p1y, double p2x, double p2y)
    {
        double tx = (p2x - p0x) / 4.0;
        double ty = (p2y - p0y) / 4.0;

        double s = 1.0;

        // C1 = P1 - s*t must be in segment 1 bounding box [P0, P1]
        s = ConstrainScale(s, p1x, -tx, Math.Min(p0x, p1x), Math.Max(p0x, p1x));
        s = ConstrainScale(s, p1y, -ty, Math.Min(p0y, p1y), Math.Max(p0y, p1y));

        // C2 = P1 + s*t must be in segment 2 bounding box [P1, P2]
        s = ConstrainScale(s, p1x, tx, Math.Min(p1x, p2x), Math.Max(p1x, p2x));
        s = ConstrainScale(s, p1y, ty, Math.Min(p1y, p2y), Math.Max(p1y, p2y));

        return ((p1x - s * tx, p1y - s * ty),
                (p1x + s * tx, p1y + s * ty));
    }

    /// <summary>
    /// Returns the largest scale factor &lt;= current s such that origin + s * delta
    /// stays within [lo, hi].
    /// </summary>
    private static double ConstrainScale(double s, double origin, double delta, double lo, double hi)
    {
        if (Math.Abs(delta) < 1e-9) return s;

        double sLo = (lo - origin) / delta;
        double sHi = (hi - origin) / delta;
        double sMax = Math.Max(sLo, sHi);

        if (sMax < 0) return 0;
        return Math.Min(s, sMax);
    }

    private static int ChooseXStep(double xMax)
    {
        if (xMax <= 200) return 50;
        if (xMax <= 500) return 100;
        if (xMax <= 1200) return 200;
        return 500;
    }

    #endregion

    #region Pointer Interaction

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;

        var pos = e.GetPosition(this);
        var chart = GetChartRect();
        double xMax = EffectiveMaxLux();

        var p0 = ChartToPixel(chart, 0, MinBrightness, xMax);
        var p1 = ChartToPixel(chart, ControlPointX, ControlPointY, xMax);
        var p2 = ChartToPixel(chart, MaxLux, 100, xMax);

        // Check P1 first (freely movable, most likely drag target)
        if (Distance(pos, p1) < HitTestRadius)
            _dragTarget = DragTarget.P1;
        else if (Distance(pos, p0) < HitTestRadius)
            _dragTarget = DragTarget.P0;
        else if (Distance(pos, p2) < HitTestRadius)
        {
            _dragTarget = DragTarget.P2;
            _maxLuxScaleDuringDrag = Math.Max(MaxLux, MinLuxScale);
        }
        else
            _dragTarget = DragTarget.None;

        if (_dragTarget != DragTarget.None)
        {
            e.Handled = true;
            e.Pointer.Capture(this);
            Cursor = new Cursor(StandardCursorType.Hand);
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_dragTarget == DragTarget.None) return;

        var pos = e.GetPosition(this);
        var chart = GetChartRect();
        double xMax = EffectiveMaxLux();
        var (cx, cy) = PixelToChart(chart, pos, xMax);

        switch (_dragTarget)
        {
            case DragTarget.P0:
                MinBrightness = (int)Math.Clamp(Math.Round(cy), 0, 99);
                break;
            case DragTarget.P1:
                ControlPointX = Math.Clamp(Math.Round(cx), 1, xMax - 1);
                ControlPointY = Math.Clamp(Math.Round(cy, 1), 0, 100);
                break;
            case DragTarget.P2:
                int newMaxLux = (int)Math.Clamp(Math.Round(cx), 100, 5000);
                MaxLux = newMaxLux;
                // Track the maximum scale reached during this drag
                _maxLuxScaleDuringDrag = Math.Max(_maxLuxScaleDuringDrag, newMaxLux);
                break;
        }

        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_dragTarget != DragTarget.None)
        {
            // If P2 was dragged, reset the scale lock so it can shrink on release
            if (_dragTarget == DragTarget.P2)
            {
                _maxLuxScaleDuringDrag = 0;
                InvalidateVisual(); // Trigger re-render to apply the new scale
            }

            _dragTarget = DragTarget.None;
            e.Pointer.Capture(null);
            Cursor = Cursor.Default;
            e.Handled = true;
        }
    }

    private Rect GetChartRect()
    {
        return new Rect(Padding_Left, Padding_Top,
            Bounds.Width - Padding_Left - Padding_Right,
            Bounds.Height - Padding_Top - Padding_Bottom);
    }

    private static double Distance(Point a, Point b)
    {
        double dx = a.X - b.X;
        double dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    #endregion
}
