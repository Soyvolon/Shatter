using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;

using NitroSharp.Discord.Services;

namespace NitroSharp.Discord.Commands.Music
{
    public class SkipToMusicCommand : BaseCommandModule
    {
        private readonly VoiceService _voice;

        public SkipToMusicCommand(VoiceService voice)
        {
            this._voice = voice;
        }

        [Command("skipto")]
        [Description("Skip x songs forward")]
        [RequireUserPermissions(Permissions.UseVoice)]
        [RequireBotPermissions(Permissions.UseVoice)]
        public async Task SkipToMusicCommandAsync(CommandContext ctx, int amount)
        {
            var conn = await _voice.GetGuildConnection(ctx);

            if(conn is null)
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "I'm not connected to any Voice Channels!");
                return;
            }

            if (_voice.IsDJ(ctx, out bool HostChanged)
                || ctx.Member.PermissionsIn(conn.Channel).HasPermission(Permissions.ManageChannels))
            {
                if (_voice.GuildQueues.TryGetValue(ctx.Guild.Id, out var queue))
                {
                    var i = 0; // ammount - 1 to include the current track.
                    LavalinkTrack? track;
                    while (queue.TryDequeue(out track) && i < amount - 1)
                        i++;

                    if (track is null)
                    {
                        _voice.GuildQueues.TryRemove(ctx.Guild.Id, out _);
                        await CommandUtils.RespondBasicSuccessAsync(ctx, $"All songs skipped.{(HostChanged ? $"\n{ctx.Member.Mention} is the new host!" : "")}");
                    }
                    else
                    {
                        await conn.PlayAsync(track);
                        await CommandUtils.RespondBasicSuccessAsync(ctx, $"Skipped {amount} songs{(HostChanged ? $"\n{ctx.Member.Mention} is the new host!" : "")}");
                    }
                }
                else
                    await CommandUtils.RespondBasicErrorAsync(ctx, "No songs to skip!");
            }
            else
                await CommandUtils.RespondBasicErrorAsync(ctx, $"You do not have permissions to force-skip a song! Start a vote skip with `{ctx.Prefix}voteskip`");
        }
    }
}
