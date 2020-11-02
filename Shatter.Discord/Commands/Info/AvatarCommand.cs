using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Info
{
    public class AvatarCommand : CommandModule
    {
        [Command("avatar")]
        [Description("Get someones avatar")]
        [Priority(2)]
        [ExecutionModule("info")]
        public async Task AvatarCommandAsync(CommandContext ctx, DiscordMember member)
        {
            await ctx.RespondAsync("Here is the avatar for: " + member.DisplayName + "\n" + member.AvatarUrl);
        }

        [Command("avatar")]
        public async Task AvatarCommandAsync(CommandContext ctx) => await AvatarCommandAsync(ctx, ctx.Member);
    }
}
