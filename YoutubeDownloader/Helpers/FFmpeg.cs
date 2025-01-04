using System.Diagnostics;

namespace YoutubeDownloader.Helpers
{
    public static class FFmpeg
    {
        private static string _outputData = string.Empty;
        public static bool CheckFFmpegInstallation()
        {
            ProcessStartInfo info = new("cmd.exe")
            {
                Arguments = "/c ffmpeg -version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process process = new();
            process.StartInfo = info;
            process.Start();
            process.BeginOutputReadLine();
            process.OutputDataReceived += (s, e) =>
            {
                _outputData = e.Data ?? string.Empty;
                process.Close();
            };
            process.WaitForExit();
            return _outputData.Contains("ffmpeg version");
        }

        //todo: prompt ffmpeg install
        public static bool InstallFFmpeg()
        {
            return false;
        }
    }
}
