using System.Collections.Generic;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace NitroSharp.Commands.Fun
{
    public class EmojifyCommand : BaseCommandModule
    {
        public readonly IReadOnlyDictionary<string, string> emojis = new Dictionary<string, string>
        {
            { "a", "🇦" },
            { "b", "🇧" },
            { "c", "🇨" },
            { "d", "🇩" },
            { "e", "🇪" },
            { "f", "🇫" },
            { "g", "🇬" },
            { "h", "🇭" },
            { "i", "🇮" },
            { "j", "🇯" },
            { "k", "🇰" },
            { "l", "🇱" },
            { "m", "🇲" },
            { "n", "🇳" },
            { "o", "🇴" },
            { "p", "🇵" },
            { "q", "🇶" },
            { "r", "🇷" },
            { "s", "🇸" },
            { "t", "🇹" },
            { "u", "🇺" },
            { "v", "🇻" },
            { "w", "🇼" },
            { "x", "🇽" },
            { "y", "🇾" },
            { "z", "🇿" },
            { "0", "0⃣" },
            { "1", "1⃣" },
            { "2", "2⃣" },
            { "3", "3⃣" },
            { "4", "4⃣" },
            { "5", "5⃣" },
            { "6", "6⃣" },
            { "7", "7⃣" },
            { "8", "8⃣" },
            { "9", "9⃣" },
            { "<", "◀" },
            { ">", "▶" },
            { "!", "❗" },
            { "?", "❓" },
            { "^", "🔼" },
            { "+", "➕" },
            { "-", "➖" },
            { "÷", "➗" },
            { ".", "🔘" },
            { "$", "💲" },
            { "#", "#️⃣" },
            { "*", "*️⃣" }
        };

        [Command("emojify")]
        [Description("Emojify your message!")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task EmojifyCommandAsync(CommandContext ctx,
            [Description("The message to emojify!")]
            [RemainingText]
            string msg)
        {
            var lower = msg.ToLower();
            var data = "";
            foreach (char c in lower)
                data += (emojis.TryGetValue(c.ToString(), out string? value) ? value : c.ToString()) + " ";

            await ctx.RespondAsync(data);
        }
    }
}
