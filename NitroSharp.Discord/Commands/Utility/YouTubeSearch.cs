using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Core.Utils;

namespace NitroSharp.Discord.Commands.Utility
{
    public class YouTubeSearch : BaseCommandModule
    {
        [Command("ytsearch")]
        [Description("Serach youtube.")]
        [Aliases("searchyt", "searchyoutube", "youtubesearch")]
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
