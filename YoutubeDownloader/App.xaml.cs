using System.Windows;
using YoutubeDownloader.Helpers;
using YoutubeDownloader.Services;

namespace YoutubeDownloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (!FFmpeg.CheckFFmpegInstallation())
                MessageBox.Show("This application uses FFmpeg. Please install latest version before proceeding", "FFmpeg not found!");
            //todo: prompt user for auto install
        }
    }
}
