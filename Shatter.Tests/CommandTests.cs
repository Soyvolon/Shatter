using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Shatter.Core;
using Shatter.Core.Extensions;
using Shatter.Core.Structures;
using Shatter.Discord;

namespace Shatter.Tests
{
	public class CommandTests
    {
        protected const ulong TestingGuildId = 431462786900688896;
        protected const ulong TestingChannelId = 758504934676234290;
        protected const ulong ActorRoleId = 758506045348773888;

        protected DiscordBot Bot { get; set; }
        protected DiscordShardedClient Client { get; set; }
        protected int TestingShardId { get; set; } = -1;
        protected DiscordGuild TestingGuild { get; set; }
        protected DiscordChannel TestingChannel { get; set; }
        protected List<DiscordMember> Actors { get; set; }
        protected CommandsNextExtension CNext { get; set; }
        protected IReadOnlyDictionary<string, Command> Commands { get; set; }

        [OneTimeSetUp]
        public async Task SetUp()
        {
            var botCfg = (BotConfig)await ConfigurationManager.RegisterBotConfiguration(null);
            var lavalConfig = (LavalinkConfig)await ConfigurationManager.RegisterLavaLink(null);
            var ytCfg = (YouTubeConfig)await ConfigurationManager.RegisterYouTube(null);

			this.Bot = new DiscordBot(botCfg, lavalConfig, ytCfg, new ServiceCollection()); // Designate test run.

            await this.Bot.InitializeAsync();

            await this.Bot.StartAsync();

			this.Client = this.Bot.Client;

            Assert.True(this.Bot.Boot == DiscordBot.BootStatus.ready, "Boot status not marked as ready.");

            foreach (var s in this.Client.ShardClients.Values)
            {
                if (s.Guilds.TryGetValue(TestingGuildId, out var guild))
                {
					this.TestingGuild = guild;
					this.TestingShardId = s.ShardId;
                }
            }

            Assert.NotNull(this.TestingGuild, "Could not get testing guild.");
            Assert.True(this.TestingShardId >= 0, "Shard Id Missing.");

            if (this.TestingGuild.Channels.TryGetValue(TestingChannelId, out var chan))
			{
				this.TestingChannel = chan;
			}

			Assert.NotNull(this.TestingChannel, "Could not get testing channel.");

            var members = await this.TestingGuild.GetAllMembersAsync();

            Assert.NotNull(members);
            Assert.NotZero(members.Count);

			this.Actors = members.Where(x => x.Roles.Any(x => x.Id == ActorRoleId)).ToList();

            Assert.NotZero(this.Actors.Count, "Missing Actors for further testing.");

            var cnext = await this.Client.GetCommandsNextAsync();
            if (cnext.TryGetValue(this.TestingShardId, out var commandsNextExtension))
            {
				this.CNext = commandsNextExtension;
				this.Commands = this.CNext.RegisteredCommands;
            }

            Assert.NotNull(this.CNext, "Missing Commands Next Extension for testing shard.");

            var msg = await this.TestingChannel.SendMessageAsync("Starting Unit Tests . . .");

            Assert.NotNull(msg, "Message failed to send.");
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {

            await this.TestingChannel.SendMessageAsync(". . . Unit Tests Complete");

            await this.Client.StopAsync();
        }

        [Order(1)]
        [Test]
        public async Task PrefixCommandTest()
        {
			this.Commands.TryGetValue("prefix", out Command cmd);

            Assert.NotNull(cmd, "Prefix Command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors.Random(), this.TestingChannel, "]prefix >", "]", cmd, ">");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = this.CNext.CreateFakeContext(this.Actors.Random(), this.TestingChannel, ">prefix ]", ">", cmd, "]");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = this.CNext.CreateFakeContext(this.Actors.Random(), this.TestingChannel, "]prefix", "]", cmd);

            res = await cmd.ExecuteAsync(ctx);

            Assert.False(res.IsSuccessful, "Command should have failed.");
        }

        [Order(2)]
        [Test]
        public async Task SAHCryptoCommandTest()
        {
			this.Commands.TryGetValue("crypto", out Command cmd);

            Assert.NotNull(cmd, "Crypto command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors.Random(), this.TestingChannel, "]crypto testdata", "]", cmd, "testdata");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = this.CNext.CreateFakeContext(this.Actors.Random(), this.TestingChannel, "]crypto", "]", cmd);

            res = await cmd.ExecuteAsync(ctx);

            Assert.False(res.IsSuccessful, "Command should have failed.");
        }

        [Order(3)]
        [Test]
        public async Task AddFundsCommandTest()
        {
			this.Commands.TryGetValue("addfunds", out Command cmd);

            Assert.NotNull(cmd, "Add Funds command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors.Random(), this.TestingChannel, $"]addfunds 1000 {this.Actors[0].Mention}", "]", cmd, $"1000 {this.Actors[0].Mention}");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(4)]
        [Test]
        public async Task BalanceCommandTest()
        {
			this.Commands.TryGetValue("balance", out Command cmd);

            Assert.NotNull(cmd, "Balance command not found.");

            for (int i = 0; i < 2; i++)
            {
                var ctxI = this.CNext.CreateFakeContext(this.Actors[i], this.TestingChannel, $"]balance", "]", cmd);

                var resI = await cmd.ExecuteAsync(ctxI);

                Assert.True(resI.IsSuccessful, "Command should have executed.");
            }

            var ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]balance {this.Actors[1].Mention}", "]", cmd, this.Actors[1].Mention);

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(5)]
        [Test]
        public async Task GiftCommandTest()
        {
			this.Commands.TryGetValue("gift", out Command cmd);

            Assert.NotNull(cmd, "Gift command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]gift {this.Actors[1].Mention} 500", "]", cmd, $"{this.Actors[1].Mention} 500");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful);

            ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]gift {this.Actors[1].Mention} 500000", "]", cmd, $"{this.Actors[1].Mention} 500000");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful);

            ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]gift {this.Client.CurrentUser.Mention} 500", "]", cmd, $"{this.Client.CurrentUser.Mention} 500");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful);

            ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]gift {this.Actors[0].Mention} 500", "]", cmd, $"{this.Actors[0].Mention} 500");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful);

            ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]gift {this.Actors[0].Mention}", "]", cmd, $"{this.Actors[0].Mention}");

            res = await cmd.ExecuteAsync(ctx);

            Assert.False(res.IsSuccessful);

            ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]gift 500", "]", cmd, $"500");

            res = await cmd.ExecuteAsync(ctx);

            Assert.False(res.IsSuccessful);
        }

        [Order(6)]
        [Test]
        public async Task SubFundsCommandTest()
        {
			this.Commands.TryGetValue("subfunds", out Command cmd);

            Assert.NotNull(cmd, "Sub Funds command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors.Random(), this.TestingChannel, $"]subfunds 500 {this.Actors[0].Mention}", "]", cmd, $"500 {this.Actors[0].Mention}");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(7)]
        [Test]
        public async Task AdviceCommandTest()
        {
			this.Commands.TryGetValue("advice", out Command cmd);

            Assert.NotNull(cmd, "Advice command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors.Random(), this.TestingChannel, $"]advice", "]", cmd);

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(8)]
        [Test]
        public async Task TableFlipCommandTest()
        {
			this.Commands.TryGetValue("tableflip", out Command cmd);

            Assert.NotNull(cmd, "Table Flip command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors.Random(), this.TestingChannel, $"]tableflip", "]", cmd);

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(9)]
        [Test]
        public async Task UnFlipCommandTest()
        {
			this.Commands.TryGetValue("unflip", out Command cmd);

            Assert.NotNull(cmd, "Un Flip command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors.Random(), this.TestingChannel, $"]unflip", "]", cmd);

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(10)]
        [Test]
        public async Task BurnCommandTest()
        {
			this.Commands.TryGetValue("burn", out Command cmd);

            Assert.NotNull(cmd, "Burn command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]burn {this.Actors[1].Mention}", "]", cmd, $"{this.Actors[1].Mention}");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            var ctx_1 = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]burn {this.Actors[1].Mention}", "]", cmd, $"{this.Actors[1].Mention}");
            var ctx_2 = this.CNext.CreateFakeContext(this.Actors[1], this.TestingChannel, $"]burn {this.Actors[0].Mention}", "]", cmd, $"{this.Actors[0].Mention}");

            var cmd_1 = cmd.ExecuteAsync(ctx_1);
            var cmd_2 = cmd.ExecuteAsync(ctx_2);

            var res_1 = await cmd_1;
            var res_2 = await cmd_2;

            Assert.True(res_1.IsSuccessful && res_2.IsSuccessful, "Both commands should have executed.");
        }

        [Order(11)]
        [Test]
        public async Task CatCommandTest()
        {
			this.Commands.TryGetValue("cat", out Command cmd);

            Assert.NotNull(cmd, "Cat command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]cat jpg png", "]", cmd, "jpg png");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]cat gif", "]", cmd, "gif");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]cat gif jpg random", "]", cmd, "gif jpg random");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]cat mine that", "]", cmd, "mine that");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]cat id=qsGk4el-D", "]", cmd, "id=qsGk4el-D");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]cat id=", "]", cmd, "id=");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(12)]
        [Test]
        public async Task RichestCommandTest()
        {
			this.Commands.TryGetValue("richest", out Command cmd);

            Assert.NotNull(cmd, "Richest command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]richest", "]", cmd);

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]richest server", "]", cmd, "server");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(13)]
        [Test]
        public async Task DonaldCommandTest()
        {
			this.Commands.TryGetValue("donald", out Command cmd);

            Assert.NotNull(cmd, "Donald command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]donald My IQ is one of the highest.", "]", cmd, "My IQ is one of the highest.");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(14)]
        [Test]
        public async Task GruCommandTest()
        {
			this.Commands.TryGetValue("gru", out Command cmd);

            Assert.NotNull(cmd, "Gru command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]gru Find a gru meme template | Create a custom gru meme generator | Nobody uses it", "]",
                cmd, "Find a gru meme template | Create a custom gru meme generator | Nobody uses it");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]gru Find a gru meme template | Create a custom gru meme generat", "]",
                cmd, "Find a gru meme template | Create a custom gru meme generat");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]gru Find a gru meme template | Create a custom | gru | meme generat", "]",
                cmd, "Find a gru meme template | Create a custom | gru | meme generat");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(15)]
        [Test]
        public async Task NutCommandTest()
        {
			this.Commands.TryGetValue("nut", out Command cmd);

            Assert.NotNull(cmd, "Nut command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]nut Find a gru meme template | Create a custom gru meme generator | Nobody uses it", "]",
                cmd, "Find a gru meme template | Create a custom gru meme generator | Nobody uses it");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(16)]
        [Test]
        public async Task PrisonerCommandTest()
        {
			this.Commands.TryGetValue("prisoner", out Command cmd);

            Assert.NotNull(cmd, "Prisoner command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]prisoner I used the Gru Meme Generator", "]",
                cmd, "I used the Gru Meme Generator");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(17)]
        [Test]
        public async Task PunchCommandTest()
        {
			this.Commands.TryGetValue("punch", out Command cmd);

            Assert.NotNull(cmd, "Punch command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]punch {this.Actors[1].Mention}", "]",
                cmd, $"{this.Actors[1].Mention}");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(18)]
        [Test]
        public async Task TheSearchCommandTest()
        {
			this.Commands.TryGetValue("thesearch", out Command cmd);

            Assert.NotNull(cmd, "The Search command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]thesearch Don't use Shatter", "]",
                cmd, "Don't use Shatter");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(19)]
        [Test]
        public async Task CoinFlipGameCommandTest()
        {
			this.Commands.TryGetValue("coinflip", out Command cmd);

            Assert.NotNull(cmd, "Coin Flip command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]coinflip", "]",
                cmd);

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(19)]
        [Test]
        public async Task TriviaGameCommandTest()
        {
			this.Commands.TryGetValue("trivia", out Command cmd);

            Assert.NotNull(cmd, "Trivia command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]trivia", "]",
                cmd);

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(20)]
        [Test]
        public async Task TriviaTopCommandTest()
        {
			this.Commands.TryGetValue("triviatop", out Command cmd);

            Assert.NotNull(cmd, "Trivia Top command not found.");

            var ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]triviatop", "]",
                cmd);

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]triviatop server", "]",
                cmd, "server");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]triviatop me", "]",
                cmd, "me");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = this.CNext.CreateFakeContext(this.Actors[0], this.TestingChannel, $"]triviatop percent", "]",
                cmd, "percent");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        // TODO Add automated testing for applicapble commands (Non-Voice commands).
    }
}