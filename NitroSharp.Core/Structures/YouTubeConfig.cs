
using Newtonsoft.Json;

namespace NitroSharp.Structures
{
    public struct YouTubeConfig
    {
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }
    }
}
