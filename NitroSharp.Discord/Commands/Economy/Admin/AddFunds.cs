using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using NitroSharp.Core.Database;
using NitroSharp.Core.Extensions;
using NitroSharp.Core.Structures;

namespace NitroSharp.Discord.Commands.Economy.Admin
{
    public class AddFunds : BaseCommandModule
    {
        private readonly NSDatabaseModel _model;

        public AddFunds(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("addfunds")]
        [Description("Adds money to a Wallet.")]
        [Aliases("addmoney")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task AddFundsCommandAsync(CommandContext ctx, int ammount, DiscordMember m)
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

            var res = wallet.Add(ammount);

            await _model.SaveChangesAsync();

            await ctx.RespondAsync($"Added {ammount.ToMoney()} to {m.Nickname}. Their balance is now {res.ToMoney()}");
        }
    }
}
