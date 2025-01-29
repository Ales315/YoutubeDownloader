using System.Windows;
using System.Windows.Controls;
using WpfAnimatedGif;
using YoutubeDownloader.ViewModels.Card;
using YoutubeDownloader.ViewModels.UserControl;

namespace YoutubeDownloader.UserControls
{
    /// <summary>
    /// Interaction logic for KeywordSearchUC.xaml
    /// </summary>
    public partial class PlaylistUC : UserControl
    {
        public PlaylistUC()
        {
            InitializeComponent();
            gifLoadingVideos.IsVisibleChanged += (s, e) => SetGifPlaybackFramePosition(s);
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
        private void gifLoadingVideos_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var img = sender as Image;
            var controller = ImageBehavior.GetAnimationController(gifLoadingVideos);
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

        private void PlaylistVideoCard_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var card = ((PlaylistVideoCard)sender).DataContext as PlaylistVideoCardViewModel;
            if (card == null) return;

            var vm = this.DataContext as KeywordSearchViewModel;
            if (vm == null) return;

            card.IsChecked = card.IsChecked ? false : true;
        }
    }
}
