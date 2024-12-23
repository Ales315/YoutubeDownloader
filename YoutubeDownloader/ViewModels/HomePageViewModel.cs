﻿using System.Collections.ObjectModel;
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
    class HomePageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private string _previousValidUrl = string.Empty;
        public ControlStateHandler StateHandler { get => ServiceProvider.StateHandler; }

        #region FIELDS
        //Video stats
        private string _url = string.Empty;
        private string _title = string.Empty;
        private string _channelName = string.Empty;
        private string _duration = string.Empty;
        private string _date = string.Empty;
        private string _downloadSize = string.Empty;
        private long _viewCount = 0;
        private ImageSource _thumbnail = null!;

        //Download
        private VideoOnlyStreamInfo _videoStreamSelected = null!;
        private AudioOnlyStreamInfo _audioStreamSelected = null!;
        private IEnumerable<VideoOnlyStreamInfo> _videoStreams = null!;
        private IEnumerable<AudioOnlyStreamInfo> _audioStreams = null!;
        private DownloadMediaType _downloadOptionSelected;
        private DownloadFormat _formatSelected;

        private ICommand _getVideoData = null!;
        private ICommand _downloadVideo = null!;
        private ICommand _goHomeCommand = null!;
        private double _progress;
        private string _errorMessage = string.Empty;

        public ObservableCollection<VideoDownloadViewModel> VideoDownloadsList
        {
            get => ServiceProvider.YoutubeService.DownloadList;
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
                return _downloadVideo ?? new RelayCommand(param => this.Download());
            }
        }
        public ICommand GoHomeCommand
        {
            get
            {
                if (_goHomeCommand == null)
                {
                    _goHomeCommand = new RelayCommand(
                        param => this.GoHome()
                    );
                }
                return _goHomeCommand;
            }
        }

        private void GoHome()
        {
            ServiceProvider.YoutubeService.GetMetadataCancellationToken.Cancel();
            StateHandler.SetUI(AppState.Home);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public HomePageViewModel()
        {
            DownloadOptionSelected = ServiceProvider.SettingsService.UserPreferences.MediaTypePreference;
            FormatSelected = DownloadOptionSelected == DownloadMediaType.AudioOnly ?
                ServiceProvider.SettingsService.UserPreferences.AudioFormatPreference : ServiceProvider.SettingsService.UserPreferences.VideoFormatPreference;

            VideoDownloadsList.CollectionChanged += (s, e) =>
            {
                StateHandler.IsDownloadListEmpty = VideoDownloadsList.Count == 0;
            };
            if (StateHandler.IsVideoFound)
                LoadLastVideoData();
            Url = "https://www.youtube.com/watch?v=HQmmM_qwG4k";
        }

        private void LoadLastVideoData()
        {
            var data = ServiceProvider.YoutubeService.GetLastVideoData();
            if (data == null)
                return;
            UpdateVideoData(data);
            if (UpdateVideoStreamData(data) == false)
                StateHandler.SetUI(AppState.Home);
        }

        #region GET METADATA
        //Get video metadata
        public async Task GetVideoMetadata()
        {
            if (!CanProcessRequest()) return;
            try
            {
                StateHandler.IsAutoDownloadError = false;
                if (ServiceProvider.SettingsService.UserPreferences.AutoDownload)
                {
                    StateHandler.IsAnalyzingAutoDownload = true;
                    var videoData = await ServiceProvider.YoutubeService.GetVideoAsync(Url);
                    var videoAndStreamData = await ServiceProvider.YoutubeService.GetStreamData(videoData);
                    StateHandler.IsAnalyzingAutoDownload = false;

                    Download(videoAndStreamData);
                }
                else
                {
                    StateHandler.SetUI(AppState.AnalyzingUrl);

                    var videoData = await ServiceProvider.YoutubeService.GetVideoAsync(Url);
                    UpdateVideoData(videoData);
                    StateHandler.SetUI(AppState.VideoFound);

                    var streamData = await ServiceProvider.YoutubeService.GetStreamData(videoData);
                    UpdateVideoStreamData(streamData);
                    StateHandler.SetUI(AppState.VideoStreamsFound);

                    if (videoData.ErrorMessage != string.Empty)
                        throw new Exception(videoData.ErrorMessage);
                }
            }
            catch (OperationCanceledException ex)
            {
                ErrorMessage = ex.Message;
                StateHandler.IsAnalyzingAutoDownload = false;
                StateHandler.IsAutoDownloadError = true;
                StateHandler.SetUI(AppState.Home);
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
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                StateHandler.IsAnalyzingAutoDownload = false;
                if (!ServiceProvider.SettingsService.UserPreferences.AutoDownload)
                    StateHandler.SetUI(AppState.VideoNotFound);
                else
                    StateHandler.IsAutoDownloadError = true;
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

        }



        private bool CanProcessRequest()
        {
            if (StateHandler.IsAnalizing)
                return false;
            if (string.IsNullOrWhiteSpace(Url))
                return false;
            if (Url == _previousValidUrl && StateHandler.IsVideoFound)
                return false;
            return true;
        }
        private void UpdateVideoData(VideoDataModel videoData)
        {
            Thumbnail = videoData.Thumbnail;
            Title = videoData.Title;
            ChannelName = videoData.ChannelName;
            Duration = videoData.Duration;
            ViewCount = videoData.ViewCount;
            Date = videoData.Date;
            FormatSelected = ServiceProvider.SettingsService.UserPreferences.MediaTypePreference == DownloadMediaType.AudioOnly ?
                ServiceProvider.SettingsService.UserPreferences.AudioFormatPreference : ServiceProvider.SettingsService.UserPreferences.VideoFormatPreference;
            _previousValidUrl = Url;
        }
        private bool UpdateVideoStreamData(VideoDataModel streamData)
        {
            AudioStreams = streamData.AudioStreams;
            VideoStreams = streamData.VideoStreams;

            if (AudioStreams.Count() == 0 || AudioStreams.Count() == 0)
                return false;

            VideoStreamSelected = VideoStreams.Last();
            AudioStreamSelected = AudioStreams.Last();
            return true;
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

            //todo: calcolo in base al formato
            //if(format == wav)
            //{
            //    long totalSeconds = ((long)TimeSpan.Parse(Duration).TotalSeconds);
            //    totalSizeBytes = totalSeconds * 44100 * 16bitdepth * 2channels / 8;
            //}


            DownloadSize = $"{new FileSize(totalSizeBytes)}";
        }

        #endregion

        #region DOWNLOAD
        //Download video
        public void Download()
        {
            try
            {
                VideoDownloadViewModel newVideoDownload = new VideoDownloadViewModel();
                newVideoDownload.Title = Title;
                newVideoDownload.Thumbnail = Thumbnail;
                newVideoDownload.Duration = Duration;
                newVideoDownload.AudioStream = AudioStreamSelected;
                newVideoDownload.VideoStream = VideoStreamSelected;
                newVideoDownload.DownloadOption = DownloadOptionSelected;
                newVideoDownload.DownloadFormat = FormatSelected;
                ServiceProvider.YoutubeService.EnqueueDownload(newVideoDownload);
                StateHandler.SetUI(AppState.Downloading);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Download(VideoDataModel videoData)
        {
            try
            {
                VideoDownloadViewModel newVideoDownload = new VideoDownloadViewModel();
                newVideoDownload.Title = videoData.Title;
                newVideoDownload.Thumbnail = videoData.Thumbnail;
                newVideoDownload.Duration = videoData.Duration;
                newVideoDownload.AudioStream = videoData.AudioStreams.Last();
                newVideoDownload.VideoStream = videoData.VideoStreams.Last();
                newVideoDownload.DownloadOption = ServiceProvider.SettingsService.UserPreferences.MediaTypePreference;
                newVideoDownload.DownloadFormat = newVideoDownload.DownloadOption == DownloadMediaType.AudioOnly ?
                    ServiceProvider.SettingsService.UserPreferences.AudioFormatPreference : ServiceProvider.SettingsService.UserPreferences.VideoFormatPreference;
                ServiceProvider.YoutubeService.EnqueueDownload(newVideoDownload);
                StateHandler.SetUI(AppState.Downloading);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        public void Invalidate()
        {
            DownloadOptionSelected = ServiceProvider.SettingsService.UserPreferences.MediaTypePreference;
            FormatSelected = DownloadOptionSelected == DownloadMediaType.AudioOnly ?
                ServiceProvider.SettingsService.UserPreferences.AudioFormatPreference : ServiceProvider.SettingsService.UserPreferences.VideoFormatPreference;

            if (StateHandler.IsVideoFound)
                LoadLastVideoData();

            CommandManager.InvalidateRequerySuggested();
        }
    }

    public class ControlStateHandler : INotifyPropertyChanged
    {
        #region FIELDS
        private bool _isVideoFound;
        private bool _isAnalizing;
        private bool _isDownloading;
        private bool _isDownloaded;
        private bool _isAnalizingError;
        private bool _isDownloadListEmpty;
        private bool _isVideoStreamsFound;
        private bool _isAnalizingStreams;
        private bool _isSearchOpen;
        private AppState _currentState;
        private bool _isAnalyzingAutoDownload;
        private bool _isAutoDownloadError;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<AppState>? CurrentStateChanged;
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
        public bool IsSearchOpen
        {
            get => _isSearchOpen;
            set
            {
                if (_isSearchOpen == value) return;
                _isSearchOpen = value;
                OnPropertyChanged(nameof(IsSearchOpen));
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
        public bool IsAnalyzingAutoDownload
        {
            get => _isAnalyzingAutoDownload;
            set
            {
                if (_isAnalyzingAutoDownload == value) return;
                _isAnalyzingAutoDownload = value;
                OnPropertyChanged(nameof(IsAnalyzingAutoDownload));
            }
        }
        public bool IsAutoDownloadError
        {
            get => _isAutoDownloadError;
            set
            {
                if (_isAutoDownloadError == value) return;
                _isAutoDownloadError = value;
                OnPropertyChanged(nameof(IsAutoDownloadError));
            }
        }

        public AppState CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState == value) return;
                _currentState = value;
                CurrentStateChanged?.Invoke(null, value);
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public ControlStateHandler()
        {
            IsSearchOpen = false;
            IsAnalizing = false;
            IsAnalizingStreams = false;
            IsVideoFound = false;
            IsVideoStreamsFound = false;
            IsAnalizingError = false;
            IsDownloaded = false;
            IsDownloading = false;
            IsDownloadListEmpty = true;
        }

        public void SetUI(AppState state)
        {
            IsSearchOpen = false;
            IsAnalizing = false;
            IsAnalizingStreams = false;
            IsVideoFound = false;
            IsVideoStreamsFound = false;
            IsAnalizingError = false;
            IsDownloaded = false;
            IsDownloading = false;

            switch (state)
            {
                case AppState.AnalyzingUrl:
                    IsSearchOpen = true;
                    IsAnalizing = true;
                    IsAnalizingStreams = true;
                    break;

                case AppState.AnalyzingStreams:
                    IsSearchOpen = true;
                    IsVideoFound = true;
                    IsAnalizingStreams = true;
                    break;

                case AppState.VideoFound:
                    IsSearchOpen = true;
                    IsVideoFound = true;
                    IsAnalizingStreams = true;
                    break;

                case AppState.VideoStreamsFound:
                    IsSearchOpen = true;
                    IsVideoStreamsFound = true;
                    IsVideoFound = true;
                    break;

                case AppState.VideoNotFound:
                    IsSearchOpen = true;
                    IsAnalizingError = true;
                    break;

                case AppState.Downloading:
                    IsDownloading = true;
                    break;

                case AppState.DownloadCompleted:
                    IsDownloaded = true;
                    break;
            }
            CurrentState = state;
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
