using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Shatter.Discord.Commands.Attributes;
using Shatter.Discord.Services;

namespace Shatter.Discord.Commands.Music
{
	public class SetMusicHostCommand : CommandModule
    {
        private readonly VoiceService _voice;

        public SetMusicHostCommand(VoiceService voice)
        {
			this._voice = voice;
        }

        [Command("sethost")]
        [Description("Set the host of the current session")]
		[Aliases("host")]
        [RequireUserPermissions(Permissions.UseVoice)]
        [RequireBotPermissions(Permissions.UseVoice)]
        [ExecutionModule("music")]
        public async Task ExampleCommandAsync(CommandContext ctx, DiscordMember newHost)
        {
            var conn = await this._voice.GetGuildConnection(ctx);

            if (conn is null)
            {
                await RespondBasicErrorAsync("I'm not connected to any Voice Channels!");
                return;
            }

            if (this._voice.IsDJ(ctx, out bool _)
                || ctx.Member.PermissionsIn(conn.Channel).HasPermission(Permissions.ManageChannels))
            {
				this._voice.DJs[ctx.Guild.Id] = newHost.Id;
                await RespondBasicSuccessAsync( $"{newHost.Mention} is the new host!");
            }
            else
			{
				await RespondBasicErrorAsync($"You do not have permissions to change the current host!");
			}
		}
    }
}
