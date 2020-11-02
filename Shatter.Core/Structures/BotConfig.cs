
using Newtonsoft.Json;

namespace Shatter.Core.Structures
{
    public struct BotConfig
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("shards")]
        public int Shards { get; set; }

        [JsonProperty("superadmins")]
        public ulong[] Admins { get; set; }
    }
}
