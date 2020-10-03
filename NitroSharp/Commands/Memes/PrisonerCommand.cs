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
    public class PrisonerCommand : BaseCommandModule
    {
        private readonly Tuple<Rectangle, string>[] captions = new Tuple<Rectangle, string>[]
        {
            new Tuple<Rectangle, string>(new Rectangle(276, 250, 76, 54), "")
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
        public async Task PrisonerCommandAsync(CommandContext ctx,
            [Description("The message for the prisoner")]
            [RemainingText] string msg)
        {
            captions[0] = new Tuple<Rectangle, string>(captions[0].Item1, msg);

            var img = await _meme.BuildMemeAsync(Resources.Images_Prison, captions, "arial", 10, new SolidBrush(Color.Black));

            await ctx.RespondWithFileAsync("prisoner-meme.png", img);

            img?.Dispose();
        }
    }
}
