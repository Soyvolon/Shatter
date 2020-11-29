using System.Collections.Generic;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Shatter.Core.Database;
using Shatter.Core.Extensions;
using Shatter.Core.Structures.Guilds;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Filter
{
	public class FilterIgnoreCommand : CommandModule
    {
        private readonly ShatterDatabaseContext _model;

        public FilterIgnoreCommand(ShatterDatabaseContext model)
        {
            _model = model;
        }

        [Command("filterignore")]
        [Description("Sets a user as exempt from filters. Users with the Manage Message permission are automatically exmept from filters.")]
        [Aliases("ignorefilter", "fignore")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [RequireBotPermissions(Permissions.ManageMessages)]
        [ExecutionModule("filter")]
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
                await RespondBasicErrorAsync("Filter name not found.");
                return;
            }

            var exsists = filter.BypassFilters.TryGetValue(id, out HashSet<string>? currentNames);

            bool added = true;
            if (currentNames is null)
            {
                currentNames = new HashSet<string>();
                currentNames.Add(name);
            }
            else
            {
                if (currentNames.Contains(name))
                {
                    currentNames.Remove(name);
                    added = false;
                }
                else
				{
					currentNames.Add(name);
				}
			}

            filter.BypassFilters.UpdateOrAddValue(id, currentNames, filter, _model);
            await _model.SaveChangesAsync();

            await RespondBasicSuccessAsync( $"Bypass filter {(added ? "added" : "removed")}.");
        }
    }
}
