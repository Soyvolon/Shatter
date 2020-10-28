using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace NitroSharp.Commands.Mod
{
    public class WarnCommand : BaseCommandModule
    {
        [Command("warn")]
        [Description("Warns a user.")]
        [RequireUserPermissions(Permissions.ManageMessages)]
        public async Task WarnCommandAsync(CommandContext ctx,
            [Description("Member to warn")]
            DiscordMember discordMember,
            
            [Description("Reason for the warn")]
            [RemainingText]
            string reason = "unspecified")
        {
            throw new NotImplementedException();
        }
    }
}
