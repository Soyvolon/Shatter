using System;
using System.Collections.Generic;
using System.Text;

namespace NitroSharp.Structures.Trivia
{
    public class TriviaQuestion
    {
        public int Worth { get; private set; }
        public int CorrectAnswerKey { get; private set; }
        public Dictionary<int, string> PossibleAnswers { get; } = new Dictionary<int, string>();
        public string QuestionString { get; private set; }
        public string DifficultyString { get; private set; }
        public string CategoryString { get; private set; }

        public TriviaQuestion(TriviaQuestionData res)
        {
            switch (Encoding.ASCII.GetString(Convert.FromBase64String(res.Difficulty)))
            {
                case "easy":
                    Worth = 50;
                    break;
                case "medium":
                    Worth = 100;
                    break;
                case "hard":
                    Worth = 150;
                    break;
            }

            PossibleAnswers.Add(0, Encoding.ASCII.GetString(Convert.FromBase64String(res.CorrectAnswer)));

            var id = 1;
            foreach (var q in res.IncorrectAnswers)
                PossibleAnswers.Add(id++, Encoding.ASCII.GetString(Convert.FromBase64String(q)));

            QuestionString = Encoding.ASCII.GetString(Convert.FromBase64String(res.QuestionString));

            DifficultyString = Encoding.ASCII.GetString(Convert.FromBase64String(res.Difficulty));

            CategoryString = Encoding.ASCII.GetString(Convert.FromBase64String(res.Category));
        }

        public Tuple<string, int> GetMappedAnswers()
        {
            string choices = "";
            int c = 1;
            int correctPos = 0;

            foreach (var answer in PossibleAnswers)
            {
                choices += $"{c}: {answer.Value}\n";
                if (answer.Key == CorrectAnswerKey)
                    correctPos = c;

                c++;
            }

            return new Tuple<string, int>(choices, correctPos);
        }
    }
}
