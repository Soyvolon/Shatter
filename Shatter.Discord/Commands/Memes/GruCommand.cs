using System;
using System.Drawing;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Shatter.Discord.Commands.Attributes;
using Shatter.Discord.Properties;
using Shatter.Discord.Services;

namespace Shatter.Discord.Commands.Memes
{
	public class GruCommand : CommandModule
    {
        private readonly Tuple<Rectangle, string, Brush?>[] captions = new Tuple<Rectangle, string, Brush?>[]
        {
            new Tuple<Rectangle, string, Brush?>(new Rectangle(310, 80, 160, 218), "", null),
            new Tuple<Rectangle, string, Brush?>(new Rectangle(826, 85, 160, 218), "", null),
            new Tuple<Rectangle, string, Brush?>(new Rectangle(313, 414, 160, 218), "", null),
            new Tuple<Rectangle, string, Brush?>(new Rectangle(824, 413, 160, 218), "", null)
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
        [ExecutionModule("memes")]
        public async Task GruCommandAsync(CommandContext ctx,
            [Description("Gru meme. Text is dilimited by `|`")]
            [RemainingText] string msg)
        {
            var captionStrings = msg.Split('|', StringSplitOptions.RemoveEmptyEntries);

            if (captionStrings.Length < 3)
            {
                await RespondBasicErrorAsync("Minimum of 3 texts required. Use `|` separate texts.\n" +
                    "Ex: Find a gru meme template | Create a custom gru meme generator | Nobody uses it");
                return;
            }

            captions[0] = new Tuple<Rectangle, string, Brush?>(captions[0].Item1, captionStrings[0], null);
            captions[1] = new Tuple<Rectangle, string, Brush?>(captions[1].Item1, captionStrings[1], null);
            captions[2] = new Tuple<Rectangle, string, Brush?>(captions[2].Item1, captionStrings[2], null);
            captions[3] = new Tuple<Rectangle, string, Brush?>(captions[3].Item1, captionStrings[2], null);

            using var img = await _meme.BuildMemeAsync(Resources.Images_Gru, captions, "robotoblack", 20, new SolidBrush(Color.Black));

            await ctx.RespondWithFileAsync("gru-meme.png", img);
        }
    }
}