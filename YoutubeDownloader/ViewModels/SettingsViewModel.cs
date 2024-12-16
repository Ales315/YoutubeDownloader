using System.ComponentModel;
using System.Windows.Input;
using YoutubeDownloader.Enums;
using YoutubeDownloader.Services;

namespace YoutubeDownloader.ViewModels
{
    class SettingsViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public DownloadMediaType MediaTypePreference
        {
            get { return ServiceProvider.SettingsService.UserPreferences.MediaTypePreference; }
            set
            {
                ServiceProvider.SettingsService.UserPreferences.MediaTypePreference = value;
                OnPropertyChanged(nameof(MediaTypePreference));
            }
        }
        public DownloadFormat AudioFormatPreference
        {
            get { return ServiceProvider.SettingsService.UserPreferences.AudioFormatPreference; }
            set
            {
                ServiceProvider.SettingsService.UserPreferences.AudioFormatPreference = value;
                OnPropertyChanged(nameof(AudioFormatPreference));
            }
        }
        public DownloadFormat VideoFormatPreference
        {
            get { return ServiceProvider.SettingsService.UserPreferences.VideoFormatPreference; }
            set
            {
                ServiceProvider.SettingsService.UserPreferences.VideoFormatPreference = value;
                OnPropertyChanged(nameof(VideoFormatPreference));
            }
        }
        public ThemeStyles ThemePreference
        {
            get { return ServiceProvider.SettingsService.UserPreferences.ThemePreference; }
            set
            {
                ServiceProvider.SettingsService.UserPreferences.ThemePreference = value;
                OnPropertyChanged(nameof(ThemePreference));
            }
        }
        public string OutputPath
        {
            get { return ServiceProvider.SettingsService.UserPreferences.OutputPath; }
            set
            {
                ServiceProvider.SettingsService.UserPreferences.OutputPath = value;
                OnPropertyChanged(nameof(OutputPath));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public SettingsViewModel()
        {
            PropertyChanged += (s, e) => SaveSettings();
        }

        private void SaveSettings()
        {
            ServiceProvider.SettingsService.Save();
        }
    }
}
