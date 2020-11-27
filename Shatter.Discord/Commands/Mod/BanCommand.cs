using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Shatter.Core.Database;
using Shatter.Core.Extensions;
using Shatter.Core.Structures.Guilds;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Mod
{
	public class BanCommand : CommandModule
    {
        private readonly ShatterDatabaseContext _model;

        public BanCommand(ShatterDatabaseContext model)
        {
            this._model = model;
        }

        [Command("ban")]
        [Description("Bans a user from the server.")]
        [Priority(4)]
        [RequirePermissions(Permissions.BanMembers)]
        [ExecutionModule("mod")]
        public async Task BanCommandAsync(CommandContext ctx,
            [Description("User to ban from the server. Can be a user mention or ID")]
            DiscordMember user,

            [Description("Length of the ban")]
            TimeSpan? banLength = null,

            [Description("Reason they were banned")]
            [RemainingText]
            string reason = "unspecified")
        {
            try
            {
                await user.BanAsync(0, reason);
            }
            catch
            {
                await RespondBasicErrorAsync($"Failed to ban {user.Mention}");
                return;
            }

            DateTime? bannedUntil = null;
            if (!(banLength is null))
            {
                var cfg = _model.Find<GuildModeration>(ctx.Guild.Id);
                if (cfg is null)
                {
                    cfg = new GuildModeration(ctx.Guild.Id);
                    _model.Add(cfg);
                    await _model.SaveChangesAsync();
                }

                bannedUntil = DateTime.UtcNow.Add((TimeSpan)banLength);
                cfg.UserBans.UpdateOrAddValue(user.Id, (DateTime)bannedUntil, cfg, _model);

                await _model.SaveChangesAsync();
            }

            await RespondBasicSuccessAsync( $"Banned {user.Mention} {(banLength is null ? "permanetly" : $"until {bannedUntil?.ToShortDateString() ?? "forever"}")}");
        }

        [Command("ban")]
        [Priority(3)]
        public async Task BanCommandAsync(CommandContext ctx,
            [Description("User to ban from the server. Can be a user mention or ID")]
            DiscordMember user,

            [Description("Reason they were banned")]
            [RemainingText]
            string reason = "unspecified")
            => await BanCommandAsync(ctx, user, null, reason);

        [Command("ban")]
        [Priority(2)]
        public async Task BanCommandAsync(CommandContext ctx,
            [Description("User id of the user to ban")]
            ulong userId,

            [Description("Length of the ban")]
            TimeSpan? banLength = null,

            [Description("Reason they were banned")]
            [RemainingText]
            string reason = "unspecified")
        {
            try
            {
                await DiscordBot.Bot.Rest.CreateGuildBanAsync(ctx.Guild.Id, userId, 0, reason);
            }
            catch
            {
                await RespondBasicErrorAsync($"Failed to ban {userId}");
                return;
            }

            DateTime? bannedUntil = null;
            if (!(banLength is null))
            {
                var cfg = _model.Find<GuildModeration>(ctx.Guild.Id);
                if (cfg is null)
                {
                    cfg = new GuildModeration(ctx.Guild.Id);
                    _model.Add(cfg);
                    await _model.SaveChangesAsync();
                }

                bannedUntil = DateTime.UtcNow.Add((TimeSpan)banLength);
                cfg.UserBans.UpdateOrAddValue(userId, (DateTime)bannedUntil, cfg, _model);

                await _model.SaveChangesAsync();
            }

            await RespondBasicSuccessAsync( $"Banned {userId} {(banLength is null ? "permanetly" : $"until {bannedUntil?.ToShortDateString() ?? "forever"}")}");
        }

        [Command("ban")]
        public async Task BanCommandAsync(CommandContext ctx,
            [Description("User id of the user to ban")]
            ulong userId,

            [Description("Reason they were banned")]
            [RemainingText]
            string reason = "unspecified")
        => await BanCommandAsync(ctx, userId, null, reason);
    }
}
