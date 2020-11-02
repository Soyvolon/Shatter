using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Fun
{
    public class NitroSayCommand : CommandModule
    {
        [Command("say")]
        [Description("Makes Nitro say a message")]
        [Aliases("echo")]
        [RequireUserPermissions(Permissions.ManageMessages)]
        [RequireBotPermissions(Permissions.ManageMessages)]
        [ExecutionModule("fun")]
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