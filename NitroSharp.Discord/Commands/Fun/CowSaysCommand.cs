using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace NitroSharp.Discord.Commands.Fun
{
    public class CowSaysCommand : BaseCommandModule
    {
        public const string cow = "        \\   ^__^\n         \\  (oo)\\_______\n            (__)\\       )\\/\\\n                ||----w |\n                ||     ||\n";

        [Command("cowsay")]
        [Description("Make the cow say something.")]
        [Aliases("cowsays")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        public async Task ExampleCommandAsync(CommandContext ctx,
            [Description("What do you want the cow to say?")]
            string msg)
        {
            await ctx.RespondAsync("`\n." + new string('_', msg.Length + 2) + "\n< " + msg + " >\n " + new string('-', msg.Length + 2) + "\n" + cow + "`");
        }
    }
}
