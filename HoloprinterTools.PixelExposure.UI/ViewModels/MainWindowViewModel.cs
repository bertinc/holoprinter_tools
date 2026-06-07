using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using HoloprinterTools.PixelExposure.UI.Common;
using HoloprinterTools.PixelExposure.UI.Models;
using HoloprinterTools.PixelExposure.UI.Services;

namespace HoloprinterTools.PixelExposure.UI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly SettingsService _settingsService;
        private AppSettings _settings;

        public AppSettings Settings
        {
            get => _settings;
            private set
            {
                if (_settings != value)
                {
                    _settings = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _minWavelength;
        public int MinWavelength
        {
            get => _minWavelength;
            set
            {
                // Constrain to valid range and ensure it doesn't exceed MaxWavelength
                int newValue = Math.Max(400, Math.Min(700, value));
                if (newValue > _maxWavelength)
                {
                    newValue = _maxWavelength;
                }

                if (_minWavelength != newValue)
                {
                    _minWavelength = newValue;
                    _settings.MinWavelength = newValue;
                    OnPropertyChanged();
                }
            }
        }

        private int _maxWavelength;
        public int MaxWavelength
        {
            get => _maxWavelength;
            set
            {
                // Constrain to valid range and ensure it doesn't go below MinWavelength
                int newValue = Math.Max(400, Math.Min(700, value));
                if (newValue < _minWavelength)
                {
                    newValue = _minWavelength;
                }

                if (_maxWavelength != newValue)
                {
                    _maxWavelength = newValue;
                    _settings.MaxWavelength = newValue;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand ShowSettingsCommand { get; }

        public event EventHandler? ShowSettingsRequested;

        public MainWindowViewModel(SettingsService settingsService, AppSettings settings)
        {
            _settingsService = settingsService;
            Settings = settings;
            _minWavelength = settings.MinWavelength;
            _maxWavelength = settings.MaxWavelength;

            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            ShowSettingsCommand = new RelayCommand(_ => OnShowSettingsRequested());
        }

        private void OnShowSettingsRequested()
        {
            ShowSettingsRequested?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public async Task SaveAsync()
        {
            await _settingsService.SaveAsync(Settings).ConfigureAwait(false);
        }
    }
}
