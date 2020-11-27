using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Fun
{
	public class TableFlipCommand : CommandModule
    {
        [Command("tableflip")]
        [Description("Animated Table Flip")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        [Cooldown(1, 3, CooldownBucketType.User)]
        [ExecutionModule("fun")]
        public static async Task TableflipCommandAsync(CommandContext ctx)
        {
            var msgStatus = ctx.RespondAsync("(°-°)\\ ┬─┬");
            await Task.Delay(500);

            var msg = await msgStatus;
            msgStatus = msg.ModifyAsync(content: "(╯°□°)╯    ]");

            await Task.Delay(500);
            msg = await msgStatus;
            await msg.ModifyAsync(content: "(╯°□°)╯  ︵  ┻━┻");
        }
    }
}
