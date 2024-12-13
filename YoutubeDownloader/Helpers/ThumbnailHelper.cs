using System.Windows.Media.Imaging;

namespace YoutubeDownloader.Helpers
{
    public static class ThumbnailHelper
    {
        public static BitmapImage ThumbnailUrlToBitmapImage(string url)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(url, uriKind: UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }
    }
}
