using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Services;

namespace NitroSharp.Commands.Music
{
    public class PauseMusicCommand : BaseCommandModule
    {
        private readonly VoiceService _voice;

        public PauseMusicCommand(VoiceService voice)
        {
            this._voice = voice;
        }

        [Command("pause")]
        [Description("Pauses the music!")]
        [RequireUserPermissions(Permissions.UseVoice)]
        [RequireBotPermissions(Permissions.UseVoice)]
        public async Task PauseMusicCommandAsync(CommandContext ctx)
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
                await conn.PauseAsync(); 
                await CommandUtils.RespondBasicSuccessAsync(ctx, $"Paused!{(HostChanged ? $"\n{ctx.Member.Mention} is the new host!" : "")}");
            }
            else
                await CommandUtils.RespondBasicErrorAsync(ctx, $"You do not have permissions to pause a song!");
        }
    }
}
