using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using NitroSharp.Commands;
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
        public IEnumerable<string> CommandList { get; private set; }
        public DatabaseConfig Database { get; private set; }
        public BotConfig Config { get; private set; }
        public DiscordShardedClient Client { get; private set; }
        public DiscordRestClient Rest { get; private set; }
        public LavalinkConfig LavaConfig { get; private set; }
        public Stopwatch Uptime { get; private set; }
        #endregion

        #region Private Variables
        private readonly LogLevel logLevel;
        private readonly bool test;
        private NSDatabaseModel eventModel;
        private YouTubeConfig YTCfg;
        #endregion

        public DiscordBot(bool test = false)
        {
            logLevel = LogLevel.Debug;
            this.test = test;
            eventModel = new NSDatabaseModel();
        }

        #region Configuration
        private async Task RegisterConfiguration()
        {
            var manualConfig = false;

            if (!Directory.Exists("Configs"))
                Directory.CreateDirectory("Configs");

            var root = @"Configs";
            var path = Path.Join(root, "bot_config.json");
            if (File.Exists(path))
            {
                using var fs = new FileStream(path, FileMode.Open);
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

                using var sw = new StreamWriter(new FileStream(path, FileMode.Create));
                var json = JsonConvert.SerializeObject(Config, Formatting.Indented);

                foreach (string line in json.Split("\n"))
                    await sw.WriteLineAsync(line).ConfigureAwait(false);

                await sw.FlushAsync().ConfigureAwait(false);
                sw.Close();

                Console.WriteLine($"New Bot Configuration generated due to missing config or error. Please open and edit {path} to new bot settings.");
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

            var root = @"Configs";
            var path = Path.Join(root, "database_config.json");
            if (File.Exists(path))
            {
                using var fs = new FileStream(path, FileMode.Open);
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
                Database = new DatabaseConfig("NitroSharp", "localhost", "postgres", "password", "GuildConfigs", 0000);

                using var sw = new StreamWriter(new FileStream(path, FileMode.Create));
                var json = JsonConvert.SerializeObject(Database, Formatting.Indented);

                foreach (string line in json.Split("\n"))
                    await sw.WriteLineAsync(line).ConfigureAwait(false);

                await sw.FlushAsync().ConfigureAwait(false);
                sw.Close();

                Console.WriteLine($"New Database Configuration generated due to missing config or error. Please open and edit {path} to new database settings.");
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

            var root = @"Configs";
            var path = Path.Join(root, "lavalink_config.json");
            if (File.Exists(path))
            {
                using var fs = new FileStream(path, FileMode.Open);
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

                using var sw = new StreamWriter(new FileStream(path, FileMode.Create));
                var json = JsonConvert.SerializeObject(LavaConfig, Formatting.Indented);

                foreach (string line in json.Split("\n"))
                    await sw.WriteLineAsync(line).ConfigureAwait(false);

                await sw.FlushAsync().ConfigureAwait(false);
                sw.Close();

                Console.WriteLine($"New LavaLink Configuration generated due to missing config or error. Please open and edit {path} to new lavalink settings.");
                if (!test)
                    Console.ReadLine();
                Environment.Exit(0);
            }
        }

        public async Task RegisterYouTube()
        {
            var manualConfig = false;

            if (!Directory.Exists("Configs"))
                Directory.CreateDirectory("Configs");

            var root = @"Configs";
            var path = Path.Join(root, "youtube_config.json");
            if (File.Exists(path))
            {
                using var fs = new FileStream(path, FileMode.Open);
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

                using var sw = new StreamWriter(new FileStream(path, FileMode.Create));
                var json = JsonConvert.SerializeObject(YTCfg, Formatting.Indented);

                foreach (string line in json.Split("\n"))
                    await sw.WriteLineAsync(line).ConfigureAwait(false);

                await sw.FlushAsync().ConfigureAwait(false);
                sw.Close();

                Console.WriteLine($"New YoutTube Configuration generated due to missing config or error. Please open and edit {path} to new youtube settings.");
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

            await eventModel.Database.MigrateAsync();

            await RegisterConfiguration().ConfigureAwait(false);

            if (Database is null) // this may already be registered
                await RegisterDatabase().ConfigureAwait(false);

            await RegisterLavaLink().ConfigureAwait(false);
            await RegisterYouTube().ConfigureAwait(false);
            YouTube.Initalize(YTCfg);

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
                c.RegisterConverter(new TimeSpanConverter());
            }

            #region Register Client Events
            Client.GuildMemberAdded += Client_GuildMemberAdded;
            Client.GuildMemberRemoved += Client_GuildMemberRemoved;
            #endregion

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
                | DiscordIntents.DirectMessages | DiscordIntents.GuildMessageReactions | DiscordIntents.GuildVoiceStates
                | DiscordIntents.GuildMembers,
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
            Uptime = Stopwatch.StartNew();

            await Client.StartAsync().ConfigureAwait(false);
            await Rest.InitializeAsync().ConfigureAwait(false);

            Boot = BootStatus.ready;
        }
        #endregion

        #region Utility Methods
        public async Task<string> ReplaceValues(string message, GuildMemberAddEventArgs e)
        {
            return await ReplaceValues(message,
                e.Member.Username,
                e.Guild.Name,
                e.Guild.MemberCount.ToString());
        }

        public async Task<string> ReplaceValues(string message, GuildMemberRemoveEventArgs e)
        {
            return await ReplaceValues(message,
                e.Member.Username,
                e.Guild.Name,
                e.Guild.MemberCount.ToString());
        }

        public async Task<string> ReplaceValues(string message, CommandContext ctx)
        {
            return await ReplaceValues(message,
                ctx.Member.Username,
                ctx.Guild.Name,
                ctx.Guild.MemberCount.ToString());
        }

        public Task<string> ReplaceValues(string message, string user, string guild, string count)
        {
            var msg = message.Replace("{server}", guild);
            msg = msg.Replace("{guild}", guild);

            msg = msg.Replace("{user}", user);
            msg = msg.Replace("{member}", user);

            msg = msg.Replace("{membercount}", count);
            return Task.FromResult(msg);
        }

        public async Task SendJoinMessageAsync(GuildConfig g, CommandContext ctx)
        {
            if (!(g.MemberlogChannel is null))
            {
                string? msg = null;
                if (!(g.JoinMessage?.Message is null))
                    msg = await ReplaceValues(g.JoinMessage.Message, ctx);

                await SendJoinMessageAsync(g, msg, ctx.Member.Username, ctx.Member?.AvatarUrl ?? "");
            }
        }

        public async Task SendJoinMessageAsync(GuildConfig g, GuildMemberAddEventArgs e)
        {
            if (!(g.MemberlogChannel is null))
            {
                string? msg = null;
                if (!(g.JoinMessage?.Message is null))
                    msg = await ReplaceValues(g.JoinMessage.Message, e);

                await SendJoinMessageAsync(g, msg, e.Member.Username, e.Member?.AvatarUrl ?? "");
            }
        }

        public async Task SendJoinMessageAsync(GuildConfig g, string? msg, string username, string avatarUrl)
        {
            if (g.JoinMessage?.IsEmbed ?? false)
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                {
                    Color = new DiscordColor(CommandUtils.Colors[ColorType.Memberlog][0]),
                    Description = msg is null ? "" : msg,
                    Footer = new DiscordEmbedBuilder.EmbedFooter()
                    {
                        IconUrl = avatarUrl,
                        Text = "User Joined"
                    },
                    Timestamp = DateTime.Now
                };

                try
                {
                    await Rest.CreateMessageAsync((ulong)g.MemberlogChannel, "", false, embed, null);
                }
                catch { } // ignore
            }
            else if (g.JoinMessage?.IsImage ?? false)
            {
                using var stream = await SvgHandler.GetWelcomeImage(true, username, avatarUrl);
                if (!(stream is null))
                {
                    try
                    {
                        await Rest.UploadFileAsync((ulong)g.MemberlogChannel, stream, "welcome-message.png", "", false, null, null);
                    }
                    catch { } // ignore
                }
            }
            else if (!(msg is null))
            {
                try
                {
                    await Rest.CreateMessageAsync((ulong)g.MemberlogChannel, msg, false, null, null);
                }
                catch { } // ignore
            }
        }

        public async Task SendLeaveMessageAsync(GuildConfig g, CommandContext ctx)
        {
            if (!(g.MemberlogChannel is null))
            {
                string? msg = null;
                if (!(g.LeaveMessage?.Message is null))
                    msg = await ReplaceValues(g.LeaveMessage.Message, ctx);

                await SendLeaveMessageAsync(g, msg, ctx.Member.Username, ctx.Member?.AvatarUrl ?? "");
            }
        }

        public async Task SendLeaveMessageAsync(GuildConfig g, GuildMemberRemoveEventArgs e)
        {
            if (!(g.MemberlogChannel is null))
            {
                string? msg = null;
                if (!(g.LeaveMessage?.Message is null))
                    msg = await ReplaceValues(g.LeaveMessage.Message, e);

                await SendLeaveMessageAsync(g, msg, e.Member.Username, e.Member?.AvatarUrl ?? "");
            }
        }

        public async Task SendLeaveMessageAsync(GuildConfig g, string? msg, string username, string avatarUrl)
        {
            if (g.LeaveMessage?.IsEmbed ?? false)
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                {
                    Color = new DiscordColor(CommandUtils.Colors[ColorType.Memberlog][1]),
                    Description = msg is null ? "" : msg,
                    Footer = new DiscordEmbedBuilder.EmbedFooter()
                    {
                        IconUrl = avatarUrl,
                        Text = "User Left"
                    },
                    Timestamp = DateTime.Now
                };

                try
                {
                    await Rest.CreateMessageAsync((ulong)g.MemberlogChannel, "", false, embed, null);
                }
                catch { } // ignore

            }
            else if (g.LeaveMessage?.IsImage ?? false)
            {
                using var stream = await SvgHandler.GetWelcomeImage(false, username, avatarUrl);
                if (!(stream is null))
                {
                    try
                    {
                        await Rest.UploadFileAsync((ulong)g.MemberlogChannel, stream, "farewell-message.png", "", false, null, null);
                    }
                    catch { } // ignore
                }
            }
            else if (!(g.LeaveMessage?.Message is null))
            {
                try
                {
                    await Rest.CreateMessageAsync((ulong)g.MemberlogChannel, msg, false, null, null);
                }
                catch { } // ignore
            }
        }

        public async Task SendJoinDMMessage(GuildConfig g, GuildMemberAddEventArgs e)
        {
            if (g.JoinDmMessage is null) return;

            var msg = await ReplaceValues(g.JoinDmMessage, e);
            try
            {
                var dms = await e.Member.CreateDmChannelAsync();

                await dms.SendMessageAsync(msg);
            }
            catch { } // ignore
        }
        #endregion

        #region Events
        private async Task Client_GuildMemberAdded(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            var guild = eventModel.Find<GuildConfig>(e.Guild.Id);

            if(!(guild is null))
            {
                if(!(guild.MemberlogChannel is null) && !(guild.JoinMessage is null))
                {
                    await SendJoinMessageAsync(guild, e);
                }

                if(!(guild.JoinDmMessage is null))
                {
                    await SendJoinDMMessage(guild, e);
                }
            }
        }

        private async Task Client_GuildMemberRemoved(DiscordClient sender, GuildMemberRemoveEventArgs e)
        {
            var guild = eventModel.Find<GuildConfig>(e.Guild.Id);
            if (!(guild is null))
            {
                if (!(guild.MemberlogChannel is null) && !(guild.LeaveMessage is null))
                {
                    await SendLeaveMessageAsync(guild, e);
                }
            }
        }
        #endregion
    }
}
