using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Shatter.Core;
using Shatter.Core.Database;
using Shatter.Core.Structures;
using Shatter.Dashboard;
using Shatter.Discord;

namespace Shatter
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return Start().GetAwaiter().GetResult();
        }

        private static async Task<int> Start()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddLogging(o => o.AddConsole());

            await using var serviceProvider = services.BuildServiceProvider();

            var botConfig = await ConfigurationManager.RegisterBotConfiguration(serviceProvider.GetService<ILogger<Program>>());

            if (botConfig is null)
                return -1;

            var lavaConfig = await ConfigurationManager.RegisterLavaLink(serviceProvider.GetService<ILogger<Program>>());

            if (lavaConfig is null)
                return -1;

            var ytConfig = await ConfigurationManager.RegisterYouTube(serviceProvider.GetService<ILogger<Program>>());

            if (ytConfig is null)
                return -1;

            using var bot = new DiscordBot((BotConfig)botConfig, (LavalinkConfig)lavaConfig, (YouTubeConfig)ytConfig, services);

            var model = new ShatterDatabaseContext();
            await model.Database.MigrateAsync();

            await bot.InitializeAsync();
            await bot.StartAsync();

            await Task.Delay(-1);

            return 0;
        }
    }
}
