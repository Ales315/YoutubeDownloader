using System.ComponentModel;
using System.Windows.Input;
using YoutubeDownloader.Enums;
using YoutubeDownloader.Services;

namespace YoutubeDownloader.ViewModels
{
    class SettingsViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private DownloadOption _downloadOptionPreference;
        private DownloadFormat _audioFormatPreference;
        private DownloadFormat _videoFormatPreference;

        public DownloadOption DownloadOptionPreference
        {
            get { return ServiceProvider.SettingsService.UserPreferences.DownloadOptionPreference; }
            set
            {
                ServiceProvider.SettingsService.UserPreferences.DownloadOptionPreference = value;
                OnPropertyChanged(nameof(DownloadOptionPreference));
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
