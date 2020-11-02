using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Shatter.Discord.Commands.Info
{
    public class UserInfoCommand : CommandModule
    {
        [Command("userinfo")]
        [Description("Get Information")]
        public async Task UserInfoCommandAsync(CommandContext ctx, DiscordMember m)
        {
            var roles = m.Roles.Where(x => !x.Name.Contains("everyone"));
            List<string> roleStrings = new List<string>();
            foreach (var role in roles)
                roleStrings.Add(role.Mention);

            var embed = CommandModule.SuccessBase()
                .WithTitle(m.Username + "#" + m.Discriminator)
                .WithDescription(m.Nickname ?? "No Nickname")
                .AddField("Roles", string.Join(", ", roleStrings))
                .WithAuthor(m.Id.ToString(), null, m.AvatarUrl)
                .AddField("Joined", m.JoinedAt.ToString("f"), true)
                .AddField("Account Created", m.CreationTimestamp.ToString("f"), true);

            await ctx.RespondAsync(embed: embed);
        }

        [Command("userinfo")]
        public async Task UserInfoCommandAsync(CommandContext ctx) => await UserInfoCommandAsync(ctx, ctx.Member);
    }
}
