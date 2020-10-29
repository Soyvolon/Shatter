using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Discord.Services;

namespace NitroSharp.Discord.Commands.Music
{
    public class MusicChannelCommand : BaseCommandModule
    {
        private readonly VoiceService _voice;

        public MusicChannelCommand(VoiceService voice)
        {
            this._voice = voice;
        }

        [Command("playingchannel")]
        [Description("Switch the playing channel to your current channel!")]
        [Aliases("musicchannel")]
        [RequireUserPermissions(Permissions.UseVoice)]
        [RequireBotPermissions(Permissions.UseVoice)]
        public async Task ExampleCommandAsync(CommandContext ctx)
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
                _voice.PlayingStatusMessages[ctx.Guild.Id] = ctx.Channel.Id;
                await CommandUtils.RespondBasicSuccessAsync(ctx, $"Playing message channel changed.{(HostChanged ? $"\n{ ctx.Member.Mention} is the new host!" : "")}");
            }
            else
                await CommandUtils.RespondBasicErrorAsync(ctx, "You do not have permission to change the playing messages channel!");
        }
    }
}
