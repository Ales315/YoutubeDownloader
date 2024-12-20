﻿namespace YoutubeDownloader.Services;

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
using YoutubeExplode.Videos.Streams;

public class YoutubeService
{
    private YoutubeClient _youtube;
    private VideoDataModel _video = null!;
    private ConcurrentQueue<VideoDownloadViewModel> _downloadQueue;
    private bool _busy;
    private VideoDataModel? _videoData;
    private readonly object _lock = new();
    public CancellationTokenSource DownloadCancellationToken = null!;
    public CancellationTokenSource GetMetadataCancellationToken = null!;

    public ObservableCollection<VideoDownloadViewModel> DownloadList { get; internal set; } = new ObservableCollection<VideoDownloadViewModel>();

    public YoutubeService()
    {
        _youtube = new YoutubeClient();
        _downloadQueue = new ConcurrentQueue<VideoDownloadViewModel>();
    }
    public async Task<VideoDataModel> GetVideoAsync(string url)
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

    private async Task<VideoDataModel> GetVideoMetadataAsync(string url)
    {
        GetMetadataCancellationToken = new CancellationTokenSource();
        var video = await _youtube.Videos.GetAsync(url, GetMetadataCancellationToken.Token);
        _videoData = new();

        Thumbnail thumbnail = ThumbnailExtensions.TryGetWithHighestResolution(video.Thumbnails)!;
        if (thumbnail == null)
            thumbnail = video.Thumbnails[0];

        _videoData = new VideoDataModel();
        _videoData.Thumbnail = ThumbnailHelper.ThumbnailUrlToBitmapImage(thumbnail.Url);
        _videoData.Url = url;
        _videoData.Title = video.Title;
        _videoData.Duration = video.Duration == null ? "Live" : ((TimeSpan)video.Duration).ToString(@"hh\:mm\:ss");
        _videoData.ChannelName = video.Author.ChannelTitle;
        _videoData.ViewCount = video.Engagement.ViewCount;
        _videoData.Date = video.UploadDate.Humanize(DateTime.Now, culture: System.Globalization.CultureInfo.CurrentCulture);

        return _videoData;
    }
    public async Task<VideoDataModel> GetStreamData(VideoDataModel videoData)
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

    public VideoDataModel GetLastVideoData()
    {
        return _videoData!;
    }
}