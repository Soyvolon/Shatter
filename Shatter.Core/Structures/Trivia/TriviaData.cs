
using Newtonsoft.Json;

namespace Shatter.Core.Structures.Trivia
{
    public enum QuestionCategory
    {
        All = 0,
        GeneralKnowledge = 9,
        Books = 10,
        Film = 11,
        Music = 12,
        Musicals = 13,
        Television = 14,
        VideoGames = 15,
        BoardGames = 16,
        Science = 17,
        Computers = 18,
        Mathematics = 19,
        Mythology = 20,
        Sports = 21,
        Geography = 22,
        History = 23,
        Politics = 24,
        Art = 25,
        Celebrities = 26,
        Animals = 27,
        Vehicles = 28,
        Comics = 29,
        Gadgets = 30,
        Anime = 31,
        Cartoons = 32
    }

    public struct TriviaResponse
    {
        [JsonProperty("response_code")]
        public int ResponseCode { get; set; }

        [JsonProperty("results")]
        public TriviaQuestionData[] Questions { get; set; }
    }

    public struct TriviaQuestionData
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("difficulty")]
        public string Difficulty { get; set; }

        [JsonProperty("question")]
        public string QuestionString { get; set; }

        [JsonProperty("correct_answer")]
        public string CorrectAnswer { get; set; }

        [JsonProperty("incorrect_answers")]
        public string[] IncorrectAnswers { get; set; }
    }

}
