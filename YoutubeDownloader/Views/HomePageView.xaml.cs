using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MaterialDesignThemes.Wpf;
using YoutubeDownloader.Services;
using YoutubeDownloader.ViewModels.UserControl;

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

            MouseDown += OnMouseDown;

            UpdateSearchButton();

            ServiceProvider.SettingsService.SettingsChanged += (s, e) => UpdateSearchButton();

            iconLoadingAutoDownload.IsVisibleChanged += (s, e) => StartAnimation();
            iconLoadingAutoDownload.Foreground = ServiceProvider.ThemeService.GetPrimaryColorBrush();

            UrlErrorBorder.IsVisibleChanged += (s, e) => StartErrorAnimation();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //handle back button press
            if (e.ChangedButton == System.Windows.Input.MouseButton.XButton1)
            {
                if (this.DataContext is HomePageViewModel vm)
                    vm.GoBackCommand.Execute(null);
            }
        }

        private void StartErrorAnimation()
        {
            if (!UrlErrorBorder.IsVisible)
                return;
            TranslateTransform translateTransform = new();
            UrlErrorBorder.RenderTransform = translateTransform;
            UrlErrorBorder.RenderTransformOrigin = new Point(0.5, 0.5);

            DoubleAnimation doubleAnimation = new DoubleAnimation()
            {
                From = 0,
                To = 35,
                Duration = TimeSpan.FromMilliseconds(280)
            };
            translateTransform.BeginAnimation(TranslateTransform.YProperty, doubleAnimation);
        }


        private void StartAnimation()
        {
            if (!iconLoadingAutoDownload.IsVisible)
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



        //Search with Enter key
        private void OnTextboxInputUrlKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            GetVideoData();
        }

        private void GetVideoData()
        {
            if (this.DataContext == null) return;
            if (this.DataContext is HomePageViewModel vm)
                vm.SearchCommand.Execute(null);
        }

        private void buttonPaste_Click(object sender, RoutedEventArgs e)
        {
            textboxInputUrl.Text = Regex.Replace(Clipboard.GetText(), @"\t|\n|\r", "");
            textboxInputUrl.Focus();
            textboxInputUrl.Select(textboxInputUrl.Text.Length, 0);
        }


    }
}
