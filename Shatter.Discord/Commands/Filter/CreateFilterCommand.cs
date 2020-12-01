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
        private readonly ShatterDatabaseContext _model;

        public CreateFilterCommand(ShatterDatabaseContext model)
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
			{
				force = true;
			}

			var filterWords = words.Where(x => GuildFilters.regex.IsMatch(x)).ToHashSet<string>();

            var filter = await this._model.FindAsync<GuildFilters>(ctx.Guild.Id);
            if (filter is null)
            {
                filter = new GuildFilters(ctx.Guild.Id);
                await this._model.AddAsync(filter);
                await this._model.SaveChangesAsync();
            }

            if (!force && filter.Filters.TryGetValue(name, out _))
            {
                await RespondBasicErrorAsync($"Filter name already exsits! Use `{ctx.Prefix}filterupdate` to update the filter.\n" +
                    $"Or, use `-force` to replace the values. Ex: `{ctx.Prefix}createfilter {name} -force {string.Join(" ", filterWords)}`");

                return;
            }

			this._model.Update(filter);
			filter.Filters[name] = new Tuple<int, HashSet<string>>(1, filterWords);

            var data = JsonConvert.SerializeObject(filter.Filters);

            await this._model.SaveChangesAsync();

            await RespondBasicSuccessAsync( $"Created new filter by the name of {name} with the words:\n" +
                $"`{string.Join("`, `", filterWords)}`");
        }
    }
}
