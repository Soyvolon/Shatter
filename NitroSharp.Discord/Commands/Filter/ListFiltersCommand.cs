using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;

using Microsoft.EntityFrameworkCore.Metadata.Internal;

using NitroSharp.Core.Database;
using NitroSharp.Core.Structures.Guilds;

namespace NitroSharp.Discord.Commands.Filter
{
    public class ListFiltersCommand : BaseCommandModule
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
        public async Task ListFiltersCommandAsync(CommandContext ctx)
        {
            var filter = await _model.FindAsync<GuildFilters>(ctx.Guild.Id);
            if(filter is null || filter.Filters.Count <= 0)
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "No filter found.");
                return;
            }

            var interact = ctx.Client.GetInteractivity();

            var data = "";
            foreach(var item in filter.Filters)
                data += $"{item.Key} [{item.Value.Item1}]: `{string.Join("`, `", item.Value.Item2)}`\n";

            var embed = CommandUtils.SuccessBase()
                .WithTitle("Filter Name [Filter Severity]: Filter Words");

            var pages = interact.GeneratePagesInEmbed(data, DSharpPlus.Interactivity.Enums.SplitType.Line, embed);

            interact.SendPaginatedMessageAsync(ctx.Channel, ctx.Member, pages);
        }
    }
}
