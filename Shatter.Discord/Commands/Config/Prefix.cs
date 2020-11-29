using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Core.Database;
using Shatter.Core.Structures.Guilds;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Config
{
	public class Prefix : CommandModule
    {
        private readonly ShatterDatabaseContext _model;

        public Prefix(ShatterDatabaseContext model)
        {
			this._model = model;
        }

        [Command("prefix")]
        [Description("Sets this bot's prefix for your server.")]
        [ExecutionModule("config")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task SetPrefixAsync(CommandContext ctx, string prefix)
        {
            var p = prefix.Trim();

            var config = await this._model.Configs.FindAsync(ctx.Guild.Id);

            if (config is null)
            {
                config = new GuildConfig
                {
                    GuildId = ctx.Guild.Id
                };
            }

            if (p != config.Prefix)
            {
                config.Prefix = p;

                await this._model.SaveChangesAsync();
            }

            await ctx.RespondAsync($"Your server's prefix is now: `{p}`!");
        }
    }
}