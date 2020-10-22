using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using NitroSharp.Database;
using NitroSharp.Structures;

namespace NitroSharp.Commands.Memberlog
{
    public class MemberlogChannelCommand : BaseCommandModule
    {
        private readonly NSDatabaseModel _model;

        public MemberlogChannelCommand(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("mlchannel")]
        [Description("Sets the channel to send memberlog messages in.")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [Aliases("memberlogchannel", "mlchan")]
        [Priority(2)]
        public async Task MemberlogChannelCommandAsync(CommandContext ctx,
            [Description("The channel memberlog messages are sent in.")]
            DiscordChannel? channel = null)
        {
            var guild = _model.Find<GuildConfig>(ctx.Guild.Id);

            if(guild is null)
            {
                guild = new GuildConfig(ctx.Guild.Id);
                await _model.AddAsync(guild);
            }

            Task res;
            if(channel is null)
            { // Disable the memberlog
                guild.MemberlogChannel = null;
                res = CommandUtils.RespondBasicSuccessAsync(ctx, "Disabled all member logs.");
            }
            else
            { // Enable the selected channel if it has the right permissions for the bot.
                var pFor = channel.PermissionsFor(ctx.Member);
                if (!(channel.Type == ChannelType.Text
                    && pFor.HasPermission(Permissions.SendMessages)
                    && pFor.HasPermission(Permissions.EmbedLinks)
                    && pFor.HasPermission(Permissions.AttachFiles)))
                {
                    await CommandUtils.RespondBasicErrorAsync(ctx, "Missing permissions in channel: Send Messages, Embed Links, Attach Files");
                    return;
                }

                guild.MemberlogChannel = channel.Id;
                res = CommandUtils.RespondBasicSuccessAsync(ctx, "Memberlogs will be sent in: " + channel.Mention);
            }

            await _model.SaveChangesAsync();
            await res;
        }
    }
}
