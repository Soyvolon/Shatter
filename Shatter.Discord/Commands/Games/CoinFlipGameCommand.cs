using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Core.Database;
using Shatter.Core.Structures;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Games
{
	public class CoinFlipGameCommand : CommandModule
    {
        private readonly ShatterDatabaseContext _model;
        private Random Random { get; set; }

        public CoinFlipGameCommand(ShatterDatabaseContext model)
        {
			this._model = model;
			this.Random = new Random();
        }

        [Command("coinflip")]
        [Description("Flip a Coin!")]
        [Aliases("coingame")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        [RequireBotPermissions(Permissions.SendMessages)]
        [ExecutionModule("games")]
        public async Task ExampleCommandAsync(CommandContext ctx)
        {
            var wallet = this._model.Wallets.Find(ctx.User.Id);
            if (wallet is null)
            {
                wallet = new Wallet(ctx.User.Id, ctx.User.Username);
            }

            if (wallet.HasEnough(1))
            {
                if (this.Random.NextDouble() >= 0.5)
                {
                    wallet.Add(1);
                    await ctx.RespondAsync("Heads! Here, keep it.");
                }
                else
                {
                    wallet.Subtract(1);
                    await ctx.RespondAsync("Tails. I'll take that coin back.");
                }
            }
            else
            {
                await ctx.RespondAsync("You don't have a coin to bet!");
            }

            await this._model.SaveChangesAsync();
        }
    }
}
