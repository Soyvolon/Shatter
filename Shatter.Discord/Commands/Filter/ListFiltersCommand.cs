using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;

using Shatter.Core.Database;
using Shatter.Core.Structures.Guilds;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Filter
{
    public class ListFiltersCommand : CommandModule
    {
        private readonly NSDatabaseModel _model;

        public ListFiltersCommand(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("listfilters")]
        [Description("Lists the filters for your server.")]
        [Aliases("filterlist", "flist")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [RequireBotPermissions(Permissions.ManageMessages)]
        [ExecutionModule("filter")]
        public async Task ListFiltersCommandAsync(CommandContext ctx)
        {
            var filter = await _model.FindAsync<GuildFilters>(ctx.Guild.Id);
            if (filter is null || filter.Filters.Count <= 0)
            {
                await RespondBasicErrorAsync("No filter found.");
                return;
            }

            var interact = ctx.Client.GetInteractivity();

            var data = "";
            foreach (var item in filter.Filters)
                data += $"{item.Key} [{item.Value.Item1}]: `{string.Join("`, `", item.Value.Item2)}`\n";

            var embed = CommandModule.SuccessBase()
                .WithTitle("Filter Name [Filter Severity]: Filter Words");

            var pages = interact.GeneratePagesInEmbed(data, DSharpPlus.Interactivity.Enums.SplitType.Line, embed);

            interact.SendPaginatedMessageAsync(ctx.Channel, ctx.Member, pages);
        }
    }
}
