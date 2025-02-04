﻿using System.IO;
using System.Windows;
using Newtonsoft.Json;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.Services;

public class SettingsService
{
    public Settings UserPreferences = null!;
    private string _settingsFilename { get; set; } = "settings.json";

    public event EventHandler? SettingsChanged;

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
                UserPreferences = JsonConvert.DeserializeObject<Settings>(json) ?? new Settings();
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load user preferences.", ex.Message);
            }
        }
        UserPreferences = new Settings();
    }

    public void Save()
    {
        try
        {
            string json = JsonConvert.SerializeObject(UserPreferences, Formatting.Indented);
            File.WriteAllText(_settingsFilename, json);
            SettingsChanged?.Invoke(null, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Failed to save user preferences.", ex.Message);
        }
    }

    public string GetOutputPath()
    {
        return UserPreferences.OutputPath;
    }
}
