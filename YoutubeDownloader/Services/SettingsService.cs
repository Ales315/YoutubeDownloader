using System.IO;
using System.Windows;
using Newtonsoft.Json;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.Services;

public class SettingsService
{
    private Settings _settings = null!;
    private string _settingsFilename { get; set; } = "settings.json";

    public SettingsService()
    {
        Load();
    }

    private void Load()
    {
        if (File.Exists("settings.json"))
        {
            try
            {
                string json = File.ReadAllText(_settingsFilename);
                _settings = JsonConvert.DeserializeObject<Settings>(json) ?? new Settings();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load user preferences.", ex.Message);
                _settings = new Settings();
            }
        }
    }

    public void Save()
    {
        try
        {
            string json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
            File.WriteAllText(_settingsFilename, json);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Failed to save user preferences.", ex.Message);
        }

    }
}
