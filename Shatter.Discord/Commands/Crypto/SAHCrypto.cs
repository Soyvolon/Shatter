﻿using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Crypto
{
	public class SAHCrypto : CommandModule
    {
        [Command("crypto")]
        [Description("Get the shasum of a string.")]
        [ExecutionModule("crypto")]
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
