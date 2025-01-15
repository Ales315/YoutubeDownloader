using System.IO;
using System.Net.Http;
using System.Windows.Media.Imaging;

namespace YoutubeDownloader.Helpers
{
    public static class ThumbnailHelper
    {
        public static async Task<BitmapImage> BitmapImageFromUrl(string url)
        {
            try
            {

                // Validate and create the URI
                if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult) ||
                    (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                {
                    url = "https:" + url;
                }
                using var httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                byte[] imageData = await httpClient.GetByteArrayAsync(url);

                using var stream = new MemoryStream(imageData);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze(); // Optional: Freeze for thread safety

                return bitmap;
            }
            catch (Exception)
            {
                return null!;
            }
            
        }
    }
}
