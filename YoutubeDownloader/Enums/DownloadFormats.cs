namespace YoutubeDownloader.Enums
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FormatCategoryAttribute : Attribute
    {
        public string Category { get; set; } = string.Empty;
        public FormatCategoryAttribute(string category) => Category = category;
    }
    public enum DownloadFormat
    {
        [FormatCategory("Audio")]
        MP3,

        [FormatCategory("Audio")]
        FLAC,

        [FormatCategory("Audio")]
        WAV,

        [FormatCategory("Audio")]
        AIFF,

        [FormatCategory("Audio")]
        OGG,

        [FormatCategory("Video")]
        MP4,

        [FormatCategory("Video")]
        AVI,

        [FormatCategory("Video")]
        MOV,

        [FormatCategory("Video")]
        WMV,

        [FormatCategory("AudioVideo")]
        WEBM
    }
}
