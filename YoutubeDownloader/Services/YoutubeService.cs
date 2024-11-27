namespace YoutubeDownloader.Services;

using System;
using YoutubeDownloader.Models;
using YoutubeExplode;
using YoutubeExplode.Videos;

public class YoutubeService
{
    private YoutubeClient _youtube;
    public YoutubeService()
    {
        _youtube = new YoutubeClient();
    }
    public async Task<VideoMetadataModel> GetInfo(string url)
    {
        VideoMetadataModel metadata = new VideoMetadataModel();
        try
        {
            var video = await _youtube.Videos.GetAsync(url);
            metadata.Title = video.Title;
            metadata.Duration = video.Duration == null ? "Live" : ((TimeSpan)video.Duration).ToString(@"hh\:mm\:ss");
            metadata.ChannelName = video.Author.ChannelTitle;
            metadata.ThumbnailUrl = video.Thumbnails[0].Url;
            return metadata;
        }
        catch (Exception ex)
        {
            metadata.ErrorMessage = ex.Message;
            metadata.Title = string.Empty;
            metadata.Duration = string.Empty;
            metadata.ChannelName = string.Empty;
            metadata.ThumbnailUrl = string.Empty;
            return metadata;
        }
    }

    public async Task DownloadVideo(string url)
    {
#warning TODO: Download video

    }
}
