using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using HoloprinterTools.PixelExposure.UI.ViewModels;

namespace HoloprinterTools.PixelExposure.UI.Controls
{
    /// <summary>
    /// A control that renders a line chart for wavelength sweep efficiency data.
    /// Draws directly using DrawingContext for proper Avalonia compatibility.
    /// </summary>
    public class LineChartControl : Control
    {
        public static readonly StyledProperty<ObservableCollection<EfficiencyDataPoint>> DataPointsProperty =
            AvaloniaProperty.Register<LineChartControl, ObservableCollection<EfficiencyDataPoint>>(nameof(DataPoints));

        public ObservableCollection<EfficiencyDataPoint> DataPoints
        {
            get => GetValue(DataPointsProperty);
            set => SetValue(DataPointsProperty, value);
        }

        private const double Padding = 70;

        public LineChartControl()
        {
            DataPointsProperty.Changed.AddClassHandler<LineChartControl>((x, e) => 
            {
                // Unsubscribe from old collection
                if (e.OldValue is ObservableCollection<EfficiencyDataPoint> oldCollection)
                {
                    oldCollection.CollectionChanged -= x.OnDataPointsCollectionChanged;
                }

                // Subscribe to new collection
                if (e.NewValue is ObservableCollection<EfficiencyDataPoint> newCollection)
                {
                    newCollection.CollectionChanged += x.OnDataPointsCollectionChanged;
                }

                x.InvalidateVisual();
            });

            // Ensure the control fills available space
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
        }

        private void OnDataPointsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            InvalidateVisual();
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (DataPoints == null || DataPoints.Count == 0)
            {
                // Render a placeholder message
                context.FillRectangle(Brushes.White, Bounds);
                var typeface = new Typeface("Arial");
                var text = new FormattedText(
                    "No data points to display. Click 'Run Wavelength Sweep' to generate results.",
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    14,
                    Brushes.Gray);
                context.DrawText(text, new Point(Bounds.Width / 2 - 250, Bounds.Height / 2));
                return;
            }

            var width = Bounds.Width;
            var height = Bounds.Height;

            if (width <= 0 || height <= 0)
                return;

            // Draw white background
            context.FillRectangle(Brushes.White, Bounds);

            // Calculate plot area
            var plotLeft = Padding;
            var plotRight = width - Padding;
            var plotTop = Padding;
            var plotBottom = height - Padding;
            var plotWidth = plotRight - plotLeft;
            var plotHeight = plotBottom - plotTop;

            if (plotWidth <= 0 || plotHeight <= 0)
                return;

            // Find data ranges
            var minWavelength = DataPoints.Min(p => p.Wavelength);
            var maxWavelength = DataPoints.Max(p => p.Wavelength);
            var minEfficiency = 0.0;
            var maxEfficiency = 1.0;

            // Draw grid
            DrawGrid(context, plotLeft, plotRight, plotTop, plotBottom);

            // Draw axes
            DrawAxes(context, plotLeft, plotRight, plotTop, plotBottom);

            // Draw data lines
            DrawLines(context, plotLeft, plotTop, plotWidth, plotHeight, minWavelength, maxWavelength, minEfficiency, maxEfficiency);

            // Draw labels
            DrawAxisLabels(context, plotLeft, plotRight, plotTop, plotBottom, plotWidth, plotHeight, 
                minWavelength, maxWavelength, minEfficiency, maxEfficiency);

            // Draw legend
            DrawLegend(context, width);

            // Draw title
            DrawTitle(context, width);
        }

        private void DrawGrid(DrawingContext context, double plotLeft, double plotRight, double plotTop, double plotBottom)
        {
            var pen = new Pen(new SolidColorBrush(Colors.LightGray), 1);

            // Vertical grid lines
            for (int i = 1; i < 5; i++)
            {
                var x = plotLeft + (plotRight - plotLeft) / 5 * i;
                context.DrawLine(pen, new Point(x, plotTop), new Point(x, plotBottom));
            }

            // Horizontal grid lines
            for (int i = 1; i < 5; i++)
            {
                var y = plotTop + (plotBottom - plotTop) / 5 * i;
                context.DrawLine(pen, new Point(plotLeft, y), new Point(plotRight, y));
            }
        }

        private void DrawAxes(DrawingContext context, double plotLeft, double plotRight, double plotTop, double plotBottom)
        {
            var pen = new Pen(Brushes.Black, 2);

            // X-axis
            context.DrawLine(pen, new Point(plotLeft, plotBottom), new Point(plotRight, plotBottom));

            // Y-axis
            context.DrawLine(pen, new Point(plotLeft, plotTop), new Point(plotLeft, plotBottom));
        }

        private void DrawLines(DrawingContext context, double plotLeft, double plotTop, double plotWidth, double plotHeight,
            double minWavelength, double maxWavelength, double minEfficiency, double maxEfficiency)
        {
            var wavelengthRange = maxWavelength - minWavelength;
            var efficiencyRange = maxEfficiency - minEfficiency;

            if (wavelengthRange <= 0 || efficiencyRange <= 0)
            {
                // If ranges are invalid, draw a simple test line
                context.DrawLine(new Pen(Brushes.Red, 2), 
                    new Point(plotLeft, plotTop), 
                    new Point(plotLeft + plotWidth, plotTop + plotHeight));
                return;
            }

            // S-Pol line (Red)
            var sPolPen = new Pen(Brushes.Red, 2);
            var sPolPoints = DataPoints.Select(point =>
            {
                var xPercent = wavelengthRange > 0 ? (point.Wavelength - minWavelength) / wavelengthRange : 0;
                var x = plotLeft + xPercent * plotWidth;
                var yPercent = efficiencyRange > 0 ? (point.SPol - minEfficiency) / efficiencyRange : 0;
                var y = plotTop + plotHeight - yPercent * plotHeight;
                return new Point(x, y);
            }).ToList();

            for (int i = 0; i < sPolPoints.Count - 1; i++)
            {
                context.DrawLine(sPolPen, sPolPoints[i], sPolPoints[i + 1]);
            }

            // P-Pol line (Blue)
            var pPolPen = new Pen(Brushes.Blue, 2);
            var pPolPoints = DataPoints.Select(point =>
            {
                var xPercent = wavelengthRange > 0 ? (point.Wavelength - minWavelength) / wavelengthRange : 0;
                var x = plotLeft + xPercent * plotWidth;
                var yPercent = efficiencyRange > 0 ? (point.PPol - minEfficiency) / efficiencyRange : 0;
                var y = plotTop + plotHeight - yPercent * plotHeight;
                return new Point(x, y);
            }).ToList();

            for (int i = 0; i < pPolPoints.Count - 1; i++)
            {
                context.DrawLine(pPolPen, pPolPoints[i], pPolPoints[i + 1]);
            }

            // Average line (Green)
            var avgPen = new Pen(Brushes.Green, 2);
            var avgPoints = DataPoints.Select(point =>
            {
                var xPercent = wavelengthRange > 0 ? (point.Wavelength - minWavelength) / wavelengthRange : 0;
                var x = plotLeft + xPercent * plotWidth;
                var yPercent = efficiencyRange > 0 ? (point.Average - minEfficiency) / efficiencyRange : 0;
                var y = plotTop + plotHeight - yPercent * plotHeight;
                return new Point(x, y);
            }).ToList();

            for (int i = 0; i < avgPoints.Count - 1; i++)
            {
                context.DrawLine(avgPen, avgPoints[i], avgPoints[i + 1]);
            }
        }

        private void DrawAxisLabels(DrawingContext context, double plotLeft, double plotRight, double plotTop, double plotBottom,
            double plotWidth, double plotHeight, double minWavelength, double maxWavelength, double minEfficiency, double maxEfficiency)
        {
            var typeface = new Typeface("Arial");

            // X-axis label
            var xLabelText = new FormattedText(
                "Wavelength (nm)",
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                14,
                Brushes.Black);
            context.DrawText(xLabelText, new Point(plotLeft + plotWidth / 2 - 60, plotBottom + 30));

            // Y-axis label
            var yLabelText = new FormattedText(
                "Efficiency (%)",
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                14,
                Brushes.Black);
            context.DrawText(yLabelText, new Point(10, plotTop + plotHeight / 2 - 30));

            // X-axis tick labels
            for (int i = 0; i <= 5; i++)
            {
                var wavelength = minWavelength + (maxWavelength - minWavelength) * (i / 5.0);
                var x = plotLeft + plotWidth / 5 * i;
                var tickLabel = new FormattedText(
                    wavelength.ToString("F0"),
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    11,
                    Brushes.Black);
                context.DrawText(tickLabel, new Point(x - 15, plotBottom + 10));
            }

            // Y-axis tick labels
            for (int i = 0; i <= 5; i++)
            {
                var efficiency = minEfficiency + (maxEfficiency - minEfficiency) * (i / 5.0);
                var y = plotBottom - plotHeight / 5 * i;
                var tickLabel = new FormattedText(
                    efficiency.ToString("F0"),
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    11,
                    Brushes.Black);
                context.DrawText(tickLabel, new Point(plotLeft - 40, y - 8));
            }
        }

        private void DrawLegend(DrawingContext context, double width)
        {
            const double legendX = 20;
            const double legendY = 20;
            const double itemHeight = 20;
            const double rectSize = 12;
            var typeface = new Typeface("Arial");

            // S-Pol legend item (Red)
            context.FillRectangle(Brushes.Red, new Rect(legendX, legendY, rectSize, rectSize));
            var sPolText = new FormattedText(
                "S-Pol",
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                12,
                Brushes.Black);
            context.DrawText(sPolText, new Point(legendX + rectSize + 8, legendY + 2));

            // P-Pol legend item (Blue)
            context.FillRectangle(Brushes.Blue, new Rect(legendX, legendY + itemHeight, rectSize, rectSize));
            var pPolText = new FormattedText(
                "P-Pol",
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                12,
                Brushes.Black);
            context.DrawText(pPolText, new Point(legendX + rectSize + 8, legendY + itemHeight + 2));

            // Average legend item (Green)
            context.FillRectangle(Brushes.Green, new Rect(legendX, legendY + itemHeight * 2, rectSize, rectSize));
            var avgText = new FormattedText(
                "Average",
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                12,
                Brushes.Black);
            context.DrawText(avgText, new Point(legendX + rectSize + 8, legendY + itemHeight * 2 + 2));
        }

        private void DrawTitle(DrawingContext context, double width)
        {
            var typeface = new Typeface("Arial", FontStyle.Normal, FontWeight.Bold);
            var title = new FormattedText(
                "Wavelength Sweep Analysis",
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                16,
                Brushes.Black);
            context.DrawText(title, new Point(width / 2 - 90, 10));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // Return the full available size to stretch the control
            if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
            {
                return new Size(800, 600);
            }
            return availableSize;
        }
    }
}
