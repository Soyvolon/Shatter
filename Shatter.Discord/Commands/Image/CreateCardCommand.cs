using System;
using System.Drawing;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Shatter.Discord.Commands.Attributes;
using Shatter.Discord.Properties;
using Shatter.Discord.Services;

namespace Shatter.Discord.Commands.Image
{
	public class CreateCardCommand : CommandModule
    {
        private readonly Tuple<Rectangle, string, Brush?>[] captions = new Tuple<Rectangle, string, Brush?>[]
        {
            new Tuple<Rectangle, string, Brush?>(new Rectangle(70, 70, 500, 500), "", null),
            new Tuple<Rectangle, string, Brush?>(new Rectangle(680, 70, 500, 500), "", null),
        };

        private readonly MemeService _meme;

        public CreateCardCommand(MemeService meme)
        {
			this._meme = meme;
        }

        [Command("createcard")]
        [Description("Create your own Cards Against Humanity cards!")]
        [Aliases("cah")]
        [RequireBotPermissions(Permissions.AttachFiles)]
        [ExecutionModule("image")]
        public async Task ExampleCommandAsync(CommandContext ctx,
            [Description("Text to include, separate the sections with `|`")]
            [RemainingText]
            string input)
        {
            if (input is null || input.Equals(""))
            {
                await RespondBasicErrorAsync($"To create a card: `{ctx.Prefix}cah This is the text on the question card | this is the text on the answer card`");
                return;
            }

            var captionStrings = input.Split("|", StringSplitOptions.RemoveEmptyEntries);

            if (captionStrings.Length < 2)
            {
                await RespondBasicErrorAsync("You need at least two different segments! Separate them with a `|` delimiter. " +
                    "Ex: `This is the text on the question card | this is the text on the answer card`");
                return;
            }

			this.captions[0] = new Tuple<Rectangle, string, Brush?>(this.captions[0].Item1, captionStrings[0], new SolidBrush(Color.White));
			this.captions[1] = new Tuple<Rectangle, string, Brush?>(this.captions[1].Item1, captionStrings[1], new SolidBrush(Color.Black));

            using var stream = await this._meme.BuildMemeAsync(Resources.Images_CAH, this.captions, "schoolbell", 24);

            await ctx.RespondAsync(new DiscordMessageBuilder().WithFile("cah-meme.png", stream));
        }
    }
}
