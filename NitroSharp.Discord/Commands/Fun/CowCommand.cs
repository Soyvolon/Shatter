using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Newtonsoft.Json;

using NitroSharp.Core.Extensions;

namespace NitroSharp.Discord.Commands.Fun
{
    public class CowCommand : BaseCommandModule
    {
        [Command("cow")]
        [Description("Get a random ASCII Cow")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task CowCommandAsync(CommandContext ctx)
        {
            using FileStream fs = new FileStream("./Utils/JSON/ascii_cows.json", FileMode.Open);
            using StreamReader sr = new StreamReader(fs);
            var json = await sr.ReadToEndAsync();

            var cows = JsonConvert.DeserializeObject<List<string>>(json);

            await ctx.RespondAsync($"```{cows.Random()}```");
        }
    }
}
