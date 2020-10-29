using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace NitroSharp.Discord.Commands.Info
{
    public class BotStatsCommand : BaseCommandModule
    {
        private const string Library = "DSharpPlus";
        private const string Creator = "Soyvolon#8016";

        [Command("stats")]
        [Description("Get bot statistics!")]
        [Aliases("botinfo")]
        public async Task BotStatsCommandAsync(CommandContext ctx)
        {
            int guilds = 0, channels = 0, users = 0;

            foreach (var shard in DiscordBot.Bot.Client.ShardClients.Values)
            {
                foreach (var g in shard.Guilds.Values)
                {
                    guilds++;
                    channels += g.Channels.Count;
                    users += g.MemberCount;
                }
            }

            var shards = "**```http\n" + string.Join("\n", new string[]
            {
                $"Count      :: {ctx.Client.ShardCount}",
                $"Servers    :: {guilds}",
                $"Channels   :: {channels}",
                $"Users      :: {users}",
            }) + "```**";

            var mods = "**```http\n" + string.Join("\n", new string[]
            {
                $"Memory GB  :: [Currently Broken]MB",
                $"CPU        :: [Currently Broken]%",
                $"Ping MS    :: {ctx.Client.Ping}MS",
                $"Uptime     :: {DiscordBot.Bot.Uptime.Elapsed:c}"
            }) + "```**";

            var dsharpplus = Assembly.GetAssembly(typeof(DiscordClient))?.GetName() ?? null;

            var envir = "**```http\n" + string.Join("\n", new string[]
            {
                $"C# Version :: {Environment.Version}",
                $"OS         :: {Environment.OSVersion.VersionString}",
                $"Library    :: {Library}",
                $"Version    :: {dsharpplus?.Version}",
            }) + "```**";

            var info = "**```http\n" + string.Join("\n", new string[]
            {
                $"Creator    :: {Creator}",
                $"Modules    :: Coming Soon",
                $"Commands   :: {DiscordBot.Bot.Commands.Count()}",
                $"Website    :: Coming Soon",
                //$"Patreon    :: Coming Soon",
                $"PayPal     :: https://paypal.me/pools/c/8tgZB4cjzU"
            }) + "```**";

            var embed = CommandUtils.SuccessBase()
                .AddField("—— Shards ——", shards, true)
                .AddField("—— Usage ——", mods, true)
                .AddField("—— Environment ——", envir, true)
                .AddField("—— Information ——", info, true);

            await ctx.RespondAsync(embed: embed);
        }
    }
}
