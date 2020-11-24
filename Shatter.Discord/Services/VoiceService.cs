using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;

using Shatter.Discord.Commands;

namespace Shatter.Discord.Services
{
    public class VoiceService
    {
        public const int MaxGuildConsPerNode = 100;
        public ConcurrentDictionary<ulong, ulong> DJs { get; } = new();
        public ConcurrentDictionary<ulong, ConcurrentQueue<LavalinkTrack>> GuildQueues { get; } = new();
        public ConcurrentDictionary<ulong, List<ulong>> VoteSkips { get; } = new();
        public ConcurrentDictionary<ulong, ulong> PlayingStatusMessages { get; } = new();
        public ConcurrentDictionary<ulong, byte> IgnoreEventsList { get; } = new();

        public struct VoiceActionResult
        {
            public bool Success { get; set; }
            public string[] Messages { get; set; }
        }

        public bool IsDJ(CommandContext ctx, out bool DJChanged)
        {
            DJChanged = false;

            if (DJs.TryGetValue(ctx.Guild.Id, out ulong dj))
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

        public async Task<Tuple<bool, LavalinkTrack?>> QueueSong(CommandContext ctx, string serach)
            => await QueueSong(ctx.Client, ctx.Guild, ctx.Member.VoiceState.Channel, ctx.Member.Id, serach);

        public async Task<Tuple<bool, LavalinkTrack?>> QueueSong(CommandContext ctx, Uri serach)
            => await QueueSong(ctx.Client, ctx.Guild, ctx.Member.VoiceState.Channel, ctx.Member.Id, serach);

        public async Task<Tuple<bool, LavalinkTrack?>> QueueSong(DiscordClient client, DiscordGuild guild, 
            DiscordChannel channel, ulong memberId, string search)
        {
            if (Uri.TryCreate(search, UriKind.Absolute, out Uri? url))
            {
                return await QueueSong(client, guild, channel, memberId, url);
            }
            else
            {
                var node = await GetOrCreateConnection(client, guild, channel);

                var searchResult = await node.GetTracksAsync(search);
                var Track = searchResult.Tracks.FirstOrDefault();

                var queueResult = await QueueSong(node, Track, memberId);

                return new Tuple<bool, LavalinkTrack?>(queueResult, Track);
            }
        }

        public async Task<Tuple<bool, LavalinkTrack?>> QueueSong(DiscordClient client, DiscordGuild guild,
            DiscordChannel channel, ulong memberId, Uri search)
        {
            var node = await GetOrCreateConnection(client, guild, channel);

            var searchResult = await node.GetTracksAsync(search);
            var Track = searchResult.Tracks.FirstOrDefault();
            
            var queueResult = await QueueSong(node, Track, memberId);

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

        public async Task<LavalinkGuildConnection> GetOrCreateConnection(DiscordClient client, DiscordGuild guild, DiscordChannel channel)
        {
            var node = await GetNodeConnection(client, guild);

            LavalinkGuildConnection? con;
            if ((con = await GetGuildConnection(guild, node)) is null)
            {
                con = await node.ConnectAsync(channel);
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
            if (IgnoreEventsList.ContainsKey(sender.Guild.Id)) return;

            if (PlayingStatusMessages.TryGetValue(sender.Guild.Id, out ulong chan))
            {
                var nowPlaying = sender.CurrentState.CurrentTrack;
                await DiscordBot.Bot.Rest.CreateMessageAsync(chan, "", false, CommandModule.SuccessBase()
                    .WithDescription($":green_circle: :notes:] {nowPlaying.Title} by {nowPlaying.Author} - `{sender.CurrentState.PlaybackPosition:mm\\:ss}/{nowPlaying.Length:mm\\:ss}`"),
                    null);
            }
        }

        private async Task GuildConnection_SongFinished(LavalinkGuildConnection sender, TrackFinishEventArgs e)
        {
            if (IgnoreEventsList.ContainsKey(sender.Guild.Id)) return;

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

        private static Task<LavalinkGuildConnection?> GetGuildConnection(DiscordGuild guild, LavalinkNodeConnection node)
        {
            return Task.FromResult<LavalinkGuildConnection?>(node.GetGuildConnection(guild));
        }

        public async Task<LavalinkGuildConnection?> GetGuildConnection(CommandContext ctx)
        {
            return await GetGuildConnection(ctx.Client, ctx.Guild);

        }

        public async Task<LavalinkGuildConnection?> GetGuildConnection(DiscordClient c, DiscordGuild g)
        {
            var node = await GetNodeConnection(c, g);

            return await GetGuildConnection(g, node);
        }

        private async Task<LavalinkNodeConnection> GetNodeConnection(DiscordClient client, DiscordGuild guild)
        {
            var lava = client.GetLavalink();

            var idealNode = lava.GetIdealNodeConnection(guild.VoiceRegion);

            if (idealNode is null || idealNode.ConnectedGuilds.Count > MaxGuildConsPerNode)
                idealNode = await lava.ConnectAsync(GetLavalinkConfiguration());

            return idealNode;
        }

        private static LavalinkConfiguration GetLavalinkConfiguration()
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
