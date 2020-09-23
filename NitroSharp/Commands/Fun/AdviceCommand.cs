using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Structures;

namespace NitroSharp.Commands.Fun
{
    public class AdviceCommand : BaseCommandModule
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