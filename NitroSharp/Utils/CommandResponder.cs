using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;

using NitroSharp.Commands;

using Microsoft.Extensions.Logging;

namespace NitroSharp.Utils
{
    public static class CommandResponder
    {
        public static Task RespondSuccess(CommandEventArgs e)
        {
            // let's log the name of the command and user
            e.Context.Client.Logger.LogInformation(Program.CommandResponder, $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);

            return Task.CompletedTask;
        }

        public static async Task RespondError(CommandErrorEventArgs e)
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
                var embed = CommandUtils.ErrorBase(e.Context);
                embed.Description = "An unknown error occoured. Please report this to Partner Bot Staff with pb!report <error message>\n\n" +
                    "Replace <error message> with the following:\n" +
                    $"```{e.Exception.Message}\n{e.Exception.StackTrace}```";

                await e.Context.RespondAsync(embed: embed).ConfigureAwait(false);
            }
        }

        private static async Task ChecksFailedResponderAsync(CommandErrorEventArgs args, ChecksFailedException e)
        {
            return;
        }

        private static async Task ArgumentResponder(CommandErrorEventArgs args)
        {
            return;
        }
    }
}
