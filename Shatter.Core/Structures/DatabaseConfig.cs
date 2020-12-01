
using Newtonsoft.Json;

namespace Shatter.Core.Structures
{
	public struct DatabaseConfig
    {
        [JsonProperty("data_source")]
        public string DataSource { get; private set; }

    }
}
