
using Newtonsoft.Json;

namespace Shatter.Core.Structures
{
	public struct FortuneData
    {
        [JsonProperty("message")]
        public string Messge { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public struct FortuneLesson
    {
        [JsonProperty("english")]
        public string English { get; set; }
        [JsonProperty("chinese")]
        public string Chinese { get; set; }
        [JsonProperty("pronunciation")]
        public string Pronunciation { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public struct FortuneLotto
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("numbers")]
        public int[] Numbers { get; set; }
    }
}
