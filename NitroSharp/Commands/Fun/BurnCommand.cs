using System.IO;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using NitroSharp.Services;

namespace NitroSharp.Commands.Fun
{
    public class BurnCommand : BaseCommandModule
    {
        private readonly ImageService _images;

        public BurnCommand(ImageService images)
        {
            this._images = images;
        }

        [Command("burn")]
        [Description("Burn another user!")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task BurnCommandAsnyc(CommandContext ctx, DiscordMember m)
        {
            var fs = _images.GetImageStream("burn");

            var msg = $"{Formatter.Bold(ctx.Member.DisplayName)} " +
                $"{Formatter.Italic("burned")} {Formatter.Bold(m.DisplayName)}";

            if (fs is null)
                await ctx.RespondAsync(msg);
            else
                await ctx.Channel.SendFileAsync(fs, msg);
        }
    }
}
