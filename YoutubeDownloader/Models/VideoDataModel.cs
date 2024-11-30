﻿using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Models
{
    public class VideoDataModel
    {
        public string Url { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public IEnumerable<VideoOnlyStreamInfo> VideoStreams { get; set; } = [];
        public IEnumerable<AudioOnlyStreamInfo> AudioStreams { get; set; } = [];
    }
}