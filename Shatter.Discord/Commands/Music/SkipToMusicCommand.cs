using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Lavalink;

using Shatter.Discord.Commands.Attributes;
using Shatter.Discord.Services;

namespace Shatter.Discord.Commands.Music
{
	public class SkipToMusicCommand : CommandModule
    {
        private readonly VoiceService _voice;

        public SkipToMusicCommand(VoiceService voice)
        {
			this._voice = voice;
        }

        [Command("skipto")]
        [Description("Skip x songs forward")]
        [RequireUserPermissions(Permissions.UseVoice)]
        [RequireBotPermissions(Permissions.UseVoice)]
        [ExecutionModule("music")]
        public async Task SkipToMusicCommandAsync(CommandContext ctx, int amount)
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
                if (this._voice.GuildQueues.TryGetValue(ctx.Guild.Id, out var queue))
                {
                    var i = 0; // ammount - 1 to include the current track.
                    LavalinkTrack? track;
                    while (queue.TryDequeue(out track) && i < amount - 1)
					{
						i++;
					}

					if (track is null)
                    {
						this._voice.GuildQueues.TryRemove(ctx.Guild.Id, out _);
                        await RespondBasicSuccessAsync( $"All songs skipped.{(HostChanged ? $"\n{ctx.Member.Mention} is the new host!" : "")}");
                    }
                    else
                    {
                        await conn.PlayAsync(track);
                        await RespondBasicSuccessAsync( $"Skipped {amount} songs{(HostChanged ? $"\n{ctx.Member.Mention} is the new host!" : "")}");
                    }
                }
                else
				{
					await RespondBasicErrorAsync("No songs to skip!");
				}
			}
            else
			{
				await RespondBasicErrorAsync($"You do not have permissions to force-skip a song! Start a vote skip with `{ctx.Prefix}voteskip`");
			}
		}
    }
}
