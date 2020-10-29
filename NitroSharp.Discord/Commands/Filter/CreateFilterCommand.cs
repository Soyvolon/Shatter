using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace NitroSharp.Discord.Commands.Filter
{
    public class CreateFilterCommand : BaseCommandModule
    {
        [Command("createfilter")]
        [Description("Create a new word based filter for your server.")]
        [Aliases("filtercreate", "fcreate")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task CreateFilterCommandAsync(CommandContext ctx,
            [Description("One word name of the new filter")]
            string filterName,
            
            [Description("Words to filter out with this filter, only alphanumeric characters are allowed.")]
            params string[] words)
        {
            throw new NotImplementedException();
        }
    }
}
