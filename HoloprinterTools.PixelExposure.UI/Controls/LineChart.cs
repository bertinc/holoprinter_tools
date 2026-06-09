using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using HoloprinterTools.PixelExposure.UI.ViewModels;

namespace HoloprinterTools.PixelExposure.UI.Controls
{
    /// <summary>
    /// Custom line chart control for visualizing efficiency curves.
    /// Renders two line series (S-Pol and P-Pol) on a canvas with auto-scaling.
    /// </summary>
    public class LineChart : UserControl
    {
        public static readonly StyledProperty<ObservableCollection<EfficiencyDataPoint>> DataPointsProperty =
            AvaloniaProperty.Register<LineChart, ObservableCollection<EfficiencyDataPoint>>(nameof(DataPoints));

        public ObservableCollection<EfficiencyDataPoint> DataPoints
        {
            get => GetValue(DataPointsProperty);
            set => SetValue(DataPointsProperty, value);
        }

        private Canvas? _canvas;
        private const double Padding = 60;
        private const double GridSpacing = 50;

        public LineChart()
        {
            InitializeComponent();
            DataPointsProperty.Changed.AddClassHandler<LineChart>((x, e) => x.RedrawChart());
        }

        private void InitializeComponent()
        {
            _canvas = new Canvas
            {
                Background = Brushes.White,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
            };

            Content = _canvas;
        }

        private void RedrawChart()
        {
            if (_canvas == null || DataPoints == null || DataPoints.Count == 0)
                return;

            _canvas.Children.Clear();

            var width = _canvas.Bounds.Width;
            var height = _canvas.Bounds.Height;

            if (width <= 0 || height <= 0)
                return;

            // Calculate plot area
            var plotWidth = width - 2 * Padding;
            var plotHeight = height - 2 * Padding;

            // Find data ranges
            var minWavelength = DataPoints.Min(p => p.Wavelength);
            var maxWavelength = DataPoints.Max(p => p.Wavelength);
            var minEfficiency = 0.0;
            var maxEfficiency = Math.Max(
                DataPoints.Max(p => p.SPol),
                DataPoints.Max(p => p.PPol)
            );

            // Add padding to efficiency range
            maxEfficiency *= 1.1;

            // Draw axes
            DrawAxes(_canvas, Padding, height - Padding, plotWidth, plotHeight);

            // Draw grid
            DrawGrid(_canvas, Padding, height - Padding, plotWidth, plotHeight, minWavelength, maxWavelength, minEfficiency, maxEfficiency);

            // Draw S-Pol line (red)
            DrawLine(_canvas, DataPoints, p => p.SPol, Brushes.Red, Padding, height - Padding, plotWidth, plotHeight,
                minWavelength, maxWavelength, minEfficiency, maxEfficiency);

            // Draw P-Pol line (blue)
            DrawLine(_canvas, DataPoints, p => p.PPol, Brushes.Blue, Padding, height - Padding, plotWidth, plotHeight,
                minWavelength, maxWavelength, minEfficiency, maxEfficiency);

            // Draw legend
            DrawLegend(_canvas, width, Padding);

            // Draw labels
            DrawLabels(_canvas, Padding, height - Padding, plotWidth, plotHeight, minWavelength, maxWavelength, minEfficiency, maxEfficiency);
        }

        private void DrawAxes(Canvas canvas, double startX, double startY, double width, double height)
        {
            // X-axis
            var xAxisLine = new Line
            {
                StartPoint = new Point(startX, startY),
                EndPoint = new Point(startX + width, startY),
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            canvas.Children.Add(xAxisLine);

            // Y-axis
            var yAxisLine = new Line
            {
                StartPoint = new Point(startX, startY),
                EndPoint = new Point(startX, startY - height),
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            canvas.Children.Add(yAxisLine);
        }

        private void DrawGrid(Canvas canvas, double startX, double startY, double plotWidth, double plotHeight,
            double minWavelength, double maxWavelength, double minEfficiency, double maxEfficiency)
        {
            var wavelengthRange = maxWavelength - minWavelength;
            var efficiencyRange = maxEfficiency - minEfficiency;

            // Vertical grid lines (wavelength)
            for (var i = 1; i < 5; i++)
            {
                var x = startX + (plotWidth / 5) * i;
                var line = new Line
                {
                    StartPoint = new Point(x, startY),
                    EndPoint = new Point(x, startY - plotHeight),
                    Stroke = new SolidColorBrush(Colors.LightGray, 0.5),
                    StrokeThickness = 1
                };
                canvas.Children.Add(line);
            }

            // Horizontal grid lines (efficiency)
            for (var i = 1; i < 5; i++)
            {
                var y = startY - (plotHeight / 5) * i;
                var line = new Line
                {
                    StartPoint = new Point(startX, y),
                    EndPoint = new Point(startX + plotWidth, y),
                    Stroke = new SolidColorBrush(Colors.LightGray, 0.5),
                    StrokeThickness = 1
                };
                canvas.Children.Add(line);
            }
        }

        private void DrawLine(Canvas canvas, ObservableCollection<EfficiencyDataPoint> dataPoints, Func<EfficiencyDataPoint, double> valueSelector,
            IBrush brush, double startX, double startY, double plotWidth, double plotHeight,
            double minWavelength, double maxWavelength, double minEfficiency, double maxEfficiency)
        {
            if (dataPoints.Count < 2)
                return;

            var wavelengthRange = maxWavelength - minWavelength;
            var efficiencyRange = maxEfficiency - minEfficiency;

            var polyline = new Polyline
            {
                Stroke = brush,
                StrokeThickness = 2,
                StrokeLineCap = PenLineCap.Round
            };

            var points = new Points();
            foreach (var dataPoint in dataPoints)
            {
                var x = startX + ((dataPoint.Wavelength - minWavelength) / wavelengthRange) * plotWidth;
                var value = valueSelector(dataPoint);
                var y = startY - ((value - minEfficiency) / efficiencyRange) * plotHeight;
                points.Add(new Point(x, y));
            }

            polyline.Points = points;
            canvas.Children.Add(polyline);
        }

        private void DrawLegend(Canvas canvas, double canvasWidth, double startY)
        {
            const double legendX = 20;
            const double legendItemHeight = 20;
            const double legendRectSize = 12;

            // S-Pol (Red)
            var sPolRect = new Rectangle
            {
                Width = legendRectSize,
                Height = legendRectSize,
                Fill = Brushes.Red
            };
            Canvas.SetLeft(sPolRect, legendX);
            Canvas.SetTop(sPolRect, startY);
            canvas.Children.Add(sPolRect);

            var sPolText = new TextBlock
            {
                Text = "S-Pol",
                Foreground = Brushes.Black,
                FontSize = 12
            };
            Canvas.SetLeft(sPolText, legendX + legendRectSize + 8);
            Canvas.SetTop(sPolText, startY + 2);
            canvas.Children.Add(sPolText);

            // P-Pol (Blue)
            var pPolRect = new Rectangle
            {
                Width = legendRectSize,
                Height = legendRectSize,
                Fill = Brushes.Blue
            };
            Canvas.SetLeft(pPolRect, legendX);
            Canvas.SetTop(pPolRect, startY + legendItemHeight);
            canvas.Children.Add(pPolRect);

            var pPolText = new TextBlock
            {
                Text = "P-Pol",
                Foreground = Brushes.Black,
                FontSize = 12
            };
            Canvas.SetLeft(pPolText, legendX + legendRectSize + 8);
            Canvas.SetTop(pPolText, startY + legendItemHeight + 2);
            canvas.Children.Add(pPolText);
        }

        private void DrawLabels(Canvas canvas, double startX, double startY, double plotWidth, double plotHeight,
            double minWavelength, double maxWavelength, double minEfficiency, double maxEfficiency)
        {
            // X-axis label
            var xLabel = new TextBlock
            {
                Text = "Wavelength (nm)",
                Foreground = Brushes.Black,
                FontSize = 12,
                FontWeight = Avalonia.Media.FontWeight.Bold
            };
            Canvas.SetLeft(xLabel, startX + plotWidth / 2 - 50);
            Canvas.SetTop(xLabel, startY + 30);
            canvas.Children.Add(xLabel);

            // Y-axis label
            var yLabel = new TextBlock
            {
                Text = "Efficiency (%)",
                Foreground = Brushes.Black,
                FontSize = 12,
                FontWeight = Avalonia.Media.FontWeight.Bold
            };
            Canvas.SetLeft(yLabel, startX - 50);
            Canvas.SetTop(yLabel, startY - plotHeight - 20);
            canvas.Children.Add(yLabel);

            // X-axis tick labels
            for (var i = 0; i <= 5; i++)
            {
                var wavelength = minWavelength + (maxWavelength - minWavelength) * (i / 5.0);
                var x = startX + (plotWidth / 5) * i;
                var tickLabel = new TextBlock
                {
                    Text = wavelength.ToString("F0"),
                    Foreground = Brushes.Black,
                    FontSize = 10
                };
                Canvas.SetLeft(tickLabel, x - 15);
                Canvas.SetTop(tickLabel, startY + 5);
                canvas.Children.Add(tickLabel);
            }

            // Y-axis tick labels
            for (var i = 0; i <= 5; i++)
            {
                var efficiency = minEfficiency + (maxEfficiency - minEfficiency) * (i / 5.0);
                var y = startY - (plotHeight / 5) * i;
                var tickLabel = new TextBlock
                {
                    Text = efficiency.ToString("F0"),
                    Foreground = Brushes.Black,
                    FontSize = 10
                };
                Canvas.SetLeft(tickLabel, startX - 40);
                Canvas.SetTop(tickLabel, y - 8);
                canvas.Children.Add(tickLabel);
            }
        }

        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);
            RedrawChart();
        }
    }
}
