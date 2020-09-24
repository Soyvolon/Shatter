using System.IO;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using NitroSharp.Modules;

namespace NitroSharp.Commands.Fun
{
    public class BurnCommand : BaseCommandModule
    {
        [Command("burn")]
        [Description("Burn another user!")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task BurnCommandAsnyc(CommandContext ctx, DiscordMember m)
        {
            var images = (ImageModule)Program.Bot.GetModule<ImageModule>();

            if (images is null) throw new InvalidModuleException("Image Module not found.");

            var fs = images.GetImageStream("burn");

            var msg = $"{Formatter.Bold(ctx.Member.DisplayName)} " +
                $"{Formatter.Italic("burned")} {Formatter.Bold(m.DisplayName)}";

            if (fs is null)
                await ctx.RespondAsync(msg);
            else
                await ctx.Channel.SendFileAsync(fs, msg);
        }
    }
}
