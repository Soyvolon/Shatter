using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Core.Database;
using Shatter.Core.Structures.Guilds;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Memberlog
{
	public class JoinDmCommand : CommandModule
    {
        private readonly ShatterDatabaseContext _model;

        public JoinDmCommand(ShatterDatabaseContext model)
        {
			this._model = model;
        }

        [Command("joindm")]
        [Description("Send a message when a user joins.")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [ExecutionModule("memberlog")]
        public async Task JoinDmCommandAsync(CommandContext ctx,
            [Description("Message to send, `info` to see more information about this command, or `disable` to disable the join DMs.")]
            [RemainingText]
            string message)
        {
            string msg = message?.Trim().ToLower() ?? "info";

            if (msg.Equals("info"))
            {
                var embed = CommandModule.SuccessBase()
                    .WithTitle("Join DM Message Information")
                    .WithDescription("Certain words wrapped in brackets `{ }` can be used to replace parts of your message progrmatically.\n" +
                    "**```http\n" +
                    "server, guild :: Gets replaced with your server name.\n" +
                    "user, member  :: Gets replaced with the name of the user who just joined.\n" +
                    "membercount   :: Gets replaced with the total member count at the time.\n" +
                    "```**\n" +
                    "Ex: Welcome to {server}, {member}! You are the {membercount} person to join!");

                await ctx.RespondAsync(embed: embed);

                return;
            }

            var guild = this._model.Find<GuildMemberlogs>(ctx.Guild.Id);

            if (guild is null)
            {
                guild = new GuildMemberlogs(ctx.Guild.Id);
				this._model.Add(guild);
            }

            if (msg.Equals("disable"))
            {
                guild.JoinDmMessage = null;
                await RespondBasicSuccessAsync( "Disabled Join DMs");
            }
            else
            {
                guild.JoinDmMessage = message;
                await RespondBasicSuccessAsync( $"Set the join DM message to: \n```\n{message}```");
            }

            await this._model.SaveChangesAsync();
        }
    }
}
