using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfAnimatedGif;
using YoutubeDownloader.Services;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader.UserControls
{
    /// <summary>
    /// Interaction logic for VideoSelectedCard.xaml
    /// </summary>
    public partial class VideoUC : UserControl
    {
        public VideoUC()
        {
            InitializeComponent();
            borderThumbnail.BorderBrush = ServiceProvider.ThemeService.GetPrimaryColorBrush();

            imgLoadingGifVideo.IsVisibleChanged += (s, e) => SetGifPlaybackFramePosition(s);
            imgLoadingGifStreams.IsVisibleChanged += (s, e) => SetGifPlaybackFramePosition(s);
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

        private void SearchResultCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var card = ((SearchResultCard)sender).DataContext as SearchResultCardViewModel;
            if (card == null) return;

            var vm = this.DataContext as HomePageViewModel;
            if (vm == null) return;

            switch (card.ResultType)
            {
                case Enums.SearchResultType.Video:
                    vm.Url = card.Url;
                    //vm.GetVideoDataCommand.Execute(null);
                    break;
            }
        }
    }
}
