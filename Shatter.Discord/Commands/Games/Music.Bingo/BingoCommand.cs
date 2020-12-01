using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Discord.Services;

namespace Shatter.Discord.Commands.Games.Music.Bingo
{
	public class BingoCommand : CommandModule
    {
        private readonly MusicBingoService _bingo;
		private readonly VoiceService _voice;

        public BingoCommand(MusicBingoService bingo, VoiceService voice)
        {
			this._bingo = bingo;
			this._voice = voice;
        }

        [Command("bingo")]
        [Description("Check to see if you have won the bingo game!")]
        [Cooldown(1, 15, CooldownBucketType.User)]
        public async Task BingoCommandAsync(CommandContext ctx)
        {
            if (ctx.Member.VoiceState?.Channel is null)
            {
                await RespondBasicErrorAsync("You need to be in a Voice Channel to use the `bingo` command!");
                return;
            }

            if (this._bingo.ActiveGames.TryGetValue(ctx.Guild.Id, out var game))
            {
                if (game.BingoBoards.TryGetValue(ctx.User.Id, out var board))
                {
                    if(game.PlayedSongs is null)
                    {
                        await RespondBasicErrorAsync("Looks like there hasnt been any songs played!");
                        return;
                    }

                    if(board.IsWinner(game.PlayedSongs))
                    {
						var conn = await _voice.GetOrCreateConnection(ctx.Client, ctx.Guild, ctx.Member.VoiceState.Channel);
						await this._bingo.StopGame(ctx.Guild.Id, conn);
                        await RespondBasicSuccessAsync($"Congrats {ctx.User.Mention}, you won the bingo game!");
                    }
                    else
                    {
                        await RespondBasicSuccessAsync("You have not won. Wait 15 seconds before using this command again.");
                    }
                }
                else
                {
                    await RespondBasicErrorAsync("You do not have a bingo board to check!");
                }
            }
            else
            {
                await RespondBasicErrorAsync("There is no bingo game currently running!");
            }
        }
    }
}
