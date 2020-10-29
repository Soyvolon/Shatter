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

using NitroSharp.Discord.Commands;
using NitroSharp.Discord.Commands.CustomArguments;
using NitroSharp.Discord.Services;

using NitroSharp.Core.Structures;
using NitroSharp.Core.Structures.Guilds;
using NitroSharp.Core.Utils;
using NitroSharp.Core.Database;
using NitroSharp.Discord.Utils;

namespace NitroSharp.Discord
{
    public class DiscordBot
    {
        #region Boot Status
        public enum BootStatus { offline, booting, ready }
        public BootStatus Boot { get; private set; } = BootStatus.offline;
        #endregion

        public static DiscordBot Bot;

        #region Public Variables
        public IEnumerable<string> CommandList { get; private set; }
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

        public DiscordBot(BotConfig botConfig, LavalinkConfig lavalinkConfig, YouTubeConfig youtubeConfig, bool test = false)
        {
            logLevel = LogLevel.Debug;
            this.test = test;
            eventModel = new NSDatabaseModel();

            Config = botConfig;
            LavaConfig = lavalinkConfig;
            YTCfg = youtubeConfig;
            
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

        public async Task SendJoinMessageAsync(GuildMemberlogs g, CommandContext ctx)
        {
            if (!(g.MemberlogChannel is null))
            {
                string? msg = null;
                if (!(g.JoinMessage?.Message is null))
                    msg = await ReplaceValues(g.JoinMessage.Message, ctx);

                await SendJoinMessageAsync(g, msg, ctx.Member.Username, ctx.Member?.AvatarUrl ?? "");
            }
        }

        public async Task SendJoinMessageAsync(GuildMemberlogs g, GuildMemberAddEventArgs e)
        {
            if (!(g.MemberlogChannel is null))
            {
                string? msg = null;
                if (!(g.JoinMessage?.Message is null))
                    msg = await ReplaceValues(g.JoinMessage.Message, e);

                await SendJoinMessageAsync(g, msg, e.Member.Username, e.Member?.AvatarUrl ?? "");
            }
        }

        public async Task SendJoinMessageAsync(GuildMemberlogs g, string? msg, string username, string avatarUrl)
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

        public async Task SendLeaveMessageAsync(GuildMemberlogs g, CommandContext ctx)
        {
            if (!(g.MemberlogChannel is null))
            {
                string? msg = null;
                if (!(g.LeaveMessage?.Message is null))
                    msg = await ReplaceValues(g.LeaveMessage.Message, ctx);

                await SendLeaveMessageAsync(g, msg, ctx.Member.Username, ctx.Member?.AvatarUrl ?? "");
            }
        }

        public async Task SendLeaveMessageAsync(GuildMemberlogs g, GuildMemberRemoveEventArgs e)
        {
            if (!(g.MemberlogChannel is null))
            {
                string? msg = null;
                if (!(g.LeaveMessage?.Message is null))
                    msg = await ReplaceValues(g.LeaveMessage.Message, e);

                await SendLeaveMessageAsync(g, msg, e.Member.Username, e.Member?.AvatarUrl ?? "");
            }
        }

        public async Task SendLeaveMessageAsync(GuildMemberlogs g, string? msg, string username, string avatarUrl)
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

        public async Task SendJoinDMMessage(GuildMemberlogs g, GuildMemberAddEventArgs e)
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
            var guild = eventModel.Find<GuildMemberlogs>(e.Guild.Id);

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
            var guild = eventModel.Find<GuildMemberlogs>(e.Guild.Id);
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
