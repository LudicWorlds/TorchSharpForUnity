using System;
using System.Collections.Generic;
using UnityEngine;
using SkiaSharp;

/// <summary>
/// A lightweight plotting library built on SkiaSharp for Unity.
/// Stripped down version for training loss curve visualization.
/// </summary>
public class SkiaPlot : IDisposable
{
    // Plot dimensions
    private readonly int _width;
    private readonly int _height;

    // Margins for axes and labels
    private int _marginLeft = 70;
    private int _marginRight = 20;
    private int _marginTop = 40;
    private int _marginBottom = 50;

    // Plot area (calculated from margins)
    private int PlotWidth => _width - _marginLeft - _marginRight;
    private int PlotHeight => _height - _marginTop - _marginBottom;

    // Data series
    private readonly List<PlotSeries> _series = new List<PlotSeries>();

    // Plot settings
    private string _title = "";
    private string _xLabel = "";
    private string _yLabel = "";
    private bool _showGrid = true;
    private bool _showLegend = false;

    /// <summary>
    /// Creates a new plot with the specified dimensions.
    /// </summary>
    public SkiaPlot(int width = 600, int height = 400)
    {
        _width = width;
        _height = height;
    }

    /// <summary>
    /// Plots Y values with automatic X values (0, 1, 2, ...).
    /// </summary>
    public SkiaPlot Plot(IList<float> yValues, SKColor color, string label = null, float lineWidth = 2f)
    {
        var xValues = new List<float>(yValues.Count);
        var yValuesList = new List<float>(yValues.Count);

        for (int i = 0; i < yValues.Count; i++)
        {
            xValues.Add(i);
            yValuesList.Add(yValues[i]);
        }

        _series.Add(new PlotSeries
        {
            XValues = xValues,
            YValues = yValuesList,
            Color = color,
            Label = label,
            LineWidth = lineWidth
        });

        if (!string.IsNullOrEmpty(label))
            _showLegend = true;

        return this;
    }

    /// <summary>
    /// Sets the plot title.
    /// </summary>
    public SkiaPlot SetTitle(string title)
    {
        _title = title;
        return this;
    }

    /// <summary>
    /// Sets the X-axis label.
    /// </summary>
    public SkiaPlot SetXLabel(string label)
    {
        _xLabel = label;
        return this;
    }

    /// <summary>
    /// Sets the Y-axis label.
    /// </summary>
    public SkiaPlot SetYLabel(string label)
    {
        _yLabel = label;
        return this;
    }

    /// <summary>
    /// Enables or disables the grid.
    /// </summary>
    public SkiaPlot ShowGrid(bool show)
    {
        _showGrid = show;
        return this;
    }

    /// <summary>
    /// Enables or disables the legend.
    /// </summary>
    public SkiaPlot ShowLegend(bool show)
    {
        _showLegend = show;
        return this;
    }

    /// <summary>
    /// Renders the plot to a Unity Texture2D.
    /// </summary>
    public Texture2D ToTexture2D()
    {
        using (var bitmap = Render())
        {
            return BitmapToTexture2D(bitmap);
        }
    }

    private SKBitmap Render()
    {
        var bitmap = new SKBitmap(_width, _height);
        using (var canvas = new SKCanvas(bitmap))
        {
            // Clear background
            canvas.Clear(SKColors.White);

            // Calculate axis ranges
            CalculateAxisRanges(out float xMin, out float xMax, out float yMin, out float yMax);

            // 1. Draw ledger first (grid + axes + tick labels) - behind data
            if (_showGrid)
                DrawGrid(canvas, xMin, xMax, yMin, yMax);
            DrawAxes(canvas, xMin, xMax, yMin, yMax);

            // 2. Draw data series (graph) on top of ledger
            foreach (var series in _series)
            {
                DrawLineSeries(canvas, series, xMin, xMax, yMin, yMax);
            }

            // 3. Draw title, axis labels, and legend on top
            if (!string.IsNullOrEmpty(_title))
                DrawTitle(canvas);
            DrawAxisLabels(canvas);
            if (_showLegend)
                DrawLegend(canvas);
        }

        return bitmap;
    }

    private void CalculateAxisRanges(out float xMin, out float xMax, out float yMin, out float yMax)
    {
        xMin = float.MaxValue;
        xMax = float.MinValue;
        yMin = float.MaxValue;
        yMax = float.MinValue;

        // Auto-calculate from data
        foreach (var series in _series)
        {
            for (int i = 0; i < series.XValues.Count; i++)
            {
                xMin = Math.Min(xMin, series.XValues[i]);
                xMax = Math.Max(xMax, series.XValues[i]);
                yMin = Math.Min(yMin, series.YValues[i]);
                yMax = Math.Max(yMax, series.YValues[i]);
            }
        }

        // Handle edge cases
        if (Math.Abs(xMax - xMin) < 0.0001f) { xMin -= 1; xMax += 1; }
        if (Math.Abs(yMax - yMin) < 0.0001f) { yMin -= 1; yMax += 1; }

        // Extend ranges to nice tick boundaries
        float xTickInterval = CalculateNiceTickInterval(xMin, xMax);
        xMin = (float)Math.Floor(xMin / xTickInterval) * xTickInterval;
        xMax = (float)Math.Ceiling(xMax / xTickInterval) * xTickInterval;

        float yTickInterval = CalculateNiceTickInterval(yMin, yMax);
        yMin = (float)Math.Floor(yMin / yTickInterval) * yTickInterval;
        yMax = (float)Math.Ceiling(yMax / yTickInterval) * yTickInterval;
    }

    /// <summary>
    /// Calculates a "nice" tick interval (multiples of 1, 2, 5, or 10 scaled appropriately).
    /// </summary>
    private float CalculateNiceTickInterval(float min, float max, int targetTicks = 5)
    {
        float range = max - min;
        if (range <= 0) return 1;

        // Calculate rough interval
        float roughInterval = range / targetTicks;

        // Find the magnitude (power of 10)
        float magnitude = (float)Math.Pow(10, Math.Floor(Math.Log10(roughInterval)));

        // Normalize to 1-10 range
        float normalized = roughInterval / magnitude;

        // Round to nice number (1, 2, 5, or 10)
        float niceNormalized;
        if (normalized <= 1.5f)
            niceNormalized = 1;
        else if (normalized <= 3)
            niceNormalized = 2;
        else if (normalized <= 7)
            niceNormalized = 5;
        else
            niceNormalized = 10;

        return niceNormalized * magnitude;
    }

    /// <summary>
    /// Generates nice tick values for an axis.
    /// </summary>
    private List<float> GenerateNiceTicks(float min, float max)
    {
        var ticks = new List<float>();
        float interval = CalculateNiceTickInterval(min, max);

        // Start from the first tick at or below min
        float firstTick = (float)Math.Ceiling(min / interval) * interval;

        for (float tick = firstTick; tick <= max + interval * 0.001f; tick += interval)
        {
            ticks.Add(tick);
        }

        return ticks;
    }

    private void DrawGrid(SKCanvas canvas, float xMin, float xMax, float yMin, float yMax)
    {
        using (var gridPaint = new SKPaint())
        {
            gridPaint.Color = new SKColor(220, 220, 220);
            gridPaint.StrokeWidth = 1;
            gridPaint.IsAntialias = true;

            // Draw horizontal grid lines at nice Y tick positions
            var yTicks = GenerateNiceTicks(yMin, yMax);
            foreach (float tickValue in yTicks)
            {
                float y = MapY(tickValue, yMin, yMax);
                canvas.DrawLine(_marginLeft, y, _marginLeft + PlotWidth, y, gridPaint);
            }

            // Draw vertical grid lines at nice X tick positions
            var xTicks = GenerateNiceTicks(xMin, xMax);
            foreach (float tickValue in xTicks)
            {
                float x = MapX(tickValue, xMin, xMax);
                canvas.DrawLine(x, _marginTop, x, _marginTop + PlotHeight, gridPaint);
            }
        }
    }

    private void DrawAxes(SKCanvas canvas, float xMin, float xMax, float yMin, float yMax)
    {
        using (var axisPaint = new SKPaint())
        using (var textPaint = new SKPaint())
        {
            axisPaint.Color = SKColors.Black;
            axisPaint.StrokeWidth = 2;
            axisPaint.IsAntialias = true;

            textPaint.Color = SKColors.Black;
            textPaint.TextSize = 12;
            textPaint.IsAntialias = true;

            // Draw axis lines
            canvas.DrawLine(_marginLeft, _marginTop + PlotHeight, _marginLeft + PlotWidth, _marginTop + PlotHeight, axisPaint);
            canvas.DrawLine(_marginLeft, _marginTop, _marginLeft, _marginTop + PlotHeight, axisPaint);

            // X-axis tick labels at nice positions
            var xTicks = GenerateNiceTicks(xMin, xMax);
            textPaint.TextAlign = SKTextAlign.Center;
            foreach (float tickValue in xTicks)
            {
                float x = MapX(tickValue, xMin, xMax);
                string label = FormatNumber(tickValue);
                canvas.DrawText(label, x, _marginTop + PlotHeight + 20, textPaint);
            }

            // Y-axis tick labels at nice positions
            var yTicks = GenerateNiceTicks(yMin, yMax);
            textPaint.TextAlign = SKTextAlign.Right;
            foreach (float tickValue in yTicks)
            {
                float y = MapY(tickValue, yMin, yMax);
                string label = FormatNumber(tickValue);
                canvas.DrawText(label, _marginLeft - 8, y + 4, textPaint);
            }
        }
    }

    private void DrawLineSeries(SKCanvas canvas, PlotSeries series, float xMin, float xMax, float yMin, float yMax)
    {
        if (series.XValues.Count < 2) return;

        using (var linePaint = new SKPaint())
        {
            linePaint.Color = series.Color;
            linePaint.StrokeWidth = series.LineWidth;
            linePaint.IsAntialias = true;
            linePaint.Style = SKPaintStyle.Stroke;

            using (var path = new SKPath())
            {
                for (int i = 0; i < series.XValues.Count; i++)
                {
                    float x = MapX(series.XValues[i], xMin, xMax);
                    float y = MapY(series.YValues[i], yMin, yMax);

                    if (i == 0)
                        path.MoveTo(x, y);
                    else
                        path.LineTo(x, y);
                }

                canvas.DrawPath(path, linePaint);
            }
        }
    }

    private void DrawTitle(SKCanvas canvas)
    {
        using (var titlePaint = new SKPaint())
        {
            titlePaint.Color = SKColors.Black;
            titlePaint.TextSize = 18;
            titlePaint.IsAntialias = true;
            titlePaint.TextAlign = SKTextAlign.Center;
            titlePaint.FakeBoldText = true;

            canvas.DrawText(_title, _width / 2f, 25, titlePaint);
        }
    }

    private void DrawAxisLabels(SKCanvas canvas)
    {
        using (var labelPaint = new SKPaint())
        {
            labelPaint.Color = SKColors.Black;
            labelPaint.TextSize = 14;
            labelPaint.IsAntialias = true;

            // X-axis label
            if (!string.IsNullOrEmpty(_xLabel))
            {
                labelPaint.TextAlign = SKTextAlign.Center;
                canvas.DrawText(_xLabel, _marginLeft + PlotWidth / 2f, _height - 10, labelPaint);
            }

            // Y-axis label (rotated)
            if (!string.IsNullOrEmpty(_yLabel))
            {
                canvas.Save();
                canvas.Translate(15, _marginTop + PlotHeight / 2f);
                canvas.RotateDegrees(-90);
                labelPaint.TextAlign = SKTextAlign.Center;
                canvas.DrawText(_yLabel, 0, 0, labelPaint);
                canvas.Restore();
            }
        }
    }

    private void DrawLegend(SKCanvas canvas)
    {
        var labeledSeries = _series.FindAll(s => !string.IsNullOrEmpty(s.Label));
        if (labeledSeries.Count == 0) return;

        using (var bgPaint = new SKPaint())
        using (var textPaint = new SKPaint())
        using (var linePaint = new SKPaint())
        {
            bgPaint.Color = new SKColor(255, 255, 255, 230);
            bgPaint.Style = SKPaintStyle.Fill;

            textPaint.Color = SKColors.Black;
            textPaint.TextSize = 12;
            textPaint.IsAntialias = true;

            // Calculate legend width based on longest label
            float maxTextWidth = 0;
            foreach (var series in labeledSeries)
            {
                float textWidth = textPaint.MeasureText(series.Label);
                if (textWidth > maxTextWidth)
                    maxTextWidth = textWidth;
            }

            float lineHeight = 20;
            float padding = 10;
            float lineIndicatorWidth = 30; // space for colored line (5 to 25) + gap
            float legendWidth = lineIndicatorWidth + maxTextWidth + padding;
            float legendHeight = labeledSeries.Count * lineHeight + padding;

            float legendMargin = 10;
            float legendX = _marginLeft + PlotWidth - legendWidth - legendMargin;
            float legendY = _marginTop + legendMargin;

            // Draw background
            canvas.DrawRect(legendX, legendY, legendWidth, legendHeight, bgPaint);

            // Draw border
            bgPaint.Style = SKPaintStyle.Stroke;
            bgPaint.Color = SKColors.Gray;
            canvas.DrawRect(legendX, legendY, legendWidth, legendHeight, bgPaint);

            // Draw legend items
            for (int i = 0; i < labeledSeries.Count; i++)
            {
                var series = labeledSeries[i];
                float itemY = legendY + 15 + i * lineHeight;

                linePaint.Color = series.Color;
                linePaint.StrokeWidth = 3;
                linePaint.IsAntialias = true;

                canvas.DrawLine(legendX + 5, itemY, legendX + 25, itemY, linePaint);
                canvas.DrawText(series.Label, legendX + 30, itemY + 4, textPaint);
            }
        }
    }

    private float MapX(float value, float min, float max)
    {
        return _marginLeft + ((value - min) / (max - min)) * PlotWidth;
    }

    private float MapY(float value, float min, float max)
    {
        return _marginTop + PlotHeight - ((value - min) / (max - min)) * PlotHeight;
    }

    private string FormatNumber(float value)
    {
        if (Math.Abs(value) >= 1000 || (Math.Abs(value) < 0.01 && value != 0))
            return value.ToString("0.##E0");
        if (Math.Abs(value) < 10)
            return value.ToString("0.##");
        return value.ToString("0.#");
    }

    private Texture2D BitmapToTexture2D(SKBitmap bitmap)
    {
        var texture = new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.RGBA32, false);
        var pixels = bitmap.Pixels;
        var colors = new Color32[pixels.Length];

        for (int i = 0; i < pixels.Length; i++)
        {
            var pixel = pixels[i];
            colors[i] = new Color32(pixel.Red, pixel.Green, pixel.Blue, pixel.Alpha);
        }

        // Flip vertically (SKBitmap is top-down, Unity is bottom-up)
        var flippedColors = new Color32[colors.Length];
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                int srcIndex = y * bitmap.Width + x;
                int dstIndex = (bitmap.Height - 1 - y) * bitmap.Width + x;
                flippedColors[dstIndex] = colors[srcIndex];
            }
        }

        texture.SetPixels32(flippedColors);
        texture.Apply();
        return texture;
    }

    public void Dispose()
    {
        _series.Clear();
    }

    // Internal class for storing series data
    private class PlotSeries
    {
        public List<float> XValues;
        public List<float> YValues;
        public SKColor Color;
        public string Label;
        public float LineWidth = 2f;
    }
}
