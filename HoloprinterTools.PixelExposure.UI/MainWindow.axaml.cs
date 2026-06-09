using Avalonia.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using HoloprinterTools.PixelExposure.Core.Services;
using HoloprinterTools.PixelExposure.Core.Models;
using HoloprinterTools.PixelExposure.UI.ViewModels;

namespace HoloprinterTools.PixelExposure.UI
{
    public partial class MainWindow : Window
    {
        private readonly SettingsService _settingsService;

        public MainWindow()
        {
            InitializeComponent();

            // Load settings from app folder (same folder as executable). If a shared settings.json
            // is dropped beside the app, it will be used.
            var exeFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? ".";
            var settingsPath = Path.Combine(exeFolder, "settings.json");

            _settingsService = new SettingsService(settingsPath);
            var settings = _settingsService.LoadAsync().GetAwaiter().GetResult();

            var viewModel = new MainWindowViewModel(_settingsService, settings);
            DataContext = viewModel;

            // Subscribe to settings dialog request
            viewModel.ShowSettingsRequested += async (s, e) => await ShowPrinterSettingsDialog();
        }

        private async Task ShowPrinterSettingsDialog()
        {
            var viewModel = (MainWindowViewModel)DataContext;
            var printerSettingsViewModel = new PrinterSettingsViewModel(viewModel.Settings);
            var settingsWindow = new PrinterSettingsWindow(printerSettingsViewModel);

            var result = await settingsWindow.ShowDialog<bool?>(this);

            if (result == true)
            {
                // Settings were applied, save them
                await _settingsService.SaveAsync(viewModel.Settings).ConfigureAwait(false);
            }
        }
    }
}
