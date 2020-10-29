using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Discord.Services;

namespace NitroSharp.Discord.Commands.Music
{
    public class ResumeMusicCommand : BaseCommandModule
    {
        private readonly VoiceService _voice;

        public ResumeMusicCommand(VoiceService voice)
        {
            this._voice = voice;
        }

        [Command("resume")]
        [Description("Resumes the music!")]
        [RequireUserPermissions(Permissions.UseVoice)]
        [RequireBotPermissions(Permissions.UseVoice)]
        public async Task ResumeMusicCommandAsync(CommandContext ctx)
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
                await conn.ResumeAsync();
                await CommandUtils.RespondBasicSuccessAsync(ctx, $"Resumed!{(HostChanged ? $"\n{ctx.Member.Mention} is the new host!" : "")}");
            }
            else
                await CommandUtils.RespondBasicErrorAsync(ctx, $"You do not have permissions to resume a song!");
        }
    }
}
