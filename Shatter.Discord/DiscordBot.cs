using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Shatter.Core.Database;
using Shatter.Core.Structures;
using Shatter.Discord.Commands.CustomArguments;
using Shatter.Discord.Services;
using Shatter.Discord.Utils;

namespace Shatter.Discord
{
    public class DiscordBot : IDisposable, IAsyncDisposable
    {
        #region Boot Status
        public enum BootStatus { offline, booting, ready }
        public BootStatus Boot { get; private set; } = BootStatus.offline;
        #endregion

        #region Event Ids
        public static EventId Event_CommandResponder { get; } = new EventId(127001, "Command Responder");
        public static EventId Event_CommandHandler { get; } = new EventId(127002, "Command Handler");
        #endregion

        #region Static Variables
        public const string VERSION = "alpha-0.1.0";
        public static DiscordBot? Bot { get; private set; }
        public static ConcurrentDictionary<CommandHandler, Tuple<Task, CancellationTokenSource>>? CommandsInProgress { get; private set; }
        #endregion

        #region Public Variables
        public IReadOnlyDictionary<string, Command> Commands { get; private set; }
        public BotConfig Config { get; private set; }
        public DiscordShardedClient Client { get; private set; }
        public DiscordRestClient Rest { get; private set; }
        public LavalinkConfig LavaConfig { get; private set; }
        public Stopwatch Uptime { get; private set; }
        #endregion

        #region Private Variables
        private readonly LogLevel logLevel;
        private readonly bool test;
        private YouTubeConfig YTCfg;
        private ServiceCollection services;
        private ServiceProvider provider;
        private DiscordEventHandler eventHandler;
        #endregion

        public DiscordBot(BotConfig botConfig, LavalinkConfig lavalinkConfig, YouTubeConfig youtubeConfig, ServiceCollection services, bool test = false)
        {
            CommandsInProgress = new ConcurrentDictionary<CommandHandler, Tuple<Task, CancellationTokenSource>>();

            logLevel = LogLevel.Debug;
            this.test = test;

            Config = botConfig;
            LavaConfig = lavalinkConfig;
            YTCfg = youtubeConfig;

            this.services = services;

            // Assign the lastest bot to to the static Bot indidcator.
            Bot = this;
        }

        #region Configuration

        #endregion

        #region Initialize
        public async Task InitializeAsync()
        {
            Boot = BootStatus.booting;

            Client = new DiscordShardedClient(GetDiscordConfiguration());
            Rest = new DiscordRestClient(GetDiscordConfiguration());

            var commands = await Client.UseCommandsNextAsync(GetCommandsNextConfiguration()).ConfigureAwait(false);

            foreach (CommandsNextExtension c in commands.Values)
            {
                c.CommandErrored += CommandResponder.RespondError;
                c.CommandExecuted += CommandResponder.RespondSuccess;

                c.RegisterCommands(Assembly.GetAssembly(typeof(DiscordBot)));

                c.SetHelpFormatter<HelpFormatter>();

                Commands = c.RegisteredCommands;

                c.RegisterConverter(new LeaderboardTypeConverter());
                c.RegisterConverter(new QuestionCategoryConverter());
                c.RegisterConverter(new TimeSpanConverter());
                c.RegisterConverter(new AddRemoveTypeConverter());
            }

            var interactionConfig = GetInteractivityConfiguration();

            await Client.UseInteractivityAsync(interactionConfig).ConfigureAwait(false);

            var lavas = await Client.UseLavalinkAsync();

            // Register any additional client events.
            eventHandler = new DiscordEventHandler(Client, Rest, provider);
            eventHandler.Initalize();

            // Register the event needed to send data to the Command Handler.
            Client.MessageCreated += Client_MessageCreated;
        }


        private DiscordConfiguration GetDiscordConfiguration()
        {
            var cfg = new DiscordConfiguration
            {
                Token = Config.Token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = logLevel,
                ShardCount = Config.Shards,
                Intents = DiscordIntents.Guilds | DiscordIntents.GuildBans | DiscordIntents.GuildMessages
                | DiscordIntents.DirectMessages | DiscordIntents.GuildMessageReactions | DiscordIntents.GuildVoiceStates
                | DiscordIntents.GuildMembers,
            };

            return cfg;
        }

        private CommandsNextConfiguration GetCommandsNextConfiguration()
        {
            this.services.AddScoped<NSDatabaseModel>()
                .AddScoped<MemeService>()
                .AddScoped<VoiceService>();

            provider = services.BuildServiceProvider();

            var ccfg = new CommandsNextConfiguration
            {
                EnableDms = false,
                EnableMentionPrefix = true,
                EnableDefaultHelp = true,
                CaseSensitive = false,
                IgnoreExtraArguments = true,
                StringPrefixes = new string[] { Config.Prefix },
                Services = provider,
                UseDefaultCommandHandler = false
            };

            return ccfg;
        }

        private InteractivityConfiguration GetInteractivityConfiguration()
        {
            var icfg = new InteractivityConfiguration
            {
                // default pagination behaviour to just ignore the reactions
                PaginationDeletion = DSharpPlus.Interactivity.Enums.PaginationDeletion.DeleteEmojis,

                // default pagination timeout to 5 minutes
                //PaginationTimeout = TimeSpan.FromMinutes(5),

                // default timeout for other actions to 2 minutes
                Timeout = TimeSpan.FromMinutes(2)
            };

            return icfg;
        }
        #endregion

        #region Command Events
        private Task Client_MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (CommandsInProgress is null)
                return Task.CompletedTask; // Looks like we can't handle any commands.

            try
            {
                var cancel = new CancellationTokenSource();
                var handler = new CommandHandler(Commands, sender, Config);
                var task = handler.MessageReceivedAsync(sender.GetCommandsNext(), e.Message, cancel.Token);
                if (task.Status == TaskStatus.Running)
                {
                    CommandsInProgress[handler] = new Tuple<Task, CancellationTokenSource>(task, cancel);
                }
            }
            catch (Exception ex)
            {
                Client.Logger.LogError(Event_CommandHandler, ex, "An unkown error occoured.");
            }

            return Task.CompletedTask;
        }
        #endregion

        #region Start
        public async Task StartAsync()
        {
            Uptime = Stopwatch.StartNew();

            await Client.StartAsync().ConfigureAwait(false);
            await Rest.InitializeAsync().ConfigureAwait(false);

            Boot = BootStatus.ready;
        }
        #endregion


        public void Dispose()
        {
            Bot = null;

            Client.MessageCreated -= Client_MessageCreated;

            eventHandler?.Dispose();

            if (!(CommandsInProgress is null))
            {

                foreach (var cmd in CommandsInProgress.AsParallel())
                {
                    cmd.Value.Item2.Cancel();
                    cmd.Value.Item2.Dispose();
                    cmd.Value.Item1.Dispose();
                }

                // Clear out the dict.
                CommandsInProgress = null;
                try
                {
                    Client.StopAsync().GetAwaiter().GetResult();
                    Rest.Dispose();
                }
                catch { }
            }
        }

        public async ValueTask DisposeAsync()
        {
            Bot = null;

            Client.MessageCreated -= Client_MessageCreated;

            eventHandler?.Dispose();

            if (!(CommandsInProgress is null))
            {

                await Task.Run(() =>
                {
                    foreach (var cmd in CommandsInProgress.AsParallel())
                    {
                        cmd.Value.Item2.Cancel();
                        cmd.Value.Item2.Dispose();
                        cmd.Value.Item1.Dispose();
                    }
                });

                // Clear out the dict.
                CommandsInProgress = null;
                try
                {
                    var stop = Client.StopAsync();
                    Rest.Dispose();

                    await stop;
                }
                catch { }
            }
        }
    }
}
