using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;

using Microsoft.Extensions.DependencyInjection;

using Shatter.Core.Structures.Music.Bingo;
using Shatter.Discord.Properties;

namespace Shatter.Discord.Services
{
    public class MusicBingoService
    {
        private const string _gameIntro = "";

        private readonly VoiceService _voice;
        private readonly IServiceProvider _services;
        public ConcurrentDictionary<ulong, MusicBingoGame> ActiveGames { get; } = new();
        private ConcurrentDictionary<ulong, Tuple<DiscordChannel, DiscordGuild, DiscordClient>> GameConnections { get; } = new();

        public MusicBingoService(VoiceService voice, IServiceProvider services)
        {
            this._voice = voice;
            this._services = services;
        }

        public async Task<bool> StartGameAsync(MusicBingoGame game, CommandContext ctx)
        {
            // A game is already in progress
            if (ActiveGames.ContainsKey(ctx.Guild.Id)) return false;

            // Get the connection used for this guild and add a reference to this event handler.
            var con = await _voice.GetOrCreateConnection(ctx.Client, ctx.Guild, ctx.Member.VoiceState.Channel);
            con.PlaybackFinished += GuildConnection_SongFinished;

            // Add the id to the ignore list.
            _voice.IgnoreEventsList[ctx.Guild.Id] = 0;

            var members = new List<ulong>();
            foreach (var m in ctx.Member.VoiceState.Channel.Users)
                if(m.Id != DiscordBot.Bot.Client.CurrentUser.Id)
                    members.Add(m.Id);

            game.Initalize(members);

            GameConnections[ctx.Guild.Id] = new Tuple<DiscordChannel, DiscordGuild, DiscordClient>
                (
                    ctx.Member.VoiceState.Channel,
                    ctx.Guild,
                    ctx.Client
                );
            ActiveGames[ctx.Guild.Id] = game;

            await SendBingoBoards(ctx.Guild.Id);

            return true;
        }

        private async Task SendBingoBoards(ulong id)
        {
            if (GameConnections.TryGetValue(id, out var con))
            {
                if (ActiveGames.TryGetValue(id, out var game))
                {
                    foreach(var board in game.BingoBoards)
                    {
                        if(con.Item2.Members.TryGetValue(board.Key, out var m))
                        {
                            var dm = await m.CreateDmChannelAsync();

                            using var img = await BuildImage(board.Value);

                            await dm.SendFileAsync("bingo-board.png", img);
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

            var _image = _services.GetRequiredService<MemeService>();
            return await _image.BuildMemeAsync(Resources.Images_BingoBoard, captions, "", 20, new SolidBrush(Color.Black));
        }

        public void StopGame(ulong guildId)
        {
            _voice.IgnoreEventsList.TryRemove(guildId, out _); // remove from ignore list.

            GameConnections.TryRemove(guildId, out _);
            ActiveGames.TryRemove(guildId, out _);
        }

        public bool CheckPlayerWin(ulong guildId, ulong userId)
        {
            if(ActiveGames.TryGetValue(guildId, out var game))
                if(game.BingoBoards.TryGetValue(userId, out var board))
                    if(game.PlayedSongs is not null)
                        return board.IsWinner(game.PlayedSongs);

            return false;
        }

        private async Task PlayIntroduction()
        {

        }

        // Special handler for the bingo games to introduce delays and anything else needed.
        private async Task GuildConnection_SongFinished(LavalinkGuildConnection sender, TrackFinishEventArgs e)
        {

        }
    }
}
