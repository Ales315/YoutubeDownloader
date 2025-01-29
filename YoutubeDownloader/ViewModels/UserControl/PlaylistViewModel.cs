using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using YoutubeDownloader.Enums;
using YoutubeDownloader.Helpers;
using YoutubeDownloader.Services;
using YoutubeDownloader.ViewModels.Card;

namespace YoutubeDownloader.ViewModels.UserControl
{
    class PlaylistViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private Task _currentGetPlaylistTask = Task.CompletedTask;

        #region FIELDS

        private DownloadMediaType _downloadOptionSelected;
        private DownloadFormat _formatSelected;

        private UIState _currentState;
        private bool _isVideoListLoaded;
        private bool _isLoadingError;
        private ObservableCollection<PlaylistVideoCardViewModel> _playlistVideoViewModels = null!;

        public bool IsVideoListLoaded
        {
            get => _isVideoListLoaded;
            set
            {
                if (_isVideoListLoaded == value) return;
                _isVideoListLoaded = value;
                OnPropertyChanged(nameof(IsVideoListLoaded));
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

        public bool IsLoadingError
        {
            get => _isLoadingError;
            set
            {
                if (_isLoadingError == value) return;
                _isLoadingError = value;
                OnPropertyChanged(nameof(IsLoadingError));
            }
        }
        public ObservableCollection<PlaylistVideoCardViewModel> PlaylistVideoViewModels
        {
            get => _playlistVideoViewModels;
            set
            {
                _playlistVideoViewModels = value;
                OnPropertyChanged(nameof(PlaylistVideoViewModels));
            }
        }

        #endregion


        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public PlaylistViewModel()
        {
            DownloadOptionSelected = ServiceProvider.SettingsService.UserPreferences.MediaTypePreference;
            FormatSelected = DownloadOptionSelected == DownloadMediaType.AudioOnly ?
                ServiceProvider.SettingsService.UserPreferences.AudioFormatPreference : ServiceProvider.SettingsService.UserPreferences.VideoFormatPreference;

            _currentState = UIState.Idle;
        }


        public async Task GetPlaylistAsync(string url)
        {
            _currentGetPlaylistTask = GetPlaylistVideosAsync(url);
            await _currentGetPlaylistTask;
        }

        private async Task GetPlaylistVideosAsync(string url)
        {
            try
            {
                SetUI(UIState.Loading);
                PlaylistVideoViewModels = new();
                var videoList = await ServiceProvider.YoutubeService.GetPlaylistAsync(url);

                foreach (var video in videoList)
                {
                    PlaylistVideoCardViewModel v = new PlaylistVideoCardViewModel();
                    v.Title = video.Title;
                    v.Url = video.Url;
                    v.Duration = ServiceProvider.YoutubeService.GetVideoDuration(video.Duration);
                    v.ChannelName = video.Author.ChannelTitle;
                    v.ContentImage = await ThumbnailHelper.BitmapImageFromUrl(video.Thumbnails[0].Url);
                    v.IsChecked = true;
                    PlaylistVideoViewModels.Add(v);
                }
                SetUI(UIState.VideosLoaded);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                PlaylistVideoViewModels = null!;
                SetUI(UIState.LoadingError);
            }
        }


        #region UTILS
        private void SetUI(UIState state)
        {
            switch (state)
            {
                case UIState.Loading:
                    IsLoadingError = false;
                    IsVideoListLoaded = false;
                    break;

                case UIState.VideosLoaded:
                    IsVideoListLoaded = true;
                    break;

                case UIState.LoadingError:
                    IsLoadingError = true;
                    IsVideoListLoaded = false;
                    break;
            }
            CommandManager.InvalidateRequerySuggested();
        }
        private enum UIState
        {
            Idle,
            Loading,
            VideosLoaded,
            LoadingError
        }
        #endregion
    }
}
