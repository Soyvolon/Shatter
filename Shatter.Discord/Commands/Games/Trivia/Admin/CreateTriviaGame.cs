using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Shatter.Discord.Services;
using System;
using System.Threading.Tasks;

namespace Shatter.Discord.Commands.Games.Trivia.Admin
{
	public class CreateTriviaGame : CommandModule
	{
		private readonly TriviaChannelService _trivia;

		public CreateTriviaGame(TriviaChannelService trivia)
		{
			_trivia = trivia;
		}

		[Command("starttriviagame")]
		[Description("Starts a new trivia game.")]
		[Aliases("startgame", "triviagame")]
		[RequireUserPermissions(Permissions.ManageChannels)]
		public async Task CreateTriviaGameAsync(CommandContext ctx,
			[Description("How much time should be spent between questions? Defaults to 3 minutes between questions.")]
			TimeSpan? timeBetweenQuestions = null,

			[Description("How long should this channel run for? Defaults to 1 hour.")]
			TimeSpan? totalRuntime = null,

			[Description("What type of questions should be used? Leave blank for both Custom and API," +
			" type false for API only, type true for Custom only.")]
			bool? customQuestions = null)
		{
			await ctx.RespondAsync(new DiscordEmbedBuilder().WithColor(DiscordColor.DarkGreen)
				.WithDescription("Starting new trivia channel!"));

			_trivia.Start(ctx.Channel, timeBetweenQuestions ?? TimeSpan.FromMinutes(3),
				totalRuntime ?? TimeSpan.FromHours(1), customQuestions);
		}
	}
}
