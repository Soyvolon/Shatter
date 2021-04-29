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
	public class PrisonerCommand : CommandModule
    {
        private readonly Tuple<Rectangle, string, Brush?>[] captions = new Tuple<Rectangle, string, Brush?>[]
        {
            new Tuple<Rectangle, string, Brush?>(new Rectangle(276, 250, 76, 54), "", null)
        };

        private readonly MemeService _meme;

        public PrisonerCommand(MemeService meme)
        {
			this._meme = meme;
        }

        [Command("prisoner")]
        [Description("The prisoners on a bench meme.")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        [RequireBotPermissions(Permissions.AttachFiles)]
        [ExecutionModule("memes")]
        public async Task PrisonerCommandAsync(CommandContext ctx,
            [Description("The message for the prisoner")]
            [RemainingText] string msg)
        {
			this.captions[0] = new Tuple<Rectangle, string, Brush?>(this.captions[0].Item1, msg, null);

            using var img = await this._meme.BuildMemeAsync(Resources.Images_Prison, this.captions, "roboto", 10, new SolidBrush(Color.Black));

            await ctx.RespondAsync(new DiscordMessageBuilder().WithFile("prisoner-meme.png", img));
        }
    }
}
