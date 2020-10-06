using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Lavalink;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using NitroSharp.Commands.CustomArguments;
using NitroSharp.Database;
using NitroSharp.Services;
using NitroSharp.Structures;
using NitroSharp.Utils;

namespace NitroSharp
{
    public class DiscordBot
    {
        #region Boot Status
        public enum BootStatus { offline, booting, ready }
        public BootStatus Boot { get; private set; } = BootStatus.offline;
        #endregion

        #region Public Variables
        public static IEnumerable<string> CommandList { get; private set; }
        public DatabaseConfig Database { get; private set; }
        public BotConfig Config { get; private set; }
        public DiscordShardedClient Client { get; private set; }
        public DiscordRestClient Rest { get; private set; }
        public LavalinkConfig LavaConfig { get; private set; }
        #endregion

        #region Private Variables
        private readonly LogLevel logLevel;
        private readonly bool test;

        private YouTubeConfig YTCfg;
        private Process LavaLink;
        #endregion

        public DiscordBot(bool test = false)
        {
            logLevel = LogLevel.Debug;
            this.test = test;
        }

        #region Configuration
        private async Task RegisterConfiguration()
        {
            var manualConfig = false;

            if (!Directory.Exists("Configs"))
                Directory.CreateDirectory("Configs");

            var root = @"Configs\";
            if (File.Exists($"{root}bot_config.json"))
            {
                using var fs = new FileStream($"{root}bot_config.json", FileMode.Open);
                using var sr = new StreamReader(fs);
                string json = await sr.ReadToEndAsync().ConfigureAwait(false);
                try
                {
                    Config = JsonConvert.DeserializeObject<BotConfig>(json);
                }
                catch
                {
                    manualConfig = true;
                }
            }
            else
            {
                manualConfig = true;
            }

            if (manualConfig)
            {
                Config = new BotConfig
                {
                    Prefix = "c!", // Discord Bot Prefix
                    Token = "fill_me", // Discord Bot Token
                    Shards = 1, // Auto sharding
                    Admins = new ulong[] { 0 }
                };

                using var sw = new StreamWriter(new FileStream($"{root}bot_config.json", FileMode.Create));
                var json = JsonConvert.SerializeObject(Config, Formatting.Indented);

                foreach (string line in json.Split("\n"))
                    await sw.WriteLineAsync(line).ConfigureAwait(false);

                await sw.FlushAsync().ConfigureAwait(false);
                sw.Close();

                Console.WriteLine(@"New Bot Configuration generated due to missing config or error. Please open and edit Configs\bot_config.json to new bot settings.");
                if (!test)
                    Console.ReadLine();
                Environment.Exit(0);
            }
        }

        public async Task RegisterDatabase()
        {
            var manualConfig = false;

            if (!Directory.Exists("Configs"))
                Directory.CreateDirectory("Configs");

            var root = @"Configs\";
            if (File.Exists($"{root}database_config.json"))
            {
                using var fs = new FileStream($"{root}database_config.json", FileMode.Open);
                using var sr = new StreamReader(fs);
                string json = await sr.ReadToEndAsync().ConfigureAwait(false);
                try
                {
                    Database = JsonConvert.DeserializeObject<DatabaseConfig>(json);
                }
                catch
                {
                    manualConfig = true;
                }
            }
            else
            {
                manualConfig = true;
            }

            if (manualConfig)
            {
                Database = new DatabaseConfig("BotMain", "localhost", "postgres", "password", "GuildConfigs", 0000);

                using var sw = new StreamWriter(new FileStream($"{root}database_config.json", FileMode.Create));
                var json = JsonConvert.SerializeObject(Database, Formatting.Indented);

                foreach (string line in json.Split("\n"))
                    await sw.WriteLineAsync(line).ConfigureAwait(false);

                await sw.FlushAsync().ConfigureAwait(false);
                sw.Close();

                Console.WriteLine(@"New Database Configuration generated due to missing config or error. Please open and edit Configs\database_config.json to new database settings.");
                if (!test)
                    Console.ReadLine();
                Environment.Exit(0);
            }
        }

        public async Task RegisterYoutube()
        {
            var manualConfig = false;

            if (!Directory.Exists("Configs"))
                Directory.CreateDirectory("Configs");

            var root = @"Configs\";
            if (File.Exists($"{root}youtube_config.json"))
            {
                using var fs = new FileStream($"{root}youtube_config.json", FileMode.Open);
                using var sr = new StreamReader(fs);
                string json = await sr.ReadToEndAsync().ConfigureAwait(false);
                try
                {
                    YTCfg = JsonConvert.DeserializeObject<YouTubeConfig>(json);
                }
                catch
                {
                    manualConfig = true;
                }
            }
            else
            {
                manualConfig = true;
            }

            if (manualConfig)
            {
                YTCfg = new YouTubeConfig()
                {
                    ApiKey = ""
                };

                using var sw = new StreamWriter(new FileStream($"{root}youtube_config.json", FileMode.Create));
                var json = JsonConvert.SerializeObject(YTCfg, Formatting.Indented);

                foreach (string line in json.Split("\n"))
                    await sw.WriteLineAsync(line).ConfigureAwait(false);

                await sw.FlushAsync().ConfigureAwait(false);
                sw.Close();

                Console.WriteLine(@"New YouTube Configuration generated due to missing config or error. Please open and edit Configs\youtube_config.json to new youtube settings.");
                if (!test)
                    Console.ReadLine();
                Environment.Exit(0);
            }
        }

        public async Task RegisterLavaLink()
        {
            var manualConfig = false;

            if (!Directory.Exists("Configs"))
                Directory.CreateDirectory("Configs");

            var root = @"Configs\";
            if (File.Exists($"{root}lavalink_config.json"))
            {
                using var fs = new FileStream($"{root}lavalink_config.json", FileMode.Open);
                using var sr = new StreamReader(fs);
                string json = await sr.ReadToEndAsync().ConfigureAwait(false);
                try
                {
                    LavaConfig = JsonConvert.DeserializeObject<LavalinkConfig>(json);
                }
                catch
                {
                    manualConfig = true;
                }
            }
            else
            {
                manualConfig = true;
            }

            if (manualConfig)
            {
                LavaConfig = new LavalinkConfig()
                {
                    Password = ""
                };

                using var sw = new StreamWriter(new FileStream($"{root}lavalink_config.json", FileMode.Create));
                var json = JsonConvert.SerializeObject(LavaConfig, Formatting.Indented);

                foreach (string line in json.Split("\n"))
                    await sw.WriteLineAsync(line).ConfigureAwait(false);

                await sw.FlushAsync().ConfigureAwait(false);
                sw.Close();

                Console.WriteLine(@"New LavaLink Configuration generated due to missing config or error. Please open and edit Configs\lavalink_config.json to new lavalink settings.");
                if (!test)
                    Console.ReadLine();
                Environment.Exit(0);
            }
        }
        #endregion

        #region Initialize
        public async Task InitializeAsync()
        {
            Boot = BootStatus.booting;

            await RegisterConfiguration().ConfigureAwait(false);

            if (Database is null) // this may already be registered
                await RegisterDatabase().ConfigureAwait(false);

            await RegisterYoutube().ConfigureAwait(false);
            YouTube.Initalize(YTCfg);

            await RegisterLavaLink().ConfigureAwait(false);

            Client = new DiscordShardedClient(GetDiscordConfiguration());
            Rest = new DiscordRestClient(GetDiscordConfiguration());

            var commands = await Client.UseCommandsNextAsync(GetCommandsNextConfiguration()).ConfigureAwait(false);

            foreach (CommandsNextExtension c in commands.Values)
            {
                c.CommandErrored += CommandResponder.RespondError;
                c.CommandExecuted += CommandResponder.RespondSuccess;

                c.RegisterCommands(Assembly.GetExecutingAssembly());

                c.SetHelpFormatter<HelpFormatter>();

                CommandList = c.RegisteredCommands.Keys;

                c.RegisterConverter(new LeaderboardTypeConverter());
                c.RegisterConverter(new QuestionCategoryConverter());
            }

            var interactionConfig = GetInteractivityConfiguration();

            await Client.UseInteractivityAsync(interactionConfig).ConfigureAwait(false);

            var lavas = await Client.UseLavalinkAsync();
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
                | DiscordIntents.DirectMessages | DiscordIntents.GuildMessageReactions | DiscordIntents.GuildVoiceStates,
            };

            return cfg;
        }

        private CommandsNextConfiguration GetCommandsNextConfiguration()
        {
            var services = new ServiceCollection()
                .AddScoped<NSDatabaseModel>()
                .AddScoped<MemeService>()
                .AddScoped<VoiceService>();

            var ccfg = new CommandsNextConfiguration
            {
                EnableDms = false,
                EnableMentionPrefix = true,
                EnableDefaultHelp = true,
                CaseSensitive = false,
                IgnoreExtraArguments = true,
                PrefixResolver = PrefixResolver,
                StringPrefixes = new string[] { Config.Prefix },
                Services = services.BuildServiceProvider(),
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

        private async Task<int> PrefixResolver(DiscordMessage msg)
        {
            if (!msg.Channel.PermissionsFor(await msg.Channel.Guild.GetMemberAsync(Client.CurrentUser.Id).ConfigureAwait(false)).HasPermission(Permissions.SendMessages)) return -1; //Checks if bot can't send messages, if so ignore.
            else if (msg.Content.StartsWith(Client.CurrentUser.Mention)) return Client.CurrentUser.Mention.Length; // Always respond to a mention.
            else
            {
                try
                {
                    using NSDatabaseModel model = new NSDatabaseModel();

                    var gConfig = await model.Configs.FindAsync(msg.Channel.GuildId);

                    if (gConfig is null)
                    {
                        gConfig = new GuildConfig
                        {
                            GuildId = msg.Channel.GuildId,
                            Prefix = Config.Prefix
                        };

                        model.Configs.Add(gConfig);

                        await model.SaveChangesAsync();
                    }

                    foreach (string cmd in CommandList) //Loop through all current commands.
                    {
                        if (msg.Content.StartsWith(gConfig.Prefix + cmd)) //Check if message starts with prefix AND command.
                        {
                            return gConfig.Prefix.Length; //Return length of server prefix.
                        }
                    }

                    return -1; //If not, ignore.
                }
                catch (Exception err)
                {
                    Client.Logger.LogError(Program.PrefixManager, $"Resolver failed in guild {msg.Channel.Guild.Name}:", DateTime.Now, err);
                    return -1;
                }
            }
        }
        #endregion

        #region Start
        public async Task StartAsync()
        {
            var processInfo = new ProcessStartInfo("java.exe", "-jar Lavalink.jar")
            {
                CreateNoWindow = true,
                UseShellExecute = Program.IsDebug,
            };

            if ((LavaLink = Process.Start(processInfo)) == null)
                throw new InvalidOperationException("Failed to start LavaLink Process");

            LavaLink.Exited += LavaLink_OnExited;

            await Task.Delay(TimeSpan.FromSeconds(2));

            await Client.StartAsync().ConfigureAwait(false);
            await Rest.InitializeAsync().ConfigureAwait(false);

            Boot = BootStatus.ready;
        }
        #endregion

        #region Utility Methods

        private void LavaLink_OnExited(object? sender, EventArgs e)
        {
            LavaLink.Start();
        }
        #endregion
    }
}
