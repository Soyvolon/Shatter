
using Newtonsoft.Json;

namespace NitroSharp.Core.Structures.Trivia
{
    public struct TriviaCategory
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}