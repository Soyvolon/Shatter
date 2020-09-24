using System.Globalization;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using NitroSharp.Database;
using NitroSharp.Extensions;
using NitroSharp.Structures;

namespace NitroSharp.Commands.Economy
{
    public class GiftCommand : BaseCommandModule
    {
        private readonly NSDatabaseModel _model;

        public GiftCommand(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("gift")]
        [Description("Gift another user money.")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        [Cooldown(1, 10, CooldownBucketType.User)]
        public async Task ExampleCommandAsync(CommandContext ctx, DiscordMember m, int ammount)
        {
            if (ctx.Member == m)
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "You can't give yourself money.");
            }
            else if(m.IsBot)
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "Robots don't know how to handle money.");
            }
            else
            {
                bool save = false;

                var from = await _model.FindAsync<Wallet>(ctx.Member.Id);

                if(from is null)
                {
                    from = new Wallet(ctx.Member.Id);
                    _model.Add(from);
                    save = true;
                }

                if(from.HasEnough(ammount))
                {
                    var to = await _model.FindAsync<Wallet>(m.Id);

                    if(to is null)
                    {
                        to = new Wallet(m.Id);
                        _model.Add(to);
                    }

                    var remains = from.Transfer(ammount, to);

                    var cfg = await _model.FindAsync<GuildConfig>(ctx.Guild.Id);

                    if(cfg is null)
                    {
                        cfg = new GuildConfig(ctx.Guild.Id);
                        _model.Add(cfg);
                    }

                    await ctx.RespondAsync($"Gave {ammount.ToMoney(cfg.Culture)} to {m.DisplayName}. You have {remains.ToMoney(cfg.Culture)}");

                    save = true;
                }
                else
                {
                    await CommandUtils.RespondBasicErrorAsync(ctx, "You do not have enough money.");
                }

                if (save)
                    await _model.SaveChangesAsync();
            }
        }
    }
}
