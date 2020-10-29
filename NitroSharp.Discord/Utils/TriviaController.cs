using System.Collections.Concurrent;
using System.Threading.Tasks;

using NitroSharp.Core.Structures.Trivia;

namespace NitroSharp.Discord.Utils
{
    public static class TriviaController
    {
        public static readonly ConcurrentDictionary<ulong, TriviaGame> ActiveTriviaGames = new ConcurrentDictionary<ulong, TriviaGame>();

        public static async Task<TriviaGame?> StartGame(ulong gameChannel, int questionCount = 1, QuestionCategory questionCategory = QuestionCategory.All)
        {
            if (ActiveTriviaGames.ContainsKey(gameChannel)) return null;

            var game = new TriviaGame(gameChannel, questionCount, questionCategory);

            await game.LoadQuestionsAsync();

            ActiveTriviaGames[gameChannel] = game;

            return game;
        }

        public static void EndGame(ulong gameChannel)
        {
            ActiveTriviaGames.TryRemove(gameChannel, out _);
        }
    }
}
