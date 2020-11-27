using System;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Shatter.Core.Structures;

namespace Shatter.Core.Utils
{
	public class Fortune
    {
        private const string FortuneRequest = "http://fortunecookieapi.herokuapp.com/v1/cookie";

        public FortuneData Data;
        public FortuneLesson Lesson;
        public FortuneLotto Lotto;

        private Fortune(JObject json)
        {
            Data = json["fortune"]?.ToObject<FortuneData>() ?? throw new Exception("API Error");
            Lesson = json["lesson"]?.ToObject<FortuneLesson>() ?? throw new Exception("API Error");
            Lotto = json["lotto"]?.ToObject<FortuneLotto>() ?? throw new Exception("API Error");
        }

        public static async Task<Fortune?> GetFortuneCookie()
        {
            var live = new HttpClient();

            try
            {
                var res = await live.GetAsync(FortuneRequest);

                var response = await res.Content.ReadAsStringAsync();

                var array = JArray.Parse(response);

                JObject data = array[0].ToObject<JObject>();

                return new Fortune(data);
            }
            catch
            {
                return null;
            }
        }
    }
}
