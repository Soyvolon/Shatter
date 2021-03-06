﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shatter.Core.Extensions;

namespace Shatter.Core.Structures.Trivia
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
					this.Worth = 50;
                    break;
                case "medium":
					this.Worth = 100;
                    break;
                case "hard":
					this.Worth = 150;
                    break;
            }

			this.PossibleAnswers.Add(0, Encoding.ASCII.GetString(Convert.FromBase64String(res.CorrectAnswer)));

            var id = 1;
            foreach (var q in res.IncorrectAnswers)
			{
				this.PossibleAnswers.Add(id++, Encoding.ASCII.GetString(Convert.FromBase64String(q)));
			}

			this.QuestionString = Encoding.ASCII.GetString(Convert.FromBase64String(res.QuestionString));

			this.DifficultyString = Encoding.ASCII.GetString(Convert.FromBase64String(res.Difficulty));

			this.CategoryString = Encoding.ASCII.GetString(Convert.FromBase64String(res.Category));
        }

        public Tuple<string, int, string> GetMappedAnswers()
        {
            var questions = this.PossibleAnswers.ToList();
            questions.Shuffle();

            string choices = "";
            int c = 1;
            int correctPos = 0;
            string correctString = "";

            foreach (var answer in questions)
            {
                choices += $"{c}: {answer.Value}\n";
                if (answer.Key == this.CorrectAnswerKey)
                {
                    correctPos = c;
                    correctString = answer.Value.ToLowerInvariant().Trim();
                }

                c++;
            }

            return new Tuple<string, int, string>(choices, correctPos, correctString);
        }
    }
}
