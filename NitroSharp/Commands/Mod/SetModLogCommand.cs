using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace NitroSharp.Commands.Mod
{
    public class SetModLogCommand : BaseCommandModule
    {
        [Command("setmodlog")]
        [Description("Sets the mod log output to the current channel, or the designated channel.")]
        [Aliases("modlogs")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task SetModLogCommandAsync(CommandContext ctx,
            [Description("Channel to set the modlogs to. Leave blank to set in the channel the command was run in.")]
            DiscordChannel? discordChannel = null)
        {
            throw new NotImplementedException();
        }
    }
}
