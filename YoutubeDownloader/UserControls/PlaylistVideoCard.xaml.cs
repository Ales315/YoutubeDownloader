using System.Windows;
using System.Windows.Controls;
using YoutubeDownloader.Enums;
using YoutubeDownloader.Services;
using YoutubeDownloader.ViewModels.Card;

namespace YoutubeDownloader.UserControls
{
    /// <summary>
    /// Interaction logic for VideoDownloadCard.xaml
    /// </summary>
    public partial class PlaylistVideoCard : UserControl
    {
        public PlaylistVideoCard()
        {
            InitializeComponent();
            ServiceProvider.ThemeService.ThemeChanged += OnThemeChanged;
        }

        private void OnThemeChanged(object? sender, bool e)
        {
            BorderBackground.Background = ServiceProvider.ThemeService.GetSurfaceColorBrush();
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

        private void CheckContentType()
        {
            var vm = this.DataContext as SearchResultCardViewModel ?? null!;
            if (vm == null)
                return;
            ContentImageBorder.Width = 110;
            ContentImageBorder.Height = 62;
            ContentImageBorder.CornerRadius = new CornerRadius(4);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CheckContentType();
        }
    }
}
