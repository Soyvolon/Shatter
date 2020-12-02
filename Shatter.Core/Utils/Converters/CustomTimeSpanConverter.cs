using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Shatter.Core.Utils.Converters
{
	public class CustomTimeSpanConverter : JsonConverter<TimeSpan>
	{
		public override TimeSpan ReadJson(JsonReader reader, Type objectType, [AllowNull] TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			string? raw = reader.Value as string;

			if (TimeSpan.TryParseExact(raw, @"mm\:ss", null, out TimeSpan res))
				return res;

			if (hasExistingValue)
				return existingValue;

			return default;
		}

		public override void WriteJson(JsonWriter writer, [AllowNull] TimeSpan value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString(@"mm\:ss"));
		}
	}
}
