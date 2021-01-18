using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using Shatter.Core.Structures;
using Shatter.Core.Structures.Music.Bingo;
using Shatter.Discord.Properties;

namespace Shatter.Discord.Services
{
	public class MusicBingoService
    {
        private const string _outOfSongs = "";

        private readonly VoiceService _voice;
        private readonly IServiceProvider _services;
		private readonly MusicBingoDefaults _defaults;
        public ConcurrentDictionary<ulong, MusicBingoGame> ActiveGames { get; } = new();
        private ConcurrentDictionary<ulong, Tuple<DiscordChannel, DiscordGuild, DiscordClient>> GameConnections { get; } = new();
        private ConcurrentDictionary<ulong, Timer> SongTimers { get; } = new();


        public MusicBingoService(VoiceService voice, IServiceProvider services)
        {
			this._voice = voice;
			this._services = services;

			using FileStream fs = new(Path.Join("Configs", "bingo_defaults.json"), FileMode.Open);
			using StreamReader sr = new(fs);
			var json = sr.ReadToEnd();

			_defaults = JsonConvert.DeserializeObject<MusicBingoDefaults>(json);
        }

        public async Task<bool> StartGameAsync(MusicBingoGame game, CommandContext ctx)
        {
            // A game is already in progress
            if (this.ActiveGames.ContainsKey(ctx.Guild.Id))
			{
				return false;
			}

			// Get the connection used for this guild and add a reference to this event handler.
			var con = await this._voice.GetOrCreateConnection(ctx.Client, ctx.Guild, ctx.Member.VoiceState.Channel);
            con.PlaybackFinished += GuildConnection_SongFinished;
            //con.PlaybackStarted += GuildConnection_SongStarted;

			// Add the id to the ignore list.
			this._voice.IgnoreEventsList[ctx.Guild.Id] = 0;

            var members = new List<ulong>();
            foreach (var m in ctx.Member.VoiceState.Channel.Users)
			{
				if (m.Id != DiscordBot.Bot?.Client?.CurrentUser.Id)
				{
					members.Add(m.Id);
				}
			}

			game.Initalize(members);

			this.GameConnections[ctx.Guild.Id] = new Tuple<DiscordChannel, DiscordGuild, DiscordClient>
                (
                    ctx.Member.VoiceState.Channel,
                    ctx.Guild,
                    ctx.Client
                );
			this.ActiveGames[ctx.Guild.Id] = game;

            await SendBingoBoards(ctx.Guild.Id);

            await PlayIntroduction(con, game);

            return true;
        }

        private async Task SendBingoBoards(ulong id)
        {
            if (this.GameConnections.TryGetValue(id, out var con))
            {
                if (this.ActiveGames.TryGetValue(id, out var game))
                {
                    foreach(var board in game.BingoBoards)
                    {
                        if(con.Item2.Members.TryGetValue(board.Key, out var m))
                        {
							var dm = await m.CreateDmChannelAsync();

							using var img = await BuildImage(board.Value);

							await dm.SendFileAsync("bingo-board.png", img);

							await Task.Delay(TimeSpan.FromSeconds(0.75));
                        }
                    }
                }
            }
        }

        private async Task<MemoryStream?> BuildImage(MusicBingoBoard board)
        {
            // send the file here.
            var captions = new Tuple<Rectangle, string, Brush?>[25];
            // 170, 250 -> 510, 250 -> 850, 250 -> 1200, 250 -> 1530, 250
            // xxx, 590
            // xxx, 940
            // xxx, 1280
            // xxx, 1630

            // 470, 550

            int i = 0;
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    var xPos = x switch
                    {
                        0 => 170,
                        1 => 510,
                        2 => 850,
                        3 => 1200,
                        4 => 1530,
                        _ => 0,
                    };

                    var yPos = y switch
                    {
                        0 => 250,
                        1 => 590,
                        2 => 940,
                        3 => 1280,
                        4 => 1630,
                        _ => 0,
                    };

                    captions[i++] = new Tuple<Rectangle, string, Brush?>(
                        new Rectangle(xPos, yPos, 300, 300),
                        board.Board[x, y]?.DisplayName ?? "Free\nSpace",
                        null);
                }
            }

            var _image = this._services.GetRequiredService<MemeService>();
            return await _image.BuildMemeAsync(Resources.Images_BingoBoard, captions, "", 35, new SolidBrush(Color.Black));
		}

		public bool CheckPlayerWin(ulong guildId, ulong userId)
		{
			if (this.ActiveGames.TryGetValue(guildId, out var game))
			{
				if (game.BingoBoards.TryGetValue(userId, out var board))
				{
					if (game.PlayedSongs is not null)
					{
						return board.IsWinner(game.PlayedSongs);
					}
				}
			}

			return false;
		}

		public async Task StopGame(ulong guildId, LavalinkGuildConnection? con, bool playWin = false)
		{ // has winner is false when all the songs run out or the game was closed by the command.
		  // TODO: Play custom end of game sounds if there is a winner.

			this._voice.IgnoreEventsList.TryRemove(guildId, out _); // remove from ignore list.

			this.GameConnections.TryRemove(guildId, out _);
			if (this.SongTimers.TryRemove(guildId, out var t))
			{
				await t.DisposeAsync();
			}

			if (this.ActiveGames.TryGetValue(guildId, out var game))
			{
				game.EndGame();

				// if this is null we dont play an exit.
				if (playWin && con is not null)
				{
					var serach = await con.GetTracksAsync(game.Epilogue ?? _defaults.Winner);
					var track = serach.Tracks?.FirstOrDefault() ?? default;

					if(track != default)
					{
						game.NoAutoStop = true;
						await con.PlayAsync(track);
					}
				}
			}
			else
			{
				this.ActiveGames.TryRemove(guildId, out _);

				if (con is not null)
					await con.DisconnectAsync();
			}
		}

        private async Task PlayIntroduction(LavalinkGuildConnection con, MusicBingoGame game)
        {
            var serach = await con.GetTracksAsync(game.Introduction ?? _defaults.Introduction);
            var track = serach.Tracks?.FirstOrDefault() ?? default;

            if (track == default)
			{
				return;
			}

			game.NoAutoStop = true;
			await con.PlayAsync(track);
        }

        private async Task PlayOutOfSongs(TrackFinishEventArgs e, ulong guildId)
        {
            await StopGame(guildId, null, false);

			await e.Player.StopAsync();

			var serach = await e.Player.GetTracksAsync(_defaults.NoWinner);
			var track = serach.Tracks?.FirstOrDefault() ?? default;

			if (track == default)
			{
				return;
			}

			await e.Player.PlayAsync(track);
		}

        // Special handler for the bingo games to introduce delays and anything else needed.
        private async Task GuildConnection_SongFinished(LavalinkGuildConnection sender, TrackFinishEventArgs e)
        {
            if(this.ActiveGames.TryGetValue(sender.Guild.Id, out var game))
            {
				if(game.Completed)
				{
					ActiveGames.TryRemove(sender.Guild.Id, out _);
					await sender.DisconnectAsync();
					return;
				}

				if (game.RunningCompletion)
				{
					game.EndGame();
					return;
				}

                var song = game.GetNextSong();
                if(song is null)
                {
                    await PlayOutOfSongs(e, sender.Guild.Id);                    
                }
                else
                {
                    var serach = e.Player.GetTracksAsync(song.SongLink);

                    //await Task.Delay(TimeSpan.FromSeconds(1));
                    var songRes = await serach;

                    LavalinkTrack? track;
                    if ((track = songRes.Tracks?.FirstOrDefault() ?? default) != default)
                    {
                        if (song.SongStart is null || song.SongEnd is null)
                        {
                            await e.Player.PlayAsync(track);
                        }
                        else
                        {
                            var start = (TimeSpan)song.SongStart;
                            var end = (TimeSpan)song.SongEnd;
                            await e.Player.PlayPartialAsync(track, start, end);
                        }
                    }
                }
            }
        }
    }
}
