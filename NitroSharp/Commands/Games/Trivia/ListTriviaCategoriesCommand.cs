using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Newtonsoft.Json.Linq;

using NitroSharp.Structures.Trivia;

namespace NitroSharp.Commands.Games.Trivia
{
    public class ListTriviaCategoriesCommand : BaseCommandModule
    {
        [Command("triviacategories")]
        [Description("Lists the categories for trivia")]
        [Aliases("tcategories")]
        [RequireUserPermissions(Permissions.AccessChannels)]
        public async Task ListTriviaCategoriesAsync(CommandContext ctx)
        {
            var client = new HttpClient();

            var res = await client.GetAsync("https://opentdb.com/api_category.php");

            var jsonString = await res.Content.ReadAsStringAsync();

            JObject json = JObject.Parse(jsonString);

            List<TriviaCategory> categories = json["trivia_categories"]?.ToObject<List<TriviaCategory>>() ?? new List<TriviaCategory>();

            string outputString = $"[id: {Formatter.Bold("0")}] All Categories\n";

            foreach (var cat in categories)
                outputString += $"[id: {Formatter.Bold(cat.Id.ToString())}] {cat.Name}\n";

            await ctx.RespondAsync(outputString);
        }
    }
}
