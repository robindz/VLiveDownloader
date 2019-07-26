using Newtonsoft.Json;
using System;

namespace VLiveDownloader
{
    public class VLiveVideo
    {
        public int VideoSeq { get; set; }
        public string VideoType { get; set; }
        public string Title { get; set; }
        public int PlayCount { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public string Thumbnail { get; set; }
        public int PickSortOrder { get; set; }
        public string ScreenOrientation { get; set; }
        public DateTime WillStartAt { get; set; }
        public DateTime WillEndAt { get; set; }
        public string UpcomingYn { get; set; }
        public string SpecialLiveYn { get; set; }
        public string LiveThumbYn { get; set; }
        public object ProductId { get; set; }
        public object PackageProductId { get; set; }
        public string ProductType { get; set; }
        public int PlayTime { get; set; }
        public string ChannelPlusPublicYn { get; set; }
        public string ExposeStatus { get; set; }
        public string RepresentChannelName { get; set; }
        public string RepresentChannelProfileImg { get; set; }
        public DateTime OnAirStartAt { get; set; }
        [JsonProperty("@type")]
        public string Type { get; set; }
    }
}
