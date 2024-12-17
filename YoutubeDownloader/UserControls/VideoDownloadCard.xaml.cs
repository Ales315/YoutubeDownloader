using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using YoutubeDownloader.Services;

namespace YoutubeDownloader.UserControls
{
    /// <summary>
    /// Interaction logic for VideoDownloadCard.xaml
    /// </summary>
    public partial class VideoDownloadCard : UserControl
    {
        public VideoDownloadCard()
        {
            InitializeComponent();
            buttonCancelDownload.MouseEnter += OnbuttonCancelDownloadMouseEnter;
            buttonCancelDownload.MouseLeave += OnbuttonCancelDownloadMouseLeave;
            iconCheck.Foreground = ServiceProvider.ThemeService.GetPrimaryColorBrush();
            iconCancel.Visibility = System.Windows.Visibility.Hidden;
        }

        private void OnbuttonCancelDownloadMouseLeave(object sender, MouseEventArgs e)
        {
            iconCancel.Visibility = System.Windows.Visibility.Hidden;
        }

        private void OnbuttonCancelDownloadMouseEnter(object sender, MouseEventArgs e)
        {
            iconCancel.Visibility = System.Windows.Visibility.Visible;
            buttonCancelDownload.Cursor = Cursors.Hand;
        }

        private void buttonOpenFolder_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void buttonPlay_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
