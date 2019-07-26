namespace VLiveDownloader
{
    public class VLiveVideoDTO
    {
        public string Id { get; set; }
        public bool UseP2P { get; set; }
        public double Duration { get; set; }
        public long Size { get; set; }
        public string Type { get; set; }
        public EncodingOption EncodingOption { get; set; }
        public Bitrate Bitrate { get; set; }
        public string P2PMetaUrl { get; set; }
        public string P2PUrl { get; set; }
        public string Source { get; set; }
    }

    public class EncodingOption
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Profile { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Bitrate
    {
        public double Video { get; set; }
        public double Audio { get; set; }
    }
}
