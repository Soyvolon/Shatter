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
    public class GruCommand : BaseCommandModule
    {
        private readonly Tuple<Rectangle, string>[] captions = new Tuple<Rectangle, string>[]
        {
            new Tuple<Rectangle, string>(new Rectangle(310, 80, 160, 218), ""),
            new Tuple<Rectangle, string>(new Rectangle(826, 85, 160, 218), ""),
            new Tuple<Rectangle, string>(new Rectangle(313, 414, 160, 218), ""),
            new Tuple<Rectangle, string>(new Rectangle(824, 413, 160, 218), "")
        };

        private readonly MemeService _meme;

        public GruCommand(MemeService meme)
        {
            this._meme = meme;
        }

        [Command("gru")]
        [Description("The Gru meme.")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        [RequireBotPermissions(Permissions.AttachFiles)]
        public async Task GruCommandAsync(CommandContext ctx,
            [Description("Gru meme. Text is dilimited by `|`")]
            [RemainingText] string msg)
        {
            var captionStrings = msg.Split('|', StringSplitOptions.RemoveEmptyEntries);

            if (captionStrings.Length < 3)
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "Minimum of 3 texts required. Use `|` separate texts.\n" +
                    "Ex: Find a gru meme template | Create a custom gru meme generator | Nobody uses it");
                return;
            }

            captions[0] = new Tuple<Rectangle, string>(captions[0].Item1, captionStrings[0]);
            captions[1] = new Tuple<Rectangle, string>(captions[1].Item1, captionStrings[1]);
            captions[2] = new Tuple<Rectangle, string>(captions[2].Item1, captionStrings[2]);
            captions[3] = new Tuple<Rectangle, string>(captions[3].Item1, captionStrings[2]);

            var img = await _meme.BuildMemeAsync(Resources.Images_Gru, captions, "arialblack", 20, new SolidBrush(Color.Black));

            await ctx.RespondWithFileAsync("gru-meme.png", img);

            img?.Dispose();
        }
    }
}