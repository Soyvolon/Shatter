using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Database;
using NitroSharp.Extensions;

namespace NitroSharp.Commands.Economy
{
    public class EconDailyCommand : BaseCommandModule
    {
        private const int DailyAmount = 50;
        private readonly NSDatabaseModel _model;

        public EconDailyCommand(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("daily")]
        [Description("Get your daily reward!")]
        public async Task EconDailyCommandAsync(CommandContext ctx)
        {
            var wallet = _model.Wallets.Find(ctx.Member.Id);

            bool save = false;

            if(wallet is null)
            {
                wallet = new Structures.Wallet(ctx.Member.Id, ctx.Member.Username);
                _model.Add(wallet);
                save = true;
            }

            TimeSpan diff;
            if((diff = DateTime.UtcNow - wallet.LastDaily).TotalDays > 1.0)
            {
                save = true;
                wallet.Add(DailyAmount);
                wallet.LastDaily = DateTime.UtcNow;
                await CommandUtils.RespondBasicSuccessAsync(ctx, $"Daily received! You are {DailyAmount.ToMoney()} richer!");
            }
            else
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, $"You have {diff:[d:]h:mm:ss} until you can get another daily reward!");
            }

            if (save)
                await _model.SaveChangesAsync();
        }
    }
}