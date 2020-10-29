using System;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

using NitroSharp.Discord.Utils;

namespace NitroSharp.Discord.Commands.Fun
{
    public class BrainfuckCommand : BaseCommandModule
    {
        private const int TimeoutSeconds = 60;

        [Command("brainfuck")]
        [Description("Complie and execute brainfuck code!")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        public async Task BrainfuckCommandAsync(CommandContext ctx,
            [Description("Brainfuck code")]
            [RemainingText]
            string code)
        {
            if(await Brainfuck.Verify(code, out string reason))
            {
                await CommandUtils.RespondBasicSuccessAsync(ctx, "Staring Execution...");

                var cts = new CancellationTokenSource();
                var loopTimer = new Timer((x) => cts.Cancel(), null, TimeSpan.FromSeconds(TimeoutSeconds), TimeSpan.FromMilliseconds(-1));
                try
                {
                    var res = await Brainfuck.Execute(ctx, ctx.Client.GetInteractivity(), code, cts.Token);

                    if (res == "")
                        await CommandUtils.RespondBasicSuccessAsync(ctx, "No Output Detected");
                    else
                        await ctx.RespondAsync(res);
                }
                catch (OperationCanceledException)
                {
                    await CommandUtils.RespondBasicErrorAsync(ctx, "Infinite Loop Detected.");
                }
            }
            else
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, $"Code failed to compile: {reason}");
            }
        }
    }
}
