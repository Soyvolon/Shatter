using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace Shatter.Core.Utils
{
	public class Advice
    {
        public const string AdviceRequest = "http://api.adviceslip.com/advice";

        public int Id { get; private set; }
        public string Contents { get; private set; }

        private Advice(JObject res)
        {
			this.Id = res["slip"]?["id"]?.ToObject<int>() ?? 0;
			this.Contents = res["slip"]?["advice"]?.ToString() ?? "";
        }

        public static async Task<Advice?> GetAdvice()
        {
            var live = new HttpClient();

            try
            {
                var res = await live.GetAsync(AdviceRequest);

                var response = await res.Content.ReadAsStringAsync();

                var data = JObject.Parse(response);

                return new Advice(data);
            }
            catch
            {
                return null;
            }
        }
    }
}
