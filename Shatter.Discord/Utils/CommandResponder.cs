using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;

using Microsoft.Extensions.Logging;

using Shatter.Discord.Commands;

namespace Shatter.Discord.Utils
{
	public static class CommandResponder
    {
        public static Task RespondSuccessAsync(CommandsNextExtension cnext, CommandExecutionEventArgs e)
        {
            // let's log the name of the command and user
            e.Context.Client.Logger.LogInformation(DiscordBot.Event_CommandResponder, $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);

            return Task.CompletedTask;
        }

        public static async Task RespondErrorAsync(CommandsNextExtension cnext, CommandErrorEventArgs e)
        {
            if (e == null)
			{
				return;
			}

			if (e.Exception is ChecksFailedException cfex)
            {
                await ChecksFailedResponderAsync(e, cfex).ConfigureAwait(false);
            }
            else if (e.Exception is ArgumentException)
            {
                await ArgumentResponderAsync(e).ConfigureAwait(false);
            }
            else
            {
                var embed = CommandModule.ErrorBase()
                    .WithDescription($"An unhadled error occoured: {e.Exception.Message}");

                await e.Context.RespondAsync(embed: embed).ConfigureAwait(false);
            }
        }

        public static async Task RespondCommandNotFoundAsync(DiscordChannel executionChannel, string prefix)
        {
            var embed = CommandModule.ErrorBase()
                .WithTitle("Command not found.")
                .WithDescription($"Use {prefix}help to see all avalible commands.");

            await executionChannel.SendMessageAsync(embed: embed);
        }

        public static async Task RespondCommandDisabledAsync(DiscordChannel executionChannel, string prefix)
        {
            var embed = CommandModule.ErrorBase()
                .WithTitle("Command Disabled")
                .WithDescription($"Use {prefix}help to see all enabled commands.");

            await executionChannel.SendMessageAsync(embed: embed);
        }

        private static async Task ChecksFailedResponderAsync(CommandErrorEventArgs args, ChecksFailedException e)
        {
            var embed = CommandModule.ErrorBase()
                .WithDescription($"Invalid Permissions: {e.Message}");

            await args.Context.RespondAsync(embed: embed);
        }

        private static async Task ArgumentResponderAsync(CommandErrorEventArgs args)
        {
            var embed = CommandModule.ErrorBase()
                .WithDescription($"Invalid Arguments");

            await args.Context.RespondAsync(embed: embed);
        }
    }
}
