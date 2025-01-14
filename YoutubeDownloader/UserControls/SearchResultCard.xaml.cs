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
            CheckContentType();
        }

        private void CheckContentType()
        {
            var vm = this.DataContext as SearchResultCardViewModel ?? null!;
            if (vm == null) 
                return;
            ContentImageBorder.Width = 80;
            ContentImageBorder.Height = 45;
            ContentImageBorder.CornerRadius = new CornerRadius(4);
            IconPlaylist.Visibility = Visibility.Collapsed;
            switch (vm.ResultType)
            {
                case SearchResultType.Channel:
                    ContentImageBorder.Width = 45;
                    ContentImageBorder.Height = 45;
                    ContentImageBorder.CornerRadius = new CornerRadius(25);
                    TextBlockChannelName.Visibility = Visibility.Collapsed;
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
        }
    }
