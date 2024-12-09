using System.ComponentModel;
using System.Windows.Media;
using YoutubeDownloader.Enums;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Models
{
    public class VideoDownloadModel : INotifyPropertyChanged
    {
        private double _progress;
        private bool _isCompleted;

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
                OnPropertyChanged(nameof(Progress));
            }
        }
        public DownloadOption DownloadOption;
        public VideoOnlyStreamInfo VideoStream { get; set; } = null!;
        public AudioOnlyStreamInfo AudioStream { get; set; } = null!;


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
