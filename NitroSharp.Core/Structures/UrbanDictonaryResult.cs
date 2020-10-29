using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace NitroSharp.Core.Structures
{
    public struct UrbanDictonaryResult
    {
        [JsonProperty("definition")]
        public string Definition { get; set; }

        [JsonProperty("permalink")]
        public string PermaLink { get; set; }

        [JsonProperty("thumbs_up")]
        public int ThumbsUp { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("word")]
        public string Word { get; set; }

        [JsonProperty("defid")]
        public int DefId { get; set; }

        [JsonProperty("written_on")]
        public string WrittenOn { get; set; }

        [JsonProperty("example")]
        public string Example { get; set; }

        [JsonProperty("thumbs_down")]
        public int ThumbsDown { get; set; }

    }
}
