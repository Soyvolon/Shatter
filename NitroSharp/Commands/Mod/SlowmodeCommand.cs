using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace NitroSharp.Commands.Mod
{
    public class SlowmodeCommand : BaseCommandModule
    {
        [Command("slowmode")]
        [Description("Actives slowmode for a channel.")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task SlowmodeCommandAsync(CommandContext ctx,
            [Description("Channel to active slowmode for. Leave blank to activate it for this channel.")]
            DiscordChannel? slowmodeChannel = null,

            [Description("Slowmode interval in seconds. Leave blank for a 3s slowmode.")]
            int interval = 3,

            [Description("How long should the slowmode last? Leave blank to never automatically turn off. (Timed slowmodes only supports time spans of hours and higher)")]
            TimeSpan? slowmodeLength = null)
        {
            throw new NotImplementedException();
        }
    }
}
