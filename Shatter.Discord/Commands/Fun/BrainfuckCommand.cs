using System;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;

using Shatter.Discord.Commands.Attributes;
using Shatter.Discord.Utils;

namespace Shatter.Discord.Commands.Fun
{
    public class BrainfuckCommand : CommandModule
    {
        private const int TimeoutSeconds = 60;

        [Command("brainfuck")]
        [Description("Complie and execute brainfuck code!")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        [ExecutionModule("fun")]
        public async Task BrainfuckCommandAsync(CommandContext ctx,
            [Description("Brainfuck code")]
            [RemainingText]
            string code)
        {
            if (await Brainfuck.Verify(code, out string reason))
            {
                await RespondBasicSuccessAsync( "Staring Execution...");

                var cts = new CancellationTokenSource();
                var loopTimer = new Timer((x) => cts.Cancel(), null, TimeSpan.FromSeconds(TimeoutSeconds), TimeSpan.FromMilliseconds(-1));
                try
                {
                    var res = await Brainfuck.Execute(ctx, ctx.Client.GetInteractivity(), code, cts.Token);

                    if (res == "")
                        await RespondBasicSuccessAsync( "No Output Detected");
                    else
                        await ctx.RespondAsync(res);
                }
                catch (OperationCanceledException)
                {
                    await RespondBasicErrorAsync("Infinite Loop Detected.");
                }
            }
            else
            {
                await RespondBasicErrorAsync($"Code failed to compile: {reason}");
            }
        }
    }
}
