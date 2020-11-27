
using Newtonsoft.Json;

namespace Shatter.Core.Structures
{
	public struct YouTubeConfig
    {
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }
    }
}
