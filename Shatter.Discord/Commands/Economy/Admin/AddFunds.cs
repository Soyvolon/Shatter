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
	public class AddFunds : CommandModule
    {
        private readonly ShatterDatabaseContext _model;

        public AddFunds(ShatterDatabaseContext model)
        {
			this._model = model;
        }

        [Command("addfunds")]
        [Description("Adds money to a Wallet.")]
        [Aliases("addmoney")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [ExecutionModule("economy")]
        public async Task AddFundsCommandAsync(CommandContext ctx, int ammount, DiscordMember m)
        {
            var wallet = await this._model.FindAsync<Wallet>(m.Id);

            if (wallet is null)
            {
                wallet = new Wallet(m.Id, ctx.Member.Username);
				this._model.Add(wallet);
            }
            else
            {
                wallet.Username = m.Username;
            }

            var res = wallet.Add(ammount);

            await this._model.SaveChangesAsync();

            await ctx.RespondAsync($"Added {ammount.ToMoney()} to {m.Nickname}. Their balance is now {res.ToMoney()}");
        }
    }
}
