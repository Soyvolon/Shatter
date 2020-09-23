using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using NitroSharp.Database;
using NitroSharp.Structures;

namespace NitroSharp.Commands.Economy
{
    public class BalanceCommand : BaseCommandModule
    {
        private readonly NSDatabaseModel _model;

        public BalanceCommand(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("balance")]
        [Description("Shows your current balance.")]
        [Aliases(new string[] { "bal", "money" })]
        [RequireUserPermissions(Permissions.AccessChannels)]
        public async Task BalanceCommandAsync(CommandContext ctx, Optional<DiscordMember> member)
        {
            ulong id;
            if(member.HasValue)
            {
                id =member.Value.Id;
            }
            else
            {
                id = ctx.Member.Id;
            }

            var wallet = _model.Wallets.Find(id);

            if(wallet is null)
                wallet = new Wallet(id);


        }
    }
}
