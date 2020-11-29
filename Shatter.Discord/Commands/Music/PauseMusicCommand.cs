using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Discord.Commands.Attributes;
using Shatter.Discord.Services;

namespace Shatter.Discord.Commands.Music
{
	public class PauseMusicCommand : CommandModule
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
        [ExecutionModule("music")]
        public async Task PauseMusicCommandAsync(CommandContext ctx)
        {
            var conn = await this._voice.GetGuildConnection(ctx);

            if (conn is null)
            {
                await RespondBasicErrorAsync("I'm not connected to any Voice Channels!");
                return;
            }

            if (this._voice.IsDJ(ctx, out bool HostChanged)
                || ctx.Member.PermissionsIn(conn.Channel).HasPermission(Permissions.ManageChannels))
            {
                await conn.PauseAsync();
                await RespondBasicSuccessAsync( $"Paused!{(HostChanged ? $"\n{ctx.Member.Mention} is the new host!" : "")}");
            }
            else
			{
				await RespondBasicErrorAsync($"You do not have permissions to pause a song!");
			}
		}
    }
}
