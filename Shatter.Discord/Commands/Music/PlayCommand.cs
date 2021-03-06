using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Discord.Commands.Attributes;
using Shatter.Discord.Services;

namespace Shatter.Discord.Commands.Music
{
	public class PlayCommand : CommandModule
    {
        private readonly VoiceService _voice;

        public PlayCommand(VoiceService voice)
        {
			this._voice = voice;
        }

        [Command("play")]
        [Description("Play some music!")]
        [RequireUserPermissions(Permissions.UseVoice)]
        [RequireBotPermissions(Permissions.UseVoice)]
        [ExecutionModule("music")]
        public async Task PlayMusicCommandAsync(CommandContext ctx, [RemainingText] string search)
        {
            if (ctx.Member.VoiceState?.Channel is null)
            {
                await RespondBasicErrorAsync("You need to be in a Voice Channel to use the play command!");
                return;
            }

            var res = await this._voice.QueueSong(ctx, search);

            if (res.Item1)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				// If item 1 is true, the second value will never be null.
				await ctx.RespondAsync($"Queued: {res.Item2.Title} by {res.Item2.Author}");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
            else
            {
                await ctx.RespondAsync("Failed to add song to Queue.");
            }
        }
    }
}
