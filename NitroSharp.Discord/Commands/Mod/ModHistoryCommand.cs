using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace NitroSharp.Discord.Commands.Mod
{
    public class ModHistoryCommand : BaseCommandModule
    {
        [Command("modhistory")]
        [Description("Gets the moderator actions for a user.")]
        [Aliases("modh")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [RequireBotPermissions(Permissions.ViewAuditLog)]
        public async Task ModHistoryCommandAsync(CommandContext ctx,
            [Description("User to get moderation logs for")]
            DiscordMember discordMember)
        {
            throw new NotImplementedException();
        }
    }
}
