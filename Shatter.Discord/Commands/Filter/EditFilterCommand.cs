using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Core.Database;
using Shatter.Core.Extensions;
using Shatter.Core.Structures.Guilds;
using Shatter.Discord.Commands.Attributes;
using Shatter.Discord.Commands.CustomArguments;

namespace Shatter.Discord.Commands.Filter
{
    public class EditFilterCommand : CommandModule
    {
        private readonly ShatterDatabaseContext _model;

        public EditFilterCommand(ShatterDatabaseContext model)
        {
            this._model = model;
        }

        [Command("editfilter")]
        [Description("Edits an exsisting filter, or adds a new one.")]
        [Aliases("filteredit", "fedit", "fupdate", "updatefilter", "filterupdate")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [RequireBotPermissions(Permissions.ManageMessages)]
        [ExecutionModule("filter")]
        public async Task EditFilterCommandAsync(CommandContext ctx,
            [Description("Name of the filter.")]
            string filterName,

            [Description("Add or Remove words from the filter.")]
            AddRemove operation,

            [Description("Words to add or remove")]
            params string[] words)
        {
            var name = filterName.ToLower();

            var filter = await _model.FindAsync<GuildFilters>(ctx.Guild.Id);
            if (filter is null || !filter.Filters.TryGetValue(name, out var filterData))
            {
                await RespondBasicErrorAsync("Filter does not exist");
                return;
            }

            HashSet<string> editWords = words.Where(x => GuildFilters.regex.IsMatch(x)).ToHashSet();
            HashSet<string> filterWords = filterData.Item2;
            switch (operation)
            {
                case AddRemove.Add:
                    filterWords.UnionWith(editWords);
                    break;
                case AddRemove.Remove:
                    filterWords.ExceptWith(editWords);
                    break;
            }

            filter.Filters.UpdateOrAddValue(name, new Tuple<int, HashSet<string>>(filterData.Item1, filterWords), filter, _model);
            await _model.SaveChangesAsync();

            await RespondBasicSuccessAsync( $"Edited filter {name}, the filter now contains the words:\n" +
                $"`{string.Join("`, `", filterWords)}`");
        }
    }
}
