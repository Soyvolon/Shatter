﻿using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using NitroSharp.Database;
using NitroSharp.Extensions;
using NitroSharp.Structures;

namespace NitroSharp.Commands.Mod
{
    public class BanCommand : BaseCommandModule
    {
        private readonly NSDatabaseModel _model;

        public BanCommand(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("ban")]
        [Description("Bans a user from the server.")]
        [Priority(4)]
        [RequirePermissions(Permissions.BanMembers)]
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
                await CommandUtils.RespondBasicErrorAsync(ctx, $"Failed to ban {user.Mention}");
                return;
            }

            DateTime? bannedUntil = null;
            if (!(banLength is null))
            {

                var cfg = _model.Find<GuildConfig>(ctx.Guild.Id);
                if (cfg is null)
                {
                    cfg = new GuildConfig(ctx.Guild.Id);
                    _model.Add(cfg);
                }

                bannedUntil = DateTime.UtcNow.Add((TimeSpan)banLength);
                cfg.UserBans.UpdateOrAddValue(user.Id, (DateTime)bannedUntil, cfg, _model);

                await _model.SaveChangesAsync();
            }

            await CommandUtils.RespondBasicSuccessAsync(ctx, $"Banned {user.Mention} {(banLength is null ? "permanetly" : $"until {bannedUntil?.ToShortDateString() ?? "forever"}")}");
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
                await Program.Bot.Rest.CreateGuildBanAsync(ctx.Guild.Id, userId, 0, reason);
            }
            catch
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, $"Failed to ban {userId}");
                return;
            }

            DateTime? bannedUntil = null;
            if (!(banLength is null))
            {

                var cfg = _model.Find<GuildConfig>(ctx.Guild.Id);
                if (cfg is null)
                {
                    cfg = new GuildConfig(ctx.Guild.Id);
                    _model.Add(cfg);
                }

                bannedUntil = DateTime.UtcNow.Add((TimeSpan)banLength);
                cfg.UserBans.UpdateOrAddValue(userId, (DateTime)bannedUntil, cfg, _model);

                await _model.SaveChangesAsync();
            }

            await CommandUtils.RespondBasicSuccessAsync(ctx, $"Banned {userId} {(banLength is null ? "permanetly" : $"until {bannedUntil?.ToShortDateString() ?? "forever"}")}");
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