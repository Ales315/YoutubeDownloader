using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MaterialDesignThemes.Wpf;
using WpfAnimatedGif;
using YoutubeDownloader.Services;
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
            textboxInputUrl.TextChanged += (s, e) => TextChanged();
            textboxInputUrl.GotFocus += (s, e) =>
            UrlBarBorder.BorderBrush = ServiceProvider.ThemeService.GetPrimaryColorBrush();
            textboxInputUrl.LostFocus += (s, e) => UrlBarBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);
#if DEBUG
            textboxInputUrl.Text = "https://www.youtube.com/watch?v=HQmmM_qwG4k";
#endif
            textboxInputUrl.KeyDown += OnTextboxInputUrlKeyDown;

            imgLoadingGifVideo.IsVisibleChanged += (s, e) => SetGifPlaybackFramePosition(s);
            imgLoadingGifStreams.IsVisibleChanged += (s, e) => SetGifPlaybackFramePosition(s);

            UpdateSearchButton();

            ServiceProvider.SettingsService.SettingsChanged += (s, e) => UpdateSearchButton();

            iconLoadingAutoDownload.IsVisibleChanged += (s, e) => StartAnimation();
            iconLoadingAutoDownload.Foreground = ServiceProvider.ThemeService.GetPrimaryColorBrush();
        }

        private void StartAnimation()
        {
            if(!iconLoadingAutoDownload.IsVisible) 
                return;
            RotateTransform rotateTransform = new RotateTransform();
            iconLoadingAutoDownload.RenderTransform = rotateTransform;
            iconLoadingAutoDownload.RenderTransformOrigin = new Point(0.5, 0.5);

            DoubleAnimation doubleAnimation = new DoubleAnimation()
            {
                From = 360,
                To = 0,
                Duration = TimeSpan.FromSeconds(1),
                RepeatBehavior = RepeatBehavior.Forever
            };
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, doubleAnimation);
        }

        private void UpdateSearchButton()
        {
            var autoDownload = ServiceProvider.SettingsService.UserPreferences.AutoDownload;
            buttonSearch.ToolTip = autoDownload ? "Download" : "Search";
            iconSearchDownload.Kind = autoDownload ? PackIconKind.Download : PackIconKind.Search;
        }

        private void TextChanged()
        {
            textBlockURLHint.Visibility = textboxInputUrl.Text.Length > 0 ? Visibility.Hidden : Visibility.Visible;
        }

        private static void SetGifPlaybackFramePosition(object sender)
        {
            var img = sender as Image;
            var controller = ImageBehavior.GetAnimationController(img);
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

        //Search with Enter key
        private void OnTextboxInputUrlKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (this.DataContext == null) return;
            if (this.DataContext is HomePageViewModel vm)
                vm.GetVideoData.Execute(null);
        }

        private void buttonPaste_Click(object sender, RoutedEventArgs e)
        {
            textboxInputUrl.Text = Regex.Replace(Clipboard.GetText(), @"\t|\n|\r", "");
            textboxInputUrl.Focus();
            textboxInputUrl.Select(textboxInputUrl.Text.Length, 0);
        }

        //Loading gif play parameters
        private void imgLoadingGifVideo_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var img = sender as Image;
            var controller = ImageBehavior.GetAnimationController(imgLoadingGifVideo);
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
