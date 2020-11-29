using System.Collections.Generic;
using System.Linq;
using System.Text;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;

using Shatter.Core.Database;
using Shatter.Core.Structures.Guilds;

namespace Shatter.Discord.Utils
{
	/// <summary>
	/// This is currently the same as the DefaultHelpFormatter from DSharpPlus. Modify this class to edit your help formatting.
	/// </summary>
	public class HelpFormatter : BaseHelpFormatter
    {
        public DiscordEmbedBuilder EmbedBuilder { get; }
        private Command? Command { get; set; }
        private ulong GuildId { get; set; }
        private string Prefix { get; set; }

        private readonly ShatterDatabaseContext _database;

        public HelpFormatter(CommandContext ctx, ShatterDatabaseContext database) : base(ctx)
        {
            _database = database;

            EmbedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Help")
                .WithColor(0x00ff95);

            GuildId = ctx.Guild.Id;
            Prefix = ctx.Prefix;
        }

        public override CommandHelpMessage Build()
        {
            if (Command is null)
			{
				EmbedBuilder.WithDescription("Listing all modules. Specify a command to see more infomration.");
			}

			return new CommandHelpMessage(embed: EmbedBuilder.Build());
        }

        public override BaseHelpFormatter WithCommand(Command command)
        {
            Command = command;

            EmbedBuilder.WithDescription($"{Formatter.InlineCode(command.Name)}: {command.Description ?? "No description provided."}");

            if (command is CommandGroup cgroup && cgroup.IsExecutableWithoutSubcommands)
			{
				EmbedBuilder.WithDescription($"{EmbedBuilder.Description}\n\nThis group can be executed as a standalone command.");
			}

			if (command.Aliases?.Any() == true)
			{
				EmbedBuilder.AddField("Aliases", string.Join(", ", command.Aliases.Select(Formatter.InlineCode)), false);
			}

			if (command.Overloads?.Any() == true)
            {
                var sb = new StringBuilder();

                foreach (var ovl in command.Overloads.OrderByDescending(x => x.Priority))
                {
                    sb.Append('`').Append(command.QualifiedName);

                    foreach (var arg in ovl.Arguments)
					{
						sb.Append(arg.IsOptional || arg.IsCatchAll ? " [" : " <").Append(arg.Name).Append(arg.IsCatchAll ? "..." : "").Append(arg.IsOptional || arg.IsCatchAll ? ']' : '>');
					}

					sb.Append("`\n");

                    foreach (var arg in ovl.Arguments)
					{
						sb.Append('`').Append(arg.Name).Append(" (").Append(CommandsNext.GetUserFriendlyTypeName(arg.Type)).Append(")`: ").Append(arg.Description ?? "No description provided.").Append('\n');
					}

					sb.Append('\n');
                }

                EmbedBuilder.AddField("Arguments", sb.ToString().Trim(), false);
            }

            return this;
        }

        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            var guild = _database.Find<GuildConfig>(GuildId);

            if(guild is null)
            {
                guild = new GuildConfig(GuildId)
                {
                    Prefix = Prefix,
                };
                _database.Add(guild);
                _database.SaveChanges();
            }

            HashSet<string> disabledCommands = new();
            disabledCommands.UnionWith(guild.DisabledCommands);

			if (DiscordBot.Bot is not null)
			{
				foreach (var module in DiscordBot.Bot.CommandGroups)
				{
					if (guild.DisabledModules.Contains(module.Key))
					{
						disabledCommands.UnionWith(module.Value.Select(x => x.Name.ToLower()));
					}
				}
			}

            disabledCommands.ExceptWith(guild.ActivatedCommands);

            var cmdList = subcommands.Where(x => !disabledCommands.Contains(x.Name.ToLower()));

            EmbedBuilder.AddField(Command != null ? "Subcommands" : "Commands", string.Join(", ", 
                cmdList.Select(x => {
                    return Formatter.InlineCode(x.Name);
                })), false);

            return this;
        }
    }
}
