using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

using Shatter.Core.Structures.Trivia;

namespace Shatter.Discord.Commands.CustomArguments
{
	public class QuestionCategoryConverter : IArgumentConverter<QuestionCategory>
    {
        public Task<Optional<QuestionCategory>> ConvertAsync(string value, CommandContext ctx)
        {
            if (int.TryParse(value, out int catNum))
			{
				return Task.FromResult(Optional.FromValue((QuestionCategory)catNum));
			}

			return Task.FromResult(Optional.FromNoValue<QuestionCategory>());
        }
    }
}
