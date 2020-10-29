using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using NitroSharp.Database;
using NitroSharp.Structures;
using NitroSharp.Structures.Guilds;

namespace NitroSharp.Commands.Mod
{
    public class UnbanCommand : BaseCommandModule
    {
        private readonly NSDatabaseModel _model;

        public UnbanCommand(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("unban")]
        [Description("Unbans a user for the server.")]
        [Aliases("uban")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task UnbanCommandAsync(CommandContext ctx,
            [Description("User to unban. Can be an ID")]
            DiscordUser user,
            
            [Description("Reason for unban.")]
            [RemainingText]
            string reason = "unspecified")
        {
            try
            {
                await user.UnbanAsync(ctx.Guild, reason);
            }
            catch
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, $"Failed to unban {user.Id}");
            }

            var cfg = _model.Find<GuildModeration>(ctx.Guild.Id);

            if(!(cfg is null))
            {
                // Remove a temp ban if it exsists.
                cfg.UserBans.TryRemove(user.Id, out _);
            }

            await CommandUtils.RespondBasicSuccessAsync(ctx, $"Unbanned {user.Id}");
        }
    }
}
