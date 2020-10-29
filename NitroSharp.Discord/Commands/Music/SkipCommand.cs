using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Discord.Services;

namespace NitroSharp.Discord.Commands.Music
{
    public class SkipCommand : BaseCommandModule
    {
        private readonly VoiceService _voice;

        public SkipCommand(VoiceService voice)
        {
            this._voice = voice;
        }

        [Command("skip")]
        [Description("Skips the current song")]
        [RequireUserPermissions(Permissions.UseVoice)]
        [RequireBotPermissions(Permissions.UseVoice)]
        public async Task SkipMusicCommandAsnyc(CommandContext ctx)
        {
            var conn = await _voice.GetGuildConnection(ctx);

            if (conn is null)
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "I'm not connected to any Voice Channels!");
                return;
            }

            if (_voice.IsDJ(ctx, out bool HostChanged)
                || ctx.Member.PermissionsIn(conn.Channel).HasPermission(Permissions.ManageChannels))
            {
                await conn.StopAsync();
                await CommandUtils.RespondBasicSuccessAsync(ctx, $"Skipping...{(HostChanged ? $"\n{ctx.Member.Mention} is the new host!" : "")}");
            }
            else
                await CommandUtils.RespondBasicErrorAsync(ctx, $"You do not have permissions to force-skip a song! Start a vote skip with `{ctx.Prefix}voteskip`");
        }
    }
}
