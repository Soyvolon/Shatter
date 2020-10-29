using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus.EventArgs;

using NitroSharp.Core.Structures.Guilds;

namespace NitroSharp.Discord.Utils
{
    public static class GuildFiltersUtil
    {
        public static ConcurrentDictionary<ulong, Tuple<Task, CancellationTokenSource>> FilterUtils = new ConcurrentDictionary<ulong, Tuple<Task, CancellationTokenSource>>();

        public static async Task Run(GuildFilters filter, MessageCreateEventArgs e, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (filter.BypassFilters.ContainsKey(e.Channel.Id))
                    return;
                if (filter.BypassFilters.ContainsKey(e.Author.Id))
                    return;

                cancellationToken.ThrowIfCancellationRequested();

                var content = e.Message.Content.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                var direct = filter.DirectMatches;
                var anywhere = filter.FoundAnywhereMatches;

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
