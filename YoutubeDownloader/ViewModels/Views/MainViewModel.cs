﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using YoutubeDownloader.Helpers;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;
using YoutubeDownloader.ViewModels.Card;
using YoutubeDownloader.ViewModels.UserControl;

namespace YoutubeDownloader.ViewModels.Views
{
    class MainViewModel : ObservableObject
    {
        private ICommand _changePageCommand = null!;
        private ICommand _changeSettingsVisibilityCommand = null!;
        private ViewModelBase _currentPageViewModel = null!;
        private List<ViewModelBase> _pageViewModels = null!;
        private SettingsViewModel _settingsViewModel = null!;
        private HomePageViewModel _homeViewModel = null!;
        private string _progressText = string.Empty;
        private double _meanProgress;

        public ObservableCollection<VideoDownloadCardViewModel> Downloads { get => ServiceProvider.YoutubeService.DownloadList; }

        public MainViewModel()
        {
            SettingsViewModel = new();
            _homeViewModel = new();
            SettingsViewModel.IsVisibleChanged += (s, e) => ((HomePageViewModel)CurrentPageViewModel).VideoVM.Invalidate();
            CurrentPageViewModel = _homeViewModel;
            //PageViewModels.Add(_homeViewModel);

            Downloads.CollectionChanged += (s, e) =>
            {
                if (e.NewItems == null) return;
                foreach (VideoDownloadCardViewModel d in e.NewItems)
                {
                    d.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(VideoDownloadCardViewModel.Progress))
                            OnPropertyChanged(nameof(MeanProgress));
                    };
                };
            };
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
                return _changeSettingsVisibilityCommand ?? new RelayCommand(param => ChangeVisibility());
            }
        }

        public string ProgressText
        {
            get => _progressText;
            set
            {
                _progressText = value;
                OnPropertyChanged(nameof(ProgressText));
            }
        }
        public double MeanProgress
        {
            get
            {
                double p = Downloads.Any() ? Downloads.Average(d => d.Progress) : 0;
                if (p == 1)
                    ProgressText = $"All downloads completed ✓";
                else
                    ProgressText = $"Dowloading {Downloads.Where(d => d.IsDownloading == true).Count()}/{Downloads.Count}";
                return p;
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
