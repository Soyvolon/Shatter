using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Core.Database;
using Shatter.Core.Extensions;
using Shatter.Core.Structures.Guilds;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Filter
{
	public class DeleteFilterCommand : CommandModule
    {
        private readonly ShatterDatabaseContext _model;

        public DeleteFilterCommand(ShatterDatabaseContext model)
        {
			this._model = model;
        }

        [Command("deletefilter")]
        [Description("Deletes a word filter.")]
        [Aliases("filterdelete", "delfilter", "fdelete")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [RequireBotPermissions(Permissions.ManageMessages)]
        [ExecutionModule("filter")]
        public async Task DeleteFilterCommandAsync(CommandContext ctx,
            [Description("Name of the filter to delete.")]
            string filterName)
        {
            var name = filterName.ToLower();

            var filter = await this._model.FindAsync<GuildFilters>(ctx.Guild.Id);
            if (filter is null || !filter.Filters.TryGetValue(name, out _))
            {
                await RespondBasicErrorAsync("Filter dosn't exist.");
                return;
            }

            filter.Filters.RemoveValue(name, filter, this._model, out _);
            await this._model.SaveChangesAsync();

            await RespondBasicSuccessAsync( "Filter deleted.");
        }
    }
}
