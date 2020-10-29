using System.IO;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using NitroSharp.Core.Utils;

namespace NitroSharp.Discord.Commands.Image
{
    public class ImageNegativeCommand : BaseCommandModule
    {
        [Command("negate")]
        [Description("The negative of an image.")]
        [Aliases("negative")]
        [RequireBotPermissions(Permissions.AttachFiles)]
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
                await CommandUtils.RespondBasicErrorAsync(ctx, "Failed to get an image!");
                return;
            }

            await ImageUtils.Negate(map);

            using MemoryStream mem = new MemoryStream();
            map.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
            mem.Seek(0, SeekOrigin.Begin);

            await ctx.RespondWithFileAsync("negated-img.png", mem);
        }
    }
}
