using System.IO;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Shatter.Discord.Commands.Attributes;
using Shatter.Discord.Properties;

namespace Shatter.Discord.Commands.Fun
{
	public class BurnCommand : CommandModule
    {

        [Command("burn")]
        [Description("Burn another user!")]
        [RequireBotPermissions(Permissions.AttachFiles)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [ExecutionModule("fun")]
        public async Task BurnCommandAsnyc(CommandContext ctx, DiscordMember m)
        {
            var msg = $"{Formatter.Bold(ctx.Member.DisplayName)} " +
                $"{Formatter.Italic("burned")} {Formatter.Bold(m.DisplayName)}";

            using var mem = new MemoryStream();
            Resources.Images_Burn?.Save(mem, System.Drawing.Imaging.ImageFormat.Gif);

            mem?.Seek(0, SeekOrigin.Begin);

            await ctx.Channel.SendFileAsync("burn-img.gif", mem, msg);
        }
    }
}
