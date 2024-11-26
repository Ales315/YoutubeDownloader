using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YoutubeDownloader.Services;

namespace YoutubeDownloader.ViewModels
{
    class HomePageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region FIELDS
        private YoutubeService _ytService = new YoutubeService();
        private string _url = string.Empty;
        private string _title = string.Empty;
        private string _channelName = string.Empty;
        private string _duration = string.Empty;
        private ImageSource _thumbnail = null!;
        public UIState UIState { get; set; }
        public string Url
        {
            get => _url;
            set
            {
                if (_url == value) return;
                _url = value;
                OnPropertyChanged(nameof(Url));
            }
        }
        public ImageSource Thumbnail
        {
            get => _thumbnail;
            set
            {
                if (_thumbnail == value) return;
                _thumbnail = value;
                OnPropertyChanged(nameof(Thumbnail));
            }
        }
        public string Title
        {
            get => _title;
            set
            {
                if (_title == value) return;
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        public string ChannelName
        {
            get => _channelName;
            set
            {
                if (_channelName == value) return;
                _channelName = value;
                OnPropertyChanged(nameof(ChannelName));
            }
        }
        public string Duration
        {
            get => _duration;
            set
            {
                if (_duration == value) return;
                _duration = value;
                OnPropertyChanged(nameof(Duration));
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public HomePageViewModel()
        {
            UIState = new UIState()
            {
                IsVideoFound = false,
                IsAnalizing = false,
                IsDownloaded = false,
                IsDownloading = false,
                IsAnalizingError = false,
                IsFirstOpening = true
            };
        }

        #region METHODS
        public async Task GetVideoMetadata(string url)
        {
#warning TODO: Bindare Url a textbox e metodo al tasto cerca
            if ((url == _url && UIState.IsVideoFound) || UIState.IsAnalizing || UIState.IsDownloading) return;
            try
            {
                UIState.IsFirstOpening = false;
                UIState.IsAnalizingError = false;
                UIState.IsDownloaded = false;
                UIState.IsAnalizing = true;
                var videoData = await _ytService.GetInfo(url);
                LoadThumbnail(videoData.ThumbnailUrl);
                Title = videoData.Title;
                ChannelName = videoData.ChannelName;
                Duration = videoData.Duration;
                _url = url;

                UIState.IsAnalizing = false;
                UIState.IsVideoFound = true;
            }
            catch (Exception)
            {
                UIState.IsAnalizingError = true;
                UIState.IsAnalizing = false;
                UIState.IsVideoFound = false;
                //todo: Gestione errore video non trovato
            }
            OnPropertyChanged(string.Empty);
        }

        private void LoadThumbnail(string thumbnailUrl)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(thumbnailUrl, uriKind: UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            Thumbnail = bitmap;
        }

        public async void Download()
        {
            try
            {
                await _ytService.DownloadVideo(_url);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }

    public class UIState : INotifyPropertyChanged
    {
        private bool _isVideoFound;
        private bool _isAnalizing;
        private bool _isDownloading;
        private bool _isDownloaded;
        private bool _isAnalizingError;
        private bool _isFirstOpening;

        public event PropertyChangedEventHandler? PropertyChanged;
        public bool IsFirstOpening { get => _isFirstOpening; set { _isFirstOpening = value; OnPropertyChanged(nameof(IsFirstOpening)); } }
        public bool IsVideoFound { get => _isVideoFound; set { _isVideoFound = value; OnPropertyChanged(nameof(IsVideoFound)); } }
        public bool IsAnalizing { get => _isAnalizing; set { _isAnalizing = value; OnPropertyChanged(nameof(IsAnalizing)); } }
        public bool IsAnalizingError { get => _isAnalizingError; set { _isAnalizingError = value; OnPropertyChanged(nameof(IsAnalizingError)); } }
        public bool IsDownloading { get => _isDownloading; set { _isDownloading = value; OnPropertyChanged(nameof(IsDownloading)); } }
        public bool IsDownloaded { get => _isDownloaded; set { _isDownloaded = value; OnPropertyChanged(nameof(IsDownloaded)); } }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
