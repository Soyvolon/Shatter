using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace NitroSharp.Commands.Mod
{
    public class UserHistoryCommand : BaseCommandModule
    {
        [Command("userhistory")]
        [Description("Gets moderation actions history of a user.")]
        [Aliases("uhistory", "userh")]
        [RequireUserPermissions(Permissions.ManageMessages)]
        public async Task UserHistoryCommandAsync(CommandContext ctx,
            [Description("User to get a history for")]
            DiscordMember discordMember,
            
            [Description("Filter strings to filter results by: `ban`, `softban`, `tempban`, `kick`, `mute`, `warn`, `unban`, `all`")]
            params string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
