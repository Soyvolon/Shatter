using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using Shatter.Core.Extensions;

namespace Shatter.Discord.Commands
{
	public enum ColorType
    {
        Nitro,
        Action,
        Memberlog
    }

    public enum ActionColorIndex
    {
        Ban = 0,
        TempBan,
        SoftBan,
        Kick,
        Mute,
        Warn,
        UnBan
    }

    public class CommandModule : BaseCommandModule
    {
        public CommandModule() { }

        private CommandContext? ctx;

        public override Task BeforeExecutionAsync(CommandContext ctx)
        {
            this.ctx = ctx;
            return base.BeforeExecutionAsync(ctx);
        }

        public static readonly Dictionary<ColorType, List<int>> Colors = new Dictionary<ColorType, List<int>>()
        {
            { ColorType.Nitro,
                new List<int>()
                {
                    0x03A9F4,
                    0x039BE5,
                    0x0288D1,
                    0x0277BD,
                    0x2196F3,
                    0x1E88E5,
                    0x1976D2,
                    0x2962FF,
                    0x448AFF,
                    0x2979FF,
                    0x2196F3,
                    0x1E88E5,
                    0x1976D2,
                    0x1565C0,
                    0x0091EA
                }
            },

            { ColorType.Action,
                new List<int>()
                {
                    0xB71C1C, // Ban
                    0xD32F2F, // Temp Ban
                    0xF44336, // Soft Ban
                    0xF57C00, // Kick
                    0xFF9800, // Mute
                    0xFDD835, // Warn
                    0x76FF03  // Unban
                }
            },

            { ColorType.Memberlog,
                new List<int>()
                {
                    0x76FF03,
                    0xD32F2F
                }
            }
        };

        public static DiscordEmbedBuilder ErrorBase()
        {
            try
            {
                return new DiscordEmbedBuilder().WithColor(DiscordColor.Red);
            }
            catch
            {
                return new DiscordEmbedBuilder();
            }
        }

        public static DiscordEmbedBuilder SuccessBase()
        {
            return new DiscordEmbedBuilder().WithColor(new DiscordColor(Colors[ColorType.Nitro].Random()));
        }

        public async Task RespondBasicSuccessAsync(string message)
		{
			if (ctx is null)
			{
				return;
			}

			var b = SuccessBase()
                .WithDescription(message);
            await ctx.RespondAsync(embed: b.Build());
        }

        public async Task RespondBasicErrorAsync(string message)
		{
			if (ctx is null)
			{
				return;
			}

			var b = ErrorBase()
                .WithDescription(message);
            await ctx.RespondAsync(embed: b.Build());
        }

        public async Task RespondWithFile(string caption, string fileName, string contents)
        {
			if (ctx is null)
			{
				return;
			}

			FileStream fs = new FileStream($"{fileName}-{ctx.User.Id}.txt", FileMode.OpenOrCreate);
            StreamWriter sr = new StreamWriter(fs);

            await sr.WriteAsync(contents);
            await sr.FlushAsync();

            var path = fs.Name;

            await sr.DisposeAsync();
            await fs.DisposeAsync();

            fs = new FileStream(path, FileMode.Open);

            await ctx.Channel.SendFileAsync(fs, caption);

            await fs.DisposeAsync();

            File.Delete(path);
        }
    }
}
