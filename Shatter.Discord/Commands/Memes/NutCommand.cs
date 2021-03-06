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
	public class NutCommand : CommandModule
    {
        private readonly Tuple<Rectangle, string, Brush?>[] captions = new Tuple<Rectangle, string, Brush?>[]
        {
            new Tuple<Rectangle, string, Brush?>(new Rectangle(8, 20, 590, 145), "", null)
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
        [ExecutionModule("memes")]
        public async Task NutCommandAsync(CommandContext ctx,
            [Description("Text to put in the meme")]
            [RemainingText] string msg)
        {
			this.captions[0] = new Tuple<Rectangle, string, Brush?>(this.captions[0].Item1, msg, null);

            using var img = await this._meme.BuildMemeAsync(Resources.Images_NutMeme, this.captions, "roboto", 40, new SolidBrush(Color.Black));

            await ctx.RespondAsync(new DiscordMessageBuilder().WithFile("nut-meme.png", img));
        }
    }
}
