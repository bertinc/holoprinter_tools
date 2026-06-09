using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HoloprinterTools.PixelExposure.Core.Common;
using HoloprinterTools.PixelExposure.Core.Models;

namespace HoloprinterTools.PixelExposure.UI.ViewModels
{
    public class PrinterSettingsViewModel : INotifyPropertyChanged
    {
        private AppSettings _settings;

        public double PrintbedWidth
        {
            get => _settings.PrintbedWidth;
            set
            {
                if (_settings.PrintbedWidth != value)
                {
                    _settings.PrintbedWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        public double PrintbedLength
        {
            get => _settings.PrintbedLength;
            set
            {
                if (_settings.PrintbedLength != value)
                {
                    _settings.PrintbedLength = value;
                    OnPropertyChanged();
                }
            }
        }

        public double LaserWavelength
        {
            get => _settings.LaserWavelength;
            set
            {
                if (_settings.LaserWavelength != value)
                {
                    _settings.LaserWavelength = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Beam1MaxAngle
        {
            get => _settings.Beam1MaxAngle;
            set
            {
                if (_settings.Beam1MaxAngle != value)
                {
                    _settings.Beam1MaxAngle = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Beam1MinAngle
        {
            get => _settings.Beam1MinAngle;
            set
            {
                if (_settings.Beam1MinAngle != value)
                {
                    _settings.Beam1MinAngle = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Beam2MaxAngle
        {
            get => _settings.Beam2MaxAngle;
            set
            {
                if (_settings.Beam2MaxAngle != value)
                {
                    _settings.Beam2MaxAngle = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Beam2MinAngle
        {
            get => _settings.Beam2MinAngle;
            set
            {
                if (_settings.Beam2MinAngle != value)
                {
                    _settings.Beam2MinAngle = value;
                    OnPropertyChanged();
                }
            }
        }

        public double PixelSize
        {
            get => _settings.PixelSize;
            set
            {
                if (_settings.PixelSize != value)
                {
                    _settings.PixelSize = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Beam1Power
        {
            get => _settings.Beam1Power;
            set
            {
                if (_settings.Beam1Power != value)
                {
                    _settings.Beam1Power = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Beam2Power
        {
            get => _settings.Beam2Power;
            set
            {
                if (_settings.Beam2Power != value)
                {
                    _settings.Beam2Power = value;
                    OnPropertyChanged();
                }
            }
        }

        public double SettleTime
        {
            get => _settings.SettleTime;
            set
            {
                if (_settings.SettleTime != value)
                {
                    _settings.SettleTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MinDiffractionEfficiency
        {
            get => _settings.MinDiffractionEfficiency;
            set
            {
                if (_settings.MinDiffractionEfficiency != value)
                {
                    _settings.MinDiffractionEfficiency = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MaxDeltaN
        {
            get => _settings.MaxDeltaN;
            set
            {
                if (_settings.MaxDeltaN != value)
                {
                    _settings.MaxDeltaN = value;
                    OnPropertyChanged();
                }
            }
        }

        public double NominalViewingHeight
        {
            get => _settings.NominalViewingHeight;
            set
            {
                if (_settings.NominalViewingHeight != value)
                {
                    _settings.NominalViewingHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MinimumViewingDistance
        {
            get => _settings.MinimumViewingDistance;
            set
            {
                if (_settings.MinimumViewingDistance != value)
                {
                    _settings.MinimumViewingDistance = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public PrinterSettingsViewModel(AppSettings settings)
        {
            _settings = settings;
            OkCommand = new RelayCommand(_ => { /* Dialog will handle closure */ });
            CancelCommand = new RelayCommand(_ => { /* Dialog will handle closure */ });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
