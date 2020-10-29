using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Core.Database;
using NitroSharp.Core.Extensions;
using NitroSharp.Core.Structures.Guilds;

namespace NitroSharp.Discord.Commands.Filter
{
    public class DeleteFilterCommand : BaseCommandModule
    {
        private readonly NSDatabaseModel _model;

        public DeleteFilterCommand(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("deletefilter")]
        [Description("Deletes a word filter.")]
        [Aliases("filterdelete", "delfilter", "fdelete")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task DeleteFilterCommandAsync(CommandContext ctx,
            [Description("Name of the filter to delete.")]
            string filterName)
        {
            var name = filterName.ToLower();

            var filter = await _model.FindAsync<GuildFilters>(ctx.Guild.Id);
            if (filter is null || !filter.Filters.TryGetValue(name, out _))
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "Filter dosn't exist.");
                return;
            }

            filter.Filters.RemoveValue(name, filter, _model, out _);
            await _model.SaveChangesAsync();

            await CommandUtils.RespondBasicSuccessAsync(ctx, "Filter deleted.");
        }
    }
}
