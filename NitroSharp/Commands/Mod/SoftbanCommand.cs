using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace NitroSharp.Commands.Mod
{
    public class SoftbanCommand : BaseCommandModule
    {
        [Command("softban")]
        [Description("Bans and then unbans a user (Kick with clearing messages).")]
        [Aliases("sban")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task SoftbanCommandAsync(CommandContext ctx,
            [Description("Member to softban.")]
            DiscordMember discordMember,
            
            [Description("Reason for the softban")]
            [RemainingText]
            string reason = "unspecified")
        {
            throw new NotImplementedException();
        }
    }
}
