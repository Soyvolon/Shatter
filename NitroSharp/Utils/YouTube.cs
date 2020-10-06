using System;
using System.Threading.Tasks;

using Google.Apis.Services;
using Google.Apis.YouTube.v3;

using NitroSharp.Structures;

namespace NitroSharp.Utils
{
    public static class YouTube
    {
        private static YouTubeService? Service { get; set; }

        public static void Initalize(YouTubeConfig yTCfg)
        {
            // Do setup work
            Service = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = yTCfg.ApiKey,
                ApplicationName = "NitroSharp",
            });
        }

        public static async Task<Tuple<string, string>?> SerachForSingle(string searchTerm)
        {
            if (Service is null) throw new Exception("YouTube service not initialized");

            var search = Service.Search.List("snippet");
            search.MaxResults = 1;
            search.Type = "video";
            search.Q = searchTerm;

            var response = await search.ExecuteAsync();

            if (response.Items.Count <= 0)
                return null;

            return new Tuple<string, string>($"https://youtube.com/watch?v={response.Items[0].Id}", response.Items[0].Snippet.Title);
        }
    }
}
