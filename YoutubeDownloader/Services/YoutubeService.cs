namespace YoutubeDownloader.Services;

using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Media;
using System.Windows;
using Humanizer;
using YoutubeDownloader.Enums;
using YoutubeDownloader.Helpers;
using YoutubeDownloader.Models;
using YoutubeDownloader.ViewModels;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Converter;
using YoutubeExplode.Search;
using YoutubeExplode.Videos.Streams;

public class YoutubeService
{
    private YoutubeClient _youtube;

    private ConcurrentQueue<VideoDownloadViewModel> _downloadQueue;
    public CancellationTokenSource DownloadCancellationToken = null!;
    public CancellationTokenSource GetMetadataCancellationToken = null!;
    public CancellationTokenSource SearchCancellationToken = null!;

    private Video? _videoData;
    private Video _video = null!;
    private readonly object _lock = new();

    private bool _busy;
    private string _lastQuery = string.Empty;

    public ObservableCollection<VideoDownloadViewModel> DownloadList { get; internal set; } = new();
    public ObservableCollection<SearchResultCardViewModel> SearchResultViewModels { get; set; } = new();

    public YoutubeService()
    {
        _youtube = new YoutubeClient();
        _downloadQueue = new ConcurrentQueue<VideoDownloadViewModel>();
    }
    public async Task<Video> GetVideoAsync(string url)
    {
        try
        {
            _video = await GetVideoMetadataAsync(url);
            return _video;
        }
        catch (Exception)
        {
            _video = null!;
            throw;
        }
    }

    private async Task<Video> GetVideoMetadataAsync(string url)
    {
        GetMetadataCancellationToken = new CancellationTokenSource();
        var video = await _youtube.Videos.GetAsync(url, GetMetadataCancellationToken.Token);
        _videoData = new();

        Thumbnail thumbnail = ThumbnailExtensions.TryGetWithHighestResolution(video.Thumbnails)!;
        if (thumbnail == null)
            thumbnail = video.Thumbnails[0];

        _videoData = new Video();
        _videoData.Thumbnail = await ThumbnailHelper.BitmapImageFromUrl(thumbnail.Url);
        _videoData.Url = url;
        _videoData.Title = video.Title;
        _videoData.Duration = video.Duration == null ? "Live" : ((TimeSpan)video.Duration).ToString(@"hh\:mm\:ss");
        _videoData.ChannelName = video.Author.ChannelTitle;
        _videoData.ViewCount = video.Engagement.ViewCount;
        _videoData.Date = video.UploadDate.Humanize(DateTime.Now, culture: System.Globalization.CultureInfo.CurrentCulture);

        return _videoData;
    }
    public async Task<Video> GetStreamData(Video videoData)
    {
        GetMetadataCancellationToken = new CancellationTokenSource();
        StreamManifest? streamManifest = null;
        try
        {
            streamManifest = await _youtube.Videos.Streams.GetManifestAsync(videoData.Url, GetMetadataCancellationToken.Token);
        }
        catch (OperationCanceledException)
        {
            _videoData = null!;
            throw;
        }

        var audioStreams = streamManifest.GetAudioOnlyStreams().OrderBy(x => x.Bitrate);
        var videoStreams = streamManifest.GetVideoOnlyStreams().Where(x => x.Container.Name == "mp4")
            .GroupBy(x => x.VideoResolution.Area)
            .Select(g => g.OrderByDescending(s => s.Bitrate).First()).Reverse();

        if (_videoData != null)
        {
            _videoData.AudioStreams = audioStreams;
            _videoData.VideoStreams = videoStreams;
        }

        videoData.AudioStreams = audioStreams;
        videoData.VideoStreams = videoStreams;
        return videoData;
    }
    public Video GetLastVideoData()
    {
        return _videoData!;
    }

    #region DOWNLOAD

    public void EnqueueDownload(VideoDownloadViewModel video)
    {
        string format = (Enum.GetName(typeof(DownloadFormat), video.DownloadFormat) ?? "WEBM").ToLower();
        string sanitizedTitle = SanitizeString(video.Title);
        video.FileName = $"{ServiceProvider.SettingsService.GetOutputPath()}\\{sanitizedTitle}.{format}";

        if (!FileAlreadyExists(video.FileName))
            return;

        DownloadList.Add(video);
        _downloadQueue.Enqueue(video);
        StartDownloads();
    }

    private string SanitizeString(string s)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        foreach (char c in invalidChars)
            s = s.Replace(c, '-');
        return s;
    }

    private void StartDownloads()
    {
        lock (_lock)
        {
            if (_busy)
                return;
            _busy = true;
        }
        Task.Run(async () =>
        {
            while (_downloadQueue.TryDequeue(out var videoDownload))
            {
                try
                {
                    videoDownload.IsDownloading = true;
                    videoDownload.CancellationToken = new CancellationTokenSource();

                    await DownloadVideo(videoDownload, videoDownload.CancellationToken.Token);
                    videoDownload.IsDownloading = false;

                    if (ServiceProvider.SettingsService.UserPreferences.UseNotifications)
                        SystemSounds.Beep.Play();
                }
                catch (OperationCanceledException)
                {
                    videoDownload.IsDownloading = false;
                    Progress<double> progress = new Progress<double>(p => videoDownload.Progress = p);
                    ((IProgress<double>)progress).Report(-0.5);
                    if (File.Exists(videoDownload.FileName))
                        File.Delete(videoDownload.FileName);
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
    /// <summary>
    /// Checks if filename exists, returns true if file does not exists or replace action is authorized by user
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    private bool FileAlreadyExists(string filename)
    {
        if (File.Exists(filename))
        {
            var dr = MessageBox.Show("File already exists \nDo you want to replace it?", "File exists", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (dr == MessageBoxResult.No)
                return false;
            return true;
        }
        return true;
    }

    public async Task DownloadVideo(VideoDownloadViewModel video, CancellationToken cancellationToken)
    {
        IStreamInfo[] streamInfo = [];
        ConversionRequestBuilder conversionBuilder;
        string format = (Enum.GetName(typeof(DownloadFormat), video.DownloadFormat) ?? "WEBM").ToLower();
        Progress<double> progress = new Progress<double>(p => video.Progress = p);
        switch (video.DownloadOption)
        {
            case DownloadMediaType.VideoWithAudio:
                streamInfo = [video.AudioStream, video.VideoStream];
                break;

            case DownloadMediaType.AudioOnly:
                streamInfo = [video.AudioStream];
                break;

            case DownloadMediaType.VideoOnly:
                streamInfo = [video.VideoStream];
                break;
        }
        conversionBuilder = new ConversionRequestBuilder($"{video.FileName}");
        conversionBuilder.SetContainer(format).SetPreset(ConversionPreset.Medium);
        await _youtube.Videos.DownloadAsync(streamInfo, conversionBuilder.Build(), progress, cancellationToken);
        ((IProgress<double>)progress).Report(1.0);
    }

    #endregion

    public async Task Search(string query)
    {
        if (query == _lastQuery)
            return;
        _lastQuery = query;

        SearchResultViewModels.Clear();
        SearchCancellationToken = new CancellationTokenSource();
        await foreach (var batch in _youtube.Search.GetResultBatchesAsync(searchQuery: query, cancellationToken: SearchCancellationToken.Token))
        {
            foreach (var item in batch.Items)
            {
                SearchResultCardViewModel resultViewModel = new();

                switch (item)
                {
                    case VideoSearchResult video:
                        resultViewModel.ResultType = SearchResultType.Video;
                        resultViewModel.Title = video.Title;
                        resultViewModel.ChannelName = video.Author.ChannelTitle;
                        resultViewModel.ThumbnailFlag = GetVideoDuration(video.Duration);
                        resultViewModel.Url = video.Url;
                        resultViewModel.ContentImage = await ThumbnailHelper.BitmapImageFromUrl(video.Thumbnails[0].Url);
                        break;

                    case PlaylistSearchResult playlist:
                        resultViewModel.ResultType = SearchResultType.Playlist;
                        resultViewModel.Title = playlist.Title;
                        resultViewModel.ChannelName = playlist.Author?.ChannelTitle ?? "Mix";
                        resultViewModel.Url = playlist.Url;
                        resultViewModel.ContentImage = await ThumbnailHelper.BitmapImageFromUrl(playlist.Thumbnails[0].Url);
                        break;

                    case ChannelSearchResult channel:
                        resultViewModel.ResultType = SearchResultType.Channel;
                        resultViewModel.Title = channel.Title;
                        resultViewModel.ChannelName = channel.Title;
                        resultViewModel.Url = channel.Url;
                        resultViewModel.ContentImage = await ThumbnailHelper.BitmapImageFromUrl(channel.Thumbnails[0].Url);
                        break;
                }

                SearchResultViewModels.Add(resultViewModel);
                continue;
            }
            return;
        }
    }

    private string GetVideoDuration(TimeSpan? duration)
    {
        return duration == null ? "Live" : ((TimeSpan)duration).ToString(@"hh\:mm\:ss");
    }
}