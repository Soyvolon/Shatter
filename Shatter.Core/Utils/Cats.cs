using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Shatter.Core.Structures;

namespace Shatter.Core.Utils
{
	public static class Cats
    {
        private const string Api = "https://api.thecatapi.com/v1/images/search";
        private const string Order = "order=";
        private const string ImageType = "mime_types=";

        private static string GetRequestString(params string[]? args)
        {
            var request = Api;

            if (!(args is null))
            {
                string order = "";
                string typeTree = "";

                request += "?";

                foreach (var arg in args)
                {
                    if (arg.Contains("random") || arg.Contains("asc") || arg.Contains("desc"))
                    {
                        order = arg + "&";
                    }

                    if (arg.Contains("gif") || arg.Contains("jpg") || arg.Contains("png"))
					{
						typeTree += arg + ",";
					}
				}

                if (order.Length > 0)
				{
					request += Order + order;
				}

				if (typeTree.Length > 0)
				{
					request += ImageType + typeTree[..(typeTree.Length - 1)] + "&";
				}

				request = request[..(request.Length - 1)];
            }

            return request;
        }

        public static async Task<CatData[]?> GetCatDataAsync(params string[]? args)
        {
            var http = new HttpClient();

            var request = GetRequestString(args);

            var res = await http.GetAsync(request);

            var jsonString = await res.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<CatData[]>(jsonString);

            return data;
        }

        public static async Task<CatData> GetCatByIdAsync(string id)
        {
            var http = new HttpClient();

            var res = await http.GetAsync($"https://api.thecatapi.com/v1/images/{id}");

            var json = await res.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<CatData>(json);

            return data;
        }
    }
}
