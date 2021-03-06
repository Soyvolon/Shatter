using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Fun
{
	public class UnFlipCommand : CommandModule
    {
        [Command("unflip")]
        [Description("Animated unflipping of the table.")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        [ExecutionModule("fun")]
        public async Task ExampleCommandAsync(CommandContext ctx)
        {
            var msgStatus = ctx.RespondAsync("(╯°□°)╯  ︵  ┻━┻");
            await Task.Delay(500);

            var msg = await msgStatus;
            msgStatus = msg.ModifyAsync(content: "(╯°□°)╯    ]");

            await Task.Delay(500);
            msg = await msgStatus;
            await msg.ModifyAsync(content: "(°-°)\\ ┬─┬");
        }
    }
}
