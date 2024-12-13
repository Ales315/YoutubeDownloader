namespace YoutubeDownloader.Services;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows;
using Humanizer;
using YoutubeDownloader.Enums;
using YoutubeDownloader.Models;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

public class YoutubeService
{
    private YoutubeClient _youtube;
    private VideoDataModel _video = null!;
    private ConcurrentQueue<VideoDownloadModel> _downloadQueue;
    private bool _busy;
    private readonly object _lock = new();

    public YoutubeService()
    {
        _youtube = new YoutubeClient();
        _downloadQueue = new ConcurrentQueue<VideoDownloadModel>();
    }
    public async Task<VideoDataModel> GetVideoAsync(string url)
    {
        try
        {
            _video = await GetVideoMetadataAsync(url);
            return _video;
        }
        catch (Exception ex)
        {
            VideoDataModel videoData = new VideoDataModel();
            videoData.ErrorMessage = ex.Message;
            videoData.Title = string.Empty;
            videoData.Duration = string.Empty;
            videoData.ChannelName = string.Empty;
            videoData.ThumbnailUrl = string.Empty;

            _video = null!;
            return videoData;
        }
    }

    private async Task<VideoDataModel> GetVideoMetadataAsync(string url)
    {
        var video = await _youtube.Videos.GetAsync(url);


        VideoDataModel videoData = new VideoDataModel();
        videoData.Url = url;
        videoData.Title = video.Title;
        videoData.Duration = video.Duration == null ? "Live" : ((TimeSpan)video.Duration).ToString(@"hh\:mm\:ss");
        videoData.ChannelName = video.Author.ChannelTitle;
        videoData.ThumbnailUrl = video.Thumbnails[0].Url;
        videoData.ViewCount = video.Engagement.ViewCount;
        videoData.Date = video.UploadDate.Humanize(DateTime.Now, culture: System.Globalization.CultureInfo.CurrentCulture);

        return videoData;
    }
    public async Task<VideoDataModel> GetStreamData(VideoDataModel videoData)
    {
        var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(videoData.Url);

        var audioStreams = streamManifest.GetAudioOnlyStreams().OrderBy(x => x.Bitrate);
        var videoStreams = streamManifest.GetVideoOnlyStreams().Where(x => x.Container.Name == "mp4")
            .GroupBy(x => x.VideoResolution.Area)
            .Select(g => g.OrderByDescending(s => s.Bitrate).First()).Reverse();
        videoData.AudioStreams = audioStreams;
        videoData.VideoStreams = videoStreams;
        return videoData;
    }

    public void EnqueueDownload(VideoDownloadModel video)
    {
        _downloadQueue.Enqueue(video);
        StartDownloads();
    }

    private void StartDownloads()
    {
        lock (_lock)
        {
            if(_busy)
                return;
            _busy = true;
        }
        Task.Run(async () => 
        {
            while(_downloadQueue.TryDequeue(out var videoDownload))
            {
                try
                {
                    videoDownload.IsDownloading = true;
                    await DownloadVideo(videoDownload);
                    videoDownload.IsDownloading = false;
                }
                catch (Exception ex)
                {
                    videoDownload.IsDownloadFailed = true;
                    videoDownload.IsDownloading = false;
                    Progress<double> progress = new Progress<double>(p => videoDownload.Progress = p);
                    ((IProgress<double>)progress).Report(-1.0);
                    MessageBox.Show(ex.Message, "Download Failed!");
                }
            }
            lock (_lock)
            {
                _busy = false;
            }
                
        });
    }

    public async Task DownloadVideo(VideoDownloadModel video)
    {
        IStreamInfo[] streamInfo = [];
        ConversionRequestBuilder conversionBuilder;
        string format = (Enum.GetName(typeof(DownloadFormat), video.DownloadFormat) ?? "WEBM").ToLower();
        string fileName = $"{ServiceProvider.SettingsService.GetOutputPath()}\\{video.Title}.{format}";
        Progress<double> progress = new Progress<double>(p => video.Progress = p);
        switch (video.DownloadOption)
        {
            case DownloadMediaType.VideoWithAudio:
                streamInfo = [video.AudioStream, video.VideoStream];
                conversionBuilder = new ConversionRequestBuilder($"{fileName}");
                conversionBuilder.SetContainer(format).SetPreset(ConversionPreset.Medium);
                await _youtube.Videos.DownloadAsync(streamInfo, conversionBuilder.Build(), progress);
                break;

            case DownloadMediaType.AudioOnly:
                streamInfo = [video.AudioStream];
                conversionBuilder = new ConversionRequestBuilder($"{fileName}");
                conversionBuilder.SetContainer(format).SetPreset(ConversionPreset.VerySlow);
                await _youtube.Videos.DownloadAsync(streamInfo,conversionBuilder.Build(), progress);
                break;

            case DownloadMediaType.VideoOnly:
                streamInfo = [video.VideoStream];
                conversionBuilder = new ConversionRequestBuilder($"{fileName}");
                conversionBuilder.SetContainer(format).SetPreset(ConversionPreset.Medium);
                await _youtube.Videos.DownloadAsync(streamInfo, conversionBuilder.Build(), progress);
                break;
        }
        ((IProgress<double>)progress).Report(1.0);
    }
}