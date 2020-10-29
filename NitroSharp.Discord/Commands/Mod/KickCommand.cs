using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace NitroSharp.Discord.Commands.Mod
{
    public class KickCommand : BaseCommandModule
    {
        [Command("kick")]
        [Description("Kicks a user from the server.")]
        [RequirePermissions(Permissions.KickMembers)]
        public async Task KickCommandAsync(CommandContext ctx,
            [Description("User to kick")]
            DiscordMember discordMember,

            [Description("Reason for the kick")]
            [RemainingText]
            string reason = "unspecified")
        {
            await discordMember.RemoveAsync(reason);
        }
    }
}
