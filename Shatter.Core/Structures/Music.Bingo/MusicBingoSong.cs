using System;

using Newtonsoft.Json;

using Shatter.Core.Utils.Converters;

namespace Shatter.Core.Structures.Music.Bingo
{
	public class MusicBingoSong
    {
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
        [JsonProperty("song_link")]
        public Uri SongLink { get; set; }
        [JsonProperty("song_start")]
		[JsonConverter(typeof(CustomTimeSpanConverter))]
		public TimeSpan? SongStart { get; set; }
        [JsonProperty("song_end")]
		[JsonConverter(typeof(CustomTimeSpanConverter))]
        public TimeSpan? SongEnd { get; set; }
    }
}
