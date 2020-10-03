using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

namespace NitroSharp.Commands.CustomArguments
{
    public enum MoneyLeaderboardType
    {
        Global,
        Server
    }

    public class MoneyLeaderboardTypeConverter : IArgumentConverter<MoneyLeaderboardType>
    {
        public Task<Optional<MoneyLeaderboardType>> ConvertAsync(string value, CommandContext ctx)
        {
            if (value is null || value == "") return Task.FromResult(Optional.FromValue(MoneyLeaderboardType.Global));

            if (value.Contains("server"))
                return Task.FromResult(Optional.FromValue(MoneyLeaderboardType.Server));

            if (value.Contains("global"))
                return Task.FromResult(Optional.FromValue(MoneyLeaderboardType.Global));

            return Task.FromResult(Optional.FromNoValue<MoneyLeaderboardType>());
        }
    }
}
