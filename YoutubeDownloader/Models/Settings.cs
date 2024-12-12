using System.Windows;
using YoutubeDownloader.Enums;

namespace YoutubeDownloader.Models
{
    public class Settings
    {
        public string FFmpegPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

        public DownloadOption DownloadOptionPreference { get; set; }
        public DownloadFormat AudioFormatPreference { get; set; }
        public DownloadFormat VideoFormatPreference { get; set; }
    }
}
