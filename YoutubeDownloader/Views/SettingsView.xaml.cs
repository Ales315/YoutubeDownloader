using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using YoutubeDownloader.Enums;

namespace YoutubeDownloader.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        private Dictionary<DownloadMediaType, string> _mediaTypeDictionary;

        public SettingsView()
        {
            _mediaTypeDictionary = new Dictionary<DownloadMediaType, string>()
            {
                { DownloadMediaType.VideoWithAudio, "Video with Audio"},
                { DownloadMediaType.AudioOnly, "Audio Only"},
                { DownloadMediaType.VideoOnly, "Video Only"}
            };
            InitializeComponent();
            SetupComboboxes();
        }

        private void SetupComboboxes()
        {
            //Media type combobox
            cbMediaType.ItemsSource = _mediaTypeDictionary;
            cbMediaType.DisplayMemberPath = "Value";
            cbMediaType.SelectedValuePath = "Key";

            var bindingDownloadType = new Binding("MediaTypePreference")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            cbMediaType.SetBinding(ComboBox.SelectedValueProperty, bindingDownloadType);

            //Formats comboboxes
            cbAudioFormat.ItemsSource = GetFormatsByCategory("Audio");
            var bindingAudioFormat = new Binding("AudioFormatPreference")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            cbAudioFormat.SetBinding(ComboBox.SelectedValueProperty, bindingAudioFormat);
            

            cbVideoFormat.ItemsSource = GetFormatsByCategory("Video");
            var bindingVideoFormat = new Binding("VideoFormatPreference")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            cbVideoFormat.SetBinding(ComboBox.SelectedValueProperty, bindingVideoFormat);
            
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

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            openFolderDialog.Multiselect = false;
            openFolderDialog.ShowDialog();
            var result = openFolderDialog.FolderName;
            if (string.IsNullOrWhiteSpace(result))
                return;
            textboxDownloadPath.Text = result;
        }
    }
}
