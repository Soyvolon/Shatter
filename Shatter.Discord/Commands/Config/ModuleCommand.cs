using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Config
{
    public class ModuleCommand : CommandModule
    {
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
                    .AddField("List Command",
                        $"Usage: {ctx.Prefix}module list <type>\n" +
                        $"Type: `module` or `command`\n" +
                        $"Returns: List of all modules or commands, and thier status."
                    ).AddField("Enable Command",
                        $"Usage: {ctx.Prefix}module enable <type> <name>\n" +
                        $"Type: `module` or `command`\n" +
                        $"Name: Name of the module/command to enable.\n" +
                        $"Returns: Confirmation that a module/command is activated."
                    ).AddField("Disable Command",
                        $"Usage: {ctx.Prefix}module enable <type> <name>\n" +
                        $"Type: `module` or `command`\n" +
                        $"Name: Name of the module/command to disable.\n" +
                        $"Returns: Confirmation that a module/command is deactivated."
                    ).AddField("Default Command",
                        $"Usage: {ctx.Prefix}module default <name>\n" +
                        $"Name: Name of the command to return to its deafult state.\n" +
                        $"Returns: Confirmation that a command has been reset. A reset command follows the enabled/disabled status of its module."
                    );

                await ctx.RespondAsync(embed: help);
                return;
            }


        }

        private async Task ListModules(CommandContext ctx)
        {

        }

        private async Task ListCommands(CommandContext ctx)
        {

        }

        private async Task DisableModule(CommandContext ctx, string module)
        {

        }

        private async Task EnableModule(CommandContext ctx, string module)
        {

        }

        private async Task DisableCommand(CommandContext ctx, string command)
        {

        }

        private async Task EnableCommand(CommandContext ctx, string command)
        {

        }

        private async Task ResetCommand(CommandContext ctx, string command)
        {

        }
    }
}
