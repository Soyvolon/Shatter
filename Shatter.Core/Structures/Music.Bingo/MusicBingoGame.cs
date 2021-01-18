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
		[JsonIgnore]
		public bool NoAutoStop { get; set; } = false;
		[JsonIgnore]
		public bool Completed { get; set; } = false;
		[JsonIgnore]
		public bool RunningCompletion { get; set; } = false;

        public MusicBingoGame()
        {
			this.QueuedSongs = new();
			this.BingoBoards = new();
			this.PlayedSongs = null;
        }

        public void Initalize(IEnumerable<ulong> playerIds)
        {
			this.Songs.Shuffle(); // shuffle the list
            foreach (var s in this.Songs) // and queue the songs
			{
				this.QueuedSongs.Enqueue(s);
			}
			// create the played songs list
			this.PlayedSongs = new(this.Songs.Count);

            // build the boards
            foreach(var player in playerIds)
            {
                var board = new MusicBingoBoard();
                board.PopulateBoard(this.Songs);
				this.BingoBoards[player] = board;
            }
        }

        public MusicBingoSong? GetNextSong()
        {
            if(this.QueuedSongs.TryDequeue(out MusicBingoSong? song))
			{
				this.PlayedSongs?.Add(song);
			}

			return song;
        }

		public void EndGame()
		{
			if (!RunningCompletion)
				RunningCompletion = true;
			else
				Completed = true;
		}
	}
}
