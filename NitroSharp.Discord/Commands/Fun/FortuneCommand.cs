using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Core.Utils;

namespace NitroSharp.Discord.Commands.Fun
{
    public class FortuneCommand : BaseCommandModule
    {
        [Command("fortune")]
        [Description("Open a fortune cookie")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task FortuneCommandAsync(CommandContext ctx)
        {
            var fortune = await Fortune.GetFortuneCookie();
            if (fortune is null)
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "Bad Cookie!");
            }
            else
            {
                var embed = CommandUtils.SuccessBase()
                    .WithTitle("Fortune Cookie!")
                    .AddField("Fortune", fortune.Data.Messge)
                    .AddField("Lesson", $"{fortune.Lesson.English}\n{fortune.Lesson.Chinese}\n{fortune.Lesson.Pronunciation}")
                    .AddField("Lotto", string.Join(", ", fortune.Lotto.Numbers));

                await ctx.RespondAsync(embed: embed);
            }
        }
    }
}
