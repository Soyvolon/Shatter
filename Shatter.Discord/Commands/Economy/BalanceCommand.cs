using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Shatter.Core.Database;
using Shatter.Core.Extensions;
using Shatter.Core.Structures;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Economy
{
	public class BalanceCommand : CommandModule
    {
        private readonly ShatterDatabaseContext _model;

        public BalanceCommand(ShatterDatabaseContext model)
        {
			this._model = model;
        }

        [Command("balance")]
        [Description("Shows your current balance.")]
        [Aliases(new string[] { "bal", "money" })]
        [ExecutionModule("economy")]
        public async Task BalanceCommandAsync(CommandContext ctx, DiscordMember? member = null)
        {
            bool save = false;

            DiscordMember m;
            if (member is null)
			{
				m = ctx.Member;
			}
			else
			{
				m = member;
			}

			var wallet = this._model.Wallets.Find(m.Id);

            if (wallet is null)
            {
                wallet = new Wallet(m.Id, ctx.User.Username);
				// Save the new wallet to the database.
				this._model.Wallets.Add(wallet);
                save = true;
            }
            else
            {
                if (wallet.Username != ctx.Member.Username)
                {
                    wallet.Username = ctx.Member.Username;
                    save = true;
                }
            }

            if (save)
			{
				_ = await this._model.SaveChangesAsync();
			}

			var b = CommandModule.SuccessBase()
                .WithTitle($"{m.DisplayName}'s Balance")
                .WithDescription(Formatter.Bold(wallet.Balance.ToMoney()));

            await ctx.RespondAsync(embed: b.Build());
        }
    }
}
