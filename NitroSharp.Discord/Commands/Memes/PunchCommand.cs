﻿using System.Drawing;
using System.IO;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using NitroSharp.Core.Utils;
using NitroSharp.Discord.Properties;

namespace NitroSharp.Discord.Commands.Memes
{
    public class PunchCommand : BaseCommandModule
    {
        [Command("punch")]
        [Description("Suckerpunch another user.")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        [RequireBotPermissions(Permissions.AttachFiles)]
        public async Task PunchCommandAsync(CommandContext ctx, [Description("The user to suckerpunch")] DiscordMember member)
        {
            using var userPfp = ImageLoader.GetBitmapFromUrl(ctx.Member.GetAvatarUrl(ImageFormat.Png, 64));
            using var userPfpGraphic = Graphics.FromImage(userPfp);

            using var punchedPfp = ImageLoader.GetBitmapFromUrl(member.GetAvatarUrl(ImageFormat.Png, 64));
            using var punchedPfpGraphic = Graphics.FromImage(punchedPfp);


            using MemoryStream img = new MemoryStream(Resources.Images_Punch);
            using Bitmap map = new Bitmap(img);
            using Graphics graphic = Graphics.FromImage(map);

            graphic.DrawImage(userPfp, new Rectangle(115, 30, 100, 100));
            graphic.DrawImage(punchedPfp, new Rectangle(400, 40, 100, 100));

            using var final = new MemoryStream();

            map.Save(final, System.Drawing.Imaging.ImageFormat.Png);

            final?.Seek(0, SeekOrigin.Begin);

            await ctx.RespondWithFileAsync("punch-meme.png", final,
                $"{Formatter.Bold(ctx.Member.DisplayName)} {Formatter.Italic("sucker punches")} {Formatter.Bold(member.DisplayName)}");
        }
    }
}
