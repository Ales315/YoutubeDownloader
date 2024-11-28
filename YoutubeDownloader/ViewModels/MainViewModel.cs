using System.Windows.Input;
using YoutubeDownloader.Helpers;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.ViewModels
{
    class MainViewModel : ObservableObject
    {
        private ICommand _changePageCommand = null!;
        private ViewModelBase _currentPageViewModel = null!;
        private List<ViewModelBase> _pageViewModels = null!;
        public MainViewModel()
        {
            PageViewModels.Add(new HomePageViewModel());
            PageViewModels.Add(new SettingsViewModel());
            CurrentPageViewModel = PageViewModels[0];
        }

        public ICommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                    _changePageCommand = new RelayCommand(p => ChangeViewModel((ViewModelBase)p), p => p is ViewModelBase);
                return _changePageCommand;
            }
        }
        public List<ViewModelBase> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<ViewModelBase>();
                return _pageViewModels;
            }
        }
        public ViewModelBase CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged(nameof(CurrentPageViewModel));
                }
            }
        }

        private void ChangeViewModel(ViewModelBase viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);
        }
    }
}
