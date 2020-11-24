using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Shatter.Core.Structures.Music.Bingo
{
    public class MusicBingoGame
    {
        [JsonProperty("game_title")]
        public string GameName { get; set; }
        [JsonProperty("songs")]
        public List<MusicBingoSong> Songs { get; set; }

        [JsonIgnore]
        public ulong GameStarter { get; set; }
    }
}
