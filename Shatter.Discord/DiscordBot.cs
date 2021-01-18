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
using Shatter.Discord.Commands.Attributes;
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
        public const string VERSION = "alpha-0.1.1";
        public static DiscordBot? Bot { get; private set; }
        public static ConcurrentDictionary<CommandHandler, Tuple<Task, CancellationTokenSource>>? CommandsInProgress { get; private set; }
		#endregion

		#region Public Variables
		public IReadOnlyDictionary<string, Command>? Commands { get; private set; }
		public ConcurrentDictionary<string, List<Command>> CommandGroups { get; private set; } = new();
        public BotConfig Config { get; private set; }
        public DiscordShardedClient? Client { get; private set; }
        public DiscordRestClient? Rest { get; private set; }
        public LavalinkConfig LavaConfig { get; private set; }
        public Stopwatch Uptime { get; private set; }
        #endregion

        #region Private Variables
        private readonly LogLevel logLevel;
        private YouTubeConfig YTCfg;
        private readonly ServiceCollection services;
        private ServiceProvider? provider;
        private DiscordEventHandler? eventHandler;
        #endregion

        public DiscordBot(BotConfig botConfig, LavalinkConfig lavalinkConfig, YouTubeConfig youtubeConfig, ServiceCollection services)
        {
            CommandsInProgress = new ConcurrentDictionary<CommandHandler, Tuple<Task, CancellationTokenSource>>();

			this.logLevel = LogLevel.Debug;

			this.Config = botConfig;
			this.LavaConfig = lavalinkConfig;
			this.YTCfg = youtubeConfig;

            this.services = services;

			this.Uptime = new Stopwatch();

            // Assign the lastest bot to to the static Bot indidcator.
            Bot = this;
        }

        #region Configuration

        #endregion

        #region Initialize
        public async Task InitializeAsync()
        {
			this.Boot = BootStatus.booting;

			this.Client = new DiscordShardedClient(GetDiscordConfiguration());
			this.Rest = new DiscordRestClient(GetDiscordConfiguration());

            var commands = await this.Client.UseCommandsNextAsync(GetCommandsNextConfiguration()).ConfigureAwait(false);

            foreach (CommandsNextExtension c in commands.Values)
            {
                c.CommandErrored += CommandResponder.RespondErrorAsync;
                c.CommandExecuted += CommandResponder.RespondSuccessAsync;

                c.RegisterCommands(Assembly.GetAssembly(typeof(DiscordBot)));

                c.SetHelpFormatter<HelpFormatter>();

				this.Commands = c.RegisteredCommands;

                c.RegisterConverter(new LeaderboardTypeConverter());
                c.RegisterConverter(new QuestionCategoryConverter());
                c.RegisterConverter(new TimeSpanConverter());
                c.RegisterConverter(new AddRemoveTypeConverter());
            }

			this.CommandGroups = new ConcurrentDictionary<string, List<Command>>();

            foreach(var c in this.Commands?.Values ?? Array.Empty<Command>())
            {
                var ExGroup = c.CustomAttributes.FirstOrDefault(x => x is ExecutionModuleAttribute);
                if (ExGroup != default)
                {
					ExecutionModuleAttribute? group = ExGroup as ExecutionModuleAttribute;

					if (group is null)
					{
						continue;
					}

					if (this.CommandGroups.ContainsKey(group.GroupName))
					{
						this.CommandGroups[group.GroupName].Add(c);
					}
					else
					{
						this.CommandGroups[group.GroupName] = new List<Command>() { c };
					}
				}
            }

            var interactionConfig = GetInteractivityConfiguration();

            await this.Client.UseInteractivityAsync(interactionConfig).ConfigureAwait(false);

            var lavas = await this.Client.UseLavalinkAsync();

			// Register any additional client events.
			this.eventHandler = new DiscordEventHandler(this.Client, this.Rest, this.provider);
			this.eventHandler.Initalize();

			// Register the event needed to send data to the Command Handler.
			this.Client.MessageCreated += Client_MessageCreated;
        }


        private DiscordConfiguration GetDiscordConfiguration()
        {
            var cfg = new DiscordConfiguration
            {
                Token = this.Config.Token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = logLevel,
                ShardCount = this.Config.Shards,
                Intents = DiscordIntents.Guilds | DiscordIntents.GuildBans | DiscordIntents.GuildMessages
                | DiscordIntents.DirectMessages | DiscordIntents.GuildMessageReactions | DiscordIntents.GuildVoiceStates
                | DiscordIntents.GuildMembers,
            };

            return cfg;
        }

        private CommandsNextConfiguration GetCommandsNextConfiguration()
        {
			this.services.AddScoped<ShatterDatabaseContext>()
                .AddScoped<MemeService>()
                .AddSingleton<VoiceService>()
                .AddSingleton<MusicBingoService>();

			this.provider = this.services.BuildServiceProvider();

            var ccfg = new CommandsNextConfiguration
            {
                EnableDms = false,
                EnableMentionPrefix = true,
                EnableDefaultHelp = true,
                CaseSensitive = false,
                IgnoreExtraArguments = true,
                StringPrefixes = new string[] { this.Config.Prefix },
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
			{
				return Task.CompletedTask; // Looks like we can't handle any commands.
			}

			try
            {
                var cancel = new CancellationTokenSource();
                var handler = new CommandHandler(this.Commands, sender, this.Config);
                var task = handler.MessageReceivedAsync(sender.GetCommandsNext(), e.Message, cancel.Token);
                if (task.Status == TaskStatus.Running)
                {
                    CommandsInProgress[handler] = new Tuple<Task, CancellationTokenSource>(task, cancel);
                }
            }
            catch (Exception ex)
            {
				this.Client?.Logger.LogError(Event_CommandHandler, ex, "An unkown error occoured.");
            }

            return Task.CompletedTask;
        }
        #endregion

        #region Start
        public async Task StartAsync()
        {
			this.Uptime = Stopwatch.StartNew();

			if(this.Client is not null)
			{
				await this.Client.StartAsync().ConfigureAwait(false);
			}

			if (this.Rest is not null)
			{
				await this.Rest.InitializeAsync().ConfigureAwait(false);
			}

			this.Boot = BootStatus.ready;
        }
        #endregion


        public void Dispose()
        {
            Bot = null;

			if(this.Client is not null)
			{
				this.Client.MessageCreated -= Client_MessageCreated;
			}

			this.eventHandler?.Dispose();

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
					if(this.Client is not null)
					{
						this.Client.StopAsync().GetAwaiter().GetResult();
					}

					if (this.Rest is not null)
					{
						this.Rest.Dispose();
					}
				}
                catch { /* If there is an error disposing these, its because they are not started and thus dont need to be stopped. Ignore it. */ }
            }
        }

        public async ValueTask DisposeAsync()
        {
            Bot = null;

			if(this.Client is not null)
			{
				this.Client.MessageCreated -= Client_MessageCreated;
			}

			this.eventHandler?.Dispose();

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
					if (this.Client is not null)
					{
						await this.Client.StopAsync();
					}

					if (this.Rest is not null)
					{
						this.Rest.Dispose();
					}
				}
				catch { /* If there is an error disposing these, its because they are not started and thus dont need to be stopped. Ignore it. */ }
			}
        }
    }
}
