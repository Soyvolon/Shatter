using System.Collections.Generic;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using NitroSharp.Core.Database;
using NitroSharp.Core.Extensions;
using NitroSharp.Core.Structures.Guilds;

namespace NitroSharp.Discord.Commands.Filter
{
    public class FilterIgnoreCommand : BaseCommandModule
    {
        private readonly NSDatabaseModel _model;

        public FilterIgnoreCommand(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("filterignore")]
        [Description("Sets a user as exempt from filters. Users with the Manage Message permission are automatically exmept from filters.")]
        [Aliases("ignorefilter", "fignore")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task FilterIgnoreCommandAsync(CommandContext ctx,
            [Description("User to toggle exemption status for.")]
            DiscordMember discordMember,

            [Description("Filter to ignore. Leave blank to apply to all filters.")]
            string? filterName = null)
            => await FilterIgnoreCommandAsync(ctx, discordMember.Id, filterName);

        [Command("filterignore")]
        public async Task FilterIgnoreCommandAsync(CommandContext ctx,
            [Description("Channel to toggle exemption status for.")]
            DiscordChannel channel,

            [Description("Filter to ignore. Leave blank to apply to all filters")]
            string? filterName = null)
            => await FilterIgnoreCommandAsync(ctx, channel.Id, filterName);

        [Command("filterignore")]
        public async Task FilterIgnoreCommandAsync(CommandContext ctx,
            [Description("Role to toggle exemption status for.")]
            DiscordRole role,

            [Description("Filter to ignore. Leave blank to apply to all filters")]
            string? filterName = null)
            => await FilterIgnoreCommandAsync(ctx, role.Id, filterName);

        private async Task FilterIgnoreCommandAsync(CommandContext ctx, ulong id, string? filterName = null)
        {
            var name = filterName?.ToLower() ?? GuildFilters.AllFilters; // A indicates ALL filters.

            var filter = await _model.FindAsync<GuildFilters>(ctx.Guild.Id);
            if (filter is null)
            {
                filter = new GuildFilters(ctx.Guild.Id);
                await _model.AddAsync(filter);
                await _model.SaveChangesAsync();
            }

            if (name != GuildFilters.AllFilters && !filter.Filters.TryGetValue(name, out var fData))
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "Filter name not found.");
                return;
            }

            var currentBypass = filter.BypassFilters.TryGetValue(id, out HashSet<string>? currentNames);

            if (currentNames is null) currentNames = new HashSet<string>();
            currentNames.Add(name);

            filter.BypassFilters.UpdateOrAddValue(id, currentNames, filter, _model);
            await _model.SaveChangesAsync();

            await CommandUtils.RespondBasicSuccessAsync(ctx, "Bypass filter added.");
        }
    }
}
