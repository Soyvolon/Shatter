using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Core.Database;
using Shatter.Core.Extensions;
using Shatter.Core.Structures;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Economy
{
	public class EconDailyCommand : CommandModule
    {
        private const int DailyAmount = 50;
        private readonly ShatterDatabaseContext _model;

        public EconDailyCommand(ShatterDatabaseContext model)
        {
			this._model = model;
        }

        [Command("daily")]
        [Description("Get your daily reward!")]
        [ExecutionModule("economy")]
        public async Task EconDailyCommandAsync(CommandContext ctx)
        {
            var wallet = this._model.Wallets.Find(ctx.Member.Id);

            bool save = false;

            if (wallet is null)
            {
                wallet = new Wallet(ctx.Member.Id, ctx.Member.Username);
				this._model.Add(wallet);
                save = true;
            }

            TimeSpan diff;
            if ((diff = DateTime.UtcNow - wallet.LastDaily).TotalDays > 1.0)
            {
                save = true;
                wallet.Add(DailyAmount);
                wallet.LastDaily = DateTime.UtcNow;
                await RespondBasicSuccessAsync( $"Daily received! You are {DailyAmount.ToMoney()} richer!");
            }
            else
            {
                await RespondBasicErrorAsync($"You have {diff:[d:]h:mm:ss} until you can get another daily reward!");
            }

            if (save)
			{
				await this._model.SaveChangesAsync();
			}
		}
    }
}