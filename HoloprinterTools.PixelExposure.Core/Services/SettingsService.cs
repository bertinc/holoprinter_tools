using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using HoloprinterTools.PixelExposure.Core.Models;

namespace HoloprinterTools.PixelExposure.Core.Services
{
    public class SettingsService
    {
        private readonly string _path;

        public SettingsService(string path)
        {
            _path = path;
        }

        public async Task<AppSettings> LoadAsync()
        {
            if (!File.Exists(_path))
            {
                return new AppSettings();
            }

            var json = await File.ReadAllTextAsync(_path).ConfigureAwait(false);
            try
            {
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var settings = JsonSerializer.Deserialize<AppSettings>(json, opts);
                return settings ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }

        public async Task SaveAsync(AppSettings settings)
        {
            var dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_path, json).ConfigureAwait(false);
        }
    }
}
