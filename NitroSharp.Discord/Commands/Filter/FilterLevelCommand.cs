using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace NitroSharp.Discord.Commands.Filter
{
    public class FilterLevelCommand : BaseCommandModule
    {
        [Command("filterlevel")]
        [Description("Sets the strictness of filters on your server.")]
        [Aliases("flevel")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task FilterLevelCommandAsync(CommandContext ctx,
            [Description("Filter to set the severity for.")]
            string filterName,

            [Description("Severity level: `1` - Only if the word directly matches, `2` - replaces lookalike characters, `3` - if the word is found anywhere.")]
            int severity = 1)
        {
            throw new NotImplementedException();
        }
    }
}
