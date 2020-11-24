using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;

using Shatter.Core.Structures.Music.Bingo;

namespace Shatter.Discord.Services
{
    public class MusicBingoService
    {
        private readonly VoiceService _voice;
        public ConcurrentDictionary<ulong, MusicBingoGame> ActiveGames { get; init; }

        public MusicBingoService(VoiceService voice)
        {
            this._voice = voice;
            ActiveGames = new ConcurrentDictionary<ulong, MusicBingoGame>();
        }

        public async Task<bool> StartGameAsync(MusicBingoGame game, ulong guildId, DiscordChannel voiceChannel, DiscordClient client)
        {

        }

        private async Task<bool> TryVoiceConnection()
        {
            throw new NotImplementedException();
        }

        private async Task StopGame(ulong guildId)
        {

        }

        private async Task CreateBingoBoards()
        {

        }

        private async Task AddBingoBoardTracking()
        {

        }

        private async Task SendBingoBoards()
        {

        }

        public async Task<bool> CheckPlayerWin(ulong userId)
        {
            throw new NotImplementedException();
        }
    }
}
