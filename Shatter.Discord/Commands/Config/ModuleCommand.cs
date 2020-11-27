using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;

using Shatter.Core.Database;
using Shatter.Core.Structures.Guilds;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Config
{
	public class ModuleCommand : CommandModule
    {
        private readonly ShatterDatabaseContext _database;

        public ModuleCommand(ShatterDatabaseContext database)
        {
            this._database = database;
        }

        [Command("module")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [ExecutionModule("config")]
        public async Task ModuleCommandAsync(CommandContext ctx,
            [Description("Command arguments. Use `help` for more information.")]
            params string[] args)
        { // Module command group help command.
            if (args is null || args.Length <= 0 || args[0].ToLower() == "help")
            {

                var help = SuccessBase()
                    .WithTitle("Command Module Help")
                    .WithDescription("Listing help for commands in the `module` Group. These commands" +
                    " are centered around disabling and enabling different commands on your server, so " +
                    "Shatter only does what you want.")
                    .AddField("List", "```http\n" +
                        $"Usage   :: {ctx.Prefix}module list <type>\n" +
                        $"Type    :: `module` or `command`\n" +
                        $"Returns :: List of all modules or commands, and thier status." +
                        $"```"
                    ).AddField("Enable", "```http\n" +
                        $"Usage   :: {ctx.Prefix}module enable <type> <name>\n" +
                        $"Type    :: module | command\n" +
                        $"Name    :: Name of the module/command to enable.\n" +
                        $"Returns :: Confirmation that a module/command is activated." +
                        $"```"
                    ).AddField("Disable", "```http\n" +
                        $"Usage   :: {ctx.Prefix}module enable <type> <name>\n" +
                        $"Type    :: module | command\n" +
                        $"Name    :: Name of the module/command to disable.\n" +
                        $"Returns :: Confirmation that a module/command is deactivated." +
                        $"```"
                    ).AddField("Default", "```http\n" +
                        $"Usage   :: {ctx.Prefix}module default <name>\n" +
                        $"Name    :: Name of the command to return to its deafult state.\n" +
                        $"Returns :: Confirmation that a command has been reset. A reset command follows the enabled/disabled status of its module." +
                        $"```"
                    );

                await ctx.RespondAsync(embed: help);
                return;
            }

            switch(args[0].ToLower().Trim())
            {
                case "list":
                    if (args.Length < 2 ||  !args[1].Equals("command"))
                        await ListModules(ctx);
                    else
                        await ListCommands(ctx);
                    break;
                case "enable":
                    if (args.Length < 3)
                    {
                        if (args.Length < 2)
                            await RespondBasicErrorAsync("Failed to provide required paramater <type>. See `]module help` for more information.");
                        else
                            await RespondBasicErrorAsync("Failed to proivde required paramater <name>. See `]module help` for more information.");
                        return;
                    }

                    if (args[1].Equals("module", StringComparison.OrdinalIgnoreCase))
                        await EnableModule(ctx, args[2].ToLower());
                    else if (args[1].Equals("command", StringComparison.OrdinalIgnoreCase))
                        await EnableCommand(ctx, args[2].ToLower());
                    else
                        await RespondBasicErrorAsync("Failed to parse paramater <type>. Type must be either `module` or `command`.");
                    break;
                case "disable":
                    if (args.Length < 3)
                    {
                        if (args.Length < 2)
                            await RespondBasicErrorAsync("Failed to provide required paramater <type>. See `]module help` for more information.");
                        else
                            await RespondBasicErrorAsync("Failed to proivde required paramater <name>. See `]module help` for more information.");
                        return;
                    }

                    if (args[1].Equals("module", StringComparison.OrdinalIgnoreCase))
                        await DisableModule(ctx, args[2].ToLower());
                    else if (args[1].Equals("command", StringComparison.OrdinalIgnoreCase))
                        await DisableCommand(ctx, args[2].ToLower());
                    else
                        await RespondBasicErrorAsync("Failed to parse paramater <type>. Type must be either `module` or `command`.");
                    break;
                case "default":
                    if(args.Length < 2)
                    {
                        await RespondBasicErrorAsync("Failed to provide required paramater <name>. See `]module help` for more information.");
                        return;
                    }

                    await ResetCommand(ctx, args[1].ToLower());
                    break;
                default:
                    await RespondBasicErrorAsync("No module command found. See `]module help` for a list of commands.");
                    return;
            }
        }

        private async Task ListModules(CommandContext ctx)
        {
            if(DiscordBot.Bot?.CommandGroups is not null)
            {
                var guild = await _database.FindAsync<GuildConfig>(ctx.Guild.Id);

                if (guild is null)
                {
                    guild = new GuildConfig(ctx.Guild.Id)
                    {
                        Prefix = ctx.Prefix,
                    };
                    await _database.AddAsync(guild);
                    await _database.SaveChangesAsync();
                }

                HashSet<string> disabledModules = new();
                HashSet<string> enabledModules = new();

                foreach(var group in DiscordBot.Bot.CommandGroups.Keys)
                {
                    if (guild.DisabledModules.Contains(group))
                        disabledModules.Add(group);
                    else
                        enabledModules.Add(group);
                }

                var dm = disabledModules.OrderBy(x => x);
                var em = enabledModules.OrderBy(x => x);

                string data = "```Disabled Modules```\n";
                foreach(var s in dm)
                {
                    data += Formatter.Bold(s) + "\n";
                    data += string.Join(", ", DiscordBot.Bot.CommandGroups[s].Select(x => Formatter.InlineCode(x.Name))) + "\n";
                }

                data += "```Enabled Modules```\n";
                foreach(var s in em)
                {
                    data += Formatter.Bold(s) + "\n";
                    data += string.Join(", ", DiscordBot.Bot.CommandGroups[s].Select(x => Formatter.InlineCode(x.Name))) + "\n";
                }

                data = data[..(data.Length - 1)];

                var interact = ctx.Client.GetInteractivity();

                var pages = interact.GeneratePagesInEmbed(data, SplitType.Line, SuccessBase().WithTitle("Module List"));

                await interact.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages);
            }
            else
            {
                await RespondBasicErrorAsync("Something went wrong receiving the Command Module list.");
            }
        }

        private async Task ListCommands(CommandContext ctx)
        {
            if (DiscordBot.Bot?.CommandGroups is not null)
            {
                var guild = await _database.FindAsync<GuildConfig>(ctx.Guild.Id);

                if (guild is null)
                {
                    guild = new GuildConfig(ctx.Guild.Id)
                    {
                        Prefix = ctx.Prefix,
                    };
                    await _database.AddAsync(guild);
                    await _database.SaveChangesAsync();
                }

                HashSet<string> disabledCommands = new();
                HashSet<string> enabledCommands = new();

                foreach (var group in DiscordBot.Bot.CommandGroups)
                {
                    if (guild.DisabledModules.Contains(group.Key))
                        disabledCommands.UnionWith(group.Value.Select(x => x.Name));
                    else
                        enabledCommands.UnionWith(group.Value.Select(x => x.Name));
                }

                disabledCommands.ExceptWith(guild.ActivatedCommands);
                enabledCommands.UnionWith(guild.ActivatedCommands);

                var dc = disabledCommands.OrderBy(x => x);
                var ec = enabledCommands.OrderBy(x => x);

                string data = "```Disabled Commands```\n";

                foreach (var s in dc)
                {
                    var desc = DiscordBot.Bot.Commands[s].Description;
                    data += Formatter.InlineCode(s) + " - " + (desc?.Length <= 50 ? desc : desc?[..47] + "...") + "\n";
                }

                data += "```Enabled Commands```\n";
                foreach (var s in ec)
                {
                    var desc = DiscordBot.Bot.Commands[s].Description;
                    data += Formatter.InlineCode(s) + " - " + (desc?.Length <= 50 ? desc : desc?[..47] + "...") + "\n";
                }

                data = data[..(data.Length - 1)];

                var interact = ctx.Client.GetInteractivity();

                var pages = interact.GeneratePagesInEmbed(data, SplitType.Line, SuccessBase().WithTitle("Command List"));

                await interact.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages);
            }
            else
            {
                await RespondBasicErrorAsync("Something went wrong receiving the Command Module list.");
            }
        }

        private async Task DisableModule(CommandContext ctx, string module)
        {
            if (DiscordBot.Bot?.CommandGroups is not null)
            {
                var guild = await _database.FindAsync<GuildConfig>(ctx.Guild.Id);

                if (guild is null)
                {
                    guild = new GuildConfig(ctx.Guild.Id)
                    {
                        Prefix = ctx.Prefix,
                    };
                    await _database.AddAsync(guild);
                    await _database.SaveChangesAsync();
                }

                if(DiscordBot.Bot.CommandGroups.ContainsKey(module))
                {
                    if (guild.DisabledModules.Add(module))
                    {
                        _database.Update(guild);
                        await _database.SaveChangesAsync();

                        await RespondBasicSuccessAsync($"Disabled module: {module}");
                    }
                    else
                        await RespondBasicErrorAsync($"Module already disabled.");
                }
                else
                {
                    await RespondBasicErrorAsync($"Module group not found. See all modules with `{ctx.Prefix}module list module`.");
                }
            }
            else
            {
                await RespondBasicErrorAsync("Something went wrong receiving the Command Module list.");
            }
        }

        private async Task EnableModule(CommandContext ctx, string module)
        {
            if (DiscordBot.Bot?.CommandGroups is not null)
            {
                var guild = await _database.FindAsync<GuildConfig>(ctx.Guild.Id);

                if (guild is null)
                {
                    guild = new GuildConfig(ctx.Guild.Id)
                    {
                        Prefix = ctx.Prefix,
                    };
                    await _database.AddAsync(guild);
                    await _database.SaveChangesAsync();
                }

                if (DiscordBot.Bot.CommandGroups.ContainsKey(module))
                {
                    if (guild.DisabledModules.Remove(module))
                    {
                        _database.Update(guild);
                        await _database.SaveChangesAsync();

                        await RespondBasicSuccessAsync($"Enabled module: {module}");
                    }
                    else
                        await RespondBasicErrorAsync($"Module already enabled.");
                }
                else
                {
                    await RespondBasicErrorAsync($"Module group not found. See all modules with `{ctx.Prefix}module list module`.");
                }
            }
            else
            {
                await RespondBasicErrorAsync("Something went wrong receiving the Command Module list.");
            }
        }

        private async Task DisableCommand(CommandContext ctx, string command)
        {
            if (DiscordBot.Bot?.CommandGroups is not null)
            {
                var guild = await _database.FindAsync<GuildConfig>(ctx.Guild.Id);

                if (guild is null)
                {
                    guild = new GuildConfig(ctx.Guild.Id)
                    {
                        Prefix = ctx.Prefix,
                    };
                    await _database.AddAsync(guild);
                    await _database.SaveChangesAsync();
                }

                if (DiscordBot.Bot.Commands.ContainsKey(command))
                {
                    if (guild.DisabledCommands.Add(command))
                    {
                        guild.ActivatedCommands.Remove(command);
                        _database.Update(guild);
                        await _database.SaveChangesAsync();

                        await RespondBasicSuccessAsync($"Disabled command: {command}");
                    }
                    else
                        await RespondBasicErrorAsync($"Command already disabled.");
                }
                else
                {
                    await RespondBasicErrorAsync($"Command not found. See all commands with `{ctx.Prefix}module list command`.");
                }
            }
            else
            {
                await RespondBasicErrorAsync("Something went wrong receiving the Command Module list.");
            }
        }

        private async Task EnableCommand(CommandContext ctx, string command)
        {
            if (DiscordBot.Bot?.CommandGroups is not null)
            {
                var guild = await _database.FindAsync<GuildConfig>(ctx.Guild.Id);

                if (guild is null)
                {
                    guild = new GuildConfig(ctx.Guild.Id)
                    {
                        Prefix = ctx.Prefix,
                    };
                    await _database.AddAsync(guild);
                    await _database.SaveChangesAsync();
                }

                if (DiscordBot.Bot.Commands.ContainsKey(command))
                {
                    if (guild.ActivatedCommands.Add(command))
                    {
                        guild.DisabledCommands.Remove(command);
                        _database.Update(guild);
                        await _database.SaveChangesAsync();

                        await RespondBasicSuccessAsync($"Enabled command: {command}");
                    }
                    else
                        await RespondBasicErrorAsync($"Command already enabled.");
                }
                else
                {
                    await RespondBasicErrorAsync($"Command not found. See all modules with `{ctx.Prefix}module list command`.");
                }
            }
            else
            {
                await RespondBasicErrorAsync("Something went wrong receiving the Command Module list.");
            }
        }

        private async Task ResetCommand(CommandContext ctx, string command)
        {
            if (DiscordBot.Bot?.CommandGroups is not null)
            {
                var guild = await _database.FindAsync<GuildConfig>(ctx.Guild.Id);

                if (guild is null)
                {
                    guild = new GuildConfig(ctx.Guild.Id)
                    {
                        Prefix = ctx.Prefix,
                    };
                    await _database.AddAsync(guild);
                    await _database.SaveChangesAsync();
                }

                if (DiscordBot.Bot.Commands.ContainsKey(command))
                {
                    if (guild.ActivatedCommands.Remove(command) | guild.DisabledCommands.Remove(command))
                    {
                        _database.Update(guild);
                        await _database.SaveChangesAsync();

                        await RespondBasicSuccessAsync($"Defaulted command: {command}");
                    }
                    else
                        await RespondBasicErrorAsync($"Command already set to default.");
                }
                else
                {
                    await RespondBasicErrorAsync($"Command not found. See all modules with `{ctx.Prefix}module list command`.");
                }
            }
            else
            {
                await RespondBasicErrorAsync("Something went wrong receiving the Command Module list.");
            }
        }
    }
}
