using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Shatter.Core.Database;
using Shatter.Core.Structures.Guilds;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Memberlog
{
	public class MemberlogChannelCommand : CommandModule
    {
        private readonly ShatterDatabaseContext _model;

        public MemberlogChannelCommand(ShatterDatabaseContext model)
        {
			this._model = model;
        }

        [Command("memberlogchannel")]
        [Description("Sets the channel to send memberlog messages in.")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [Aliases("mlchannel", "mlchan")]
        [Priority(2)]
        [ExecutionModule("memberlog")]
        public async Task MemberlogChannelCommandAsync(CommandContext ctx,
            [Description("The channel memberlog messages are sent in.")]
            DiscordChannel? channel = null)
        {
            var guild = this._model.Find<GuildMemberlogs>(ctx.Guild.Id);

            if (guild is null)
            {
                guild = new GuildMemberlogs(ctx.Guild.Id);
                await this._model.AddAsync(guild);
            }

            Task res;
            if (channel is null)
            { // Disable the memberlog
                guild.MemberlogChannel = null;
                res = RespondBasicSuccessAsync( "Disabled all member logs.");
            }
            else
            { // Enable the selected channel if it has the right permissions for the bot.
                var pFor = channel.PermissionsFor(ctx.Member);
                if (!(channel.Type == ChannelType.Text
                    && pFor.HasPermission(Permissions.SendMessages)
                    && pFor.HasPermission(Permissions.EmbedLinks)
                    && pFor.HasPermission(Permissions.AttachFiles)))
                {
                    await RespondBasicErrorAsync("Missing permissions in channel: Send Messages, Embed Links, Attach Files");
                    return;
                }

                guild.MemberlogChannel = channel.Id;
                res = RespondBasicSuccessAsync( "Memberlogs will be sent in: " + channel.Mention);
            }

            await this._model.SaveChangesAsync();
            await res;
        }
    }
}
