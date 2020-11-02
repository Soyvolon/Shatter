using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Shatter.Core.Database;
using Shatter.Core.Extensions;
using Shatter.Core.Structures;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Economy.Admin
{
    public class SubFunds : CommandModule
    {
        private readonly NSDatabaseModel _model;

        public SubFunds(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("subfunds")]
        [Description("Remove funds from a Wallet.")]
        [Aliases("submoney")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [ExecutionModule("economy")]
        public async Task SubtractFundsCommandAsync(CommandContext ctx, int ammount, DiscordMember m)
        {
            var wallet = await _model.FindAsync<Wallet>(m.Id);

            if (wallet is null)
            {
                wallet = new Wallet(m.Id, ctx.Member.Username);
                _model.Add(wallet);
            }
            else
            {
                wallet.Username = m.Username;
            }

            var res = wallet.Subtract(ammount);

            if (res < 0)
                res = wallet.Add(Math.Abs(res));

            await _model.SaveChangesAsync();

            await ctx.RespondAsync($"Removed {ammount.ToMoney()} from {m.Nickname}. Their balance is now {res.ToMoney()}");
        }
    }
}
