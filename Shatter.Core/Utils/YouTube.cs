using System;
using System.Threading.Tasks;

using Google.Apis.Services;
using Google.Apis.YouTube.v3;

using Shatter.Core.Structures;

namespace Shatter.Core.Utils
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
                ApplicationName = "Shatter",
            });
        }

        public static async Task<Tuple<string, string>?> SerachForSingle(string searchTerm)
        {
            if (Service is null) throw new Exception("YouTube service not initialized");

            var search = Service.Search.List("snippet");
            search.MaxResults = 1;
            //search.Type = "video";
            search.Q = searchTerm;

            var response = await search.ExecuteAsync();

            if (response.Items.Count <= 0)
                return null;

            switch (response.Items[0].Id.Kind)
            {
                case "youtube#video":
                    return new Tuple<string, string>($"https://youtube.com/watch?v={response.Items[0].Id.VideoId}", response.Items[0].Snippet.Title);

                case "youtube#channel":
                    return new Tuple<string, string>($"https://youtube.com/channel/{response.Items[0].Id.ChannelId}", response.Items[0].Snippet.Title);

                case "youtube#playlist":
                    return new Tuple<string, string>($"https://youtube.com/playlist?list={response.Items[0].Id.PlaylistId}", response.Items[0].Snippet.Title);

                default:
                    return null;
            }
        }
    }
}
