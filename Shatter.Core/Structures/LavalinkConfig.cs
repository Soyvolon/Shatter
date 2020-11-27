
using Newtonsoft.Json;

namespace Shatter.Core.Structures
{
	public struct LavalinkConfig
    {
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
