using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using YoutubeDownloader.Enums;
using YoutubeDownloader.Helpers;
using YoutubeDownloader.Services;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Models
{
    public class VideoDownloadModel : INotifyPropertyChanged
    {
        private double _progress;
        private bool _isDownloading = false;
        private bool _isDownloadCompleted = false;
        private bool _isDownloadFailed = false;
        private ICommand _cancelDownloadCommand = null!;
        private ICommand _playCommand = null!;
        private ICommand _openDownloadFolderCommand = null!;
        private ICommand _removeFromListCommand = null!;

        public ImageSource Thumbnail { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public double Progress
        {
            get => _progress;
            set
            {
                if (_progress == value) return;
                _progress = value;
                IsDownloadCompleted = value == 1 ? true : false;
                OnPropertyChanged(nameof(Progress));
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
        public bool IsDownloadCompleted
        {
            get => _isDownloadCompleted;
            set
            {
                if (_isDownloadCompleted == value) return;
                _isDownloadCompleted = value;
                OnPropertyChanged(nameof(IsDownloadCompleted));
            }
        }
        public bool IsDownloadFailed
        {
            get => _isDownloadFailed;
            set
            {
                if (_isDownloadFailed == value) return;
                _isDownloadFailed = value;
                OnPropertyChanged(nameof(IsDownloadFailed));
            }
        }
        public VideoOnlyStreamInfo VideoStream { get; set; } = null!;
        public AudioOnlyStreamInfo AudioStream { get; set; } = null!;

        public DownloadMediaType DownloadOption { get; set; }
        public DownloadFormat DownloadFormat { get; set; }
        public CancellationTokenSource CancellationToken { get; internal set; } = null!;
        public ICommand CancelDownloadCommand
        {
            get
            {
                if (_cancelDownloadCommand == null)
                    return new RelayCommand(p => CancelDownload());
                return _cancelDownloadCommand;
            }
        }
        public ICommand PlayCommand
        {
            get
            {
                if (_playCommand == null)
                    return new RelayCommand(p => Play());
                return _playCommand;
            }
        }
        public ICommand OpenFolderCommand
        {
            get
            {
                if (_openDownloadFolderCommand == null)
                    return new RelayCommand(p => OpenDownloadFolder());
                return _openDownloadFolderCommand;
            }
        }
        public ICommand RemoveFromListCommand
        {
            get
            {
                if (_removeFromListCommand == null)
                    return new RelayCommand(p => RemoveFromList());
                return _removeFromListCommand;
            }
        }

        public string FileName { get; set; } = string.Empty;


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void CancelDownload()
        {
            CancellationToken?.Cancel();
        }
        private void OpenDownloadFolder()
        {
            if (!File.Exists(FileName))
            {
                MessageBox.Show("File spostato o mancante", "File non trovato");
                return;
            }
            Process.Start("explorer.exe", Path.GetDirectoryName(FileName) ?? string.Empty);
        }
        private void Play()
        {
            if (!File.Exists(FileName))
            {
                MessageBox.Show("File spostato o mancante", "File non trovato");
                return;
            }
            Process.Start("explorer.exe", FileName);
        }
        private void RemoveFromList()
        {
            if (IsDownloading)
                CancelDownload();
            ServiceProvider.YoutubeService.DownloadList.Remove(this);
        }
    }
}
