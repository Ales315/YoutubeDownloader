using System.Reflection.Metadata;
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
            cbDownloadFormat.ItemsSource = GetFormatsByCategory("Video");
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
                    cbDownloadFormat.ItemsSource = GetFormatsByCategory("Video");
                    cbDownloadFormat.SelectedIndex = 0;
                    break;

                case DownloadOption.AudioOnly:
                    cbAudioQuality.Visibility = Visibility.Visible;
                    cbVideoQuality.Visibility = Visibility.Collapsed;
                    cbDownloadFormat.ItemsSource = GetFormatsByCategory("Audio");
                    cbDownloadFormat.SelectedIndex = 0;
                    break;

                case DownloadOption.VideoWithAudio:
                    cbAudioQuality.Visibility = Visibility.Visible;
                    cbVideoQuality.Visibility = Visibility.Visible;
                    cbDownloadFormat.ItemsSource = GetFormatsByCategory("Video");
                    cbDownloadFormat.SelectedIndex = 0;
                    break;
            }
        }

        private IEnumerable<DownloadFormat> GetFormatsByCategory(string category)
        {
            return Enum.GetValues(typeof(DownloadFormat)).Cast<DownloadFormat>()
                       .Where(format =>
                       {
                           var memberInfo = typeof(DownloadFormat).GetMember(format.ToString()).FirstOrDefault();
                           var attribute = memberInfo?.GetCustomAttributes(typeof(FormatCategoryAttribute), false).FirstOrDefault() as FormatCategoryAttribute;
                           return attribute?.Category.Contains(category) ?? false;
                       });
        }
    }
}
