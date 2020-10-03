using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using NitroSharp.Database;
using NitroSharp.Extensions;
using NitroSharp.Structures;

namespace NitroSharp.Commands.Economy.Admin
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

            if(wallet is null)
            {
                wallet = new Wallet(m.Id, ctx.Member.Username);
                _model.Add(wallet);
            }
            else
            {
                wallet.Username = m.Username;
            }

            var cfg = await _model.FindAsync<GuildConfig>(ctx.Guild.Id);

            if(cfg is null)
            {
                cfg = new GuildConfig(ctx.Guild.Id);
                _model.Add(cfg);
            }

            var res = wallet.Add(ammount);

            await _model.SaveChangesAsync();

            await ctx.RespondAsync($"Added {ammount.ToMoney(cfg.Culture)} to {m.Nickname}. Their balance is now {res.ToMoney(cfg.Culture)}");
        }
    }
}
