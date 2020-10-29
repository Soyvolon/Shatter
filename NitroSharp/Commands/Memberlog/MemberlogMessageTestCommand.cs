using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.EventArgs;

using NitroSharp.Database;
using NitroSharp.Structures;
using NitroSharp.Structures.Guilds;

namespace NitroSharp.Commands.Memberlog
{
    public class MemberlogMessageTestCommand : BaseCommandModule
    {
        private readonly NSDatabaseModel _model;

        public MemberlogMessageTestCommand(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("mlmtest")]
        [Description("Tests your set memberlog messages")]
        [Aliases("mlmessagetest", "memberlogmessagetest")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task MemberlogMessageTestCommandAsync(CommandContext ctx)
        {
            var guild = await _model.FindAsync<GuildMemberlogs>(ctx.Guild.Id);

            if(guild is null)
            {
                guild = new GuildMemberlogs(ctx.Guild.Id);
                await _model.AddAsync(guild);
            }

            if(guild.MemberlogChannel is null)
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, $"No memberlog channel set. Set one with `{ctx.Prefix}mlchannel <#channel name>`");
                return;
            }

            if(guild.JoinMessage is null && guild.LeaveMessage is null)
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "No join or leave message set. Set one with `{ctx.Prefix}mlmsg`");
                return;
            }

            if(!(guild.JoinMessage is null))
            {
                await Program.Bot.SendJoinMessageAsync(guild, ctx);
            }

            if(!(guild.LeaveMessage is null))
            {
                await Program.Bot.SendLeaveMessageAsync(guild, ctx);
            }

            await CommandUtils.RespondBasicSuccessAsync(ctx, "Test complete.");
        }
    }
}
