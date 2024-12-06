using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            cbDownloadType.ItemsSource = Enum.GetValues(typeof(DownloadOptions));
            cbAudioFormat.ItemsSource = Enum.GetValues(typeof(AudioFormats));
            cbDownloadType.SelectionChanged += OnDownloadTypeSelectionChanged;
        }

        private void OnDownloadTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = cbDownloadType.SelectedItem;
            switch (selectedItem)
            {
                case DownloadOptions.VideoOnly:
                    cbAudioQuality.Visibility = Visibility.Collapsed;
                    cbVideoQuality.Visibility = Visibility.Visible;
                    break;

                case DownloadOptions.AudioOnly:
                    cbAudioQuality.Visibility = Visibility.Visible;
                    cbVideoQuality.Visibility = Visibility.Collapsed;
                    break;

                case DownloadOptions.VideoWithAudio:
                    cbAudioQuality.Visibility = Visibility.Visible;
                    cbVideoQuality.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
