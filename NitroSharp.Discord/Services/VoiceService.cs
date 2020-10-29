using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;

using Google.Apis.YouTube.v3;

using NitroSharp.Discord.Commands;

namespace NitroSharp.Discord.Services
{
    public class VoiceService
    {
        public const int MaxGuildConsPerNode = 100;
        public readonly ConcurrentDictionary<ulong, ulong> DJs = new ConcurrentDictionary<ulong, ulong>();
        public readonly ConcurrentDictionary<ulong, ConcurrentQueue<LavalinkTrack>> GuildQueues = new ConcurrentDictionary<ulong, ConcurrentQueue<LavalinkTrack>>();
        public readonly ConcurrentDictionary<ulong, List<ulong>> VoteSkips = new ConcurrentDictionary<ulong, List<ulong>>();
        public readonly ConcurrentDictionary<ulong, ulong> PlayingStatusMessages = new ConcurrentDictionary<ulong, ulong>();

        public struct VoiceActionResult
        {
            public bool Success { get; set; }
            public string[] Messages { get; set; }
        }
        
        public bool IsDJ(CommandContext ctx, out bool DJChanged)
        {
            DJChanged = false;

            if(DJs.TryGetValue(ctx.Guild.Id, out ulong dj))
            {
                if (ctx.Member.Id == dj)
                    return true;

                if (ctx.Member.VoiceState?.Channel.Users.Any(x => x.Id == dj) ?? false)
                {
                    return false;
                }
            }

            DJChanged = true;
            DJs[ctx.Guild.Id] = ctx.Member.Id;
            return true;
        }

        public async Task<Tuple<bool, LavalinkTrack?>> QueueSong(CommandContext ctx, string search)
        {
            var node = await GetOrCreateConnection(ctx);

            LavalinkTrack Track;
            if (Uri.TryCreate(search, UriKind.Absolute, out Uri? url))
            {
                var searchResult = await node.GetTracksAsync(url);
                Track = searchResult.Tracks.FirstOrDefault();
            }
            else
            {
                var searchResult = await node.GetTracksAsync(search);
                Track = searchResult.Tracks.FirstOrDefault();
            }

            var queueResult = await QueueSong(node, Track, ctx.Member.Id);

            return new Tuple<bool, LavalinkTrack?>(queueResult, Track);
        }

        private async Task<bool> QueueSong(LavalinkGuildConnection conn, LavalinkTrack track, ulong memberId)
        {
            if (track is null)
                return false;

            if (GuildQueues.TryGetValue(conn.Guild.Id, out var queue))
            {
                queue.Enqueue(track);
            }
            else
            {
                GuildQueues[conn.Guild.Id] = new ConcurrentQueue<LavalinkTrack>();
                await conn.PlayAsync(track);
            }

            if (!DJs.TryGetValue(conn.Guild.Id, out var id) || id == 0)
            {
                DJs[conn.Guild.Id] = memberId;
            }

            return true;
        }

        public async Task<LavalinkGuildConnection> GetOrCreateConnection(CommandContext ctx)
        {
            var node = await GetNodeConnection(ctx);

            LavalinkGuildConnection? con;
            if ((con = await GetGuildConnection(ctx, node)) is null)
            {
                con = await node.ConnectAsync(ctx.Member.VoiceState.Channel);
                con.PlaybackFinished += GuildConnection_SongFinished;
                con.PlaybackStarted += GuildConnection_SongStarted;

                // Remove any old data that is stored when a new connection is created
                DJs.TryRemove(con.Guild.Id, out _);
                GuildQueues.TryRemove(con.Guild.Id, out _);
                PlayingStatusMessages.TryRemove(con.Guild.Id, out _);
            }

            return con;
        }

        private async Task GuildConnection_SongStarted(LavalinkGuildConnection sender, TrackStartEventArgs e)
        {
            if (PlayingStatusMessages.TryGetValue(sender.Guild.Id, out ulong chan))
            {
                var nowPlaying = sender.CurrentState.CurrentTrack;
                await DiscordBot.Bot.Rest.CreateMessageAsync(chan, "", false, CommandUtils.SuccessBase()
                    .WithDescription($":green_circle: :notes:] {nowPlaying.Title} by {nowPlaying.Author} - `{sender.CurrentState.PlaybackPosition:mm\\:ss}/{nowPlaying.Length:mm\\:ss}`"),
                    null);
            }
        }

        private async Task GuildConnection_SongFinished(LavalinkGuildConnection sender, TrackFinishEventArgs e)
        {
            if (e.Reason == TrackEndReason.Finished || e.Reason == TrackEndReason.Stopped)
            {
                if (GuildQueues.TryGetValue(sender.Guild.Id, out var queue))
                {
                    if (queue.TryDequeue(out var track))
                    {
                        await sender.PlayAsync(track);
                    }
                    else
                    {
                        GuildQueues.TryRemove(sender.Guild.Id, out _);
                    }
                }
            }
        }

        private Task<LavalinkGuildConnection?> GetGuildConnection(CommandContext ctx, LavalinkNodeConnection node)
        {
            return Task.FromResult<LavalinkGuildConnection?>(node.GetGuildConnection(ctx.Guild));
        }

        public async Task<LavalinkGuildConnection?> GetGuildConnection(CommandContext ctx)
        {
            var node = await GetNodeConnection(ctx);

            return await GetGuildConnection(ctx, node);
        }

        private async Task<LavalinkNodeConnection> GetNodeConnection(CommandContext ctx)
        {
            var lava = ctx.Client.GetLavalink();

            var idealNode = lava.GetIdealNodeConnection(ctx.Guild.VoiceRegion);

            if (idealNode is null || idealNode.ConnectedGuilds.Count > MaxGuildConsPerNode)
                idealNode = await lava.ConnectAsync(GetLavalinkConfiguration());

            return idealNode;
        }

        private LavalinkConfiguration GetLavalinkConfiguration()
        {
            var lcfg = new LavalinkConfiguration
            {
                RestEndpoint = new DSharpPlus.Net.ConnectionEndpoint { Hostname = "localhost", Port = 2333 },
                SocketEndpoint = new DSharpPlus.Net.ConnectionEndpoint { Hostname = "localhost", Port = 2333 },
                Password = DiscordBot.Bot.LavaConfig.Password
            };

            return lcfg;
        }
    }
}
