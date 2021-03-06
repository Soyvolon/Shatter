using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Discord.Commands.Attributes;
using Shatter.Discord.Services;

namespace Shatter.Discord.Commands.Music
{
	public class ClearMusicQueueCommand : CommandModule
    {
        private readonly VoiceService _voice;

        public ClearMusicQueueCommand(VoiceService voice)
        {
			this._voice = voice;
        }

        [Command("clearsongs")]
        [Description("Clears the song Queue. Leaves the current song playing.")]
        [Aliases("clearqueue")]
        [RequirePermissions(Permissions.UseVoice)]
        [ExecutionModule("music")]
        public async Task ClearSongsCommandAsync(CommandContext ctx)
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
				this._voice.GuildQueues[ctx.Guild.Id] = new System.Collections.Concurrent.ConcurrentQueue<DSharpPlus.Lavalink.LavalinkTrack>();
                await RespondBasicSuccessAsync( $"Cleared the Queue.{(HostChanged ? $"\n{ ctx.Member.Mention} is the new host!" : "")}");
            }
            else
			{
				await RespondBasicErrorAsync("You do not have permission to clear the queue!");
			}
		}
    }
}
