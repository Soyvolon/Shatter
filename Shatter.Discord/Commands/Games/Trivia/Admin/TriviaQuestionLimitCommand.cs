using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Core.Database;
using Shatter.Core.Structures.Guilds;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Games.Trivia.Admin
{
	public class TriviaQuestionLimitCommand : CommandModule
    {
        private readonly ShatterDatabaseContext _model;

        public TriviaQuestionLimitCommand(ShatterDatabaseContext model)
        {
            this._model = model;
        }

        [Command("limittrivia")]
        [Description("Limits the Questions per single trivia game.")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [ExecutionModule("games")]
        public async Task ExampleCommandAsync(CommandContext ctx, int limit)
        {
            var cfg = _model.Configs.Find(ctx.Guild.Id);

            if (cfg is null)
            {
                cfg = new GuildConfig(ctx.Guild.Id);
                _model.Add(cfg);
            }

            cfg.TriviaQuestionLimit = limit > 50 ? 50 : limit;
            if (cfg.TriviaQuestionLimit <= 0)
                cfg.TriviaQuestionLimit = 1;

            await _model.SaveChangesAsync();

            await ctx.RespondAsync($"Set the question limit to {cfg.TriviaQuestionLimit}. The max questions per game can be no larger than 50.");
        }
    }
}
