using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using YoutubeDownloader.Enums;
using YoutubeDownloader.Helpers;
using YoutubeDownloader.Services;

namespace YoutubeDownloader.ViewModels
{
    class HomePageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private string _previousValidUrl = string.Empty;
        
        public VideoViewModel VideoViewModel;
        
        #region FIELDS
        //data
        private string _searchQuery = string.Empty;
        private double _progress;
        private string _errorMessage = string.Empty;
        private string _autoDownloadStatus = string.Empty;

        //commands
        private ICommand _searchCommand = null!;
        private ICommand _goHomeCommand = null!;
        private ICommand _cancelAutoDownloadCommand = null!;

        //current view
        private AppState _currentState;
        private ViewModelBase _currentViewModel;

        public ObservableCollection<VideoDownloadViewModel> VideoDownloadViewModels
        {
            get => ServiceProvider.YoutubeService.DownloadList;
        }
        public ObservableCollection<SearchResultCardViewModel> SearchResultViewModels
        {
            get => ServiceProvider.YoutubeService.SearchResultViewModels;
        }
        public string Url { get; set; } = string.Empty;

        //data
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
        public string AutoDownloadStatus
        {
            get => _autoDownloadStatus;
            set
            {
                if (_autoDownloadStatus == value) return;
                _autoDownloadStatus = value;
                OnPropertyChanged(nameof(AutoDownloadStatus));
            }
        }
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery == value) return;
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
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

        //Commands
        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand ?? (_searchCommand = new RelayCommandAsync(Search, (c) => true));
            }
        }

        public ICommand CancelAutoDownloadCommand
        {
            get
            {
                return _cancelAutoDownloadCommand ?? new RelayCommand(param => this.CancelAutoDownload());
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

        //current view
        public AppState CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                OnPropertyChanged(nameof(CurrentState));
            }
        }
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
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
            VideoViewModel = new VideoViewModel();
            //VideoViewModel.DownloadStarted += OnVideoVMDownloadStarted;

            //VideoDownloadViewModels.CollectionChanged += (s, e) =>
            //{
            //    StateHandler.IsDownloadListEmpty = VideoDownloadViewModels.Count == 0;
            //};
        }

        #region SEARCH

        public async Task Search()
        {
            if (isUrl(SearchQuery))
                await GetVideoMetadata(SearchQuery);
            else
                await GetSearchResults();
        }

        private async Task GetSearchResults()
        {
            SetUI(AppState.KeywordSearchForm);
            
            //todo: move this to separate Usercontrol vm
            await ServiceProvider.YoutubeService.Search(SearchQuery);
        }

        private bool isUrl(string query)
        {
            if (Uri.TryCreate(query, UriKind.Absolute, out var uri))
                return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
            return false;
        }

        #endregion

        #region GET METADATA
        //Get video metadata
        public async Task GetMetadataFromUrl()
        {
            await VideoViewModel.GetMetadataFromUrl();
        }
        public async Task GetVideoMetadata(string url)
        {
            if (ServiceProvider.SettingsService.UserPreferences.AutoDownload)
            {
                try
                {
                    await ServiceProvider.YoutubeService.EnqueueDownloadFromUrl(url);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
            }
            else
            {
                SetUI(AppState.VideoDownloadForm);
                await VideoViewModel.GetVideoMetadata(url);
            }
        }

        #endregion

        #region COMMANDS

        private void GoHome()
        {
            ServiceProvider.YoutubeService.GetMetadataCancellationToken?.Cancel();
            ServiceProvider.YoutubeService.SearchCancellationToken?.Cancel();
            SetUI(AppState.DownloadListForm);
        }
        private void CancelAutoDownload()
        {
            ServiceProvider.YoutubeService.GetMetadataCancellationToken?.Cancel();
        }

        #endregion

        #region UI

        private void SetUI(AppState state)
        {
            switch (state)
            {
                case AppState.DownloadListForm:
                    break;
                case AppState.VideoDownloadForm:
                    CurrentViewModel = VideoViewModel;
                    break;
                case AppState.PlaylistDownloadForm:
                    break;
                case AppState.ChannelForm:
                    break;
                case AppState.KeywordSearchForm:
                    break;
            }
        }

        #endregion
    }
}
