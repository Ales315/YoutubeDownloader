using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YoutubeDownloader.Enums;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;

namespace YoutubeDownloader.ViewModels
{
    class HomePageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region FIELDS
        
        private string _url = string.Empty;
        private string _title = string.Empty;
        private string _channelName = string.Empty;
        private string _duration = string.Empty;
        private ImageSource _thumbnail = null!;

        private YoutubeService _ytService = new YoutubeService();
        private string _previousValidUrl = string.Empty;    

        public ControlStateHandler StateHandler { get; set; }
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
            StateHandler = new ControlStateHandler()
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
        //Get video metadata
        //todo: binding metodo al bottone e Url al textbox
        public async Task GetVideoMetadata()
        {
            if(!CanProcessRequest()) return;
            try
            {
                StateHandler.SetUI(AppState.AnalyzingUrl);

                var videoData = await _ytService.GetInfo(Url);
                if (videoData.ErrorMessage != string.Empty)
                    throw new Exception(videoData.ErrorMessage);

                UpdateVideoData(videoData);

                StateHandler.SetUI(AppState.VideoFound);
            }
            catch (Exception)
            {
                StateHandler.SetUI(AppState.VideoNotFound);
            }
        }

        private bool CanProcessRequest()
        {
            if ((Url == _previousValidUrl && StateHandler.IsVideoFound) || StateHandler.IsAnalizing || StateHandler.IsDownloading) 
                return false;
            return true;
        }

        private void UpdateVideoData(VideoMetadataModel videoData)
        {
            LoadThumbnail(videoData.ThumbnailUrl);
            Title = videoData.Title;
            ChannelName = videoData.ChannelName;
            Duration = videoData.Duration;
            _previousValidUrl = Url;
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

        //Download video
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

    public class ControlStateHandler : INotifyPropertyChanged
    {
        #region FIELDS
        private bool _isVideoFound;
        private bool _isAnalizing;
        private bool _isDownloading;
        private bool _isDownloaded;
        private bool _isAnalizingError;
        private bool _isFirstOpening;

        public event PropertyChangedEventHandler? PropertyChanged;
        public bool IsFirstOpening 
        { 
            get => _isFirstOpening; 
            set 
            { 
                if (_isFirstOpening == value) return;
                _isFirstOpening = value; 
                OnPropertyChanged(nameof(IsFirstOpening)); 
            } 
        }
        public bool IsVideoFound 
        { 
            get => _isVideoFound; 
            set 
            { 
                if (_isVideoFound == value) return;
                _isVideoFound = value; 
                OnPropertyChanged(nameof(IsVideoFound)); 
            } 
        }
        public bool IsAnalizing 
        { 
            get => _isAnalizing; 
            set 
            { 
                if (_isAnalizing == value) return;
                _isAnalizing = value; 
                OnPropertyChanged(nameof(IsAnalizing)); 
            } 
        }
        public bool IsAnalizingError 
        { 
            get => _isAnalizingError; 
            set 
            { 
                if (_isAnalizingError == value) return;
                _isAnalizingError = value; 
                OnPropertyChanged(nameof(IsAnalizingError)); 
            } 
        }
        public bool IsDownloading 
        { 
            get => _isDownloading; 
            set 
            { 
                if (_isDownloading == value) return;
                _isDownloading = value; 
                OnPropertyChanged(nameof(IsDownloading)); 
            } 
        }
        public bool IsDownloaded 
        { 
            get => _isDownloaded; 
            set 
            { 
                if (_isDownloaded == value) return;
                _isDownloaded = value; 
                OnPropertyChanged(nameof(IsDownloaded)); 
            } 
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public void SetUI(AppState state)
        {
            IsFirstOpening = false;
            IsAnalizing = false;
            IsVideoFound = false;
            IsAnalizingError = false;
            IsDownloaded = false;
            IsDownloading = false;

            switch (state)
            {
                case AppState.FirstOpening:
                    IsFirstOpening = true;
                    break;

                case AppState.AnalyzingUrl:
                    IsAnalizing = true;
                    break;

                case AppState.VideoFound:
                    IsVideoFound = true;
                    break;

                case AppState.VideoNotFound:
                    IsAnalizingError = true;
                    break;

                case AppState.Downloading:
                    IsDownloading = true;
                    break;

                case AppState.DownloadCompleted:
                    IsDownloaded = true;
                    break;
            }
        }
    }
}
