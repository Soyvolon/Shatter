using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace NitroSharp.Discord.Commands.Filter
{
    public class ListFiltersCommand : BaseCommandModule
    {
        [Command("listfilters")]
        [Description("Lists the filters for your server.")]
        [Aliases("filterlist", "flist")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task ListFiltersCommandAsync(CommandContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
