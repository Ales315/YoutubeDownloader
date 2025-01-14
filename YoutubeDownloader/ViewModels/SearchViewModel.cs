using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices;
using YoutubeDownloader.Services;

namespace YoutubeDownloader.ViewModels
{
    public class SearchViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private ObservableCollection<SearchResult> _searchResults { get; set; } = null!;

        public ObservableCollection<SearchResult> SearchResults
        {
            get { return ServiceProvider.YoutubeService.SearchResults; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
