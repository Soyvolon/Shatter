using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

using Shatter.Core.Database;
using Shatter.Core.Extensions;
using Shatter.Core.Structures;
using Shatter.Core.Structures.Guilds;
using Shatter.Core.Structures.Trivia;
using Shatter.Discord.Commands.Attributes;
using Shatter.Discord.Utils;

namespace Shatter.Discord.Commands.Games.Trivia
{
	public class TriviaGameCommand : CommandModule
    {
        private readonly ShatterDatabaseContext _model;

        public TriviaGameCommand(ShatterDatabaseContext model)
        {
            _model = model;
        }

        [Command("trivia")]
        [Description("Play a game of Trivia!")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        [RequireBotPermissions(Permissions.SendMessages)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [ExecutionModule("games")]
        public async Task SingleTriviaCommandAsync(CommandContext ctx,
            [Description("How many questions to ask. Leave blank for a single question.")]
            int questions = 1,

            [Description("The category to pick questions from. If you want this random, leave it blank or use an id of 0.")]
            QuestionCategory questionCategory = QuestionCategory.All)
        {
            var cfg = _model.Configs.Find(ctx.Guild.Id);

            if (cfg is null)
            {
                cfg = new GuildConfig(ctx.Guild.Id);
                _model.Configs.Add(cfg);
                await _model.SaveChangesAsync();
            }

            if (cfg.TriviaQuestionLimit <= 0)
            {
                cfg.TriviaQuestionLimit = 1;
            }

            int qNumber = questions;
            if (qNumber > cfg.TriviaQuestionLimit)
            {
                qNumber = cfg.TriviaQuestionLimit;
                await ctx.RespondAsync($"Set the question number to this server's question limit: {qNumber}");
            }

            if (qNumber <= 0)
            {
                qNumber = 1;
            }

            var game = await TriviaController.StartGame(ctx.Channel.Id, qNumber, questionCategory);

            if (game is null)
            {
                await ctx.RespondAsync("Failed to start a new game - A game is already in progress!");
                return;
            }

            try
            {
                var interact = ctx.Client.GetInteractivity();

                await ctx.RespondAsync($"Starting a new game of trivia with {qNumber} questions!");

                while (await game.PopNextQuestion(out TriviaQuestion? question))
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
					// False positive.
					var mapped = question.GetMappedAnswers();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

					var embed = new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.Purple)
                        .WithTitle("Trivia!")
                        .WithDescription("You have 20 seconds to answer this question! Type the number of the answer that is correct to win!")
                        .AddField(question.QuestionString, mapped.Item1)
                        .AddField("Difficulty", $"`{question.DifficultyString}`", true)
                        .AddField("Category", question.CategoryString);

                    await ctx.RespondAsync(embed: embed);
                    var response = await interact.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Member.Id, TimeSpan.FromSeconds(20));

                    if (response.TimedOut)
                    {
                        await ctx.RespondAsync($"The question wans't answered in time! The correct answer was: {question.PossibleAnswers[question.CorrectAnswerKey]}");
                        break; // Goto finally block
                    }

                    var trivia = _model.TriviaPlayers.Find(response.Result.Author.Id);

                    if (trivia is null)
                    {
                        trivia = new TriviaPlayer(response.Result.Author.Id);
                        _model.Add(trivia);
                    }

                    var answerString = response.Result.Content.ToLowerInvariant().Trim();
                    if (answerString == mapped.Item2.ToString() || answerString == mapped.Item3)
                    { // Response is correct
                        await ctx.RespondAsync($"Thats the correct answer! You earned {question.Worth.ToMoney()}");
                        var wallet = _model.Wallets.Find(response.Result.Author.Id);
                        if (wallet is null)
                        {
                            wallet = new Wallet(response.Result.Author.Id);
                            await _model.AddAsync(wallet);
                        }

                        wallet.Add(question.Worth);

                        trivia.Points += question.Worth / 10;
                        trivia.QuestionsCorrect++;
                    }
                    else
                    { // Response is incorrect
                        await ctx.RespondAsync($"Thats not the right answer! The correct answer was: {question.PossibleAnswers[question.CorrectAnswerKey]}");

                        trivia.Points--;
                        trivia.QuestionsIncorrect++;
                    }

                    if (trivia.Username != ctx.Member.Username)
					{
						trivia.Username = ctx.Member.Username;
					}

					await _model.SaveChangesAsync();
                }
            }
            finally
            {
                TriviaController.EndGame(ctx.Channel.Id);
                await ctx.RespondAsync("Trivia Game Complete!");
            }
        }
    }
}
