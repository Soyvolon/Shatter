using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Shatter.Core.Database;
using Shatter.Core.Structures.Trivia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shatter.Discord.Commands.Games.Trivia.Admin
{
	public class AddTriviaQuestion : CommandModule
	{
		private readonly ShatterDatabaseContext _model;

		public AddTriviaQuestion(ShatterDatabaseContext model)
		{
			this._model = model;
		}

		[Command("addquestion")]
		[RequireUserPermissions(DSharpPlus.Permissions.ManageMessages)]
		public async Task AddTriviaQuestionCommand(CommandContext ctx)
		{
			var interact = ctx.Client.GetInteractivity();

			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				.WithDescription("What is the question?")
				.WithTitle("Add Trivia Question")
				.WithColor(DiscordColor.Purple);

			await ctx.RespondAsync(builder);

			var res = await interact.WaitForMessageAsync(x => x.Author.Id == ctx.User.Id && x.Channel.Id == ctx.Channel.Id);

			var question = new CustomTriviaQuestion();

			if(res.TimedOut)
			{
				await ctx.RespondAsync(builder.WithColor(DiscordColor.DarkRed).WithDescription("Timed out."));
				return;
			}
			else if(res.Result.Content.ToLower().Equals("exit"))
			{
				await ctx.RespondAsync(builder.WithColor(DiscordColor.DarkRed).WithDescription("Aborted."));
				return;
			}
			else
			{
				question.Question = res.Result.Content;
			}

			await ctx.RespondAsync(builder.WithDescription($"Your question is:\n{question.Question}\n\n" +
				$"Please enter the answer:"));

			res = await interact.WaitForMessageAsync(x => x.Author.Id == ctx.User.Id && x.Channel.Id == ctx.Channel.Id);

			if (res.TimedOut)
			{
				await ctx.RespondAsync(builder.WithColor(DiscordColor.DarkRed).WithDescription("Timed out."));
				return;
			}
			else if (res.Result.Content.ToLower().Equals("exit"))
			{
				await ctx.RespondAsync(builder.WithColor(DiscordColor.DarkRed).WithDescription("Aborted."));
				return;
			}
			else
			{
				question.Answer = res.Result.Content;
			}

			_model.Add(question);
			await _model.SaveChangesAsync();

			await ctx.RespondAsync(builder.WithDescription($"Your question is:\n{question.Question}\n\n" +
				$"Your answer is:\n{question.Answer}\n\n" +
				$"This question has been saved as question {question.Key}.")
				.WithColor(DiscordColor.DarkGreen));
		}
	}
}
