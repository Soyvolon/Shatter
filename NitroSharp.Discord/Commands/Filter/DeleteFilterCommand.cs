using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace NitroSharp.Discord.Commands.Filter
{
    public class DeleteFilterCommand : BaseCommandModule
    {
        [Command("deletefilter")]
        [Description("Deletes a word filter.")]
        [Aliases("filterdelete", "delfilter", "fdelete")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task DeletFilterCommandAsync(CommandContext ctx,
            [Description("Name of the filter to delete.")]
            string filterName)
        {
            throw new NotImplementedException();
        }
    }
}
