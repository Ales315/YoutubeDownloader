using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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
        }

        private void OnbuttonCancelDownloadMouseLeave(object sender, MouseEventArgs e)
        {
            imgCancel.Source = null;
        }

        private void OnbuttonCancelDownloadMouseEnter(object sender, MouseEventArgs e)
        {
            imgCancel.Source = new BitmapImage(new Uri(@"/Resources/Images/close24Red.png", UriKind.Relative));
            buttonCancelDownload.Cursor = Cursors.Hand;
        }
    }
}
