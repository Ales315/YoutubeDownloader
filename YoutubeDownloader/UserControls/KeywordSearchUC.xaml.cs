using System.Windows.Controls;
using System.Windows.Input;
using YoutubeDownloader.ViewModels.Card;
using YoutubeDownloader.ViewModels.UserControl;

namespace YoutubeDownloader.UserControls
{
    /// <summary>
    /// Interaction logic for KeywordSearchUC.xaml
    /// </summary>
    public partial class KeywordSearchUC : UserControl
    {
        public KeywordSearchUC()
        {
            InitializeComponent();
        }
        private void SearchResultCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var card = ((SearchResultCard)sender).DataContext as SearchResultCardViewModel;
            if (card == null) return;

            var vm = this.DataContext as KeywordSearchViewModel;
            if (vm == null) return;

            switch (card.ResultType)
            {
                case Enums.SearchResultType.Video:
                    vm.GetVideoData(card.Url);
                    break;
                case Enums.SearchResultType.Playlist:
                    vm.GetPlaylistVideos(card.Url);
                    break;
            }
        }
    }
}
