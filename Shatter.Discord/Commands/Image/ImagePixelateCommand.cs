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
    public class ImagePixelateCommand : CommandModule
    {
        [Command("pixelate")]
        [Description("Pixelate an image!")]
        [Aliases("pixel")]
        [RequireBotPermissions(Permissions.AttachFiles)]
        [ExecutionModule("image")]
        public async Task ImagePixelateCommandAsync(CommandContext ctx)
        {
            DiscordAttachment[] attach;
            if ((attach = ctx.Message.Attachments.ToArray()).Length > 0)
            {
                await ImagePixelateCommandAsync(ctx, attach[0].Url.ToString());
            }
            else
            {
                await RespondBasicErrorAsync("No image to pixelate!");
            }
        }

        [Command("pixelate")]
        [Priority(2)]
        public async Task ImagePixelateCommandAsync(CommandContext ctx, string URL)
        {
            using var map = ImageLoader.GetBitmapFromUrl(URL);

            if (map is null)
            {
                await RespondBasicErrorAsync("Failed to get an image!");
                return;
            }

            using var scaledMap = await ImageUtils.Pixelate(map);

            using MemoryStream mem = new MemoryStream();
            scaledMap.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
            mem.Seek(0, SeekOrigin.Begin);

            await ctx.RespondWithFileAsync("pixelate-img.png", mem);
        }
    }
}
