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

namespace Shatter.Discord.Commands.Memes
{
	public class TheSearchCommand : CommandModule
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
        [RequireBotPermissions(Permissions.AttachFiles)]
        [ExecutionModule("memes")]
        public async Task TheSearchCommandAsync(CommandContext ctx,
            [Description("A stupid idea")]
            [RemainingText] string msg)
        {
			this.captions[0] = new Tuple<Rectangle, string, Brush?>(this.captions[0].Item1, msg.ToUpper(), null);
			// System differences for this font between ubuntu and windows? Causes odd font sizing and will need release testing adjustments.
#if DEBUG
			var font = 2;
#else
			var font = 12;
#endif
			using var img = await this._meme.BuildMemeAsync(Resources.Images_TheSearch, this.captions, "architect", font, new SolidBrush(Color.DarkSlateGray));

            await ctx.RespondAsync(new DiscordMessageBuilder().WithFile("thesearch-meme.png", img));
        }
    }
}
