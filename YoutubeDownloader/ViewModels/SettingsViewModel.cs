using System.ComponentModel;
using YoutubeDownloader.Services;

namespace YoutubeDownloader.ViewModels
{
    class SettingsViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public SettingsViewModel()
        {

        }
    }
}
