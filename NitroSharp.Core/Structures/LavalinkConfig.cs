
using Newtonsoft.Json;

namespace NitroSharp.Core.Structures
{
    public struct LavalinkConfig
    {
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
