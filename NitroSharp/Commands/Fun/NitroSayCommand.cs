﻿using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace NitroSharp.Commands.Fun
{
    public class NitroSayCommand : BaseCommandModule
    {
        [Command("say")]
        [Description("Makes Nitro say a message")]
        [Aliases("echo")]
        [RequireUserPermissions(Permissions.ManageMessages)]
        [RequireBotPermissions(Permissions.ManageMessages)]
        public async Task NitroSayCommandAsync(CommandContext ctx,
            [Description("What do you want Nitro to say?")]
            [RemainingText]
            string msg)
        {
            await ctx.Message.DeleteAsync();
            await ctx.RespondAsync(msg);
        }
    }
}