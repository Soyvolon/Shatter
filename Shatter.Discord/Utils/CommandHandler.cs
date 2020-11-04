﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Shatter.Core.Database;
using Shatter.Core.Structures;
using Shatter.Core.Structures.Guilds;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Utils
{
    public class CommandHandler
    {
        private readonly IReadOnlyDictionary<string, Command> _commands;
        private readonly BotConfig _config;
        private readonly DiscordClient _client;
        private readonly ILogger<BaseDiscordClient> _logger;

        public CommandHandler(IReadOnlyDictionary<string, Command> commands, DiscordClient client, BotConfig botConfig)
        {
            this._commands = commands;
            this._config = botConfig;
            this._client = client;
            this._logger = this._client.Logger;
        }

        // TODO: Update to save guild config state. This will run as is, but will not hold any saved data between sessions.
        public async Task MessageReceivedAsync(CommandsNextExtension cnext, DiscordMessage msg, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var model = cnext.Services.GetService<NSDatabaseModel>();

                var guildConfig = await model.Configs.FindAsync(msg.Channel.GuildId);

                if (guildConfig is null)
                {
                    guildConfig = new GuildConfig
                    {
                        GuildId = msg.Channel.GuildId,
                        Prefix = _config.Prefix
                    };

                    model.Configs.Add(guildConfig);

                    await model.SaveChangesAsync();
                }

                cancellationToken.ThrowIfCancellationRequested();

                int prefixPos = await PrefixResolver(msg, guildConfig);

                if (prefixPos == -1)
                    return; // Prefix is wrong, dont respond to this message.

                var prefix = msg.Content.Substring(0, prefixPos);
                string commandString = msg.Content.Replace(prefix, string.Empty);

                var command = cnext.FindCommand(commandString, out string args);

                cancellationToken.ThrowIfCancellationRequested();

                if (command is null)
                { // Looks like that command does not exsist!
                    await CommandResponder.RespondCommandNotFound(msg.Channel, prefix);
                }
                else
                {   // We found a command, lets deal with it.

                    if (guildConfig.DisabledCommands.Contains(command.Name))
                        return; // Command is disabled. Dont do a thing.

                    var moduleAttribute = command.CustomAttributes.FirstOrDefault(x => x is ExecutionModuleAttribute);

                    if(moduleAttribute != default)
                    {
                        var m = moduleAttribute as ExecutionModuleAttribute;
                        if (!guildConfig.ActivatedCommands.Contains(command.Name)
                            && guildConfig.DisabledModules.Contains(m.GroupName))
                            return; // Command is disabled, dont do a thing.
                    }

                    var ctx = cnext.CreateContext(msg, prefix, command, args);
                    // We are done here, its up to CommandsNext now.

                    cancellationToken.ThrowIfCancellationRequested();

                    await cnext.ExecuteCommandAsync(ctx);
                }
            }
            finally
            {
                if (!(DiscordBot.CommandsInProgress is null))
                {
                    if (DiscordBot.CommandsInProgress.TryRemove(this, out var taskData))
                    {
                        taskData.Item2.Dispose();
                        taskData.Item1.Dispose();
                    }
                }
            }
        }

        public async Task<int> PrefixResolver(DiscordMessage msg, GuildConfig guildConfig)
        {
            if (!msg.Channel.PermissionsFor(await msg.Channel.Guild.GetMemberAsync(_client.CurrentUser.Id).ConfigureAwait(false)).HasPermission(Permissions.SendMessages)) return -1; //Checks if bot can't send messages, if so ignore.
            else if (msg.Content.StartsWith(_client.CurrentUser.Mention)) return _client.CurrentUser.Mention.Length; // Always respond to a mention.
            else
            {
                try
                {
                    return guildConfig.Prefix.Length; //Return length of server prefix.
                }
                catch (Exception err)
                {
                    _logger.LogError(DiscordBot.Event_CommandHandler, $"Prefix Resolver failed in guild {msg.Channel.Guild.Name}:", DateTime.Now, err);
                    return -1;
                }
            }
        }
    }
}