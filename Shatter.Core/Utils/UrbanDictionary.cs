using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Shatter.Core.Structures;

namespace Shatter.Core.Utils
{
	public static class UrbanDictionary
    {
        private const string RequestString = "https://api.urbandictionary.com/v0/define?term=";

        public static async Task<List<UrbanDictonaryResult>?> Search(string term)
        {
            var live = new HttpClient();

            try
            {
                var res = await live.GetAsync(RequestString + term);

                var response = await res.Content.ReadAsStringAsync();

                var json = JObject.Parse(response);

                return json["list"]?.ToObject<List<UrbanDictonaryResult>>() ?? null;
            }
            catch
            {
                return null;
            }
        }
    }
}
