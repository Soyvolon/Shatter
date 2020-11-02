using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Newtonsoft.Json;

using Shatter.Core.Database;
using Shatter.Core.Extensions;
using Shatter.Core.Structures.Guilds;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Filter
{
    public class CreateFilterCommand : CommandModule
    {
        private readonly NSDatabaseModel _model;

        public CreateFilterCommand(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("createfilter")]
        [Description("Create a new word based filter for your server.")]
        [Aliases("filtercreate", "fcreate")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [RequireBotPermissions(Permissions.ManageMessages)]
        [ExecutionModule("filter")]
        public async Task CreateFilterCommandAsync(CommandContext ctx,
            [Description("One word name of the new filter")]
            string filterName,

            [Description("Words to filter out with this filter, only letters are allowed.")]
            params string[] words)
        {
            var name = filterName.ToLower();

            bool force = false;
            if (words.Contains("-force"))
                force = true;

            var filterWords = words.Where(x => GuildFilters.regex.IsMatch(x)).ToHashSet<string>();

            var filter = await _model.FindAsync<GuildFilters>(ctx.Guild.Id);
            if (filter is null)
            {
                filter = new GuildFilters(ctx.Guild.Id);
                await _model.AddAsync(filter);
                await _model.SaveChangesAsync();
            }

            if (!force && filter.Filters.TryGetValue(name, out _))
            {
                await RespondBasicErrorAsync($"Filter name already exsits! Use `{ctx.Prefix}filterupdate` to update the filter.\n" +
                    $"Or, use `-force` to replace the values. Ex: `{ctx.Prefix}createfilter {name} -force {string.Join(" ", filterWords)}`");

                return;
            }

            filter.Filters.UpdateOrAddValue(name, new Tuple<int, HashSet<string>>(1, filterWords), filter, _model);

            var data = JsonConvert.SerializeObject(filter.Filters);

            await _model.SaveChangesAsync();

            await RespondBasicSuccessAsync( $"Created new filter by the name of {name} with the words:\n" +
                $"`{string.Join("`, `", filterWords)}`");
        }
    }
}
