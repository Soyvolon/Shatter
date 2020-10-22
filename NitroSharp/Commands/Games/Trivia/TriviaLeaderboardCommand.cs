using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;

using Microsoft.EntityFrameworkCore;

using NitroSharp.Commands.CustomArguments;
using NitroSharp.Database;
using NitroSharp.Structures.Trivia;

namespace NitroSharp.Commands.Games.Trivia
{
    public class TriviaLeaderboardCommand : BaseCommandModule
    {
        private readonly NSDatabaseModel _model;

        public TriviaLeaderboardCommand(NSDatabaseModel model)
        {
            this._model = model;
        }

        [Command("triviatop")]
        [Description("The trivia leaderboards!")]
        [Aliases("trivialeaderboard")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        public async Task TriviaLeaderboardCommandAsync(CommandContext ctx,
            [Description("Filter the leaderboards in various ways! Use the keyword `info` to see more filtering options.")]
            [RemainingText] string args)
        {
            LeaderboardType leaderboardType = LeaderboardType.Global;
            bool sortByPercentCorrect = false;
            bool displayPersonalData = false;

            // Look at the args and see if any fliters need to be applied.
            if (!(args is null) && args.Length > 0)
            {
                var lowerArgs = args.ToLower().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

                if (lowerArgs.Contains("info"))
                {
                    // Display argmuent information here.
                    var infoEmbed = CommandUtils.SuccessBase()
                        .WithTitle("Trivia Leaderboards")
                        .WithDescription("Additional Arguments for the Trivia Leadboards.\n" +
                        $"Example: `{ctx.Prefix}triviatop server")
                        .AddField("server", "Show only leaderboards for this server.")
                        .AddField("percent", "Show the people with the highest correct answer percent rather than highest score. " +
                        "Only users with at least 25 questions answered are shown.")
                        .AddField("me", "Shows only your stats for Trivia");

                    await ctx.RespondAsync(embed: infoEmbed);
                    return;
                }
                else
                {
                    // Complie the search filters here.
                    foreach (var arg in lowerArgs)
                    {
                        if (arg == "server")
                            leaderboardType = LeaderboardType.Server;

                        if (arg == "percent")
                            sortByPercentCorrect = true;

                        if (arg == "me")
                            displayPersonalData = true;
                    }
                }
            }

            if (displayPersonalData)
            {
                var player = _model.TriviaPlayers.Find(ctx.Member.Id);

                if (player is null)
                {
                    await ctx.RespondAsync($"No statistic found! Play a game of trivia with {ctx.Prefix}trivia");
                }
                else
                {
                    var embed = new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.Purple)
                        .WithTitle("Personal Trivia Stats")
                        .AddField("Total Questions:", player.TotalQuestions.ToString(), true)
                        .AddField("Percent Correct:", player.PercentCorrect.ToString("0.##") + "%", false)
                        .AddField("Correct Answers:", player.QuestionsCorrect.ToString(), true)
                        .AddField("Incorrect Answers:", player.QuestionsIncorrect.ToString(), false)
                        .AddField("Total Points:", player.Points.ToString());

                    await ctx.RespondAsync(embed: embed);
                }
            }
            else
            {
                List<TriviaPlayer> players;

                players = _model.TriviaPlayers
                    .AsNoTracking()
                    .ToList();

                if (leaderboardType == LeaderboardType.Server)
                {
                    var users = await ctx.Guild.GetAllMembersAsync();

                    players = players.Where(x => users.Any(y => y.Id == x.UserId)).ToList();
                }

                if (sortByPercentCorrect)
                {
                    players = players.Where(x => (x.QuestionsCorrect + x.QuestionsIncorrect) >= 25).ToList();
                    players.Sort((x, y) => x.PercentCorrect.CompareTo(y.PercentCorrect));
                }
                else
                {
                    players.Sort((x, y) => x.Points.CompareTo(y.Points));
                }

                var interact = ctx.Client.GetInteractivity();

                string data = "";
                int i = 1;

                foreach (var player in players)
                    data += $"{i}. {(player.Username == "" ? player.UserId.ToString() : player.Username)} - " +
                        $"{(sortByPercentCorrect ? player.PercentCorrect.ToString("0.##") + "%" : player.Points + " points")}";

                var embed = new DiscordEmbedBuilder()
                    .WithTitle($"{(leaderboardType == LeaderboardType.Global ? "Global" : ctx.Guild.Name)} Trivia Leaderboard")
                    .WithColor(DiscordColor.Purple);

                var pages = interact.GeneratePagesInEmbed(data, SplitType.Line, embed);

                interact.SendPaginatedMessageAsync(ctx.Channel, ctx.Member, pages);
            }
        }
    }
}
