using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using YoutubeDownloader.Enums;
using YoutubeDownloader.Helpers;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.ViewModels
{
    class VideoViewModel : ViewModelBase, INotifyPropertyChanged
    {
        
        #region FIELDS

        //Ui state
        private bool _isSearchError = false;
        private bool _isLoadingStreams = false;
        private bool _isMetadataFound = false;
        private bool _isStreamsFound = false;


        //Video data & stats
        private string _errorMessage = string.Empty;
        private string _title = string.Empty;
        private string _channelName = string.Empty;
        private string _duration = string.Empty;
        private string _date = string.Empty;
        private string _downloadSize = string.Empty;
        private long _viewCount;
        private ImageSource _thumbnail = null!;

        //Download data
        private VideoOnlyStreamInfo _videoStreamSelected = null!;
        private AudioOnlyStreamInfo _audioStreamSelected = null!;
        private IEnumerable<VideoOnlyStreamInfo> _videoStreams = null!;
        private IEnumerable<AudioOnlyStreamInfo> _audioStreams = null!;
        private DownloadMediaType _downloadOptionSelected;
        private DownloadFormat _formatSelected;

        //Commands
        private ICommand? _downloadVideo;

        //Video data & stats
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
        public string DownloadSize
        {
            get => _downloadSize;
            set
            {
                if (_downloadSize == value) return;
                _downloadSize = value;
                OnPropertyChanged(nameof(DownloadSize));
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
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage == value) return;
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        //Download data
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
                CalculateDownloadSize();
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
                CalculateDownloadSize();
            }
        }
        public DownloadMediaType DownloadOptionSelected
        {
            get => _downloadOptionSelected;
            set
            {
                if (_downloadOptionSelected == value) return;
                _downloadOptionSelected = value;
                OnPropertyChanged(nameof(DownloadOptionSelected));
                CalculateDownloadSize();
            }
        }
        public DownloadFormat FormatSelected
        {
            get => _formatSelected;
            set
            {
                if (_formatSelected == value) return;
                _formatSelected = value;
                OnPropertyChanged(nameof(FormatSelected));
            }
        }
        //Commands
        public ICommand DownloadVideo
        {
            get
            {
                return _downloadVideo ?? new RelayCommand(param => this.Download());
            }
        }

        //Ui state
        public bool IsSearchError
        {
            get => _isSearchError;
            set
            {
                if (_isSearchError == value) return;
                _isSearchError = value;
                OnPropertyChanged(nameof(IsSearchError));
            }
        }
        public bool IsMetadataFound
        {
            get => _isMetadataFound;
            set
            {
                if (_isMetadataFound == value) return;
                _isMetadataFound = value;
                OnPropertyChanged(nameof(IsMetadataFound));
            }
        }
        public bool IsStreamsFound
        {
            get => _isStreamsFound;
            set
            {
                if (_isStreamsFound == value) return;
                _isStreamsFound = value;
                OnPropertyChanged(nameof(IsStreamsFound));
            }
        }


        #endregion

        private string _previousValidUrl = string.Empty;
        private UIState _currentState;
        public string Url { get; set; } = string.Empty;
        public event EventHandler<string>? LoadingError;
        public event EventHandler? DownloadStarted;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public VideoViewModel()
        {
            DownloadOptionSelected = ServiceProvider.SettingsService.UserPreferences.MediaTypePreference;
            FormatSelected = DownloadOptionSelected == DownloadMediaType.AudioOnly ?
                ServiceProvider.SettingsService.UserPreferences.AudioFormatPreference : ServiceProvider.SettingsService.UserPreferences.VideoFormatPreference;

            _currentState = UIState.Idle;
        }

        #region GET METADATA

        //Show video data and available streams

        //Used for loading videos using property Url
        public async Task GetMetadataFromUrl()
        {
            await GetVideoMetadata(Url);
            _previousValidUrl = string.Empty;
        }

        public async Task GetVideoMetadata(string url)
        {
            if(_currentState != UIState.Idle) 
                return;

            //if (url == _previousValidUrl && IsStreamsFound)
            //    if(LoadLastVideoData(url)) 
            //        return;
            ClearCurrentVideoData();
            SetUI(UIState.Loading);
            Video videoData = null!;
            try
            {
                videoData = await ServiceProvider.YoutubeService.GetVideoAsync(url);
                UpdateVideoData(videoData);

                SetUI(UIState.MetadataFound);
                var streamData = await ServiceProvider.YoutubeService.GetStreamData(videoData);
                UpdateVideoStreamData(streamData);

                SetUI(UIState.StreamsFound);
            }
            catch (OperationCanceledException)
            {
                ClearCurrentVideoData();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                SetUI(UIState.SearchError);
                LoadingError?.Invoke(this, ex.Message);
                ClearCurrentVideoData();
            }
            _currentState = UIState.Idle;
        }

        private void ClearCurrentVideoData()
        {
            Thumbnail = null!;
            Title = null!;
            ChannelName = null!;
            ViewCount = 0;
            Date = null!;
            AudioStreams = null!;
            VideoStreams = null!;
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void UpdateVideoData(Video videoData)
        {
            Thumbnail = videoData.Thumbnail;
            Title = videoData.Title;
            ChannelName = videoData.ChannelName;
            Duration = videoData.Duration;
            ViewCount = videoData.ViewCount;
            Date = videoData.Date;
            FormatSelected = ServiceProvider.SettingsService.UserPreferences.MediaTypePreference == DownloadMediaType.AudioOnly ?
                ServiceProvider.SettingsService.UserPreferences.AudioFormatPreference : ServiceProvider.SettingsService.UserPreferences.VideoFormatPreference;
            _previousValidUrl = videoData.Url;
        }
        private bool UpdateVideoStreamData(Video streamData)
        {
            AudioStreams = streamData.AudioStreams;
            VideoStreams = streamData.VideoStreams;

            if (AudioStreams.Count() == 0 || AudioStreams.Count() == 0)
                return false;

            //select best quality by default
            VideoStreamSelected = VideoStreams.Last();
            AudioStreamSelected = AudioStreams.Last();
            return true;
        }
        #endregion

        #region DOWNLOAD
        public void Download()
        {
            try
            {
                VideoDownloadViewModel newVideoDownloadVM = new VideoDownloadViewModel();
                newVideoDownloadVM.Title = Title;
                newVideoDownloadVM.Thumbnail = Thumbnail;
                newVideoDownloadVM.Duration = Duration;
                newVideoDownloadVM.AudioStream = AudioStreamSelected;
                newVideoDownloadVM.VideoStream = VideoStreamSelected;
                newVideoDownloadVM.DownloadOption = DownloadOptionSelected;
                newVideoDownloadVM.DownloadFormat = FormatSelected;
                ServiceProvider.YoutubeService.EnqueueDownload(newVideoDownloadVM);
                DownloadStarted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region UTILS
        private void SetUI(UIState state)
        {
            switch (state)
            {
                case UIState.MetadataFound:
                    IsMetadataFound = true;
                    break;

                case UIState.StreamsFound:
                    IsStreamsFound = true;
                    break;

                case UIState.Loading:
                    IsSearchError = false;
                    IsStreamsFound = false;
                    IsMetadataFound = false;
                    break;

                case UIState.SearchError:
                    IsStreamsFound = false;
                    IsMetadataFound = false;
                    IsSearchError = true;
                    break;
            }
            _currentState = state;
            CommandManager.InvalidateRequerySuggested();
        }

        private void CalculateDownloadSize()
        {
            var videoSizeBytes = VideoStreamSelected == null ? 0 : VideoStreamSelected.Size.Bytes;
            var audioSizeBytes = AudioStreamSelected == null ? 0 : AudioStreamSelected.Size.Bytes;
            var totalSizeBytes = videoSizeBytes + audioSizeBytes;

            if (DownloadOptionSelected == DownloadMediaType.VideoOnly)
                totalSizeBytes -= audioSizeBytes;
            else if (DownloadOptionSelected == DownloadMediaType.AudioOnly)
                totalSizeBytes -= videoSizeBytes;

            //todo: calculate size based on format selection
            //if(format == wav)
            //{
            //    long totalSeconds = ((long)TimeSpan.Parse(Duration).TotalSeconds);
            //    totalSizeBytes = totalSeconds * 44100 * 16bitdepth * 2channels / 8;
            //}

            DownloadSize = $"{new FileSize(totalSizeBytes)}";
        }

        private bool LoadLastVideoData(string url)
        {
            var data = ServiceProvider.YoutubeService.GetLastVideoData();
            if (data == null)
                return false;
            UpdateVideoData(data);
            if (UpdateVideoStreamData(data) == false)
                return false;
            return true;
        }

        private enum UIState
        {
            Idle,
            Loading,
            MetadataFound,
            StreamsFound,
            SearchError
        }

        public void Invalidate()
        {
            DownloadOptionSelected = ServiceProvider.SettingsService.UserPreferences.MediaTypePreference;
            FormatSelected = DownloadOptionSelected == DownloadMediaType.AudioOnly ?
                ServiceProvider.SettingsService.UserPreferences.AudioFormatPreference : ServiceProvider.SettingsService.UserPreferences.VideoFormatPreference;

            if (IsStreamsFound)
                LoadLastVideoData(_previousValidUrl);

            CommandManager.InvalidateRequerySuggested();
        }
        #endregion
    }
}
