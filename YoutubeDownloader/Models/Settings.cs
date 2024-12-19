using YoutubeDownloader.Enums;

namespace YoutubeDownloader.Models
{
    public class Settings
    {
        public event EventHandler<bool>? AutoDownloadChanged;
        public string FFmpegPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

        public DownloadMediaType MediaTypePreference { get; set; } = DownloadMediaType.VideoWithAudio;
        public DownloadFormat AudioFormatPreference { get; set; } = DownloadFormat.MP3;
        public DownloadFormat VideoFormatPreference { get; set; } = DownloadFormat.MP4;
        public ThemeStyles ThemePreference { get; set; } = ThemeStyles.System;
        public bool UseNotifications { get; set; } = true;
        public bool AutoDownload {  get; set; } = false;
    }
}
