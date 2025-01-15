using System.Windows;
using System.Windows.Controls;
using YoutubeDownloader.Enums;
using YoutubeDownloader.Services;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader.UserControls
{
    /// <summary>
    /// Interaction logic for VideoDownloadCard.xaml
    /// </summary>
    public partial class SearchResultCard : UserControl
    {
        public SearchResultCard()
        {
            InitializeComponent();
            ServiceProvider.ThemeService.ThemeChanged += OnThemeChanged;
        }

        private void OnThemeChanged(object? sender, bool e)
        {
            BorderBackground.Background = ServiceProvider.ThemeService.GetSurfaceColorBrush();
        }

        private void CheckContentType()
        {
            var vm = this.DataContext as SearchResultCardViewModel ?? null!;
            if (vm == null) 
                return;
            ContentImageBorder.Width = 110;
            ContentImageBorder.Height = 62;
            ContentImageBorder.CornerRadius = new CornerRadius(4);
            IconPlaylist.Visibility = Visibility.Collapsed;
            switch (vm.ResultType)
            {
                case SearchResultType.Channel:
                    TextblockTitle.Margin = new Thickness(15, 20, 0, 0);
                    ContentImageBorder.Margin = new Thickness(20, 0, 0, 0);
                    ContentImageBorder.Width = 62;
                    ContentImageBorder.Height = 62;
                    ContentImageBorder.CornerRadius = new CornerRadius(ContentImageBorder.Width/2);
                    TextBlockChannelName.Visibility = Visibility.Collapsed;
                    BorderThumbnailFlags.Visibility = Visibility.Collapsed;
                    break;

                case SearchResultType.Video:
                    break;

                case SearchResultType.Playlist:
                    TextBlockFlagValue.Visibility = Visibility.Collapsed;
                    IconPlaylist.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void VideoCard_Loaded(object sender, RoutedEventArgs e)
        {
            CheckContentType();
        }

        private void BorderBackground_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BorderBackground.Background = ServiceProvider.ThemeService.GetSurfaceHoverColorBrush();
            DropShadowEffectMainBorder.Opacity = 0.4;
        }

        private void BorderBackground_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BorderBackground.Background = ServiceProvider.ThemeService.GetSurfaceColorBrush();
            DropShadowEffectMainBorder.Opacity = 0.25;
        }
    }
}
