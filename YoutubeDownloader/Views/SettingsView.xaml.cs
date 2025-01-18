using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using YoutubeDownloader.Enums;
using YoutubeDownloader.Services;
using YoutubeDownloader.ViewModels.Views;

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
            cbTheme.SelectionChanged += OnCbSelectionChanged;
            checkboxNotifications.Checked += (s, e) => ServiceProvider.SettingsService.UserPreferences.UseNotifications = true;
            checkboxNotifications.Unchecked += (s, e) => ServiceProvider.SettingsService.UserPreferences.UseNotifications = false;
            checkboxNotifications.IsChecked = ServiceProvider.SettingsService.UserPreferences.UseNotifications;

            checkboxAutoDownload.Checked += (s, e) => ServiceProvider.SettingsService.UserPreferences.AutoDownload = true;
            checkboxAutoDownload.Unchecked += (s, e) => ServiceProvider.SettingsService.UserPreferences.AutoDownload = false;
            checkboxAutoDownload.IsChecked = ServiceProvider.SettingsService.UserPreferences.AutoDownload;
        }

        private void OnCbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTheme.SelectedIndex == 0)
            {
                ServiceProvider.ThemeService.SetSystemTheme();
                return;
            }

            if (ServiceProvider.ThemeService.IsLightTheme)
                ServiceProvider.ThemeService.SetDarkTheme();
            else
                ServiceProvider.ThemeService.SetLightTheme();
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

            //theme combobox
            cbTheme.ItemsSource = Enum.GetValues(typeof(ThemeStyles));
            var bindingTheme = new Binding("ThemePreference")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            cbTheme.SetBinding(ComboBox.SelectedValueProperty, bindingTheme);
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

        private void cbChangeTheme_Checked(object sender, RoutedEventArgs e)
        {
            ServiceProvider.ThemeService.SetLightTheme();
        }

        private void cbChangeTheme_Unchecked(object sender, RoutedEventArgs e)
        {
            ServiceProvider.ThemeService.SetDarkTheme();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is SettingsViewModel vm)
                vm.IsVisible = false;
        }

        private void buttonAbout_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            about.ShowDialog();
        }
    }
}
