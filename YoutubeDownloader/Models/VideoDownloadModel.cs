using System.ComponentModel;
using System.Windows.Media;
using YoutubeDownloader.Enums;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Models
{
    public class VideoDownloadModel : INotifyPropertyChanged
    {
        private double _progress;
        private bool _isDownloading = false;
        private bool _isDownloadCompleted = false;
        private bool _isDownloadFailed = false;
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


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
