using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HoloprinterTools.PixelExposure.UI.ViewModels;

namespace HoloprinterTools.PixelExposure.UI
{
    public partial class PrinterSettingsWindow : Window
    {
        public PrinterSettingsWindow()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public PrinterSettingsWindow(PrinterSettingsViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(true);
        }

        private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(false);
        }
    }
}
