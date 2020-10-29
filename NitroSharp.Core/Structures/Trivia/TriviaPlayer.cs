using System.ComponentModel.DataAnnotations;

namespace NitroSharp.Structures.Trivia
{
    public class TriviaPlayer
    {
        [Key]
        public ulong UserId { get; set; }
        public string Username { get; set; }
        public int Points { get; set; }
        public int QuestionsCorrect { get; set; }
        public int QuestionsIncorrect { get; set; }

        public int TotalQuestions
        {
            get
            {
                return QuestionsCorrect + QuestionsIncorrect;
            }
        }

        public float PercentCorrect
        {
            get
            {
                return (QuestionsCorrect / (float)TotalQuestions) * 100;
            }
        }

        public TriviaPlayer() { }
        public TriviaPlayer(ulong uId) { this.UserId = uId; }
    }
}
