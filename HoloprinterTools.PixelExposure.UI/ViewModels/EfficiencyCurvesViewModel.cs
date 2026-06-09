using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using HoloprinterTools.PixelExposure.Core.Services;
using HoloprinterTools.PixelExposure.Core.Common;

namespace HoloprinterTools.PixelExposure.UI.ViewModels
{
    /// <summary>
    /// Data class for displaying efficiency results
    /// </summary>
    public class EfficiencyDataPoint
    {
        public double Wavelength { get; set; }
        public double SPol { get; set; }
        public double PPol { get; set; }
        public double Average { get; set; }
    }

    /// <summary>
    /// ViewModel for the Efficiency Curves tab.
    /// Displays wavelength sweep analysis results in a data grid.
    /// </summary>
    public class EfficiencyCurvesViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<EfficiencyDataPoint> _dataPoints;
        private string _title = "Wavelength Sweep Analysis";
        private bool _isLoading;
        private string _statusMessage = "Click 'Run Wavelength Sweep' to generate results";

        public ObservableCollection<EfficiencyDataPoint> DataPoints
        {
            get => _dataPoints;
            set
            {
                if (_dataPoints != value)
                {
                    _dataPoints = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand RunWavelengthSweepCommand { get; }

        public EfficiencyCurvesViewModel()
        {
            DataPoints = new ObservableCollection<EfficiencyDataPoint>();
            RunWavelengthSweepCommand = new RelayCommand(_ => RunWavelengthSweep());
        }

        /// <summary>
        /// Runs the wavelength sweep analysis and updates the data grid.
        /// </summary>
        public async void RunWavelengthSweep()
        {
            IsLoading = true;
            StatusMessage = "Running wavelength sweep...";
            DataPoints.Clear();

            try
            {
                // Get wavelength range parameters in nanometers
                double startWavelengthNm = 760.0;
                double stopWavelengthNm = 900.0;
                double stepWavelengthNm = 2.0;

                // Convert wavelengths from nanometers to micrometers
                const double NmToUmFactor = 0.001; // 1 nm = 0.001 µm
                double startWavelengthUm = startWavelengthNm * NmToUmFactor;
                double stopWavelengthUm = stopWavelengthNm * NmToUmFactor;
                double stepWavelengthUm = stepWavelengthNm * NmToUmFactor;

                // Run the sweep on a background thread
                var (sPol, pPol) = await Task.Run(() =>
                {
                    // Call RcwaCalculator with typical holographic grating parameters
                    double spatialFreq = 1200.0;
                    double effThickness = 4.0;
                    double deltaN = 0.112;
                    double bulkIndex = 1.33;
                    double glassIndex = 1.444;
                    double braggTilt = 90.0;
                    int harmonicOrder = 1;
                    double centerWavelengthUm = 830.0 * NmToUmFactor; // Convert 830 nm to µm
                    double theta = 29.87;

                    return RcwaCalculator.WavelengthSweep(
                        spatialFreq,
                        effThickness,
                        deltaN,
                        bulkIndex,
                        glassIndex,
                        braggTilt,
                        harmonicOrder,
                        centerWavelengthUm,
                        startWavelengthUm,
                        stopWavelengthUm,
                        stepWavelengthUm,
                        theta
                    );
                });

                // Populate data grid on the UI thread
                var wavelengthsUm = Enumerable.Range(0, sPol.Length)
                    .Select(i => startWavelengthUm + i * stepWavelengthUm)
                    .ToList();

                // Clear and add items on the current (UI) thread
                DataPoints.Clear();
                foreach (var (wlUm, i) in wavelengthsUm.Select((w, i) => (w, i)))
                {
                    // Convert back to nm for display
                    double wlNm = wlUm / NmToUmFactor;
                    DataPoints.Add(new EfficiencyDataPoint
                    {
                        Wavelength = wlNm,
                        SPol = sPol[i],
                        PPol = pPol[i],
                        Average = (sPol[i] + pPol[i]) / 2.0
                    });
                }

                Title = $"Wavelength Sweep Results - {sPol.Length} points";
                StatusMessage = $"Completed: {sPol.Length} wavelength points analyzed";
                IsLoading = false;
            }
            catch (InvalidOperationException ex)
            {
                Title = "Error";
                StatusMessage = $"Error: {ex.Message}";
                IsLoading = false;
            }
            catch (Exception ex)
            {
                Title = "Error";
                StatusMessage = $"Unexpected error: {ex.Message}";
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
