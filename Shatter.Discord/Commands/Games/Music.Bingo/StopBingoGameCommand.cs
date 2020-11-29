using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Discord.Commands.Attributes;
using Shatter.Discord.Services;

namespace Shatter.Discord.Commands.Games.Music.Bingo
{
	public class StopBingoGameCommand : CommandModule
    {
        private readonly MusicBingoService _bingo;
        private readonly VoiceService _voice;

        public StopBingoGameCommand(MusicBingoService bingo, VoiceService voice)
        {
			this._bingo = bingo;
			this._voice = voice;
        }

        [Command("stopbingo")]
        [Description("Stops the currently in progress bingo game. Can only be run by the person who started the game" +
            " or a member with the manage ")]
        [ExecutionModule("bingo")]
        public async Task StopBingoGameCommandAsync(CommandContext ctx)
        {
            if (ctx.Member.VoiceState?.Channel is null)
            {
                await RespondBasicErrorAsync("You need to be in a Voice Channel to use the `stopbingo` command!");
                return;
            }

            var conn = await this._voice.GetGuildConnection(ctx);

            if (conn is not null)
            {
                if (!(this._voice.IsDJ(ctx, out bool _)
                || ctx.Member.PermissionsIn(conn.Channel).HasPermission(Permissions.ManageChannels)))
                {
                    await RespondBasicErrorAsync("You are not the DJ and cannot stop a bingo game!");
                    return;
                }
                else
                { // stop anything playing first.
					this._voice.GuildQueues[ctx.Guild.Id].Clear();
                    await conn.StopAsync();
					this._bingo.StopGame(ctx.Guild.Id, false);
                    await RespondBasicSuccessAsync("Bingo game stopped.");
                }
            }
        }
    }
}
