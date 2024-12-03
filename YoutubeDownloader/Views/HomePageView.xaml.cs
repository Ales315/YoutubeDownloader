using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfAnimatedGif;
using YoutubeDownloader.ViewModels;

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
            textboxInputUrl.KeyDown += OnTextboxInputUrlKeyDown;
        }

        //Search with Enter key
        private void OnTextboxInputUrlKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (this.DataContext == null) return;
            if(this.DataContext is HomePageViewModel vm)
                vm.GetVideoData.Execute(null);
        }

        private void buttonPaste_Click(object sender, RoutedEventArgs e)
        {
            textboxInputUrl.Text = Regex.Replace(Clipboard.GetText(), @"\t|\n|\r", "");
            textboxInputUrl.Focus();
            textboxInputUrl.Select(textboxInputUrl.Text.Length, 0);
        }

        private void buttonDownload_Click(object sender, RoutedEventArgs e)
        {
            //todo: da togliere
        }

        //Loading gif play parameters
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
