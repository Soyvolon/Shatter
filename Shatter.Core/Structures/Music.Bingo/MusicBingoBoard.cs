using System.Collections.Generic;

using static Shatter.Core.Extensions.ListExtensions;

namespace Shatter.Core.Structures.Music.Bingo
{
	public class MusicBingoBoard
    {
        public MusicBingoSong?[,] Board { get; init; }
        public int PlacedSongs { get; private set; }

        public MusicBingoBoard()
        {
            Board = new MusicBingoSong[5, 5];
            Board[2, 2] = null; // free space
            PlacedSongs = 0;
        }

        public void PopulateBoard(List<MusicBingoSong> songList)
        {
            var shuffledSongs = new List<MusicBingoSong>();
            shuffledSongs.AddRange(songList);
            shuffledSongs.Shuffle();

            foreach (var song in shuffledSongs)
            {
                if (PlacedSongs >= 24)
				{
					return;
				}
				// we have no empty spots left!

				int x, y;
                do
                {
                    x = ThreadSafeRandom.ThisThreadsRandom.Next(0, 5);
                    y = ThreadSafeRandom.ThisThreadsRandom.Next(0, 5);
                }
                while ((x == 2 && y == 2) || Board[x,y] is not null);
                // make sure this isnt the free space.
                // or that the board already has a place filled there.

                Board[x, y] = song;

                PlacedSongs++;
            }
        }

        public bool IsWinner(List<MusicBingoSong> playedSongs)
        {
            return CheckHorizontal(playedSongs)
                || CheckVertical(playedSongs)
                || CheckLeftDiag(playedSongs)
                || CheckRightDiag(playedSongs);
        }

        private bool CheckHorizontal(List<MusicBingoSong> playedSongs)
        {
            int c = 0;
            for(int x = 0; x < 5; x++)
            {
                for(int y = 0; y < 5; y++)
                {
                    if (Board[x, y] is null || playedSongs.Contains(Board[x, y]))
                    {
                        c++;
                    }
                    else
                    {
                        c = 0;
                        break;
                    }

                    if (c == 5)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckVertical(List<MusicBingoSong> playedSongs)
        {
            int c = 0;
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    if (Board[x, y] is null || playedSongs.Contains(Board[x, y]))
                    {
                        c++;
                    }
                    else
                    {
                        c = 0;
                        break;
                    }

                    if (c == 5)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckLeftDiag(List<MusicBingoSong> playedSongs)
        {
            return Board[0, 0] is null || playedSongs.Contains(Board[0, 0])
                && Board[1, 1] is null || playedSongs.Contains(Board[1, 1])
                && Board[2, 2] is null || playedSongs.Contains(Board[2, 2])
                && Board[3, 3] is null || playedSongs.Contains(Board[3, 3])
                && Board[4, 4] is null || playedSongs.Contains(Board[4, 4]);
        }

        private bool CheckRightDiag(List<MusicBingoSong> playedSongs)
        {
            return Board[0, 4] is null || playedSongs.Contains(Board[0, 4])
                && Board[1, 3] is null || playedSongs.Contains(Board[1, 3])
                && Board[2, 2] is null || playedSongs.Contains(Board[2, 2])
                && Board[3, 1] is null || playedSongs.Contains(Board[3, 1])
                && Board[4, 0] is null || playedSongs.Contains(Board[4, 0]);
        }
    }
}
