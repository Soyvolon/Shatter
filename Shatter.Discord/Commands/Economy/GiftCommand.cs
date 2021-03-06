﻿using System.Threading.Tasks;

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
	public class GiftCommand : CommandModule
    {
        private readonly ShatterDatabaseContext _model;

        public GiftCommand(ShatterDatabaseContext model)
        {
			this._model = model;
        }

        [Command("gift")]
        [Description("Gift another user money.")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        [Cooldown(1, 10, CooldownBucketType.User)]
        [ExecutionModule("economy")]
        public async Task ExampleCommandAsync(CommandContext ctx, DiscordMember m, int ammount)
        {
            if (ctx.Member == m)
            {
                await RespondBasicErrorAsync("You can't give yourself money.");
            }
            else if (m.IsBot)
            {
                await RespondBasicErrorAsync("Robots don't know how to handle money.");
            }
            else
            {
                bool save = false;

                var from = await this._model.FindAsync<Wallet>(ctx.Member.Id);

                if (from is null)
                {
                    from = new Wallet(ctx.Member.Id, ctx.Member.Username);
					this._model.Add(from);
                    save = true;
                }

                if (from.HasEnough(ammount))
                {
                    var to = await this._model.FindAsync<Wallet>(m.Id);

                    if (to is null)
                    {
                        to = new Wallet(m.Id, m.Username);
						this._model.Add(to);
                    }
                    else
                    {
                        to.Username = m.Username;
                    }

                    from.Username = ctx.Member.Username;

                    var remains = from.Transfer(ammount, to);

                    await ctx.RespondAsync($"Gave {ammount.ToMoney()} to {m.DisplayName}. You have {remains.ToMoney()}");

                    save = true;
                }
                else
                {
                    await RespondBasicErrorAsync("You do not have enough money.");
                }

                if (save)
				{
					await this._model.SaveChangesAsync();
				}
			}
        }
    }
}
