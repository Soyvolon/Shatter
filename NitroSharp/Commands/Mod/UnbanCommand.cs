using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace NitroSharp.Commands.Mod
{
    public class UnbanCommand : BaseCommandModule
    {
        [Command("unban")]
        [Description("Unbans a user for the server.")]
        [Aliases("uban")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task UnbanCommandAsync(CommandContext ctx,
            [Description("User ID to unban.")]
            ulong userId,
            
            [Description("Reason for unban.")]
            [RemainingText]
            string reason = "unspecified")
        {
            throw new NotImplementedException();
        }
    }
}
