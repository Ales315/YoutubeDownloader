using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YoutubeDownloader.Services;

namespace YoutubeDownloader.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        private YoutubeService _ytService = new YoutubeService();
        private string _url = string.Empty;

        public Dictionary<string, string> AudioFormatOptions { get; } = new Dictionary<string, string>
        {
            { "MP3", "mp3" },
            { "WAV", "wav" },
            { "M4A", "m4a" },
            { "AAC", "aac" }
        };
        public Dictionary<string, int> AudioQualityOptions { get; } = new Dictionary<string, int>
        {
            { "Best", 0 },
            { "High", 1 },
            { "Medium", 2 },
            { "Low", 3 },
            { "Worst", 5 }
        };
        public KeyValuePair<string, string> SelectedAudioFormat { get; set; }
        public KeyValuePair<string, int> SelectedAudioQuality { get; set; }
        public ImageSource Thumbnail { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;

        public UIState UIState { get; set; }
        

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel()
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

        public async void GetVideoMetadata(string url)
        {
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
