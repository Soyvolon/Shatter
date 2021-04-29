using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Shatter.Core.Database;
using Shatter.Core.Structures.Trivia;
using System.Threading.Tasks;

namespace Shatter.Discord.Commands.Games.Trivia.Admin
{
	public class RemoveTriviaQuestions : CommandModule
	{
		private readonly ShatterDatabaseContext _model;

		public RemoveTriviaQuestions(ShatterDatabaseContext model)
		{
			this._model = model;
		}

		[Command("removequestion")]
		[Description("Removes a custom trivia question.")]
		[RequireUserPermissions(Permissions.ManageMessages)]
		public async Task RemoveTriviaQuestionCommandAsync(CommandContext ctx, int questionId)
		{
			var question = await this._model.FindAsync<CustomTriviaQuestion>(questionId);

			if(question is null)
			{
				await RespondBasicErrorAsync("No question by that ID found.");
				return;
			}

			this._model.Remove(question);
			await this._model.SaveChangesAsync();

			await RespondBasicSuccessAsync($"Reemoved question {question.Key}:\n" +
				$"{question.Question}\n\n" +
				$"{question.Answer}");
		}
	}
}
