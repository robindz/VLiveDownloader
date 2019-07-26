using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VLiveDownloader
{
    class Program
    {
        private static readonly string _appId = "8c6cc7b45d2568fb668be6e05b6e5a3b";
        private static readonly string _base = "https://api-vfan.vlive.tv/vproxy/channelplus/";
        private static readonly string _decodeChannelCode = "decodeChannelCode?app_id={0}&channelCode={1}";
        private static readonly string _channelVideoList = "getChannelVideoList?app_id={0}&channelSeq={1}&maxNumOfRows={2}&pageNo={3}";
        private static readonly string _videoData = "https://global.apis.naver.com/rmcnmv/rmcnmv/vod_play_videoInfo.json?key={0}&videoId={1}";

        private static HttpClient client;
        private static bool done = false;

        public static async Task Main(string[] args)
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36");

            List<VLiveVideo> videos = new List<VLiveVideo>();
            int channelSeq = await DecodeChannelCodeAsync("EDBF");
            int page = 1;
            //while (!done)
            //{
                videos.AddRange(await GetVideoListAsync(channelSeq, 50, page++));
            //}

            videos = videos.Where(x => x.VideoType != "PLAYLIST").ToList();
            videos.Sort((a, b) => b.VideoSeq.CompareTo(a.VideoSeq));

            List<(List<VLiveVideoDTO> videoData, List<VLiveVideoCaptionDTO> captionData)> data = new List<(List<VLiveVideoDTO> videoData, List<VLiveVideoCaptionDTO> captionData)>();

            foreach (var video in videos)
            {
                (string key, string videoId) cred = await GetCredentialsAsync(video.VideoSeq);
                data.Add(await GetVideoDataAsync(cred.key, cred.videoId));
            }

            Console.ReadKey();
        }

        private static async Task<int> DecodeChannelCodeAsync(string code)
        {
            string endpoint = string.Format($"{_base}{_decodeChannelCode}", _appId, code);
            string response = await client.GetStringAsync(endpoint);
            var data = JsonConvert.DeserializeObject<dynamic>(response);
            return data["result"]["channelSeq"];
        }

        private static async Task<List<VLiveVideo>> GetVideoListAsync(int channelSeq, int maxNumOfRows, int pageNo)
        {
            List<VLiveVideo> videos = new List<VLiveVideo>();

            string endpoint = string.Format($"{_base}{_channelVideoList}", _appId, channelSeq, maxNumOfRows, pageNo);
            string response = await client.GetStringAsync(endpoint);
            var data = JsonConvert.DeserializeObject<dynamic>(response);

            if (data["result"]["videoList"] == null)
            {
                done = true;
            }
            else
            {
                videos = JsonConvert.DeserializeObject<List<VLiveVideo>>(data["result"]["videoList"].ToString());
                if (videos.Count < 50)
                {
                    done = true;
                }
            }
            return videos;
        }

        private static async Task<(string, string)> GetCredentialsAsync(int videoSeq)
        {
            Regex regex = new Regex("vlive\\.video\\.init.+\\n.{4}(?<id>.+)\\\",\\n.{4}(?<key>.+)\\\"");

            string html = await client.GetStringAsync($"https://www.vlive.tv/video/{videoSeq}");

            MatchCollection mc = regex.Matches(html);

            foreach (Match m in mc)
            {
                if (m.Groups["id"] != null && m.Groups["key"] != null)
                {
                    return (m.Groups["key"].Value, m.Groups["id"].Value);
                }
            }

            return (string.Empty, string.Empty);
        }

        private static async Task<(List<VLiveVideoDTO>, List<VLiveVideoCaptionDTO>)> GetVideoDataAsync(string key, string videoId)
        {
            string endpoint = string.Format(_videoData, key, videoId);
            string response = await client.GetStringAsync(endpoint);
            var data = JsonConvert.DeserializeObject<dynamic>(response);

            List<VLiveVideoDTO> videos = JsonConvert.DeserializeObject<List<VLiveVideoDTO>>(data["videos"]["list"].ToString());
            List<VLiveVideoCaptionDTO> captions = new List<VLiveVideoCaptionDTO>();

            if (data["captions"] != null)
            {
                captions = JsonConvert.DeserializeObject<List<VLiveVideoCaptionDTO>>(data["captions"]["list"].ToString());
            }

            return (videos, captions);
        }
    }
}
