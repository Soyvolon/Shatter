﻿using System;
using System.Drawing;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Properties;
using NitroSharp.Services;

namespace NitroSharp.Commands.Memes
{
    public class TheSearchCommand : BaseCommandModule
    {
        private readonly Tuple<Rectangle, string, Brush?>[] captions = new Tuple<Rectangle, string, Brush?>[]
        {
            new Tuple<Rectangle, string, Brush?>(new Rectangle(60, 330, 162, 69), "", null)
        };

        private readonly MemeService _meme;

        public TheSearchCommand(MemeService meme)
        {
            this._meme = meme;
        }

        [Command("thesearch")]
        [Description("The search continues meme")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        [RequireBotPermissions(Permissions.AttachFiles)]
        public async Task TheSearchCommandAsync(CommandContext ctx,
            [Description("A stupid idea")]
            [RemainingText] string msg)
        {
            captions[0] = new Tuple<Rectangle, string, Brush?>(captions[0].Item1, msg.ToUpper(), null);

            var font = 2;

            using var img = await _meme.BuildMemeAsync(Resources.Images_TheSearch, captions, "architect", font, new SolidBrush(Color.DarkSlateGray));

            await ctx.RespondWithFileAsync("thesearch-meme.png", img);
        }
    }
}