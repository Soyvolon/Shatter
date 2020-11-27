using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;

namespace Shatter.Discord.Utils
{
	public static class Brainfuck
    {
        private const string AllowedChars = "><+-.,[]";

        public static Task<bool> Verify(string code, out string reason)
        {
            int startLoops = 0;
            int endLoops = 0;

            foreach (var c in code)
            {
                if (!AllowedChars.Contains(c))
                {
                    reason = $"Invalid Symbols Detected. Symbols must be one of thsee: `{AllowedChars}`";
                    return Task.FromResult(false);
                }
                else
                {
                    if (c == '[')
                        startLoops++;
                    else if (c == ']')
                        endLoops++;
                }
            }

            if (startLoops != endLoops)
            {
                reason = "Start loop symbols do not match end loop symbols.";
                return Task.FromResult(false);
            }

            reason = "";
            return Task.FromResult(true);
        }

        public static async Task<string> Execute(CommandContext ctx, InteractivityExtension interact, string code, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            byte[] program = Encoding.ASCII.GetBytes(code);
            ulong programPointer = 0;

            byte[] memory = new byte[1024];
            ulong pointer = 0;

            Stack<ulong> loopPointers = new Stack<ulong>();
            Dictionary<ulong, ulong> loopCache = new Dictionary<ulong, ulong>();

            string output = "";

            while (programPointer < (ulong)program.Length)
            {
                ct.ThrowIfCancellationRequested();

                switch (program[programPointer])
                {
                    case 0x3E: // >
                        ++pointer;
                        break;

                    case 0x3C: // <
                        --pointer;
                        break;

                    case 0x2B: // +
                        ++memory[pointer];
                        break;

                    case 0x2D: // -
                        --memory[pointer];
                        break;

                    case 0x2E: // .
                        output += Convert.ToChar(memory[pointer]);
                        break;

                    case 0x2C: // ,
                        await ctx.RespondAsync("Awaiting Char Input:");
                        var interactResult = await interact.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id, TimeSpan.FromSeconds(10));
                        if (interactResult.TimedOut) { return "`Failed to receive input.`"; }

                        byte item = (byte)interactResult.Result.Content.FirstOrDefault();
                        memory[pointer] = item;
                        break;

                    case 0x5B: // [
                        if (memory[pointer] != 0x00)
                        {
                            loopPointers.Push(programPointer);
                        }
                        else
                        {
                            if (loopCache.ContainsKey(programPointer))
                            {
                                programPointer = loopCache[programPointer];
                            }
                            else
                            {
                                programPointer++;

                                // Skip the loop
                                var currentPointer = programPointer;
                                var depth = 1;

                                for (var p = programPointer; p < (ulong)program.Length; p++)
                                {
                                    switch (program[p])
                                    {
                                        case 0x5B: // [
                                            depth++;
                                            break;
                                        case 0x5D: // ]
                                            depth--;
                                            break;
                                    }

                                    if (depth == 0)
                                    {
                                        loopCache[currentPointer] = p;
                                        programPointer = p;
                                        break;
                                    }
                                }
                            }
                        }
                        break;

                    case 0x5D: // ]
                        var oldPointer = programPointer;

                        if (loopPointers.TryPop(out programPointer))
                        {
                            loopCache[programPointer] = oldPointer;
                            programPointer--;
                        }
                        break;
                }

                programPointer++;
            }

            return output;
        }
    }
}
