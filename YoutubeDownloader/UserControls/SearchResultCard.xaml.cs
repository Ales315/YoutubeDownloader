using System.Windows;
using System.Windows.Controls;
using YoutubeDownloader.Enums;
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
                    ContentImageBorder.Width = 62;
                    ContentImageBorder.Height = 62;
                    ContentImageBorder.CornerRadius = new CornerRadius(ContentImageBorder.Width/2);
                    TextBlockChannelName.Visibility = Visibility.Collapsed;
                    BorderThumbnailFlags.Visibility = Visibility.Collapsed;
                    break;

                case SearchResultType.Video:
                    break;

                case SearchResultType.Playlist:
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
    }
    }
