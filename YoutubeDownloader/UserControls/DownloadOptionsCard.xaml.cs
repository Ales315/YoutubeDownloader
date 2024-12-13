using System.CodeDom;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YoutubeDownloader.Enums;

namespace YoutubeDownloader.UserControls
{
    /// <summary>
    /// Interaction logic for DownloadOptionsCard.xaml
    /// </summary>
    public partial class DownloadOptionsCard : UserControl
    {
        private Dictionary<DownloadMediaType, string> _contentTypeDictionary;
        public DownloadOptionsCard()
        {
            _contentTypeDictionary = new Dictionary<DownloadMediaType, string>()
            {
                { DownloadMediaType.VideoWithAudio, "Video with Audio"},
                { DownloadMediaType.AudioOnly, "Audio Only"},
                { DownloadMediaType.VideoOnly, "Video Only"}
            };

            InitializeComponent();
            this.IsVisibleChanged += OnVisibleChanged;
            cbDownloadType.ItemsSource = _contentTypeDictionary;
            cbDownloadType.DisplayMemberPath = "Value";
            cbDownloadType.SelectedValuePath = "Key";

            var bindingDownloadType = new Binding("DownloadOptionSelected")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            cbDownloadType.SetBinding(ComboBox.SelectedValueProperty, bindingDownloadType);

            var bindingDownloadFormat = new Binding("FormatSelected")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            cbDownloadFormat.SetBinding(ComboBox.SelectedValueProperty, bindingDownloadFormat);

            cbDownloadFormat.ItemsSource = GetFormatsByCategory("Video");
            cbDownloadType.SelectionChanged += OnDownloadTypeSelectionChanged;
            cbDownloadFormat.SelectedIndex = 0;
            cbDownloadType.SelectedIndex = 0;
        }

        private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //todo: load saved preset
            cbDownloadFormat.SelectedIndex = 0;
            var selection = cbDownloadFormat.SelectedIndex;
        }

        private void OnDownloadTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ((KeyValuePair<DownloadMediaType, string>)cbDownloadType.SelectedItem);
            switch (selectedItem.Key)
            {
                case DownloadMediaType.VideoOnly:
                    cbAudioQuality.Visibility = Visibility.Collapsed;
                    cbVideoQuality.Visibility = Visibility.Visible;
                    cbDownloadFormat.ItemsSource = GetFormatsByCategory("Video");
                    cbDownloadFormat.SelectedIndex = 0;
                    break;

                case DownloadMediaType.AudioOnly:
                    cbAudioQuality.Visibility = Visibility.Visible;
                    cbVideoQuality.Visibility = Visibility.Collapsed;
                    cbDownloadFormat.ItemsSource = GetFormatsByCategory("Audio");
                    cbDownloadFormat.SelectedIndex = 0;
                    break;

                case DownloadMediaType.VideoWithAudio:
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
