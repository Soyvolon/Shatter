using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace NitroSharp.Commands.Mod
{
    public class MuteCommand : BaseCommandModule
    {
        [Command("mute")]
        [Description("Mutes a user for a designated duration")]
        [RequirePermissions(Permissions.MuteMembers)]
        public async Task MuteCommandAsync(CommandContext ctx,
            [Description("Member to mute.")]
            DiscordMember discordMember,
            
            [Description("Time to mute for")]
            DateTime? muteLenght = null)
        {
            throw new NotImplementedException();
        }
    }
}
