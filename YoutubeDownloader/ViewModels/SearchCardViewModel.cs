using System.Windows.Media.Imaging;
using YoutubeDownloader.Enums;

namespace YoutubeDownloader.ViewModels
{
    public class SearchCardViewModel
    {
        public SearchResultType ResultType { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;
        public string ThumbnailFlag { get; set; } = string.Empty;
        public string ViewCount { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public BitmapImage ContentImage { get; set; } = null!;
    }
}
