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
	public class DeathNoteCommand : CommandModule
    {
        private readonly Tuple<Rectangle, string, Brush?>[] captions = new Tuple<Rectangle, string, Brush?>[]
        {
            new Tuple<Rectangle, string, Brush?>(new Rectangle(275, 55, 100, 180), "", null),
        };

        private readonly MemeService _meme;

        public DeathNoteCommand(MemeService meme)
        {
			this._meme = meme;
        }

        [Command("deathnote")]
        [Description("Add a user to your deathnote")]
        [RequireBotPermissions(Permissions.AttachFiles)]
        [ExecutionModule("image")]
        public async Task DeathNoteCommandAsync(CommandContext ctx,
            [Description("User to add to your DeathNote")]
            DiscordMember member)
        {
			this.captions[0] = new Tuple<Rectangle, string, Brush?>(this.captions[0].Item1, member.DisplayName, null);

            using var stream = await this._meme.BuildMemeAsync(Resources.Images_DeathNote, this.captions, "apple", 12, new SolidBrush(Color.Black));

            await ctx.RespondAsync(new DiscordMessageBuilder()
				.WithFile("deathnote-meme.png", stream)
				.WithContent(ctx.Member.Id == member.Id ? "*Looks like you killed yourself*" : ""));
        }
    }
}
