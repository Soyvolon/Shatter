using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Newtonsoft.Json;

using Shatter.Core.Extensions;

namespace Shatter.Core.Structures.Music.Bingo
{
	public class MusicBingoGame
    {
        [JsonProperty("game_title")]
        public string GameName { get; set; }
        [JsonProperty("songs")]
        public List<MusicBingoSong> Songs { get; set; }
        [JsonProperty("introduction")]
        public Uri? Introduction { get; set; }
        [JsonProperty("epilogue")]
        public Uri? Epilogue { get; set; }

        [JsonIgnore]
        public ulong GameStarter { get; set; }
        [JsonIgnore]
        private ConcurrentQueue<MusicBingoSong> QueuedSongs { get; init; }
        [JsonIgnore]
        public ConcurrentDictionary<ulong, MusicBingoBoard> BingoBoards { get; init; }
        [JsonIgnore]
        public List<MusicBingoSong>? PlayedSongs { get; private set; }

        public MusicBingoGame()
        {
            QueuedSongs = new();
            BingoBoards = new();
            PlayedSongs = null;
        }

        public void Initalize(IEnumerable<ulong> playerIds)
        {
            Songs.Shuffle(); // shuffle the list
            foreach (var s in Songs) // and queue the songs
			{
				QueuedSongs.Enqueue(s);
			}
			// create the played songs list
			PlayedSongs = new(Songs.Count);

            // build the boards
            foreach(var player in playerIds)
            {
                var board = new MusicBingoBoard();
                board.PopulateBoard(Songs);
                BingoBoards[player] = board;
            }
        }

        public MusicBingoSong? GetNextSong()
        {
            if(QueuedSongs.TryDequeue(out MusicBingoSong? song))
			{
				PlayedSongs?.Add(song);
			}

			return song;
        }
    }
}
