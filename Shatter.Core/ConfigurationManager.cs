using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Shatter.Core.Structures;

namespace Shatter.Core
{
	public static class ConfigurationManager
    {
        private static readonly string Root = "Configs";

        public static async Task<BotConfig?> RegisterBotConfiguration(ILogger? logger)
        {
            var path = Path.Join(Root, "bot_config.json");
            if (File.Exists(path))
            {
                using var fs = new FileStream(path, FileMode.Open);
                using var sr = new StreamReader(fs);
                string json = await sr.ReadToEndAsync().ConfigureAwait(false);
                try
                {
                    return JsonConvert.DeserializeObject<BotConfig>(json);
                }
                catch { /* Ignore JSON erros, a null value will be returned later after logging. */ }
            }

            if (logger is null)
			{
				Console.WriteLine("Missing bot_config.json file in Configs volume.");
			}
			else
			{
				logger.LogError($"Missing bot_config.json file in Configs volume.");
			}

			return null;
        }

        public static async Task<DatabaseConfig?> RegisterDatabase(ILogger? logger)
        {
            var path = Path.Join(Root, "database_config.json");

            if (File.Exists(path))
            {
                using var fs = new FileStream(path, FileMode.Open);
                using var sr = new StreamReader(fs);
                string json = await sr.ReadToEndAsync().ConfigureAwait(false);
                try
                {
                    return JsonConvert.DeserializeObject<DatabaseConfig>(json);
				}
				catch { /* Ignore JSON erros, a null value will be returned later after logging. */ }
			}

            if (logger is null)
			{
				Console.WriteLine("Missing database_config.json file in Configs volume.");
			}
			else
			{
				logger.LogError($"Missing database_config.json file in Configs volume.");
			}

			return null;
        }

        public static async Task<LavalinkConfig?> RegisterLavaLink(ILogger? logger)
        {
            var path = Path.Join(Root, "lavalink_config.json");
            if (File.Exists(path))
            {
                using var fs = new FileStream(path, FileMode.Open);
                using var sr = new StreamReader(fs);
                string json = await sr.ReadToEndAsync().ConfigureAwait(false);
                try
                {
                    return JsonConvert.DeserializeObject<LavalinkConfig>(json);
				}
				catch { /* Ignore JSON erros, a null value will be returned later after logging. */ }
			}

            if (logger is null)
			{
				Console.WriteLine("Missing lavalink_config.json file in Configs volume.");
			}
			else
			{
				logger.LogError($"Missing lavalink_config.json file in Configs volume.");
			}

			return null;
        }

        public static async Task<YouTubeConfig?> RegisterYouTube(ILogger? logger)
        {
            var path = Path.Join(Root, "youtube_config.json");

            if (File.Exists(path))
            {
                using var fs = new FileStream(path, FileMode.Open);
                using var sr = new StreamReader(fs);
                string json = await sr.ReadToEndAsync().ConfigureAwait(false);
                try
                {
                    return JsonConvert.DeserializeObject<YouTubeConfig>(json);
				}
				catch { /* Ignore JSON erros, a null value will be returned later after logging. */ }
			}

            if (logger is null)
			{
				Console.WriteLine("Missing youtube_config.json file in Configs volume.");
			}
			else
			{
				logger.LogError($"Missing youtube_config.json file in Configs volume.");
			}

			return null;
        }
    }
}
