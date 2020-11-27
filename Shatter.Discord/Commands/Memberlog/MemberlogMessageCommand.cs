using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Core.Database;
using Shatter.Core.Structures;
using Shatter.Core.Structures.Guilds;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Memberlog
{
    public class MemberlogMessageCommand : CommandModule
    {
        private readonly ShatterDatabaseContext _model;

        public MemberlogMessageCommand(ShatterDatabaseContext model)
        {
            this._model = model;
        }

        [Command("mlmessage")]
        [Description("Set the memberlog join or leave message and type")]
        [Aliases("mlmsg", "memberlogmessage")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [ExecutionModule("memberlog")]
        public async Task MemberlogMessageCommandAsync(CommandContext ctx,
            [Description("For the `join` or `leave` message")]
            string selection,

            [Description("The type of message to send. `text`, `embed`, `image`, or `disable` to turn off.")]
            string type,

            [Description("The text sent in the message")]
            [RemainingText]
            string message)
        {
            var s = selection.Trim().ToLower();
            if (!new string[] { "join", "leave" }.Contains(s))
            {
                await RespondBasicErrorAsync("Selection must be one of the following: `join` or `leave`");
                return;
            }

            var t = type.Trim().ToLower();
            if (!new string[] { "text", "embed", "image", "disable" }.Contains(t))
            {
                await RespondBasicErrorAsync("Type must be one of the following: `text`, `embed`, `image`, or `disable` to disable.");
                return;
            }

            if (t == "text" && (message is null || message == ""))
            {
                await RespondBasicErrorAsync("Type `text` needs a message.");
                return;
            }

            var guild = await _model.FindAsync<GuildMemberlogs>(ctx.Guild.Id);

            if (guild is null)
            {
                guild = new GuildMemberlogs(ctx.Guild.Id);
                await _model.AddAsync(guild);
            }

            Task res;
            if (s == "join")
            {
                if (t == "disable")
                {
                    guild.JoinMessage = null;

                    res = RespondBasicSuccessAsync( "Disabled the join message.");
                }
                else
                {
                    guild.JoinMessage = new MemberlogMessage()
                    {
                        Message = message,
                        IsImage = t == "image",
                        IsEmbed = t == "embed",
                    };

                    res = RespondBasicSuccessAsync( "Set the join message.");
                }
            }
            else
            {
                if (t == "disable")
                {
                    guild.LeaveMessage = null;

                    res = RespondBasicSuccessAsync( "Disabled the leave message.");
                }
                else
                {
                    guild.LeaveMessage = new MemberlogMessage()
                    {
                        Message = message,
                        IsImage = t == "image",
                        IsEmbed = t == "embed",
                    };

                    res = RespondBasicSuccessAsync( "Set the join message.");
                }
            }

            await _model.SaveChangesAsync();
        }
    }
}
