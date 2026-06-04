using Avalonia.Controls;
using System.IO;
using HoloprinterTools.PixelExposure.UI.Services;
using HoloprinterTools.PixelExposure.UI.ViewModels;
using HoloprinterTools.PixelExposure.UI.Models;

namespace HoloprinterTools.PixelExposure.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Load settings from app folder (same folder as executable). If a shared settings.json
            // is dropped beside the app, it will be used.
            var exeFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? ".";
            var settingsPath = Path.Combine(exeFolder, "settings.json");

            var settingsService = new SettingsService(settingsPath);
            var settings = settingsService.LoadAsync().GetAwaiter().GetResult();

            DataContext = new MainWindowViewModel(settingsService, settings);
        }
    }
}