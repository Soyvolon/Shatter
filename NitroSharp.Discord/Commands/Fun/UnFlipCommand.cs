using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace NitroSharp.Discord.Commands.Fun
{
    public class UnFlipCommand : BaseCommandModule
    {
        [Command("unflip")]
        [Description("Animated unflipping of the table.")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        [Cooldown(1, 3, CooldownBucketType.User)]
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
