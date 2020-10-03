using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using NitroSharp.Properties;

namespace NitroSharp.Commands.Memes
{
    public class DonaldCommand : BaseCommandModule
    {
        const string pfp = "https://images-ext-1.discordapp.net/external/0dhj6pqQcWlVWCUOtIC4TTNKLWm9AHSTN7QjNOkeGD8/https/pbs.twimg.com/profile_images/874276197357596672/kUuht00m_bigger.jpg";
        const string twtr = "https://images-ext-1.discordapp.net/external/bXJWV2Y_F3XSra_kEqIYXAAsI3m1meckfLhYuWzxIfI/https/abs.twimg.com/icons/apple-touch-icon-192x192.png";

        private readonly Random rand = new Random();

        [Command("donald")]
        [Description("Tweet like the Donald")]
        [Aliases("trump")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        public async Task ExampleCommandAsync(CommandContext ctx,
            [RemainingText]
            [Description("Message in the tweet")] string msg)
        {
            var embed = new DiscordEmbedBuilder()
                .WithAuthor("Donald J. Trump (@realDonaldTrump)", "https://twitter.moc/realDonaldTrump", pfp)
                .WithDescription(msg)
                .AddField("Retweets", Math.Floor(rand.Next() * 1e2).ToString(), true)
                .AddField("Likes", Math.Floor(rand.Next() * 1e2).ToString(), true)
                .WithFooter("Twitter", twtr);

            await ctx.RespondAsync($@"https://twitter.moc/realDonaldTrump/status/{ctx.Message.Id}", embed: embed);
        }
    }
}