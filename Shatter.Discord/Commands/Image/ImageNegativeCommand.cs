using System.IO;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Shatter.Core.Utils;
using Shatter.Discord.Commands.Attributes;

namespace Shatter.Discord.Commands.Image
{
	public class ImageNegativeCommand : CommandModule
    {
        [Command("negate")]
        [Description("The negative of an image.")]
        [Aliases("negative")]
        [RequireBotPermissions(Permissions.AttachFiles)]
        [ExecutionModule("image")]
        public async Task ImageNegativeCommandAsync(CommandContext ctx)
        {
            DiscordAttachment[] attach;
            if ((attach = ctx.Message.Attachments.ToArray()).Length > 0)
            {
                await ImageNegativeCommandAsync(ctx, attach[0].Url.ToString());
            }
            else
            {
                await ImageNegativeCommandAsync(ctx, ctx.Member.AvatarUrl);
            }
        }

        [Command("negate")]
        [Priority(2)]
        public async Task ImageNegativeCommandAsync(CommandContext ctx, string URL)
        {
            using var map = ImageLoader.GetBitmapFromUrl(URL);

            if (map is null)
            {
                await RespondBasicErrorAsync("Failed to get an image!");
                return;
            }

            await ImageUtils.Negate(map);

            using MemoryStream mem = new MemoryStream();
            map.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
            mem.Seek(0, SeekOrigin.Begin);

            await ctx.RespondAsync(new DiscordMessageBuilder().WithFile("negated-img.png", mem));
        }
    }
}
