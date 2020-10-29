using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using NitroSharp.Core.Database;
using NitroSharp.Core.Structures.Guilds;

namespace NitroSharp.Discord.Commands.Mod
{
    public class SetModLogCommand : BaseCommandModule
    {
        private readonly NSDatabaseModel _model;

        public SetModLogCommand(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("setmodlog")]
        [Description("Sets the mod log output to the current channel, or the designated channel.")]
        [Aliases("modlogs")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task SetModLogCommandAsync(CommandContext ctx,
            [Description("Channel to set the modlogs to. Leave blank to disable Mod Logs.")]
            DiscordChannel? discordChannel = null)
        {
            var cfg = _model.Find<GuildModeration>(ctx.Guild.Id);
            if (cfg is null)
            {
                cfg = new GuildModeration(ctx.Guild.Id);
                _model.Add(cfg);
            }

            cfg.ModLogChannel = discordChannel?.Id ?? null;

            if (discordChannel is null)
            {
                await CommandUtils.RespondBasicSuccessAsync(ctx, "Modlogs disabled.");
            }
            else
            {
                await CommandUtils.RespondBasicSuccessAsync(ctx, $"Modlogs set to {discordChannel.Mention}");
            }

            await _model.SaveChangesAsync();
        }
    }
}
