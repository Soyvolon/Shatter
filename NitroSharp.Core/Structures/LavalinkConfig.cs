
using Newtonsoft.Json;

namespace NitroSharp.Structures
{
    public struct LavalinkConfig
    {
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
