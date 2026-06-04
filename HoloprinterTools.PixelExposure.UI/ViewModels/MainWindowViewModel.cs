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

        public AppSettings Settings { get; private set; }

        public ICommand SaveCommand { get; }

        public MainWindowViewModel(SettingsService settingsService, AppSettings settings)
        {
            _settingsService = settingsService;
            Settings = settings;

            SaveCommand = new RelayCommand(async _ => await SaveAsync());
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
