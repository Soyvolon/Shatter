﻿using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;

using Microsoft.Extensions.Logging;

using NitroSharp.Commands;

namespace NitroSharp.Utils
{
    public static class CommandResponder
    {
        // TODO Update Class
        public static Task RespondSuccess(CommandsNextExtension cnext, CommandExecutionEventArgs e)
        {
            // let's log the name of the command and user
            e.Context.Client.Logger.LogInformation(Program.CommandResponder, $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);

            return Task.CompletedTask;
        }

        public static async Task RespondError(CommandsNextExtension cnext, CommandErrorEventArgs e)
        {
            if (e == null) return;
            if (e.Exception is ChecksFailedException cfex)
            {
                await ChecksFailedResponderAsync(e, cfex).ConfigureAwait(false);
            }
            else if (e.Exception is ArgumentException)
            {
                await ArgumentResponder(e).ConfigureAwait(false);
            }
            else
            {
                var embed = CommandUtils.ErrorBase(e.Context)
                    .WithDescription($"An unhadled error occoured: {e.Exception.Message}");

                await e.Context.RespondAsync(embed: embed).ConfigureAwait(false);
            }
        }

        private static async Task ChecksFailedResponderAsync(CommandErrorEventArgs args, ChecksFailedException e)
        {
            var embed = CommandUtils.ErrorBase(args.Context)
                .WithDescription($"Invalid Permissions: {e.Message}");

            await args.Context.RespondAsync(embed: embed);
        }

        private static async Task ArgumentResponder(CommandErrorEventArgs args)
        {
            var embed = CommandUtils.ErrorBase(args.Context)
                .WithDescription($"Invalid Arguments");

            await args.Context.RespondAsync(embed: embed);
        }
    }
}
