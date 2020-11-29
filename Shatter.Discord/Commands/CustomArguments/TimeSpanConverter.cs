﻿using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

namespace Shatter.Discord.Commands.CustomArguments
{
	public class TimeSpanConverter : IArgumentConverter<TimeSpan>
    {
        public Task<Optional<TimeSpan>> ConvertAsync(string value, CommandContext ctx)
        {
            if (value is null)
			{
				return Task.FromResult(Optional.FromNoValue<TimeSpan>());
			}

			var span = new TimeSpan();

            for (int i = 0, c = 0; i < value.Length; i++)
            {
                if (!char.IsDigit(value[i]))
                {
                    switch (value[i].ToString())
                    {
                        case "Y":
                        case "y":
                            if (int.TryParse(value[c..i], out int num))
                            {
                                span = span.Add(TimeSpan.FromDays(num * 365));
                            }
                            break;
                        case "M":
                            if (int.TryParse(value[c..i], out num))
                            {
                                span = span.Add(TimeSpan.FromDays(num * 30));
                            }
                            break;
                        case "W":
                        case "w":
                            if (int.TryParse(value[c..i], out num))
                            {
                                span = span.Add(TimeSpan.FromDays(num * 7));
                            }
                            break;
                        case "D":
                        case "d":
                            if (int.TryParse(value[c..i], out num))
                            {
                                span = span.Add(TimeSpan.FromDays(num));
                            }
                            break;
                        case "h":
                            if (int.TryParse(value[c..i], out num))
                            {
                                span = span.Add(TimeSpan.FromHours(num));
                            }
                            break;
                        case "m":
                            if (int.TryParse(value[c..i], out num))
                            {
                                span = span.Add(TimeSpan.FromMinutes(num));
                            }
                            break;
                    }

                    c = i + 1;
                }
            }

            return Task.FromResult(Optional.FromValue(span));
        }
    }
}
