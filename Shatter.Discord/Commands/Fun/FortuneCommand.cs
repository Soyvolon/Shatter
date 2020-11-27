using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Core.Utils;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Fun
{
	public class FortuneCommand : CommandModule
    {
        [Command("fortune")]
        [Description("Open a fortune cookie")]
        [RequireBotPermissions(Permissions.SendMessages)]
        [ExecutionModule("fun")]
        public async Task FortuneCommandAsync(CommandContext ctx)
        {
            var fortune = await Fortune.GetFortuneCookie();
            if (fortune is null)
            {
                await RespondBasicErrorAsync("Bad Cookie!");
            }
            else
            {
                var embed = CommandModule.SuccessBase()
                    .WithTitle("Fortune Cookie!")
                    .AddField("Fortune", fortune.Data.Messge)
                    .AddField("Lesson", $"{fortune.Lesson.English}\n{fortune.Lesson.Chinese}\n{fortune.Lesson.Pronunciation}")
                    .AddField("Lotto", string.Join(", ", fortune.Lotto.Numbers));

                await ctx.RespondAsync(embed: embed);
            }
        }
    }
}
