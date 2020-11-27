using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Core.Database;
using Shatter.Core.Structures.Guilds;
using Shatter.Discord.Commands.Attributes;
using Shatter.Discord.Utils;

namespace Shatter.Discord.Commands.Memberlog
{
    public class MemberlogMessageTestCommand : CommandModule
    {
        private readonly ShatterDatabaseContext _model;

        public MemberlogMessageTestCommand(ShatterDatabaseContext model)
        {
            this._model = model;
        }

        [Command("mlmtest")]
        [Description("Tests your set memberlog messages")]
        [Aliases("mlmessagetest", "memberlogmessagetest")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [ExecutionModule("memberlog")]
        public async Task MemberlogMessageTestCommandAsync(CommandContext ctx)
        {
            var guild = await _model.FindAsync<GuildMemberlogs>(ctx.Guild.Id);

            if (guild is null)
            {
                guild = new GuildMemberlogs(ctx.Guild.Id);
                await _model.AddAsync(guild);
            }

            if (guild.MemberlogChannel is null)
            {
                await RespondBasicErrorAsync($"No memberlog channel set. Set one with `{ctx.Prefix}mlchannel <#channel name>`");
                return;
            }

            if (guild.JoinMessage is null && guild.LeaveMessage is null)
            {
                await RespondBasicErrorAsync("No join or leave message set. Set one with `{ctx.Prefix}mlmsg`");
                return;
            }

            if (!(guild.JoinMessage is null))
            {
                await MemberLogingUtils.SendJoinMessageAsync(guild, ctx);
            }

            if (!(guild.LeaveMessage is null))
            {
                await MemberLogingUtils.SendLeaveMessageAsync(guild, ctx);
            }

            await RespondBasicSuccessAsync( "Test complete.");
        }
    }
}
