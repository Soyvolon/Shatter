using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace NitroSharp.Commands.Crypto
{
    public class SAHCrypto : BaseCommandModule
    {
        [Command("crypto")]
        [Description("Get the shasum of a string.")]
        public async Task SAHCryptoCommandAsync(CommandContext ctx, [RemainingText] string toConvert)
        {
            var sha = SHA256.Create();
            var converted = sha.ComputeHash(Encoding.ASCII.GetBytes(toConvert));

            var readable = BitConverter.ToString(converted).Replace("-", "");

            var output = $"{toConvert} - {Formatter.BlockCode(readable)}";

            await ctx.RespondAsync(output);
        }
    }
}
