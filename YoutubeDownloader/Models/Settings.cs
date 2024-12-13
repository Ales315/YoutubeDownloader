using System.Windows;
using YoutubeDownloader.Enums;

namespace YoutubeDownloader.Models
{
    public class Settings
    {
        public string FFmpegPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

        public DownloadMediaType MediaTypePreference { get; set; } = DownloadMediaType.VideoWithAudio;
        public DownloadFormat AudioFormatPreference { get; set; } = DownloadFormat.MP3;
        public DownloadFormat VideoFormatPreference { get; set; } = DownloadFormat.MP4;
    }
}
