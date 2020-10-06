using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Services;

namespace NitroSharp.Commands.Music
{
    public class PlayCommand : BaseCommandModule
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
        public async Task PlayMusicCommandAsync(CommandContext ctx, [RemainingText] string search)
        {
            if (ctx.Member.VoiceState?.Channel is null)
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "You need to be in a Voice Channel to use the play command!");
                return;
            }

            var res = await _voice.QueueSong(ctx, search);

            if (res.Item1)
            {
                await ctx.RespondAsync($"Queued: {res.Item2.Title} by {res.Item2.Author}");
            }
            else
            {
                await ctx.RespondAsync("Failed to add song to Queue.");
            }
        }
    }
}
