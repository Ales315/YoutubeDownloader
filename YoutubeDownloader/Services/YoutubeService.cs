﻿namespace YoutubeDownloader.Services;

using System;
using YoutubeDownloader.Models;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

public class YoutubeService
{
    private YoutubeClient _youtube;
    private VideoDataModel _video = null!;

    public YoutubeService()
    {
        _youtube = new YoutubeClient();
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
        var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(url);

        var audioStreams = streamManifest.GetAudioOnlyStreams().OrderBy(x => x.Bitrate);
        var videoStreams = streamManifest.GetVideoOnlyStreams().Where(x => x.Container.Name == "mp4").OrderBy(x => x.VideoResolution.Area);

        VideoDataModel videoData = new VideoDataModel();
        videoData.Url = url;
        videoData.Title = video.Title;
        videoData.Duration = video.Duration == null ? "Live" : ((TimeSpan)video.Duration).ToString(@"hh\:mm\:ss");
        videoData.ChannelName = video.Author.ChannelTitle;
        videoData.ThumbnailUrl = video.Thumbnails[0].Url;
        videoData.AudioStreams = audioStreams;
        videoData.VideoStreams = videoStreams;

        return videoData;
    }

    private void GetPlaylist(string url)
    {
        throw new NotImplementedException();
    }

    public async Task DownloadVideo(string url, Progress<double> progress)
    {
#warning TODO: Download video
        var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(url);
        var audioStreams = streamManifest.GetAudioOnlyStreams().OrderBy(x => x.Bitrate);
        var videoStreams = streamManifest.GetVideoOnlyStreams().Where(x => x.Container.Name == "mp4").OrderBy(x => x.VideoResolution.Area);
        var streamInfo = new IStreamInfo[] { audioStreams.First(), videoStreams.First() };
        //await _youtube.Videos.DownloadAsync(streamInfo,);
    }
}