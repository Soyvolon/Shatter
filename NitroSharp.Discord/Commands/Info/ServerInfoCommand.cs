using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace NitroSharp.Commands.Info
{
    public class ServerInfoCommand : BaseCommandModule
    {
        [Command("serverinfo")]
        [Description("Get server information")]
        public async Task ServerInfoCommandAsync(CommandContext ctx)
        {
            int users = 0, bots = 0;
            var memberStats = "";

            var members = await ctx.Guild.GetAllMembersAsync();

            foreach (var m in members)
            {
                try
                {
                    if (m.IsBot) bots++;
                    else users++;
                }
                catch
                {
                    users++;
                }
            }

            int total = bots + users;

            memberStats += $"Total: {total}\nMembers: {users}\nBots: {bots}";

            var embed = CommandUtils.SuccessBase()
                .WithAuthor(ctx.Guild.Name, null, ctx.Guild.IconUrl)
                .WithTitle("Server Statistics")
                .AddField("Members", memberStats)
                .AddField("Created On", ctx.Guild.CreationTimestamp.ToString("f"), true)
                .AddField("Roles:", ctx.Guild.Roles.Count.ToString(), true);

            await ctx.RespondAsync(embed: embed);
        }
    }
}
