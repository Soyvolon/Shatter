using System;
using System.Drawing;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

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
        public async Task DeathNoteCommandAsync(CommandContext ctx,
            [Description("User to add to your DeathNote")]
            DiscordMember member)
        {
            captions[0] = new Tuple<Rectangle, string, Brush?>(captions[0].Item1, member.DisplayName, null);

            using var stream = await _meme.BuildMemeAsync(Resources.Images_DeathNote, captions, "apple", 12, new SolidBrush(Color.Black));

            await ctx.RespondWithFileAsync("deathnote-meme.png", stream, ctx.Member.Id == member.Id ? "*Looks like you killed yourself*" : "");
        }
    }
}
