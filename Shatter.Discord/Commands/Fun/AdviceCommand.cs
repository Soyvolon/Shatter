using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Core.Utils;

namespace Shatter.Discord.Commands.Fun
{
    public class AdviceCommand : CommandModule
    {
        [Command("advice")]
        [Description("Advice for the soul")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [RequireUserPermissions(Permissions.AccessChannels)]
        public async Task AdviceCommandAsync(CommandContext ctx)
        {
            var advice = await Advice.GetAdvice();
            if (!(advice is null) || (advice?.Id == 0 && advice?.Contents == ""))
            {
                await ctx.RespondAsync(Formatter.Bold($"[{advice.Id}]") + $" {advice.Contents}");
            }
            else
            {
                await ctx.RespondAsync("Advice machine :b:roke");
            }
        }
    }
}