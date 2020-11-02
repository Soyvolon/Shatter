using System;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Discord.Services;

namespace Shatter.Discord.Commands.Music
{
    public class VoteSkipMusicCommand : CommandModule
    {
        private readonly VoiceService _voice;

        public VoteSkipMusicCommand(VoiceService voice)
        {
            this._voice = voice;
        }

        [Command("voteskip")]
        [Description("Start or join a Voteskip!")]
        [RequireUserPermissions(Permissions.UseVoice)]
        [RequireBotPermissions(Permissions.UseVoice)]
        public async Task VoteSkipMusicCommandAsync(CommandContext ctx)
        {
            var conn = await _voice.GetGuildConnection(ctx);

            if (conn is null)
            {
                await RespondBasicErrorAsync("I'm not connected to any Voice Channels!");
                return;
            }

            if (ctx.Member.VoiceState?.Channel.Id != conn.Channel.Id)
            {
                await RespondBasicErrorAsync("You need to be in the voice channel with the bot for you to start a voteskip!");
                return;
            }

            var required = Math.Ceiling(conn.Channel.Users.Count() / 2.0);

            if (_voice.VoteSkips.TryGetValue(ctx.Guild.Id, out var votes) || required == 1)
            {
                if (votes.Contains(ctx.Member.Id))
                {
                    await RespondBasicErrorAsync("You already voted to skip this song!");
                    return;
                }

                if (votes?.Count + 1 >= required)
                {
                    _voice.VoteSkips.TryRemove(ctx.Guild.Id, out _);
                    await conn.StopAsync();
                    await RespondBasicSuccessAsync( "Vote skip complete, Skipping song!");
                }
                else
                {
                    _voice.VoteSkips[ctx.Guild.Id].Add(ctx.Member.Id);
                    await RespondBasicSuccessAsync( $"Voted to skip! {votes.Count + 1}/{required} votes have been cast.");
                }
            }
            else
            {
                _voice.VoteSkips[ctx.Guild.Id] = new System.Collections.Generic.List<ulong>() { ctx.Member.Id };
                await RespondBasicSuccessAsync( $"Stared a vote to skip the current song! {required} votes required to skip.");
            }
        }
    }
}