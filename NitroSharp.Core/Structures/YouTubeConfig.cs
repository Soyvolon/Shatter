
using Newtonsoft.Json;

namespace NitroSharp.Core.Structures
{
    public struct YouTubeConfig
    {
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }
    }
}
