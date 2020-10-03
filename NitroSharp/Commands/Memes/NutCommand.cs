using System;
using System.Drawing;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using NitroSharp.Properties;
using NitroSharp.Services;

namespace NitroSharp.Commands.Memes
{
    public class NutCommand : BaseCommandModule
    {
        private readonly Tuple<Rectangle, string>[] captions = new Tuple<Rectangle, string>[]
        {
            new Tuple<Rectangle, string>(new Rectangle(8, 20, 590, 145), "")
        };

        private readonly MemeService _meme;

        public NutCommand(MemeService meme)
        {
            this._meme = meme;
        }

        [Command("nut")]
        [Description("Make your own Nut Meme")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        [RequireBotPermissions(Permissions.AttachFiles)]
        public async Task NutCommandAsync(CommandContext ctx,
            [Description("Text to put in the meme")]
            [RemainingText] string msg)
        {
            captions[0] = new Tuple<Rectangle, string>(captions[0].Item1, msg);

            var img = await _meme.BuildMemeAsync(Resources.Images_NutMeme, captions, "arial", 40, new SolidBrush(Color.Black));

            await ctx.RespondWithFileAsync("nut-meme.png", img);

            img?.Dispose();
        }
    }
}
