using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Discord.Commands.CustomArguments;

namespace NitroSharp.Discord.Commands.Filter
{
    public class EditFilterCommand : BaseCommandModule
    {
        [Command("editfilter")]
        [Description("Edits an exsisting filter, or adds a new one.")]
        [Aliases("filteredit", "fedit")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task EditFilterCommandAsync(CommandContext ctx,
            [Description("Name of the filter.")]
            string filterName,
            
            [Description("Add or Remove words from the filter.")]
            AddRemove operation,
            
            [Description("Words to add or remove")]
            params string[] words)
        {
            throw new NotImplementedException();
        }
    }
}
