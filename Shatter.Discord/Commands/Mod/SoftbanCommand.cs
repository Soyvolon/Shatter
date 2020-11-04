using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Mod
{
    public class SoftbanCommand : CommandModule
    {
        [Command("softban")]
        [Description("Bans and then unbans a user (Kick with clearing messages).")]
        [Aliases("sban")]
        [RequirePermissions(Permissions.BanMembers)]
        [ExecutionModule("mod")]
        public async Task SoftbanCommandAsync(CommandContext ctx,
            [Description("Member to softban.")]
            DiscordMember discordMember,

            [Description("Reason for the softban")]
            [RemainingText]
            string reason = "unspecified")
        {
            try
            {
                await discordMember.BanAsync(7, reason);
            }
            catch
            {
                await RespondBasicErrorAsync("Failed to start the softban.");
                return;
            }

            try
            {
                await discordMember.UnbanAsync("Softban unban");
            }
            catch
            {
                await RespondBasicErrorAsync("Failed to unban the user after a ban. Please manually unban the user.");
                return;
            }

            await RespondBasicSuccessAsync( $"Sucesffuly softbanned {discordMember.Username}");
        }
    }
}
