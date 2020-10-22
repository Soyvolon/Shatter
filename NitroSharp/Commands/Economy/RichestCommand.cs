using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;

using Microsoft.EntityFrameworkCore;

using NitroSharp.Commands.CustomArguments;
using NitroSharp.Database;
using NitroSharp.Extensions;
using NitroSharp.Structures;

namespace NitroSharp.Commands.Economy
{
    public class RichestCommand : BaseCommandModule
    {
        private readonly NSDatabaseModel _model;

        public RichestCommand(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("richest")]
        [Description("Get the richest users.")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        [Cooldown(1, 10, CooldownBucketType.User)]
        [Priority(2)]
        public async Task RichestCommandAsync(CommandContext ctx, LeaderboardType type)
        {
            List<Wallet> wallets;

            wallets = _model.Wallets
                    .AsNoTracking()
                    .OrderByDescending(x => x.Balance)
                    .ToList();

            if (type == LeaderboardType.Server)
            {
                var users = await ctx.Guild.GetAllMembersAsync();

                wallets = wallets.Where(x => users.Any(y => y.Id == x.UserId)).ToList();
            }

            var cfgFinder = _model.FindAsync<GuildConfig>(ctx.Guild.Id);

            int total = 0;

            var totalCalc = Task.Run(() => total = wallets.AsParallel().Sum(x => x.Balance));

            var interact = ctx.Client.GetInteractivity();

            string data = "";

            int c = 1;

            foreach (var user in wallets)
            {
                data += $"{c++}. {Formatter.Bold(user.Username == "" ? user.UserId.ToString() : user.Username)} - {user.Balance.ToMoney()}\n";
            }

            if (data.Count() > 0)
            {
                data = data[..(data.Count() - 1)];

                await totalCalc;

                var embed = new DiscordEmbedBuilder()
                    .WithTitle($"{(type == LeaderboardType.Global ? "Global" : ctx.Guild.Name)} Leaderboard")
                    .AddField("In circulation:", $"{total.ToMoney()}")
                    .WithColor(CommandUtils.Colors[ColorType.Nitro].Random());

                var pages = interact.GeneratePagesInEmbed(data, SplitType.Line, embed);

                interact.SendPaginatedMessageAsync(ctx.Channel, ctx.Member, pages);
            }
            else
            {
                await CommandUtils.RespondBasicSuccessAsync(ctx, "No members with balances found!");
            }
        }

        [Command("richest")]
        [Priority(1)]
        public async Task RichestCommandAsync(CommandContext ctx) => await RichestCommandAsync(ctx, LeaderboardType.Global);
    }
}
