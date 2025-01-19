using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using YoutubeDownloader.Enums;
using YoutubeDownloader.Helpers;
using YoutubeDownloader.Services;
using YoutubeDownloader.ViewModels.Card;

namespace YoutubeDownloader.ViewModels.UserControl
{
    class HomePageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private string _previousValidUrl = string.Empty;
        private string _keywordSearchReservedUrl = string.Empty;

        public VideoViewModel VideoVM;
        public DownloadListViewModel DownloadListVM;
        public KeywordSearchViewModel KeywordSearchVM;

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
        private ICommand _goBackCommand = null!;

        //current view
        private AppState _currentState;
        private AppState _previousState;
        private ViewModelBase _currentViewModel = null!;
        private ViewModelBase _previousViewModel = null!;

        //UI state
        private bool _isLoadingAutoDownload;
        private bool _isAutoDownloadFailed;
        private bool _isHome;
        private bool _isBackEnabled;

        public ObservableCollection<VideoDownloadCardViewModel> VideoDownloadViewModels
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
                return _cancelAutoDownloadCommand ?? new RelayCommand(param => CancelAutoDownload());
            }
        }
        public ICommand GoBackCommand
        {
            get
            {
                return _goBackCommand ?? new RelayCommand(param => GoToPreviousPage());
            }
        }

        

        public ICommand GoHomeCommand
        {
            get
            {
                if (_goHomeCommand == null)
                {
                    _goHomeCommand = new RelayCommand(
                        param => GoHome()
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

        //UI state
        public bool IsLoadingAutoDownload
        {
            get => _isLoadingAutoDownload;
            set
            {
                _isLoadingAutoDownload = value;
                OnPropertyChanged(nameof(IsLoadingAutoDownload));
            }
        }
        public bool IsAutoDownloadFailed
        {
            get => _isAutoDownloadFailed;
            set
            {
                _isAutoDownloadFailed = value;
                OnPropertyChanged(nameof(IsAutoDownloadFailed));
            }
        }
        public bool IsHome
        {
            get => _isHome;
            set
            {
                _isHome = value;
                OnPropertyChanged(nameof(IsHome));
            }
        }
        public bool IsBackEnabled
        {
            get => _isBackEnabled;
            set
            {
                _isBackEnabled = value;
                OnPropertyChanged(nameof(IsBackEnabled));
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
            IsHome = true;
            ServiceProvider.YoutubeService.DirectDownloadStatus += (s, e) => { AutoDownloadStatus = e; };
            VideoVM = new VideoViewModel();
            DownloadListVM = new DownloadListViewModel();
            KeywordSearchVM = new KeywordSearchViewModel();
            SetUI(AppState.DownloadListForm);

            VideoVM.DownloadStarted += (s, e) => SetUI(AppState.DownloadListForm);

            KeywordSearchVM.VideoSearchResultClicked += async (s, e) => {
                SetUI(AppState.VideoDownloadForm);
                await VideoVM.OverrideGetVideoMetadataAsync(e); 
            };
            //todo: implement other events
        }


        #region SEARCH

        public async Task Search()
        {
            AutoDownloadStatus = string.Empty;
            IsAutoDownloadFailed = false;
            if (isUrl(SearchQuery))
                await GetVideoMetadata(SearchQuery);
            else
                await GetSearchResults();
        }

        public async Task GetVideoMetadata(string url)
        {
            if (ServiceProvider.SettingsService.UserPreferences.AutoDownload)
            {
                try
                {
                    IsAutoDownloadFailed = false;
                    IsLoadingAutoDownload = true;
                    await ServiceProvider.YoutubeService.EnqueueDownloadFromUrl(url);
                    AutoDownloadStatus = "Download started ✓";
                }
                catch (OperationCanceledException)
                {
                    AutoDownloadStatus = string.Empty;
                }
                catch (Exception ex)
                {
                    IsAutoDownloadFailed = true;
                    AutoDownloadStatus = string.Empty;
                    ErrorMessage = ex.Message;
                }

                IsLoadingAutoDownload = false;
            }
            else
            {
                SetUI(AppState.VideoDownloadForm);
                await VideoVM.GetMetadataAsync(url);
            }
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

        #region COMMANDS
        private void GoToPreviousPage()
        {
            ServiceProvider.YoutubeService.GetMetadataCancellationToken?.Cancel();
            SetUIFromViewModel(_previousViewModel);
        }
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
            IsBackEnabled = false;
            IsHome = false;
            _previousViewModel = CurrentViewModel;
            switch (state)
            {
                case AppState.DownloadListForm:
                    IsHome = true;
                    CurrentViewModel = DownloadListVM;
                    break;
                case AppState.VideoDownloadForm:
                    CurrentViewModel = VideoVM;
                    break;
                case AppState.PlaylistDownloadForm:
                    break;
                case AppState.ChannelForm:
                    break;
                case AppState.KeywordSearchForm:
                    CurrentViewModel = KeywordSearchVM;
                    break;
            }
            if ((state == AppState.VideoDownloadForm || state == AppState.PlaylistDownloadForm || state == AppState.ChannelForm) 
                && _previousState == AppState.KeywordSearchForm)
                IsBackEnabled = true;

            _previousState = state;
        }
        private void SetUIFromViewModel(object tagetViewModel)
        {
            switch (tagetViewModel)
            {
                case KeywordSearchViewModel:
                    SetUI(AppState.KeywordSearchForm);
                    break;
            }
        }

        #endregion
    }
}
