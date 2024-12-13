using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace YoutubeDownloader.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
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
