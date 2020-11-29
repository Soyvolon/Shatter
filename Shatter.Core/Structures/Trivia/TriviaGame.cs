using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Shatter.Core.Structures.Trivia
{
	public class TriviaGame
    {
        public readonly ulong ChannelId;
        public readonly QuestionCategory Category;
        public readonly int QuestionCount;

        private ConcurrentBag<TriviaQuestion>? Questions;
        private readonly ConcurrentDictionary<ulong, TriviaPlayer> Players;

        private const string request_base = "https://opentdb.com/api.php?encode=base64";
        private const string amount_base = "amount=";
        private const string category_base = "category=";

        public TriviaGame(ulong channel, int questions = 1, QuestionCategory category = QuestionCategory.All)
        {
			this.ChannelId = channel;
			this.Category = category;
			this.QuestionCount = questions;
			this.Players = new ConcurrentDictionary<ulong, TriviaPlayer>();
        }

        public async Task LoadQuestionsAsync()
        {
            var client = new HttpClient();
            string request = request_base + "&" + amount_base + this.QuestionCount;

            if (this.Category != QuestionCategory.All)
			{
				request += $"&{category_base}{(int)this.Category}";
			}

			var res = await client.GetAsync(request);

            var json = await res.Content.ReadAsStringAsync();

            var triviaResponse = JsonConvert.DeserializeObject<TriviaResponse>(json);

            if (triviaResponse.ResponseCode != 0)
			{
				throw new Exception("API Error. Check input string.");
			}

			this.Questions = new ConcurrentBag<TriviaQuestion>();

            foreach (var question in triviaResponse.Questions)
            {
				this.Questions.Add(new TriviaQuestion(question));
            }
        }

        public Task<bool> PopNextQuestion(out TriviaQuestion? question)
        {
            if (this.Questions is null)
			{
				throw new Exception("Triva Questions not Loaded.");
			}

			if (this.Questions.TryTake(out question))
			{
				return Task.FromResult(true);
			}

			return Task.FromResult(false);
        }
    }
}
