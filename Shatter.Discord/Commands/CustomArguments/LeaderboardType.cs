using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

namespace Shatter.Discord.Commands.CustomArguments
{
	public enum LeaderboardType
    {
        Global,
        Server
    }

    public class LeaderboardTypeConverter : IArgumentConverter<LeaderboardType>
    {
        public Task<Optional<LeaderboardType>> ConvertAsync(string value, CommandContext ctx)
        {
            if (value is null || value == "") return Task.FromResult(Optional.FromValue(LeaderboardType.Global));

            if (value.Contains("server"))
                return Task.FromResult(Optional.FromValue(LeaderboardType.Server));

            if (value.Contains("global"))
                return Task.FromResult(Optional.FromValue(LeaderboardType.Global));

            return Task.FromResult(Optional.FromNoValue<LeaderboardType>());
        }
    }
}
