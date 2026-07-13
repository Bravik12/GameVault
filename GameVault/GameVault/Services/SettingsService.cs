using System.IO;
using System.Text.Json;
using GameVault.Models;

namespace GameVault.Services
{
    public class SettingsService
    {
        private readonly string filePath;


        public SettingsService()
        {
            var folder =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData);

            var appFolder =
                Path.Combine(folder, "GameVault");

            Directory.CreateDirectory(appFolder);

            filePath = Path.Combine(
                appFolder,
                "settings.json");
        }


        public void SaveSettings(AppSettings settings)
        {
            var json = JsonSerializer.Serialize(
                settings,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(filePath, json);
        }


        public AppSettings LoadSettings()
        {
            if (!File.Exists(filePath))
            {
                return new AppSettings();
            }


            var json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<AppSettings>(json)
                   ?? new AppSettings();
        }
    }
}