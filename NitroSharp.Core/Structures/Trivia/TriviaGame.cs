using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace NitroSharp.Core.Structures.Trivia
{
    public class TriviaGame
    {
        public readonly ulong ChannelId;
        public readonly QuestionCategory Category;
        public readonly int QuestionCount;

        private ConcurrentBag<TriviaQuestion>? Questions;
        private ConcurrentDictionary<ulong, TriviaPlayer> Players;

        private const string request_base = "https://opentdb.com/api.php?encode=base64";
        private const string amount_base = "amount=";
        private const string category_base = "category=";

        public TriviaGame(ulong channel, int questions = 1, QuestionCategory category = QuestionCategory.All)
        {
            ChannelId = channel;
            Category = category;
            QuestionCount = questions;
            Players = new ConcurrentDictionary<ulong, TriviaPlayer>();
        }

        public async Task LoadQuestionsAsync()
        {
            var client = new HttpClient();
            string request = request_base + "&" + amount_base + QuestionCount;

            if (Category != QuestionCategory.All)
                request += $"&{category_base}{(int)Category}";

            var res = await client.GetAsync(request);

            var json = await res.Content.ReadAsStringAsync();

            var triviaResponse = JsonConvert.DeserializeObject<TriviaResponse>(json);

            if (triviaResponse.ResponseCode != 0) throw new Exception("API Error. Check input string.");

            Questions = new ConcurrentBag<TriviaQuestion>();

            foreach (var question in triviaResponse.Questions)
            {
                Questions.Add(new TriviaQuestion(question));
            }
        }

        public Task<bool> PopNextQuestion(out TriviaQuestion? question)
        {
            if (Questions is null) throw new Exception("Triva Questions not Loaded.");

            if (Questions.TryTake(out question))
                return Task.FromResult(true);

            return Task.FromResult(false);
        }
    }
}
