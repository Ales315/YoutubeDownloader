using System.Windows.Input;
using YoutubeDownloader.Helpers;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;

namespace YoutubeDownloader.ViewModels
{
    class MainViewModel : ObservableObject
    {
        private ICommand _changePageCommand = null!;
        private ICommand _changeSettingsVisibilityCommand = null!;
        private ViewModelBase _currentPageViewModel = null!;
        private List<ViewModelBase> _pageViewModels = null!;
        private SettingsViewModel _settingsViewModel = null!;
        private HomePageViewModel _homeViewModel = null!;

        public MainViewModel()
        {
            _homeViewModel = new();
            SettingsViewModel = new();
            SettingsViewModel.IsVisibleChanged += (s,e) => ((HomePageViewModel)CurrentPageViewModel).Invalidate();
            //PageViewModels.Add(_homeViewModel);
            CurrentPageViewModel = _homeViewModel;
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
        public ICommand ChangeSettingsVisibilityCommand
        {
            get
            {
                return _changeSettingsVisibilityCommand ?? new RelayCommand(param => this.ChangeVisibility());
            }
        }

        private void ChangeVisibility()
        {
            SettingsViewModel.IsVisible = true;
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
        public SettingsViewModel SettingsViewModel
        {
            get
            {
                return _settingsViewModel;
            }
            set
            {
                if (_settingsViewModel != value)
                {
                    _settingsViewModel = value;
                    OnPropertyChanged(nameof(SettingsViewModel));
                }
            }
        }

        private void ChangeViewModel(ViewModelBase viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);
            CurrentPageViewModel = PageViewModels.FirstOrDefault(vm => vm == viewModel) ?? throw new Exception("Error");
        }
    }
}
