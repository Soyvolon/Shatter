using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Database;
using NitroSharp.Structures.Guilds;

namespace NitroSharp.Commands.Games.Trivia.Admin
{
    public class TriviaQuestionLimitCommand : BaseCommandModule
    {
        private readonly NSDatabaseModel _model;

        public TriviaQuestionLimitCommand(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("limittrivia")]
        [Description("Limits the Questions per single trivia game.")]
        [RequireUserPermissions(Permissions.ManageGuild)]
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
