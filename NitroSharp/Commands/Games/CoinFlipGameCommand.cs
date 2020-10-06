using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Database;
using NitroSharp.Structures;

namespace NitroSharp.Commands.Games
{
    public class CoinFlipGameCommand : BaseCommandModule
    {
        private readonly NSDatabaseModel _model;
        private Random Random { get; set; }

        public CoinFlipGameCommand(NSDatabaseModel model)
        {
            this._model = model;
            Random = new Random();
        }

        [Command("coinflip")]
        [Description("Flip a Coin!")]
        [Aliases("coingame")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task ExampleCommandAsync(CommandContext ctx)
        {
            var wallet = _model.Wallets.Find(ctx.User.Id);
            if (wallet is null)
            {
                wallet = new Wallet(ctx.User.Id, ctx.User.Username);
            }

            if (wallet.HasEnough(1))
            {
                if (Random.NextDouble() >= 0.5)
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

            await _model.SaveChangesAsync();
        }
    }
}
