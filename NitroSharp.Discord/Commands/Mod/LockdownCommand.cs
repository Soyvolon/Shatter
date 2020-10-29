using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace NitroSharp.Discord.Commands.Mod
{
    public class LockdownCommand : BaseCommandModule
    {
        [Command("lockdown")]
        [Description("Locks down a channel and prevents everyone from sending messages there. Use this command to unlock a locked channel.")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task LockdownCommandAsync(CommandContext ctx,
            [Description("The channel to lockdown. Leave blank to lockdown the channel the command is run in.")]
            DiscordChannel? channel = null)
        {
            throw new NotImplementedException();
        }
    }
}
