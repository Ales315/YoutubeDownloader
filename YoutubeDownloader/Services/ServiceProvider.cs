namespace YoutubeDownloader.Services
{
    public static class ServiceProvider
    {
        public static SettingsService SettingsService { get; } = new SettingsService();
    }
}
