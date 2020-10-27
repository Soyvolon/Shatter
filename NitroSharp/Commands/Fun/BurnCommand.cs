﻿using System.IO;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using NitroSharp.Properties;

namespace NitroSharp.Commands.Fun
{
    public class BurnCommand : BaseCommandModule
    {

        [Command("burn")]
        [Description("Burn another user!")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        [RequireBotPermissions(Permissions.AttachFiles)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task BurnCommandAsnyc(CommandContext ctx, DiscordMember m)
        {
            var msg = $"{Formatter.Bold(ctx.Member.DisplayName)} " +
                $"{Formatter.Italic("burned")} {Formatter.Bold(m.DisplayName)}";

            await ctx.Channel.SendFileAsync("burn-img.gif", new MemoryStream(Resources.Images_Burn), msg);
        }
    }
}