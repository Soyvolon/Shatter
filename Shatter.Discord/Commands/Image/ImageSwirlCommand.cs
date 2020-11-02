using System.IO;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Shatter.Core.Utils;

namespace Shatter.Discord.Commands.Image
{
    public class ImageSwirlCommand : CommandModule
    {
        [Command("swirl")]
        [Description("Swirl and image!")]
        [RequireBotPermissions(Permissions.AttachFiles)]
        public async Task ImageSwirlCommandAsync(CommandContext ctx,
            [Description("The ammount of times to twist the iamge. Choose a value between 0.01 and 5.00")]
            double swirlTwists = .5)
        {
            DiscordAttachment[] attach;
            if ((attach = ctx.Message.Attachments.ToArray()).Length > 0)
            {
                await ImageSwirlCommandAsync(ctx, attach[0].Url.ToString(), swirlTwists);
            }
            else
            {
                await RespondBasicErrorAsync("No image to swirl!");
            }
        }

        [Command("swirl")]
        public async Task ImageSwirlCommandAsync(CommandContext ctx, string URL,
            [Description("The ammount of times to twist the iamge. Choose a value between 0.01 and 5.00")]
            double swirlTwists = .5)
        {
            var swirl = swirlTwists > 5.0 ? 5.0 : swirlTwists;
            swirl = swirl < 0.01 ? 0.01 : swirl;

            using var map = ImageLoader.GetBitmapFromUrl(URL);

            if (map is null)
            {
                await RespondBasicErrorAsync("Failed to get an image!");
                return;
            }

            using var swirledMap = await ImageUtils.Swirl(map, swirl);

            using MemoryStream mem = new MemoryStream();
            swirledMap.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
            mem.Seek(0, SeekOrigin.Begin);

            await ctx.RespondWithFileAsync("swirl-img.png", mem);
        }
    }
}
