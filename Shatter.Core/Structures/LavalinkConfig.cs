
using Newtonsoft.Json;

namespace Shatter.Core.Structures
{
	public struct LavalinkConfig
    {
        [JsonProperty("password")]
        public string Password { get; set; }
		[JsonProperty("ip")]
		public string Ip { get; set; }
		[JsonProperty("port")]
		public int Port { get; set; }
    }
}
