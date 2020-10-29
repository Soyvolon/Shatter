using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NitroSharp.Core;
using NitroSharp.Core.Database;
using NitroSharp.Core.Structures;
using NitroSharp.Discord;

namespace NitroSharp
{
    public class Program
    {
        public static int Main(string[] args)
        {
            CommandLineApplication app = new CommandLineApplication();

            app.HelpOption("-h|--help");

            app.OnExecute(async () =>
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

                await bot.InitializeAsync();
                await bot.StartAsync();



                Console.ReadLine();

                return 0;
            });

            return app.Execute(args);
        }
    }
}
