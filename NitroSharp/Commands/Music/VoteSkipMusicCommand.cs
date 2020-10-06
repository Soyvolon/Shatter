using System;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Google.Apis.YouTube.v3.Data;

using NitroSharp.Services;

namespace NitroSharp.Commands.Music
{
    public class VoteSkipMusicCommand : BaseCommandModule
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
                await CommandUtils.RespondBasicErrorAsync(ctx, "I'm not connected to any Voice Channels!");
                return;
            }

            if(ctx.Member.VoiceState?.Channel.Id != conn.Channel.Id)
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "You need to be in the voice channel with the bot for you to start a voteskip!");
                return;
            }

            var required = Math.Ceiling(conn.Channel.Users.Count() / 2.0);

            if(_voice.VoteSkips.TryGetValue(ctx.Guild.Id, out var votes) || required == 1)
            {
                if(votes.Contains(ctx.Member.Id))
                {
                    await CommandUtils.RespondBasicErrorAsync(ctx, "You already voted to skip this song!");
                    return;
                }

                if(votes?.Count + 1 >= required)
                {
                    _voice.VoteSkips.TryRemove(ctx.Guild.Id, out _);
                    await conn.StopAsync();
                    await CommandUtils.RespondBasicSuccessAsync(ctx, "Vote skip complete, Skipping song!");
                }
                else
                {
                    _voice.VoteSkips[ctx.Guild.Id].Add(ctx.Member.Id);
                    await CommandUtils.RespondBasicSuccessAsync(ctx, $"Voted to skip! {votes.Count + 1}/{required} votes have been cast.");
                }
            }
            else
            {
                _voice.VoteSkips[ctx.Guild.Id] = new System.Collections.Generic.List<ulong>() { ctx.Member.Id };
                await CommandUtils.RespondBasicSuccessAsync(ctx, $"Stared a vote to skip the current song! {required} votes required to skip.");
            }
        }
    }
}