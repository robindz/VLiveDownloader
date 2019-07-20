using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VLiveDownloader
{
    class Program
    {
        private readonly string baseUrl = "https://api-vfan.vlive.tv/vproxy/channelplus/getChannelVideoList?app_id=8c6cc7b45d2568fb668be6e05b6e5a3b&gcc=US&channelSeq=";

        private static HttpClient client;

        public static async Task Main(string[] args)
        {
            client = new HttpClient();
            string html = await client.GetStringAsync(args[0]);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            string snsShareImg = doc.DocumentNode
                .Descendants("meta")
                .Where(x => x.GetAttributeValue("property", "") == "og:image")
                .FirstOrDefault()
                .GetAttributeValue("content", "");

            Regex regex = new Regex(@"^.+g_(\d+).+$");
            int channelSeq = int.Parse(regex.Match(snsShareImg).Groups[1].Value);

            Console.WriteLine(channelSeq);
            Console.ReadKey();
        }
    }
}
