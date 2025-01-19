using System.Collections.ObjectModel;
using System.ComponentModel;
using YoutubeDownloader.Services;
using YoutubeDownloader.ViewModels.Card;

namespace YoutubeDownloader.ViewModels.UserControl
{
    class DownloadListViewModel : ViewModelBase, INotifyPropertyChanged
    {

        #region FIELDS

        private bool _isDownloadListEmpty = false;

        public bool IsDownloadListEmpty
        {
            get => _isDownloadListEmpty;
            set
            {
                if (_isDownloadListEmpty == value) return;
                _isDownloadListEmpty = value;
                OnPropertyChanged(nameof(IsDownloadListEmpty));
            }
        }
        public ObservableCollection<VideoDownloadCardViewModel> VideoDownloadViewModels
        {
            get => ServiceProvider.YoutubeService.DownloadList;
        }

        #endregion


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DownloadListViewModel()
        {
            IsDownloadListEmpty = VideoDownloadViewModels.Count == 0;
            VideoDownloadViewModels.CollectionChanged += (s, e) =>
            {
                IsDownloadListEmpty = VideoDownloadViewModels.Count == 0;
            };
        }
    }
}
