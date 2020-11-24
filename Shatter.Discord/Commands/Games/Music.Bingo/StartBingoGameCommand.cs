using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Shatter.Discord.Commands.Games.Music.Bingo
{
    public class StartBingoGameCommand : CommandModule
    {
        [Command("startbingo")]
        [Description("Starts a new bingo game! You must be in VC and the bot can not be being used by another user in a VC!")]
        public async Task StartBingoGameCommandAsync(CommandContext ctx)
        {

        }
    }
}
