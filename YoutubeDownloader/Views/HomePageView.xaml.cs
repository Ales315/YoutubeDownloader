using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfAnimatedGif;

namespace YoutubeDownloader.Views
{
    /// <summary>
    /// Interaction logic for HomePageView.xaml
    /// </summary>
    public partial class HomePageView : UserControl
    {
        public HomePageView()
        {
            InitializeComponent();
            textboxInputUrl.TextChanged += (s, e) => textBlockURLHint.Visibility = textboxInputUrl.Text.Length > 0 ? Visibility.Hidden : Visibility.Visible;
            textboxInputUrl.GotFocus += (s, e) => UrlBarBorder.BorderBrush = new SolidColorBrush(Colors.Blue);
            textboxInputUrl.LostFocus += (s, e) => UrlBarBorder.BorderBrush = new SolidColorBrush(Colors.DarkSlateGray);
        }

        private void buttonPaste_Click(object sender, RoutedEventArgs e)
        {
            textboxInputUrl.Text = Regex.Replace(Clipboard.GetText(), @"\t|\n|\r", "");
        }

        private void buttonDownload_Click(object sender, RoutedEventArgs e)
        {
            //todo
        }

        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            string inputUrl = textboxInputUrl.Text.Trim();
            if (string.IsNullOrWhiteSpace(inputUrl)) return;
            textboxInputUrl.Text = inputUrl;
            //_mainViewModel.GetVideoMetadata(inputUrl);
        }

        private void imgLoadingGif_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var img = sender as Image;
            var controller = ImageBehavior.GetAnimationController(imgLoadingGif);
            if (controller == null) return;
            if (img?.Visibility == Visibility.Hidden || img?.Visibility == Visibility.Collapsed)
            {
                controller.Pause();
                controller.GotoFrame(1);
            }
            else
            {
                controller.Play();
            }
        }
    }
}
