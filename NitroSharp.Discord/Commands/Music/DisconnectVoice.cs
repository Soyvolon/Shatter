using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Discord.Services;

namespace NitroSharp.Discord.Commands.Music
{
    public class DisconnectVoice : BaseCommandModule
    {
        private readonly VoiceService _voice;

        public DisconnectVoice(VoiceService voice)
        {
            this._voice = voice;
        }

        [Command("disconnect")]
        [Description("Disconnect form a voice channel")]
        [Aliases("dc")]
        [RequireUserPermissions(Permissions.UseVoice)]
        [RequireBotPermissions(Permissions.UseVoice)]
        public async Task DisconnectVoiceCommandAsync(CommandContext ctx)
        {
            var conn = await _voice.GetGuildConnection(ctx);
            if (conn is null)
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "Already disconnected!");
                return;
            }

            bool disconected = false;

            if (_voice.IsDJ(ctx, out bool _))
                disconected = true;

            if (ctx.Member.PermissionsIn(conn.Channel).HasPermission(Permissions.ManageChannels))
                disconected = true;

            if (disconected)
            {
                await conn.DisconnectAsync();
                await CommandUtils.RespondBasicSuccessAsync(ctx, "Bye Bye :wave:");
            }
            else
                await CommandUtils.RespondBasicErrorAsync(ctx, "You don't have permission for that!");
        }
    }
}
