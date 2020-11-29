using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Core.Structures;
using Shatter.Core.Utils;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Fun
{
	public class CatCommand : CommandModule
    {
        [Command("cat")]
        [Description("Cats!")]
        [Aliases("cats")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        [ExecutionModule("fun")]
        public async Task CatCommandAsnyc(CommandContext ctx, params string[] args)
        {
            CatData cat = default;
            bool skipRandom = false;

            foreach (var arg in args)
            {
                if (arg.StartsWith("id="))
                {
                    cat = await Cats.GetCatByIdAsync(arg[3..]);
                    skipRandom = true;
                }
            }

            if (!skipRandom)
            {
                var cats = await Cats.GetCatDataAsync(args);
                if (cats?.Length > 0)
                {
                    cat = cats[0];
                }
            }

            if (cat.Id is null)
            {
                await ctx.RespondAsync(":sob: " + Formatter.Italic("no cats were found..."));
            }
            else
            {
                var embed = CommandModule.SuccessBase()
                    .WithTitle($"Cats! [id={cat.Id}]")
                    .WithImageUrl(cat.Url);

                await ctx.RespondAsync(embed: embed);
            }
        }
    }
}
