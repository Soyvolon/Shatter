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

        }
    }
}
