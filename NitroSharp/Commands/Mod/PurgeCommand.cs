using System;
using System.Threading.Tasks;
using System.Xml;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace NitroSharp.Commands.Mod
{
    public class PurgeCommand : BaseCommandModule
    {
        [Command("purge")]
        [Description("Purges messages from the server.")]
        [RequirePermissions(Permissions.ManageMessages)]
        public async Task PurgeCommandAsync(CommandContext ctx, 
            [Description("Argument array for the purge command. Use purge -h | --help for more information.")]
            params string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
