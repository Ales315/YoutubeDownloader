using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using YoutubeDownloader.Enums;
using YoutubeDownloader.Helpers;
using YoutubeDownloader.Services;

namespace YoutubeDownloader.ViewModels
{
    class KeywordSearchViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region FIELDS
        private ICollectionView _filteredSearchResultViewModels = null!;
        private bool _isVideosChecked;
        private bool _isPlaylistsChecked;
        private bool _isChannelsChecked;

        public bool IsVideosChecked
        {
            get => _isVideosChecked;
            set
            {
                if (_isVideosChecked == value) return;
                _isVideosChecked = value;
                OnPropertyChanged(nameof(IsVideosChecked));
                FilteredSearchResultViewModels?.Refresh();
            }
        }
        public bool IsPlaylistsChecked
        {
            get => _isPlaylistsChecked;
            set
            {
                if (_isPlaylistsChecked == value) return;
                _isPlaylistsChecked = value;
                OnPropertyChanged(nameof(IsPlaylistsChecked));
                FilteredSearchResultViewModels?.Refresh();
            }
        }
        public bool IsChannelsChecked
        {
            get => _isChannelsChecked;
            set
            {
                if (_isChannelsChecked == value) return;
                _isChannelsChecked = value;
                OnPropertyChanged(nameof(IsChannelsChecked));
                FilteredSearchResultViewModels?.Refresh();
            }
        }

        public ObservableCollection<SearchResultCardViewModel> SearchResultViewModels
        {
            get => ServiceProvider.YoutubeService.SearchResultViewModels;
        }

        public ICollectionView FilteredSearchResultViewModels
        {
            get => _filteredSearchResultViewModels;
            set
            {
                _filteredSearchResultViewModels = value;
                OnPropertyChanged(nameof(FilteredSearchResultViewModels));
            }
        }

        #endregion


        public event EventHandler<string>? VideoSearchResultClicked;
        public event EventHandler<string>? PlaylistSearchResultClicked;
        public event EventHandler<string>? ChannelSearchResultClicked;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public KeywordSearchViewModel()
        {
            IsVideosChecked = true;
            IsPlaylistsChecked = true;
            IsChannelsChecked = true;
            
            FilteredSearchResultViewModels = CollectionViewSource.GetDefaultView(SearchResultViewModels);
            FilteredSearchResultViewModels.Filter = FilterResults;
            //SearchResultViewModels.CollectionChanged += (s, e) => FilteredSearchResultViewModels.Refresh();
        }

        private bool FilterResults(object item)
        {
            if (item is not SearchResultCardViewModel result)
                return false;

            if (IsVideosChecked && result.ResultType == SearchResultType.Video ||
                IsPlaylistsChecked && result.ResultType == SearchResultType.Playlist ||
                IsChannelsChecked && result.ResultType == SearchResultType.Channel)
                return true;

            return false;
        }

        public void GetVideoData(string url)
        {
            VideoSearchResultClicked?.Invoke(this, url);
        }
    }
}
