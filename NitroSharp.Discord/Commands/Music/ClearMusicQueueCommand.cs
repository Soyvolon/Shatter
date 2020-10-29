using System.Threading.Tasks;
using System.Xml;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Services;

namespace NitroSharp.Commands.Music
{
    public class ClearMusicQueueCommand : BaseCommandModule
    {
        private readonly VoiceService _voice;

        public ClearMusicQueueCommand(VoiceService voice)
        {
            this._voice = voice;
        }

        [Command("clearsongs")]
        [Description("Clears the song Queue. Leaves the current song playing.")]
        [Aliases("clearqueue")]
        [RequireUserPermissions(Permissions.UseVoice)]
        [RequireBotPermissions(Permissions.UseVoice)]
        public async Task ClearSongsCommandAsync(CommandContext ctx)
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
                _voice.GuildQueues[ctx.Guild.Id] = new System.Collections.Concurrent.ConcurrentQueue<DSharpPlus.Lavalink.LavalinkTrack>();
                await CommandUtils.RespondBasicSuccessAsync(ctx, $"Cleared the Queue.{(HostChanged ? $"\n{ ctx.Member.Mention} is the new host!" : "")}");
            }
            else
                await CommandUtils.RespondBasicErrorAsync(ctx, "You do not have permission to clear the queue!");
        }
    }
}
