using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Database;
using NitroSharp.Structures;

namespace NitroSharp.Commands.Config
{
    public class Prefix : BaseCommandModule
    {
        private readonly NSDatabaseModel _model;

        public Prefix(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("prefix")]
        [Description("Sets this bot's prefix for your server.")]
        //[Aliases("")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        public async Task SetPrefixAsync(CommandContext ctx, string prefix)
        {
            var p = prefix.Trim();

            var config = await _model.Configs.FindAsync(ctx.Guild.Id);

            if(config is null)
            {
                config = new GuildConfig
                {
                    GuildId = ctx.Guild.Id
                };
            }

            if (p != config.Prefix)
            {
                config.Prefix = p;

                await _model.SaveChangesAsync();
            }

            await ctx.RespondAsync($"Your server's prefix is now: `{p}`!");
        }
    }
}