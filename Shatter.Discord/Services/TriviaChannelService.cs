using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.EntityFrameworkCore;
using Shatter.Core.Database;
using Shatter.Core.Extensions;
using Shatter.Core.Structures.Trivia;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shatter.Discord.Services
{
	public class TriviaChannelService
	{
		public ConcurrentDictionary<ulong, Timer> ActiveChannels { get; set; }
		public ConcurrentDictionary<ulong, CustomTriviaQuestion> LastQuestion { get; set; }
		private readonly ShatterDatabaseContext _context;

		private static readonly Random _rand = new();

		public TriviaChannelService(ShatterDatabaseContext context)
		{
			ActiveChannels = new();
			LastQuestion = new();

			_context = context;

			DiscordBot.Bot!.Client!.MessageCreated += (x, y) =>
			{
				_ = Task.Run(async () => await MessageReceived(x, y));
				return Task.CompletedTask;
			};
		}

		public void Start(DiscordChannel channel, TimeSpan tbq,
			TimeSpan totalTime, bool? questionTypes)
		{
			ActiveChannels[channel.Id] = new Timer(async (x) =>
			{
				var tte = DateTime.Now.Add(totalTime);

				await ExecuteQuestions(channel, tte, questionTypes);

			}, null, TimeSpan.Zero, tbq);
		}

		public async Task MessageReceived(DiscordClient c, MessageCreateEventArgs e)
		{
			if(LastQuestion.TryGetValue(e.Channel.Id, out var q))
			{
				if(e.Message.Content.ToLower().Equals(q.Answer.ToLower()))
				{
					_ = LastQuestion.TryRemove(e.Channel.Id, out _);

					await e.Channel.SendMessageAsync(new DiscordEmbedBuilder()
						.WithDescription($"The correct answer goes to: {e.Author.Username}")
						.WithColor(DiscordColor.DarkGreen));
				}
			}
		}

		private async Task ExecuteQuestions(DiscordChannel c, DateTime end, bool? types)
		{
			if(DateTime.Now.Subtract(end).TotalSeconds > 0)
			{
				if(ActiveChannels.TryRemove(c.Id, out var timer))
					timer.Dispose();
				_ = LastQuestion.TryRemove(c.Id, out _);


				await c.SendMessageAsync(new DiscordEmbedBuilder()
					.WithColor(DiscordColor.DarkGreen)
					.WithDescription("Trivia game complete!"));
				return;
			}
			else if(LastQuestion.TryRemove(c.Id, out var q))
			{
				await c.SendMessageAsync(new DiscordEmbedBuilder()
					.WithColor(DiscordColor.DarkRed)
					.WithDescription($"The last question was never answerd! The answer was: {q.Answer}"));
			}

			bool custom = false;
			if(types is null)
			{
				// Both types of questions.

				if(_rand.NextDouble() >= .5)
				{
					// Pull Custom.
					custom = true;
				}
			}
			else
			{
				custom = types.Value;
			}

			CustomTriviaQuestion? question = null;
			if(custom)
			{
				var questions = await _context.TriviaQuestions.AsNoTracking().ToListAsync();
				question = questions.Random();
			}
			else
			{
				var game = new TriviaGame(c.Id);

				await game.LoadQuestionsAsync();

				if(await game.PopNextQuestion(out var qData))
				{
					question = new()
					{
						Question = qData.QuestionString,
						Answer = qData.GetMappedAnswers().Item3
					};
				}
			}

			if(question is not null)
			{
				await c.SendMessageAsync(new DiscordEmbedBuilder()
					.WithColor(DiscordColor.Purple)
					.WithTitle("Trivia!")
					.WithDescription(question.Question));

				LastQuestion[c.Id] = question;
			}
			else
			{
				await c.SendMessageAsync(new DiscordEmbedBuilder()
					.WithColor(DiscordColor.DarkRed)
					.WithTitle("Trivia!")
					.WithDescription("No valid question was found! Stopping game early."));

				if (ActiveChannels.TryRemove(c.Id, out var timer))
					timer.Dispose();
				_ = LastQuestion.TryRemove(c.Id, out _);
			}
		}
	}
}
