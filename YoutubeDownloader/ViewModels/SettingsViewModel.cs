using System.ComponentModel;
using YoutubeDownloader.Services;

namespace YoutubeDownloader.ViewModels
{
    class SettingsViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private SettingsService _settingsService;
        public event PropertyChangedEventHandler? PropertyChanged;
        public SettingsViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }
    }
}
