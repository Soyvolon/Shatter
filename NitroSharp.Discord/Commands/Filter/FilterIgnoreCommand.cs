using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace NitroSharp.Discord.Commands.Filter
{
    public class FilterIgnoreCommand : BaseCommandModule
    {
        [Command("filterignore")]
        [Description("Sets a user as exempt from filters. Users with the Manage Message permission are automatically exmept from filters.")]
        [Aliases("ignorefilter", "fignore")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task FilterIgnoreCommandAsync(CommandContext ctx,
            [Description("User to toggle exemption status for.")]
            DiscordMember discordMember)
        {
            throw new NotImplementedException();
        }
    }
}
