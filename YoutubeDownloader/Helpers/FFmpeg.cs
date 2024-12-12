using System.Diagnostics;
using System.IO;

namespace YoutubeDownloader.Helpers
{
    public static class FFmpeg
    {
        public static void CheckFFmpegInstallation()
        {
            ProcessStartInfo info = new("cmd.exe")
            {
                Arguments = "/c ffmpeg -version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = false,
                UseShellExecute = false
            };
            Process process = new();
            process.StartInfo = info;
            process.Start();
            process.BeginOutputReadLine();
            process.OutputDataReceived += (s, e) =>
            {
                var output = e.Data;
            };
        }

        public static bool InstallFFmpeg()
        {
            return false;
        }
    }
}
