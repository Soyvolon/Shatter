using System.Diagnostics;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Shatter.Discord.Commands.Info
{
    public class PingCommand : CommandModule
    {
        [Command("ping")]
        [Description("Check if Nitro is alive")]
        public async Task PingCommandAsync(CommandContext ctx)
        {
            Stopwatch timer = new Stopwatch();
            var pingEmbed = CommandModule.SuccessBase().WithTitle($"Ping for Shard {ctx.Client.ShardId}");
            pingEmbed.AddField("WS Latency:", $"{ctx.Client.Ping}ms");
            timer.Start();
            DiscordMessage msg = await ctx.RespondAsync(null, false, pingEmbed);
            await msg.ModifyAsync(null, pingEmbed.AddField("Response Time: (:ping_pong:)", $"{timer.ElapsedMilliseconds}ms").Build());
            timer.Stop();
        }
    }
}