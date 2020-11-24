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
        public DateTime? SongStart { get; set; }
        [JsonProperty("song_end")]
        public DateTime? SongEnd { get; set; }
    }
}
