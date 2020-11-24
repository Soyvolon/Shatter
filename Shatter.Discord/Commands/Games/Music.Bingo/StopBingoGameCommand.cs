using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Games.Music.Bingo
{
    public class StopBingoGameCommand : CommandModule
    {
        [Command("stopbingo")]
        [Description("Stops the currently in progress bingo game. Can only be run by the person who started the game" +
            " or a member with the manage ")]
        [ExecutionModule("bingo")]
        public async Task StopBingoGameCommandAsync(CommandContext ctx)
        {

        }
    }
}
