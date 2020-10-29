using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace NitroSharp.Discord.Commands.Mod
{
    public class SoftbanCommand : BaseCommandModule
    {
        [Command("softban")]
        [Description("Bans and then unbans a user (Kick with clearing messages).")]
        [Aliases("sban")]
        [RequirePermissions(Permissions.BanMembers)]
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
                await CommandUtils.RespondBasicErrorAsync(ctx, "Failed to start the softban.");
                return;
            }

            try
            {
                await discordMember.UnbanAsync("Softban unban");
            }
            catch
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "Failed to unban the user after a ban. Please manually unban the user.");
                return;
            }

            await CommandUtils.RespondBasicSuccessAsync(ctx, $"Sucesffuly softbanned {discordMember.Username}");
        }
    }
}
