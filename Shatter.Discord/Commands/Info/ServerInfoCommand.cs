using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Info
{
	public class ServerInfoCommand : CommandModule
    {
        [Command("serverinfo")]
        [Description("Get server information")]
        [ExecutionModule("info")]
        public async Task ServerInfoCommandAsync(CommandContext ctx)
        {
            int users = 0, bots = 0;
            var memberStats = "";

            var members = await ctx.Guild.GetAllMembersAsync();

            foreach (var m in members)
            {
                try
                {
                    if (m.IsBot)
					{
						bots++;
					}
					else
					{
						users++;
					}
				}
                catch
                {
                    users++;
                }
            }

            int total = bots + users;

            memberStats += $"Total: {total}\nMembers: {users}\nBots: {bots}";

            var embed = CommandModule.SuccessBase()
                .WithAuthor(ctx.Guild.Name, null, ctx.Guild.IconUrl)
                .WithTitle("Server Statistics")
                .AddField("Members", memberStats)
                .AddField("Created On", ctx.Guild.CreationTimestamp.ToString("f"), true)
                .AddField("Roles:", ctx.Guild.Roles.Count.ToString(), true);

            await ctx.RespondAsync(embed: embed);
        }
    }
}
