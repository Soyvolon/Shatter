using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Core.Database;
using NitroSharp.Core.Extensions;
using NitroSharp.Core.Structures.Guilds;

namespace NitroSharp.Discord.Commands.Filter
{
    public class FilterLevelCommand : BaseCommandModule
    {
        private readonly NSDatabaseModel _model;

        public FilterLevelCommand(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("filterlevel")]
        [Description("Sets the strictness of filters on your server.")]
        [Aliases("flevel")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task FilterLevelCommandAsync(CommandContext ctx,
            [Description("Filter to set the severity for.")]
            string filterName,

            [Description("Severity level: `1` - Only if the word directly matches, `2` - if the word is found anywhere.")]
            int severity = 1)
        {
            var name = filterName.ToLower();

            if(severity < 1 || severity > 2)
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "Severity is out of bounds. Please pick a level, 1 or 2:\n" +
                    "`1` - Only if the word directly matches.\n" +
                    "`2` - If the word is found anywhere.");
                return;
            }

            var filter = await _model.FindAsync<GuildFilters>(ctx.Guild.Id);
            if(filter is null || !filter.Filters.TryGetValue(name, out var filterData))
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "Filter does not exist.");
                return;
            }

            filter.Filters.UpdateOrAddValue(name, new Tuple<int, HashSet<string>>(severity, filterData.Item2), filter, _model);

            await _model.SaveChangesAsync();

            await CommandUtils.RespondBasicSuccessAsync(ctx, $"Updated severity to level `{severity}` - " +
                $"{(severity == 1 ? "Only if the word directly matches." : "If the word is found anywhere.")}");
        }
    }
}
