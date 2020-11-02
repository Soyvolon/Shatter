using System;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

namespace Shatter.Discord.Commands.CustomArguments
{
    public enum AddRemove
    {
        Add,
        Remove
    }

    public class AddRemoveTypeConverter : IArgumentConverter<AddRemove>
    {
        public static readonly string[] AddWords = new string[] { "add", "a", "+" };
        public static readonly string[] RemoveWords = new string[] { "remove", "r", "-" };

        public Task<Optional<AddRemove>> ConvertAsync(string value, CommandContext ctx)
        {
            var lower = value.ToLower();
            if (AddWords.Contains(lower))
                return Task.FromResult(Optional.FromValue(AddRemove.Add));
            else if (RemoveWords.Contains(lower))
                return Task.FromResult(Optional.FromValue(AddRemove.Remove));
            else
                return Task.FromResult(Optional.FromNoValue<AddRemove>());
        }
    }
}
