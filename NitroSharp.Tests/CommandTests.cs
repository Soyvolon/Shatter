using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using NitroSharp.Extensions;

using NUnit.Framework;

namespace NitroSharp.Tests
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

            Bot = new DiscordBot(true); // Designate test run.

            await Bot.InitializeAsync();

            await Bot.StartAsync();

            Client = Bot.Client;

            Assert.True(Bot.Boot == DiscordBot.BootStatus.ready, "Boot status not marked as ready.");

            foreach (var s in Client.ShardClients.Values)
            {
                if (s.Guilds.TryGetValue(TestingGuildId, out var guild))
                {
                    TestingGuild = guild;
                    TestingShardId = s.ShardId;
                }
            }

            Assert.NotNull(TestingGuild, "Could not get testing guild.");
            Assert.True(TestingShardId >= 0, "Shard Id Missing.");

            if (TestingGuild.Channels.TryGetValue(TestingChannelId, out var chan))
                TestingChannel = chan;

            Assert.NotNull(TestingChannel, "Could not get testing channel.");

            var members = await TestingGuild.GetAllMembersAsync();

            Assert.NotNull(members);
            Assert.NotZero(members.Count);

            Actors = members.Where(x => x.Roles.Any(x => x.Id == ActorRoleId)).ToList();

            Assert.NotZero(Actors.Count, "Missing Actors for further testing.");

            var cnext = await Client.GetCommandsNextAsync();
            if (cnext.TryGetValue(TestingShardId, out var commandsNextExtension))
            {
                this.CNext = commandsNextExtension;
                Commands = CNext.RegisteredCommands;
            }

            Assert.NotNull(CNext, "Missing Commands Next Extension for testing shard.");

            var msg = await TestingChannel.SendMessageAsync("Starting Unit Tests . . .");

            Assert.NotNull(msg, "Message failed to send.");
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {

            await TestingChannel.SendMessageAsync(". . . Unit Tests Complete");

            await Client.StopAsync();
        }

        [Order(1)]
        [Test]
        public async Task PrefixCommandTest()
        {
            Commands.TryGetValue("prefix", out Command cmd);

            Assert.NotNull(cmd, "Prefix Command not found.");

            var ctx = CNext.CreateFakeContext(Actors.Random(), TestingChannel, "]prefix >", "]", cmd, ">");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = CNext.CreateFakeContext(Actors.Random(), TestingChannel, ">prefix ]", ">", cmd, "]");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = CNext.CreateFakeContext(Actors.Random(), TestingChannel, "]prefix", "]", cmd);

            res = await cmd.ExecuteAsync(ctx);

            Assert.False(res.IsSuccessful, "Command should have failed.");
        }

        [Order(2)]
        [Test]
        public async Task SAHCryptoCommandTest()
        {
            Commands.TryGetValue("crypto", out Command cmd);

            Assert.NotNull(cmd, "Crypto command not found.");

            var ctx = CNext.CreateFakeContext(Actors.Random(), TestingChannel, "]crypto testdata", "]", cmd, "testdata");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = CNext.CreateFakeContext(Actors.Random(), TestingChannel, "]crypto", "]", cmd);

            res = await cmd.ExecuteAsync(ctx);

            Assert.False(res.IsSuccessful, "Command should have failed.");
        }

        [Order(3)]
        [Test]
        public async Task AddFundsCommandTest()
        {
            Commands.TryGetValue("addfunds", out Command cmd);

            Assert.NotNull(cmd, "Add Funds command not found.");

            var ctx = CNext.CreateFakeContext(Actors.Random(), TestingChannel, $"]addfunds 1000 {Actors[0].Mention}", "]", cmd, $"1000 {Actors[0].Mention}");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(4)]
        [Test]
        public async Task BalanceCommandTest()
        {
            Commands.TryGetValue("balance", out Command cmd);

            Assert.NotNull(cmd, "Balance command not found.");

            for (int i = 0; i < 2; i++)
            {
                var ctxI = CNext.CreateFakeContext(Actors[i], TestingChannel, $"]balance", "]", cmd);

                var resI = await cmd.ExecuteAsync(ctxI);

                Assert.True(resI.IsSuccessful, "Command should have executed.");
            }

            var ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]balance {Actors[1].Mention}", "]", cmd, Actors[1].Mention);

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(5)]
        [Test]
        public async Task GiftCommandTest()
        {
            Commands.TryGetValue("gift", out Command cmd);

            Assert.NotNull(cmd, "Gift command not found.");

            var ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]gift {Actors[1].Mention} 500", "]", cmd, $"{Actors[1].Mention} 500");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful);

            ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]gift {Actors[1].Mention} 500000", "]", cmd, $"{Actors[1].Mention} 500000");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful);

            ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]gift {Client.CurrentUser.Mention} 500", "]", cmd, $"{Client.CurrentUser.Mention} 500");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful);

            ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]gift {Actors[0].Mention} 500", "]", cmd, $"{Actors[0].Mention} 500");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful);

            ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]gift {Actors[0].Mention}", "]", cmd, $"{Actors[0].Mention}");

            res = await cmd.ExecuteAsync(ctx);

            Assert.False(res.IsSuccessful);

            ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]gift 500", "]", cmd, $"500");

            res = await cmd.ExecuteAsync(ctx);

            Assert.False(res.IsSuccessful);
        }

        [Order(6)]
        [Test]
        public async Task SubFundsCommandTest()
        {
            Commands.TryGetValue("subfunds", out Command cmd);

            Assert.NotNull(cmd, "Sub Funds command not found.");

            var ctx = CNext.CreateFakeContext(Actors.Random(), TestingChannel, $"]subfunds 500 {Actors[0].Mention}", "]", cmd, $"500 {Actors[0].Mention}");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(7)]
        [Test]
        public async Task AdviceCommandTest()
        {
            Commands.TryGetValue("advice", out Command cmd);

            Assert.NotNull(cmd, "Advice command not found.");

            var ctx = CNext.CreateFakeContext(Actors.Random(), TestingChannel, $"]advice", "]", cmd);

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(8)]
        [Test]
        public async Task TableFlipCommandTest()
        {
            Commands.TryGetValue("tableflip", out Command cmd);

            Assert.NotNull(cmd, "Table Flip command not found.");

            var ctx = CNext.CreateFakeContext(Actors.Random(), TestingChannel, $"]tableflip", "]", cmd);

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(9)]
        [Test]
        public async Task UnFlipCommandTest()
        {
            Commands.TryGetValue("unflip", out Command cmd);

            Assert.NotNull(cmd, "Un Flip command not found.");

            var ctx = CNext.CreateFakeContext(Actors.Random(), TestingChannel, $"]unflip", "]", cmd);

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(10)]
        [Test]
        public async Task BurnCommandTest()
        {
            Commands.TryGetValue("burn", out Command cmd);

            Assert.NotNull(cmd, "Burn command not found.");

            var ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]burn {Actors[1].Mention}", "]", cmd, $"{Actors[1].Mention}");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            var ctx_1 = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]burn {Actors[1].Mention}", "]", cmd, $"{Actors[1].Mention}");
            var ctx_2 = CNext.CreateFakeContext(Actors[1], TestingChannel, $"]burn {Actors[0].Mention}", "]", cmd, $"{Actors[0].Mention}");

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
            Commands.TryGetValue("cat", out Command cmd);

            Assert.NotNull(cmd, "Cat command not found.");

            var ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]cat jpg png", "]", cmd, "jpg png");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]cat gif", "]", cmd, "gif");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]cat gif jpg random", "]", cmd, "gif jpg random");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]cat mine that", "]", cmd, "mine that");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]cat id=qsGk4el-D", "]", cmd, "id=qsGk4el-D");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]cat id=", "]", cmd, "id=");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(12)]
        [Test]
        public async Task RichestCommandTest()
        {
            Commands.TryGetValue("richest", out Command cmd);

            Assert.NotNull(cmd, "Richest command not found.");

            var ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]richest", "]", cmd);

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]richest server", "]", cmd, "server");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(13)]
        [Test]
        public async Task DonaldCommandTest()
        {
            Commands.TryGetValue("donald", out Command cmd);

            Assert.NotNull(cmd, "Donald command not found.");

            var ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]donald My IQ is one of the highest.", "]", cmd, "My IQ is one of the highest.");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(14)]
        [Test]
        public async Task GruCommandTest()
        {
            Commands.TryGetValue("gru", out Command cmd);

            Assert.NotNull(cmd, "Gru command not found.");

            var ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]gru Find a gru meme template | Create a custom gru meme generator | Nobody uses it", "]",
                cmd, "Find a gru meme template | Create a custom gru meme generator | Nobody uses it");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]gru Find a gru meme template | Create a custom gru meme generat", "]",
                cmd, "Find a gru meme template | Create a custom gru meme generat");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]gru Find a gru meme template | Create a custom | gru | meme generat", "]",
                cmd, "Find a gru meme template | Create a custom | gru | meme generat");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(15)]
        [Test]
        public async Task NutCommandTest()
        {
            Commands.TryGetValue("nut", out Command cmd);

            Assert.NotNull(cmd, "Nut command not found.");

            var ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]nut Find a gru meme template | Create a custom gru meme generator | Nobody uses it", "]",
                cmd, "Find a gru meme template | Create a custom gru meme generator | Nobody uses it");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(16)]
        [Test]
        public async Task PrisonerCommandTest()
        {
            Commands.TryGetValue("prisoner", out Command cmd);

            Assert.NotNull(cmd, "Prisoner command not found.");

            var ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]prisoner I used the Gru Meme Generator", "]",
                cmd, "I used the Gru Meme Generator");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(17)]
        [Test]
        public async Task PunchCommandTest()
        {
            Commands.TryGetValue("punch", out Command cmd);

            Assert.NotNull(cmd, "Punch command not found.");

            var ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]punch {Actors[1].Mention}", "]",
                cmd, $"{Actors[1].Mention}");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(18)]
        [Test]
        public async Task TheSearchCommandTest()
        {
            Commands.TryGetValue("thesearch", out Command cmd);

            Assert.NotNull(cmd, "The Search command not found.");

            var ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]thesearch Don't use NitroSharp", "]",
                cmd, "Don't use NitroSharp");

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(19)]
        [Test]
        public async Task CoinFlipGameCommandTest()
        {
            Commands.TryGetValue("coinflip", out Command cmd);

            Assert.NotNull(cmd, "Coin Flip command not found.");

            var ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]coinflip", "]",
                cmd);

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(19)]
        [Test]
        public async Task TriviaGameCommandTest()
        {
            Commands.TryGetValue("trivia", out Command cmd);

            Assert.NotNull(cmd, "Trivia command not found.");

            var ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]trivia", "]",
                cmd);

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        [Order(20)]
        [Test]
        public async Task TriviaTopCommandTest()
        {
            Commands.TryGetValue("triviatop", out Command cmd);

            Assert.NotNull(cmd, "Trivia Top command not found.");

            var ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]triviatop", "]",
                cmd);

            var res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]triviatop server", "]",
                cmd, "server");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]triviatop me", "]",
                cmd, "me");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");

            ctx = CNext.CreateFakeContext(Actors[0], TestingChannel, $"]triviatop percent", "]",
                cmd, "percent");

            res = await cmd.ExecuteAsync(ctx);

            Assert.True(res.IsSuccessful, "Command should have executed.");
        }

        // TODO Add automated testing for applicapble commands (Non-Voice commands).
    }
}