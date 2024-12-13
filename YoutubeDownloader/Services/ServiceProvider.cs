using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader.Services
{
    public static class ServiceProvider
    {
        public static SettingsService SettingsService { get; } = new SettingsService();
        public static YoutubeService YoutubeService { get; } = new YoutubeService();
        public static ControlStateHandler StateHandler { get; } = new ControlStateHandler();
    }
}
