using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Discord.Commands.Attributes;
using Shatter.Discord.Services;

namespace Shatter.Discord.Commands.Music
{
	public class MusicChannelCommand : CommandModule
    {
        private readonly VoiceService _voice;

        public MusicChannelCommand(VoiceService voice)
        {
            _voice = voice;
        }

        [Command("playingchannel")]
        [Description("Switch the playing channel to your current channel!")]
        [Aliases("musicchannel")]
        [RequireUserPermissions(Permissions.UseVoice)]
        [RequireBotPermissions(Permissions.UseVoice)]
        [ExecutionModule("music")]
        public async Task MusicChannelCommandAsync(CommandContext ctx)
        {
            var conn = await _voice.GetGuildConnection(ctx);

            if (conn is null)
            {
                await RespondBasicErrorAsync("I'm not connected to any Voice Channels!");
                return;
            }

            if (_voice.IsDJ(ctx, out bool HostChanged)
                || ctx.Member.PermissionsIn(conn.Channel).HasPermission(Permissions.ManageChannels))
            {
                _voice.PlayingStatusMessages[ctx.Guild.Id] = ctx.Channel.Id;
                await RespondBasicSuccessAsync( $"Playing message channel changed.{(HostChanged ? $"\n{ ctx.Member.Mention} is the new host!" : "")}");
            }
            else
			{
				await RespondBasicErrorAsync("You do not have permission to change the playing messages channel!");
			}
		}
    }
}
