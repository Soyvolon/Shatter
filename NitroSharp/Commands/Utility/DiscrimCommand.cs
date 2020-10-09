using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace NitroSharp.Commands.Utility
{
    public class DiscrimCommand : BaseCommandModule
    {
        [Command("discrim")]
        [Description("Search for a user by their discriminator")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task ExampleCommandAsync(CommandContext ctx, 
            [Description("Discriminator. Ex: 1234")]
            int discrim)
        {
            await CommandUtils.RespondBasicErrorAsync(ctx, "Currently Broke.");
            return;


            var discrimString = discrim.ToString();
            if(discrimString.Length != 4)
            {
                await CommandUtils.RespondBasicErrorAsync(ctx, "Discriminators are 4 digits. Ex: 1234");
                return;
            }

            var members = await ctx.Guild.GetAllMembersAsync();
            var usersByDiscrim = members.Where(x => x.Discriminator?.Equals(discrimString) ?? false);
            List<DiscordEmbedBuilder> builders = new List<DiscordEmbedBuilder>();

            var builderBase = CommandUtils.SuccessBase()
                .WithTitle($"**Users with the discriminator: {discrimString}**");

            int items = 0, fields = 0;

            foreach(var user in usersByDiscrim)
            {
                if (fields == 0)
                    builders.Add(new DiscordEmbedBuilder(builderBase));

                builders[items].AddField(user.Username, user.Id.ToString(), false);
                fields++;

                if(fields == 24)
                {
                    fields = 0;
                    items++;
                }
            }

            foreach (var b in builders)
                await ctx.RespondAsync(embed: b);
        }
    }
}
