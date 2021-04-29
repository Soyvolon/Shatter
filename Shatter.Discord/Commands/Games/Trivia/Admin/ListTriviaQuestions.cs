using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.EntityFrameworkCore;
using Shatter.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shatter.Discord.Commands.Games.Trivia.Admin
{
	public class ListTriviaQuestions : CommandModule
	{
		private readonly ShatterDatabaseContext _model;

		public ListTriviaQuestions(ShatterDatabaseContext model)
		{
			this._model = model;
		}

		[Command("listquestions")]
		[Description("Lists custom trivia questions.")]
		[RequireUserPermissions(Permissions.ManageMessages)]
		public async Task ListTriviaQuestionsAsync(CommandContext ctx)
		{
			var questions = await this._model.TriviaQuestions.AsNoTracking().ToListAsync();

			StringBuilder builder = new StringBuilder();
			foreach(var q in questions)
			{
				builder.AppendLine($"`{q.Key}` - {q.Question}");
			}

			var interact = ctx.Client.GetInteractivity();

			var pages = interact.GeneratePagesInEmbed(builder.ToString(), DSharpPlus.Interactivity.Enums.SplitType.Line, new DiscordEmbedBuilder()
				.WithColor(DiscordColor.Purple)
				.WithTitle("Custom Tiriva Questions"));

			_ = Task.Run(async () => await interact.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages));
		}
	}
}
