using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Core.Utils;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Utility
{
	public class YouTubeSearch : CommandModule
    {
        [Command("youtubesearch")]
        [Description("Serach youtube.")]
        [Aliases("searchyt", "searchyoutube", "ytsearch")]
        [ExecutionModule("utility")]
        public async Task YouTubeSearchAsync(CommandContext ctx,
            [Description("Search string")]
            [RemainingText]
            string search)
        {
            var searchResult = await YouTube.SerachForSingle(search);

            await ctx.RespondAsync(searchResult?.Item1 ?? "Nothing found!");
        }
    }
}
