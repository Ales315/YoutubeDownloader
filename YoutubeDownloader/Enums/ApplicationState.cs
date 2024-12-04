namespace YoutubeDownloader.Enums
{
    public enum AppState
    {
        FirstOpening,
        AnalyzingUrl,
        VideoFound,
        VideoStreamsFound,
        VideoNotFound,
        Downloading,
        DownloadCompleted,
        AnalyzingStreams
    }
}
