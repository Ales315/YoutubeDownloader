namespace YoutubeDownloader.Services;

using System;
using YoutubeDownloader.Models;
using YoutubeExplode;
public class YoutubeService
{
    private YoutubeClient _youtube;
    public YoutubeService()
    {
        _youtube = new YoutubeClient();
    }
    public async Task<VideoMetadataModel> GetInfo(string url)
    {
        var video = await _youtube.Videos.GetAsync(url);
        VideoMetadataModel metadata = new VideoMetadataModel();
        metadata.Title = video.Title;
        metadata.Duration = video.Duration == null ? "Live" : ((TimeSpan)video.Duration).ToString(@"hh\:mm\:ss");
        metadata.ChannelName = video.Author.ChannelTitle;
        metadata.ThumbnailUrl = video.Thumbnails[0].Url;
        return metadata;
    }

    public async Task DownloadVideo(string url)
    {
#warning TODO: Download video

    }
}
