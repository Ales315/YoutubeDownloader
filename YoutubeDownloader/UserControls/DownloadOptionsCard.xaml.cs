using System.Windows;
using System.Windows.Controls;
using YoutubeDownloader.Enums;

namespace YoutubeDownloader.UserControls
{
    /// <summary>
    /// Interaction logic for DownloadOptionsCard.xaml
    /// </summary>
    public partial class DownloadOptionsCard : UserControl
    {
        public DownloadOptionsCard()
        {
            InitializeComponent();
            cbDownloadType.ItemsSource = Enum.GetValues(typeof(DownloadOption));
            cbAudioFormat.ItemsSource = Enum.GetValues(typeof(AudioFormats));
            cbDownloadType.SelectionChanged += OnDownloadTypeSelectionChanged;
        }

        private void OnDownloadTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = cbDownloadType.SelectedItem;
            switch (selectedItem)
            {
                case DownloadOption.VideoOnly:
                    cbAudioQuality.Visibility = Visibility.Collapsed;
                    cbVideoQuality.Visibility = Visibility.Visible;
                    break;

                case DownloadOption.AudioOnly:
                    cbAudioQuality.Visibility = Visibility.Visible;
                    cbVideoQuality.Visibility = Visibility.Collapsed;
                    break;

                case DownloadOption.VideoWithAudio:
                    cbAudioQuality.Visibility = Visibility.Visible;
                    cbVideoQuality.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
