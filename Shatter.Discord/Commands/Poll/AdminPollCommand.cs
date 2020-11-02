using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Shatter.Discord.Commands.Poll
{
    public class AdminPollCommand : CommandModule
    {
        [Command("poll")]
        [Description("Create a poll.")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task ExampleCommandAsync(CommandContext ctx,
            [Description("Channel for the poll to be dispalyed in")]
            DiscordChannel pollChannel,

            [Description("Duration of the poll. Ex. 3h30m")]
            TimeSpan duration,

            [Description("Questions and options seperated by a `|`.\nEx: Who is the best? | Option A | Option B")]
            string dataString)
        {
            throw new NotImplementedException();
        }
    }
}
