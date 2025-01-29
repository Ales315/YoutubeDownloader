using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace YoutubeDownloader.ViewModels.Card
{
    public class PlaylistVideoCardViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private bool _isChecked;
        public string Title { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;
        public string ViewCount { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public BitmapImage ContentImage { get; set; } = null!;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
