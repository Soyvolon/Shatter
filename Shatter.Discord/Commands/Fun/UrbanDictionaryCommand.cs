using System;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Core.Utils;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Fun
{
    public class UrbanDictionaryCommand : CommandModule
    {
        [Command("urban")]
        [Description("Serach the Urban Dictionary")]
        [RequireBotPermissions(Permissions.SendMessages)]
        [ExecutionModule("fun")]
        public async Task UrbanDictionaryCommandAsync(CommandContext ctx,
            [Description("What do you want to serach for?")]
            string searchTerm)
        {
            var res = await UrbanDictionary.Search(searchTerm);

            if (res is null)
            {
                await RespondBasicErrorAsync("Bad API Request.");
                return;
            }

            var item = res.FirstOrDefault();

            if (item.Word?.Equals("") ?? false)
            {
                await RespondBasicErrorAsync("Failed to reterive any results!");
                return;
            }

            var embed = CommandModule.SuccessBase()
                .WithTitle(item.Word)
                .WithAuthor(item.DefId.ToString(), item.PermaLink)
                .WithDescription(item.Definition)
                .AddField("Example:", item.Example)
                .WithFooter($"\U0001F44D {item.ThumbsUp} | \U0001F44E {item.ThumbsDown}")
                .WithTimestamp(DateTime.TryParse(item.WrittenOn, out var time) ? time : new DateTime());

            await ctx.RespondAsync(embed: embed);
        }
    }
}
