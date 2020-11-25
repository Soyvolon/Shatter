using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Shatter.Core.Structures.Music.Bingo
{
    public class MusicBingoSong
    {
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
        [JsonProperty("song_link")]
        public Uri SongLink { get; set; }
        [JsonProperty("song_start")]
        public TimeSpan? SongStart { get; set; }
        [JsonProperty("song_end")]
        public TimeSpan? SongEnd { get; set; }
    }
}
