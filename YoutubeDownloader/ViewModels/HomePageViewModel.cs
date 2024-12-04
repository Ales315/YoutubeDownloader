using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YoutubeDownloader.Enums;
using YoutubeDownloader.Helpers;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;
using YoutubeExplode.Videos.Streams;

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
        private ICommand _getVideoData = null!;
        private ICommand _downloadVideo = null!;
        private double _progress;
        private long _viewCount;
        private string _date;
        private IEnumerable<VideoOnlyStreamInfo> _videoStreams;
        private IEnumerable<AudioOnlyStreamInfo> _audioStreams;
        private VideoOnlyStreamInfo _videoStreamSelected;
        private AudioOnlyStreamInfo _audioStreamSelected;

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
        public string Date
        {
            get => _date;
            set
            {
                if (_date == value) return;
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        public long ViewCount
        {
            get => _viewCount;
            set
            {
                if (_viewCount == value) return;
                _viewCount = value;
                OnPropertyChanged(nameof(ViewCount));
            }
        }
        public double Progress
        {
            get => _progress;
            set
            {
                if (_progress == value) return;
                _progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }
        public IEnumerable<VideoOnlyStreamInfo> VideoStreams
        {
            get => _videoStreams;
            set
            {
                if (_videoStreams == value) return;
                _videoStreams = value;
                OnPropertyChanged(nameof(VideoStreams));
            }
        }
        public IEnumerable<AudioOnlyStreamInfo> AudioStreams
        {
            get => _audioStreams;
            set
            {
                if (_audioStreams == value) return;
                _audioStreams = value;
                OnPropertyChanged(nameof(AudioStreams));
            }
        }
        public VideoOnlyStreamInfo VideoStreamSelected
        {
            get => _videoStreamSelected;
            set
            {
                if (_videoStreamSelected == value) return;
                _videoStreamSelected = value;
                OnPropertyChanged(nameof(VideoStreamSelected));
            }
        }
        public AudioOnlyStreamInfo AudioStreamSelected
        {
            get => _audioStreamSelected;
            set
            {
                if (_audioStreamSelected == value) return;
                _audioStreamSelected = value;
                OnPropertyChanged(nameof(AudioStreamSelected));
            }
        }

        public ICommand GetVideoData
        {
            get
            {
                return _getVideoData ?? (_getVideoData = new RelayCommandAsync(GetVideoMetadata, (c) => true));
            }
        }
        public ICommand DownloadVideo
        {
            get
            {
                return _downloadVideo ?? (_downloadVideo = new RelayCommandAsync(Download, (c) => true));
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
        public async Task GetVideoMetadata()
        {
            if(!CanProcessRequest()) return;
            try
            {
                StateHandler.SetUI(AppState.AnalyzingUrl);

                var videoData = await _ytService.GetVideoAsync(Url);
                UpdateVideoData(videoData);
                StateHandler.SetUI(AppState.VideoFound);

                var streamData = await _ytService.GetStreamData(videoData);
                UpdateVideoStreamData(streamData);
                StateHandler.SetUI(AppState.VideoStreamsFound);

                if (videoData.ErrorMessage != string.Empty)
                    throw new Exception(videoData.ErrorMessage);
                
            }
            catch (Exception)
            {
                StateHandler.SetUI(AppState.VideoNotFound);
            }
        }

        

        private bool CanProcessRequest()
        {
            if(StateHandler.IsAnalizing || StateHandler.IsDownloading)
                return false;
            if (string.IsNullOrWhiteSpace(Url)) 
                return false;
            if (Url == _previousValidUrl && StateHandler.IsVideoFound) 
                return false;
            return true;
        }

        private void UpdateVideoData(VideoDataModel videoData)
        {
            LoadThumbnail(videoData.ThumbnailUrl);
            Title = videoData.Title;
            ChannelName = videoData.ChannelName;
            Duration = videoData.Duration;
            ViewCount = videoData.ViewCount;
            Date = videoData.Date;
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
        private void UpdateVideoStreamData(VideoDataModel streamData)
        {
            AudioStreams = streamData.AudioStreams;
            VideoStreams = streamData.VideoStreams;
            VideoStreamSelected = VideoStreams.Last();
            AudioStreamSelected = AudioStreams.Last();
        }

        //Download video
        public async Task Download()
        {
            try
            {
                var progress = new Progress<double>(p=> Progress = p);
                await _ytService.DownloadVideo(_url, progress, VideoStreamSelected, AudioStreamSelected);
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
        private bool _isVideoStreamsFound;
        private bool _isAnalizingStreams;

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
        public bool IsVideoStreamsFound
        {
            get => _isVideoStreamsFound;
            set
            {
                if (_isVideoStreamsFound == value) return;
                _isVideoStreamsFound = value;
                OnPropertyChanged(nameof(IsVideoStreamsFound));
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
        public bool IsAnalizingStreams
        {
            get => _isAnalizingStreams;
            set
            {
                if (_isAnalizingStreams == value) return;
                _isAnalizingStreams = value;
                OnPropertyChanged(nameof(IsAnalizingStreams));
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
            IsAnalizingStreams = false;
            IsVideoFound = false;
            IsVideoStreamsFound = false;
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
                    IsAnalizingStreams = true;
                    break;

                case AppState.AnalyzingStreams:
                    IsVideoFound = true;
                    IsAnalizingStreams = true;
                    break;

                case AppState.VideoFound:
                    IsVideoFound = true;
                    IsAnalizingStreams = true;
                    break;

                case AppState.VideoStreamsFound:
                    IsVideoStreamsFound = true;
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
