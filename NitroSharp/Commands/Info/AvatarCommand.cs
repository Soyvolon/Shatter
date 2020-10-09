using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace NitroSharp.Commands.Info
{
    public class AvatarCommand : BaseCommandModule
    {
        [Command("avatar")]
        [Description("Get someones avatar")]
        [Priority(2)]
        public async Task AvatarCommandAsync(CommandContext ctx, DiscordMember member)
        {
            await ctx.RespondAsync("Here is the avatar for: " + member.DisplayName + "\n" + member.AvatarUrl);
        }

        [Command("avatar")]
        public async Task AvatarCommandAsync(CommandContext ctx) => await AvatarCommandAsync(ctx, ctx.Member);
    }
}
