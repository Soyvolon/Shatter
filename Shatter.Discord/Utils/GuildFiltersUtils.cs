using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.EventArgs;

using Microsoft.EntityFrameworkCore.Metadata.Conventions;

using Shatter.Core.Structures.Guilds;

namespace Shatter.Discord.Utils
{
    public static class GuildFiltersUtil
    {
        public static ConcurrentDictionary<ulong, Tuple<Task, CancellationTokenSource>> FilterUtils = new ConcurrentDictionary<ulong, Tuple<Task, CancellationTokenSource>>();

        public static async Task Run(GuildFilters filter, MessageCreateEventArgs e, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var direct = filter.DirectMatches;
                var anywhere = filter.FoundAnywhereMatches;

                if (filter.BypassFilters.ContainsKey(e.Channel.Id))
                {
                    var bypasses = filter.BypassFilters[e.Channel.Id];
                    if (bypasses.Contains(GuildFilters.AllFilters))
                        return;
                    else
                    {
                        foreach(var b in bypasses)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            direct.ExceptWith(filter.Filters[b].Item2);
                            anywhere.ExceptWith(filter.Filters[b].Item2);
                        }
                    }
                }

                if (filter.BypassFilters.ContainsKey(e.Author.Id))
                {
                    var bypasses = filter.BypassFilters[e.Author.Id];
                    if (bypasses.Contains(GuildFilters.AllFilters))
                        return;
                    else
                    {
                        foreach (var b in bypasses)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            direct.ExceptWith(filter.Filters[b].Item2);
                            anywhere.ExceptWith(filter.Filters[b].Item2);
                        }
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();

                var member = await e.Guild.GetMemberAsync(e.Author.Id);

                // Guild managers auto bypass.
                if (member.PermissionsIn(e.Channel).HasPermission(Permissions.ManageGuild))
                    return;

                foreach(var role in member.Roles)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if(filter.BypassFilters.ContainsKey(role.Id))
                    {
                        var bypasses = filter.BypassFilters[role.Id];
                        if (bypasses.Contains(GuildFilters.AllFilters))
                            return;
                        else
                        {
                            foreach (var b in bypasses)
                            {
                                cancellationToken.ThrowIfCancellationRequested();

                                direct.ExceptWith(filter.Filters[b].Item2);
                                anywhere.ExceptWith(filter.Filters[b].Item2);
                            }
                        }
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();

                var content = e.Message.Content.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                foreach (var word in content)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (direct.Contains(word))
                    {
                        await e.Message.DeleteAsync();
                        return;
                    }
                    else
                    {
                        foreach (var part in anywhere)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            if (word.Contains(part))
                            {
                                await e.Message.DeleteAsync();
                                return;
                            }
                        }
                    }
                }
            }
            finally
            {
                FilterUtils.TryRemove(e.Message.Id, out _);
            }
        }
    }
}
