using System;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Core.Utils;

namespace NitroSharp.Discord.Commands.Fun
{
    public class UrbanDictionaryCommand : BaseCommandModule
    {
        [Command("urban")]
        [Description("Serach the Urban Dictionary")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task UrbanDictionaryCommandAsync(CommandContext ctx,
            [Description("What do you want to serach for?")]
            string searchTerm)
        {
            var res = await UrbanDictionary.Search(searchTerm);

            if (res is null)
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "Bad API Request.");
                return;
            }

            var item = res.FirstOrDefault();

            if (item.Word?.Equals("") ?? false)
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "Failed to reterive any results!");
                return;
            }

            var embed = CommandUtils.SuccessBase()
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
